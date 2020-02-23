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
        private readonly TextArea txtRegexPattern;
        private readonly TextArea txtReplacementString;
        private readonly TextArea txtInputText;
        private readonly ListBox lboPatternHistory;
        private readonly ListBox lboInputHistory;
        private readonly CheckBox chkCompiled;
        private readonly CheckBox chkCultureInvariant;
        private readonly CheckBox chkEcmaScript;
        private readonly CheckBox chkExplicitCapture;
        private readonly CheckBox chkIgnoreWhite;
        private readonly CheckBox chkIgnoreCase;
        private readonly CheckBox chkMultiline;
        private readonly CheckBox chkRightToLeft;
        private readonly CheckBox chkSingleLine;
        private readonly NumericStepper nudTimeout;
        private readonly TreeGridView tvwResultExplorer;

        private readonly LinkedList<RegexPattern> patternHistory = new LinkedList<RegexPattern>();
        private readonly LinkedList<string> inputHistory = new LinkedList<string>();

        public MainForm()
        {
            XamlReader.Load(this);
            txtRegexPattern = FindChild<TextArea>("txtRegexPattern");
            txtReplacementString = FindChild<TextArea>("txtReplacementString");
            txtInputText = FindChild<TextArea>("txtInputText");
            lboPatternHistory = FindChild<ListBox>("lboPatternHistory");
            lboInputHistory = FindChild<ListBox>("lboInputHistory");
            chkCompiled = FindChild<CheckBox>("chkCompiled");
            chkCultureInvariant = FindChild<CheckBox>("chkCultureInvariant");
            chkEcmaScript = FindChild<CheckBox>("chkEcmaScript");
            chkExplicitCapture = FindChild<CheckBox>("chkExplicitCapture");
            chkIgnoreWhite = FindChild<CheckBox>("chkIgnoreWhite");
            chkIgnoreCase = FindChild<CheckBox>("chkIgnoreCase");
            chkMultiline = FindChild<CheckBox>("chkMultiline");
            chkRightToLeft = FindChild<CheckBox>("chkRightToLeft");
            chkSingleLine = FindChild<CheckBox>("chkSingleLine");
            nudTimeout = FindChild<NumericStepper>("nudTimeout");
            tvwResultExplorer = FindChild<TreeGridView>("tvwResultExplorer");

            tvwResultExplorer.Columns.Add(new GridColumn()
            {
                HeaderText = "Match",
                DataCell = new TextBoxCell(0)
            });
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
            MatchCollection matches = regex.Matches(txtInputText.Text);
            PopulateResultExplorer(matches);
        }

        private void PopulateResultExplorer(MatchCollection matches)
        {
            tvwResultExplorer.SuspendLayout();
            var treeGridItemCollection = new TreeGridItemCollection();
            foreach (Match match in matches)
            {
                var item = new TreeGridItem
                {
                    Values = new string[] { match.Value },
                    Tag = match
                };

                treeGridItemCollection.Add(item);
            }

            tvwResultExplorer.DataStore = treeGridItemCollection;
            tvwResultExplorer.ResumeLayout();
        }

        private void SaveHistory()
        {
            RegexPattern pattern = GetPattern();

            patternHistory.AddFirst(pattern);
            UpdatePatternHistoryListBox();

            inputHistory.AddFirst(txtInputText.Text);
            UpdateInputHistoryListBox();
        }

        private RegexPattern GetPattern()
        {
            return new RegexPattern()
            {
                Pattern = txtRegexPattern.Text,
                ReplacementText = txtReplacementString.Text,
                IsCompiled = chkCompiled.Checked ?? false,
                IsCultureInvariant = chkCultureInvariant.Checked ?? false,
                IsEcmaScript = chkEcmaScript.Checked ?? false,
                IsExplicitCapture = chkExplicitCapture.Checked ?? false,
                IsIgnoreWhite = chkIgnoreWhite.Checked ?? false,
                IsIgnoreCase = chkIgnoreCase.Checked ?? false,
                IsMultiline = chkMultiline.Checked ?? false,
                IsRightToLeft = chkRightToLeft.Checked ?? false,
                IsSingleLine = chkSingleLine.Checked ?? false,
                Timeout = (int)nudTimeout.Value
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
            lboPatternHistory.SuspendLayout();
            lboPatternHistory.Items.Clear();
            foreach (var item in patternHistory)
            {
                lboPatternHistory.Items.Add(
                    new ListItem()
                    {
                        Tag = item,
                        Text = item.ToString()
                    });
            }
            lboPatternHistory.ResumeLayout();
        }

        private void UpdateInputHistoryListBox()
        {
            lboInputHistory.SuspendLayout();
            lboInputHistory.Items.Clear();
            foreach (var item in inputHistory)
            {
                lboInputHistory.Items.Add(
                    new ListItem()
                    {
                        Tag = item,
                        Text = new string(item.Take(20).ToArray())
                    });
            }
            lboInputHistory.ResumeLayout();
        }
    }
}
