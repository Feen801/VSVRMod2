using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using UnityEngine;
using UnityEngine.Networking;
using Vosk;

public class VoskSpeechRecognizer : IDisposable
{
    private readonly string _modelPath;
    private readonly VoiceProcessor _voiceProcessor;
    private readonly int _maxAlternatives;
    private readonly List<string> _keyPhrases;
    private readonly List<PhraseHandler> _phraseHandlers;

    private Model _model;
    private VoskRecognizer _recognizer;
    private bool _recognizerReady;
    private bool _isDecompressing;
    private bool _isInitialized;
    private bool _running;
    private bool _disposed;
    private Task _workerTask;

    private string _decompressedModelPath;
    private string _grammar = "";

    private readonly ConcurrentQueue<short[]> _threadedBufferQueue = new ConcurrentQueue<short[]>();
    private readonly ConcurrentQueue<string> _threadedResultQueue = new ConcurrentQueue<string>();

    public event Action<string> OnStatusUpdated;
    public event Action<string> OnTranscriptionResult;

    public class PhraseHandler
    {
        public string Phrase { get; set; }
        public Action<string> Callback { get; set; }
        public int Priority { get; set; }

        public PhraseHandler(string phrase, Action<string> callback, int priority = 0)
        {
            Phrase = phrase?.ToLower();
            Callback = callback;
            Priority = priority;
        }
    }

    public bool IsInitialized => _isInitialized;
    public bool IsRecording => _running;

    public VoskSpeechRecognizer(
        string modelPath,
        VoiceProcessor voiceProcessor,
        List<string> keyPhrases = null,
        int maxAlternatives = 3)
    {
        _modelPath = modelPath;
        _voiceProcessor = voiceProcessor;
        _phraseHandlers = new List<PhraseHandler>();
        _keyPhrases = keyPhrases ?? new List<string>();
        _maxAlternatives = maxAlternatives;
    }

    public void AddPhrase(string phrase, Action<string> callback, int priority = 0)
    {
        if (string.IsNullOrEmpty(phrase))
        {
            Debug.LogWarning("Cannot add empty phrase");
            return;
        }

        var handler = new PhraseHandler(phrase, callback, priority);
        _phraseHandlers.Add(handler);

        // Sort by priority (highest first)
        _phraseHandlers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public void RemovePhrase(string phrase)
    {
        _phraseHandlers.RemoveAll(h => h.Phrase == phrase.ToLower());
    }

    public void ClearPhrases()
    {
        _phraseHandlers.Clear();
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("Already initialized");
            return;
        }

        await WaitForMicrophoneInput();
        await DecompressModel();

        OnStatusUpdated?.Invoke("Loading Model from: " + _decompressedModelPath);
        _model = new Model(_decompressedModelPath);

        OnStatusUpdated?.Invoke("Initialized");

        _voiceProcessor.OnFrameCaptured += OnFrameCaptured;
        _voiceProcessor.OnRecordingStop += OnRecordingStop;

        _isInitialized = true;
    }

    public void StartRecording()
    {
        if (!_isInitialized)
        {
            Debug.LogError("Must initialize before recording");
            return;
        }
        if (_running)
        {
            Debug.LogWarning("Already recording");
            return;
        }

        _running = true;
        _voiceProcessor.StartRecording();
        _workerTask = Task.Run(ThreadedWork);
    }

    public void StopRecording()
    {
        if (!_running)
        {
            Debug.LogWarning("Not currently recording");
            return;
        }

        Debug.Log("Stop Recording");
        _running = false;
        _voiceProcessor.StopRecording();
    }

    public void Update()
    {
        while (_threadedResultQueue.TryDequeue(out string result))
        {
            OnTranscriptionResult?.Invoke(result);
        }
    }

    private async Task WaitForMicrophoneInput()
    {
        while (Microphone.devices.Length <= 0)
            await Task.Delay(100);
    }

