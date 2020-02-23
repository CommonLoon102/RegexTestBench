using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using System.Text.RegularExpressions;

namespace RegexTestBench
{
    public class MainForm : Form
    {
        private readonly LinkedList<RegexPattern> patternHistory = new LinkedList<RegexPattern>();
        private readonly LinkedList<string> inputHistory = new LinkedList<string>();

        public MainForm()
        {
            XamlReader.Load(this);
        }

        protected void HandleMatch(object sender, EventArgs e)
        {
            SaveHistory();
            RunMatch();
        }

        protected void HandleReplace(object sender, EventArgs e)
        {
            SaveHistory();
            MessageBox.Show("I was clicked!");
        }

        protected void HandleValidate(object sender, EventArgs e)
        {
            SaveHistory();
            MessageBox.Show("I was clicked!");
        }

        protected void HandleSplit(object sender, EventArgs e)
        {
            SaveHistory();
            MessageBox.Show("I was clicked!");
        }

        protected void HandleAbout(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        protected void HandleQuit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        private void RunMatch()
        {
            RegexPattern pattern = GetPattern();
            RegexOptions regexOptions = GetRegexOptions(pattern);
            Regex regex = new Regex(pattern.Pattern, regexOptions, TimeSpan.FromMilliseconds(pattern.Timeout));
            string inputText = FindChild<TextArea>("txtInputText").Text;
            MatchCollection matches = regex.Matches(inputText);
            PopulateResultExplorer(matches);
        }

        private void PopulateResultExplorer(MatchCollection matches)
        {
            var resultExplorer = FindChild<TreeGridView>("tvwResultExplorer");
            resultExplorer.SuspendLayout();
            resultExplorer.Columns.Clear();
            
            resultExplorer.ResumeLayout();
        }

        private void SaveHistory()
        {
            RegexPattern pattern = GetPattern();

            patternHistory.AddFirst(pattern);
            UpdatePatternHistoryListBox();

            string inputText = FindChild<TextArea>("txtInputText").Text;
            inputHistory.AddFirst(inputText);
            UpdateInputHistoryListBox();
        }

        private RegexPattern GetPattern()
        {
            return new RegexPattern()
            {
                Pattern = FindChild<TextArea>("txtRegexPattern").Text,
                ReplacementText = FindChild<TextArea>("txtReplacementString").Text,
                IsCompiled = FindChild<CheckBox>("chkCompiled").Checked ?? false,
                IsCultureInvariant = FindChild<CheckBox>("chkCultureInvariant").Checked ?? false,
                IsEcmaScript = FindChild<CheckBox>("chkEcmaScript").Checked ?? false,
                IsExplicitCapture = FindChild<CheckBox>("chkExplicitCapture").Checked ?? false,
                IsIgnoreWhite = FindChild<CheckBox>("chkIgnoreWhite").Checked ?? false,
                IsIgnoreCase = FindChild<CheckBox>("chkIgnoreCase").Checked ?? false,
                IsMultiline = FindChild<CheckBox>("chkMultiline").Checked ?? false,
                IsRightToLeft = FindChild<CheckBox>("chkRightToLeft").Checked ?? false,
                IsSingleLine = FindChild<CheckBox>("chkSingleLine").Checked ?? false,
                Timeout = (int)FindChild<NumericStepper>("nudTimeout").Value
            };
        }

        private RegexOptions GetRegexOptions(RegexPattern pattern)
        {
            RegexOptions regexOptions = RegexOptions.None;
            if (pattern.IsCompiled)
                regexOptions |= RegexOptions.Compiled;
            if (pattern.IsCultureInvariant)
                regexOptions |= RegexOptions.CultureInvariant;
            if (pattern.IsEcmaScript)
                regexOptions |= RegexOptions.ECMAScript;
            if (pattern.IsExplicitCapture)
                regexOptions |= RegexOptions.ExplicitCapture;
            if (pattern.IsIgnoreCase)
                regexOptions |= RegexOptions.IgnoreCase;
            if (pattern.IsIgnoreWhite)
                regexOptions |= RegexOptions.IgnorePatternWhitespace;
            if (pattern.IsMultiline)
                regexOptions |= RegexOptions.Multiline;
            if (pattern.IsRightToLeft)
                regexOptions |= RegexOptions.RightToLeft;
            if (pattern.IsSingleLine)
                regexOptions |= RegexOptions.Singleline;

            return regexOptions;
        }

        private void UpdatePatternHistoryListBox()
        {
            var patternHistoryListBox = FindChild<ListBox>("lboPatternHistory");
            patternHistoryListBox.SuspendLayout();
            patternHistoryListBox.Items.Clear();
            foreach (var item in patternHistory)
            {
                patternHistoryListBox.Items.Add(
                    new ListItem()
                    {
                        Tag = item,
                        Text = item.ToString()
                    });
            }
            patternHistoryListBox.ResumeLayout();
        }

        private void UpdateInputHistoryListBox()
        {
            var inputHistoryListBox = FindChild<ListBox>("lboInputHistory");
            inputHistoryListBox.SuspendLayout();
            inputHistoryListBox.Items.Clear();
            foreach (var item in inputHistory)
            {
                inputHistoryListBox.Items.Add(
                    new ListItem()
                    {
                        Tag = item,
                        Text = new string(item.Take(20).ToArray())
                    });
            }
            inputHistoryListBox.ResumeLayout();
        }
    }
}
