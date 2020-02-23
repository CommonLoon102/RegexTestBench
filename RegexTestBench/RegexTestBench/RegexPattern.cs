using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
