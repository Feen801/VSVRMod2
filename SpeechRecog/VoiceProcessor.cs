using System;
using System.Collections.Generic;
using UnityEngine;
using VSVRMod2;

/// <summary>
/// Class that records audio and delivers frames for real-time audio processing
/// </summary>
public class VoiceProcessor : IDisposable
{
    /// <summary>
    /// Indicates whether microphone is capturing or not
    /// </summary>
    public bool IsRecording
    {
        get { return _isRecording && _audioClip != null && Microphone.IsRecording(CurrentDeviceName); }
    }

    /// <summary>
    /// Sample rate of recorded audio
    /// </summary>
    public int SampleRate { get; private set; }

    /// <summary>
    /// Size of audio frames that are delivered
    /// </summary>
    public int FrameLength { get; private set; }

    /// <summary>
    /// Event where frames of audio are delivered
    /// </summary>
    public event Action<short[]> OnFrameCaptured;

    /// <summary>
    /// Event when audio capture thread stops
    /// </summary>
    public event Action OnRecordingStop;

    /// <summary>
    /// Event when audio capture thread starts
    /// </summary>
    public event Action OnRecordingStart;

    /// <summary>
    /// Available audio recording devices
    /// </summary>
    public List<string> Devices { get; private set; }

    /// <summary>
    /// Index of selected audio recording device
    /// </summary>
    public int CurrentDeviceIndex { get; private set; }

    /// <summary>
    /// Name of selected audio recording device
    /// </summary>
    public string CurrentDeviceName
    {
        get
        {
            if (CurrentDeviceIndex < 0 || CurrentDeviceIndex >= Microphone.devices.Length)
                return string.Empty;
            return Devices[CurrentDeviceIndex];
        }
    }

    private float _minimumSpeakingSampleValue = 0.05f;
    private float _silenceTimer = 1.0f;
    private bool _autoDetect;

    private float _timeAtSilenceBegan;
    private bool _audioDetected;
    private bool _didDetect;
    private bool _transmit;
    private bool _disposed;
    private bool _isRecording;

    private AudioClip _audioClip;
    private event Action RestartRecording;

    private int _startReadPos;
    private float[] _sampleBuffer;

    public VoiceProcessor(int microphoneIndex = 0, float minimumSpeakingSampleValue = 0.05f,
        float silenceTimer = 1.0f, bool autoDetect = false)
    {
        _minimumSpeakingSampleValue = minimumSpeakingSampleValue;
        _silenceTimer = silenceTimer;
        _autoDetect = autoDetect;

        UpdateDevices();
        CurrentDeviceIndex = microphoneIndex;
    }

    /// <summary>
    /// Updates list of available audio devices
    /// </summary>
    public void UpdateDevices()
    {
        Devices = new List<string>();
        foreach (var device in Microphone.devices)
            Devices.Add(device);

        if (Devices == null || Devices.Count == 0)
        {
            CurrentDeviceIndex = -1;
            Debug.LogError("There is no valid recording device connected");
            return;
        }

        if (CurrentDeviceIndex < 0 || CurrentDeviceIndex >= Devices.Count)
            CurrentDeviceIndex = 0;
    }

    /// <summary>
    /// Change audio recording device
    /// </summary>
    /// <param name="deviceIndex">Index of the new audio capture device</param>
    public void ChangeDevice(int deviceIndex)
    {
        if (deviceIndex < 0 || deviceIndex >= Devices.Count)
        {
            Debug.LogError($"Specified device index {deviceIndex} is not a valid recording device");
            return;
        }

        if (IsRecording)
        {
            // one time event to restart recording with the new device 
            // the moment the last session has completed
            RestartRecording += () =>
            {
                CurrentDeviceIndex = deviceIndex;
                StartRecording(SampleRate, FrameLength);
                RestartRecording = null;
            };
            StopRecording();
        }
        else
        {
            CurrentDeviceIndex = deviceIndex;
        }
    }