    private async Task DecompressModel()
    {
        if (!Path.HasExtension(_modelPath) ||
            Directory.Exists(Path.Combine(Application.persistentDataPath,
                Path.GetFileNameWithoutExtension(_modelPath))))
        {
            OnStatusUpdated?.Invoke("Using existing decompressed model.");
            _decompressedModelPath = Path.Combine(Application.persistentDataPath,
                Path.GetFileNameWithoutExtension(_modelPath));
            return;
        }

        OnStatusUpdated?.Invoke("Decompressing model...");
        string dataPath = Path.Combine(Application.streamingAssetsPath, _modelPath);

        Stream dataStream;

        dataStream = File.OpenRead(dataPath);

        var zipFile = ZipFile.Read(dataStream);
        zipFile.ExtractProgress += ZipFileOnExtractProgress;

        OnStatusUpdated?.Invoke("Reading Zip file");
        zipFile.ExtractAll(Application.persistentDataPath);

        while (!_isDecompressing)
            await Task.Delay(100);

        _decompressedModelPath = Path.Combine(Application.persistentDataPath,
            Path.GetFileNameWithoutExtension(_modelPath));

        OnStatusUpdated?.Invoke("Decompressing complete!");
        await Task.Delay(1000);
        zipFile.Dispose();
    }

    private void ZipFileOnExtractProgress(object sender, ExtractProgressEventArgs e)
    {
        if (e.EventType == ZipProgressEventType.Extracting_AfterExtractAll)
        {
            _isDecompressing = true;
            _decompressedModelPath = e.ExtractLocation;
        }
    }

    private void UpdateGrammar()
    {
        if (_keyPhrases.Count == 0)
        {
            _grammar = "";
            return;
        }

        JSONArray keywords = new JSONArray();
        foreach (string keyphrase in _keyPhrases)
        {
            keywords.Add(new JSONString(keyphrase.ToLower()));
        }

        keywords.Add(new JSONString("[unk]"));
        _grammar = keywords.ToString();
    }

    private void OnFrameCaptured(short[] samples)
    {
        _threadedBufferQueue.Enqueue(samples);
    }

    private void OnRecordingStop()
    {
        Debug.Log("Recording stopped");
    }

    private async Task ThreadedWork()
    {
        Debug.Log("[VoskRecognizer] ThreadedWork started");

        if (!_recognizerReady)
        {
            UpdateGrammar();

            Debug.Log($"[VoskRecognizer] Creating recognizer with grammar: {(_grammar.Length > 0 ? "yes" : "no")}");

            if (string.IsNullOrEmpty(_grammar))
            {
                _recognizer = new VoskRecognizer(_model, 16000.0f);
            }
            else
            {
                _recognizer = new VoskRecognizer(_model, 16000.0f, _grammar);
            }

            _recognizer.SetMaxAlternatives(_maxAlternatives);
            _recognizerReady = true;

            Debug.Log("Recognizer ready");
        }

        int processedFrames = 0;
        while (_running)
        {
            bool processedAnyFrames = false;

            for (int i = 0; i < 10 && _threadedBufferQueue.TryDequeue(out short[] voiceResult); i++)
            {
                processedFrames++;
                processedAnyFrames = true;

                if (processedFrames % 100 == 0)
                {
                    Debug.Log($"[VoskRecognizer] Processed {processedFrames} frames, queue size: {_threadedBufferQueue.Count}");
                }

                if (_recognizer.AcceptWaveform(voiceResult, voiceResult.Length))
                {
                    var result = _recognizer.Result();
                    Debug.Log($"[VoskRecognizer] Got final result from AcceptWaveform");
                    _threadedResultQueue.Enqueue(result);
                }
            }

            if (!processedAnyFrames)
            {
                await Task.Delay(10);
            }
            else
            {
                await Task.Yield();
            }
        }

        if (_recognizer != null)
        {
            var finalResult = _recognizer.FinalResult();
            if (!string.IsNullOrEmpty(finalResult))
            {
                Debug.Log($"[VoskRecognizer] Final result on stop: {finalResult}");
                _threadedResultQueue.Enqueue(finalResult);
            }
        }

        Debug.Log("[VoskRecognizer] ThreadedWork ended");
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        _running = false;

        if (_voiceProcessor != null)
        {
            _voiceProcessor.OnFrameCaptured -= OnFrameCaptured;
            _voiceProcessor.OnRecordingStop -= OnRecordingStop;
            _voiceProcessor.StopRecording();
        }

        try
        {
            _workerTask?.Wait(TimeSpan.FromSeconds(3));
        }
        catch (AggregateException) { }

        _recognizer?.Dispose();
        _model?.Dispose();
    }
}