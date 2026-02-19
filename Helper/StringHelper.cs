using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VSVRMod2;

public class StringHelper
{
    public static string[] GetWords(string input)
    {
        string cleaned = Regex.Replace(input.ToLower(), @"[^a-z' ]+", "");
        return cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    public static string ExtractQuoted(string input)
    {
        Match match = Regex.Match(input, @"[""\u201C\u201D\u201E\u201F\u00AB\u00BB](.+?)[""\u201C\u201D\u201E\u201F\u00AB\u00BB]");
        return match.Success ? match.Groups[1].Value : null;
    }
    public static float MatchPercent(string[] words, string[] reference)
    {
        HashSet<string> wordSet = new HashSet<string>(words);
        int matches = reference.Count(w => wordSet.Contains(w));
        return reference.Length == 0 ? 0f : (float)matches / reference.Length * 100f;
    }
}
