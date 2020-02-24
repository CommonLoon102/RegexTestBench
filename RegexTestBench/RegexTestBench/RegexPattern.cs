using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexTestBench
{
    class RegexPattern
    {
        public string Pattern { get; set; }
        public string ReplacementText { get; set; }
        public bool IsCompiled { get; set; }
        public bool IsIgnoreCase { get; set; }
        public bool IsMultiline { get; set; }
        public bool IsExplicitCapture { get; set; }
        public bool IsEcmaScript { get; set; }
        public bool IsIgnoreWhite { get; set; }
        public bool IsSingleLine { get; set; }
        public bool IsRightToLeft { get; set; }
        public bool IsCultureInvariant { get; set; }
        public int Timeout { get; set; }

        public override string ToString() => Pattern;

        public Regex Regex => new Regex(Pattern, RegexOptions, TimeSpan.FromMilliseconds(Timeout));

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