    /// <summary>
    /// Start recording audio
    /// </summary>
    /// <param name="sampleRate">Sample rate to record at</param>
    /// <param name="frameSize">Size of audio frames to be delivered</param>
    /// <param name="autoDetect">Should the audio continuously record based on the volume</param>
    public void StartRecording(int sampleRate = 16000, int frameSize = 512, bool? autoDetect = null)
    {
        if (autoDetect != null)
        {
            _autoDetect = (bool)autoDetect;
        }

        if (IsRecording)
        {
            // if sample rate or frame size have changed, restart recording
            if (sampleRate != SampleRate || frameSize != FrameLength)
            {
                RestartRecording += () =>
                {
                    StartRecording(SampleRate, FrameLength, autoDetect);
                    RestartRecording = null;
                };
                StopRecording();
            }

            return;
        }

        SampleRate = sampleRate;
        FrameLength = frameSize;

        _audioClip = Microphone.Start(CurrentDeviceName, true, 1, sampleRate);
        _isRecording = true;
        _startReadPos = 0;
        _sampleBuffer = new float[FrameLength];

        OnRecordingStart?.Invoke();
    }

    /// <summary>
    /// Stops recording audio
    /// </summary>
    public void StopRecording()
    {
        if (!_isRecording)
            return;

        _isRecording = false;

        Microphone.End(CurrentDeviceName);

        if (_audioClip != null)
        {
            UnityEngine.Object.Destroy(_audioClip);
            _audioClip = null;
        }

        _didDetect = false;

        OnRecordingStop?.Invoke();
        RestartRecording?.Invoke();
    }

    /// <summary>
    /// Call this from Update or FixedUpdate to process audio frames.
    /// This must be called from the main Unity thread.
    /// </summary>
    public void ProcessAudioFrame()
    {
        if (!_isRecording || _audioClip == null)
        {
            return;
        }

        int curClipPos = Microphone.GetPosition(CurrentDeviceName);
        if (curClipPos < _startReadPos)
            curClipPos += _audioClip.samples;

        int samplesAvailable = curClipPos - _startReadPos;
        if (samplesAvailable < FrameLength)
        {
            return;
        } 

        int endReadPos = _startReadPos + FrameLength;
        if (endReadPos > _audioClip.samples)
        {
            // fragmented read (wraps around to beginning of clip)
            // read bit at end of clip
            int numSamplesClipEnd = _audioClip.samples - _startReadPos;
            float[] endClipSamples = new float[numSamplesClipEnd];
            _audioClip.GetData(endClipSamples, _startReadPos);

            // read bit at start of clip
            int numSamplesClipStart = endReadPos - _audioClip.samples;
            float[] startClipSamples = new float[numSamplesClipStart];
            _audioClip.GetData(startClipSamples, 0);

            // combine to form full frame
            Array.Copy(endClipSamples, 0, _sampleBuffer, 0, numSamplesClipEnd);
            Array.Copy(startClipSamples, 0, _sampleBuffer, numSamplesClipEnd, numSamplesClipStart);
        }
        else
        {
            _audioClip.GetData(_sampleBuffer, _startReadPos);
        }

        _startReadPos = endReadPos % _audioClip.samples;

        if (_autoDetect == false)
        {
            _transmit = _audioDetected = true;
        }
        else
        {
            float maxVolume = 0.0f;

            for (int i = 0; i < _sampleBuffer.Length; i++)
            {
                if (_sampleBuffer[i] > maxVolume)
                {
                    maxVolume = _sampleBuffer[i];
                }
            }

            if (maxVolume >= _minimumSpeakingSampleValue)
            {
                _transmit = _audioDetected = true;
                _timeAtSilenceBegan = Time.time;
            }
            else
            {
                _transmit = false;

                if (_audioDetected && Time.time - _timeAtSilenceBegan > _silenceTimer)
                {
                    _audioDetected = false;
                }
            }
        }

        if (_audioDetected)
        {
            _didDetect = true;
            // converts to 16-bit int samples
            short[] pcmBuffer = new short[_sampleBuffer.Length];
            for (int i = 0; i < FrameLength; i++)
            {
                pcmBuffer[i] = (short)Math.Floor(_sampleBuffer[i] * short.MaxValue);
            }

            // raise buffer event
            if (OnFrameCaptured != null && _transmit)
                OnFrameCaptured.Invoke(pcmBuffer);
        }
        else
        {
            if (_didDetect)
            {
                OnRecordingStop?.Invoke();
                _didDetect = false;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        StopRecording();

        _disposed = true;
    }
}