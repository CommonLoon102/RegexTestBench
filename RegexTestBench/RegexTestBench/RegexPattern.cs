using System;
using System.Text.RegularExpressions;

namespace RegexTestBench
{
    internal class RegexPattern
    {
        private readonly string _pattern;

        public RegexPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException($"The parameter '{nameof(pattern)}' must not be null or empty string.", nameof(pattern));

            _pattern = pattern;
        }

        public string Pattern => _pattern;
        public string ReplacementText { get; init; }
        public bool IsCompiled { get; init; }
        public bool IsIgnoreCase { get; init; }
        public bool IsMultiline { get; init; }
        public bool IsExplicitCapture { get; init; }
        public bool IsEcmaScript { get; init; }
        public bool IsIgnoreWhite { get; init; }
        public bool IsSingleLine { get; init; }
        public bool IsRightToLeft { get; init; }
        public bool IsCultureInvariant { get; init; }
        public int Timeout { get; init; }

        public Regex Regex => new(_pattern,
            RegexOptions,
            Timeout == 0 ? Regex.InfiniteMatchTimeout : TimeSpan.FromMilliseconds(Timeout));

        public bool IsSame(RegexPattern other) => IsCompiled == other.IsCompiled
            && IsCultureInvariant == other.IsCultureInvariant
            && IsEcmaScript == other.IsEcmaScript
            && IsExplicitCapture == other.IsExplicitCapture
            && IsIgnoreCase == other.IsIgnoreCase
            && IsIgnoreWhite == other.IsIgnoreWhite
            && IsMultiline == other.IsMultiline
            && IsRightToLeft == other.IsRightToLeft
            && IsSingleLine == other.IsSingleLine
            && Pattern == other.Pattern
            && ReplacementText == other.ReplacementText
            && Timeout == other.Timeout;

        public override string ToString() => _pattern;

        private RegexOptions RegexOptions
        {
            get
            {
                RegexOptions regexOptions = RegexOptions.None;
                if (IsCompiled)
                    regexOptions |= RegexOptions.Compiled;
                if (IsCultureInvariant)
                    regexOptions |= RegexOptions.CultureInvariant;
                if (IsEcmaScript)
                    regexOptions |= RegexOptions.ECMAScript;
                if (IsExplicitCapture)
                    regexOptions |= RegexOptions.ExplicitCapture;
                if (IsIgnoreCase)
                    regexOptions |= RegexOptions.IgnoreCase;
                if (IsIgnoreWhite)
                    regexOptions |= RegexOptions.IgnorePatternWhitespace;
                if (IsMultiline)
                    regexOptions |= RegexOptions.Multiline;
                if (IsRightToLeft)
                    regexOptions |= RegexOptions.RightToLeft;
                if (IsSingleLine)
                    regexOptions |= RegexOptions.Singleline;

                return regexOptions;
            }
        }
    }
}
