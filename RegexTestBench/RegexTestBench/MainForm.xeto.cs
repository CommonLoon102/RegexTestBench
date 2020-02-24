using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace RegexTestBench
{
    public class MainForm : Form
    {
        #region Controls
        private readonly TextArea txtRegexPattern;
        private readonly TextArea txtReplacementString;
        private readonly TextArea txtInputText;
        private readonly TextArea txtMatchValue;
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
        private readonly Splitter splResultExplorer;
        private readonly TreeGridView tvwResultExplorer;
        private readonly Label lblStatusMessage;
        private readonly Label lblPosition;
        #endregion // Controls

        private readonly LinkedList<RegexPattern> patternHistory = new LinkedList<RegexPattern>();
        private readonly LinkedList<string> inputHistory = new LinkedList<string>();

        private RegexPattern CurrentPattern =>
            new RegexPattern()
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

        private Regex CurrentRegex => CurrentPattern.Regex;

        public MainForm()
        {
            XamlReader.Load(this);
            #region Initialize Controls
            txtRegexPattern = FindChild<TextArea>("txtRegexPattern");
            txtReplacementString = FindChild<TextArea>("txtReplacementString");
            txtInputText = FindChild<TextArea>("txtInputText");
            txtMatchValue = FindChild<TextArea>("txtMatchValue");
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
            splResultExplorer = FindChild<Splitter>("splResultExplorer");
            tvwResultExplorer = FindChild<TreeGridView>("tvwResultExplorer");
            lblStatusMessage = FindChild<Label>("lblStatusMessage");
            lblPosition = FindChild<Label>("lblPosition");
            #endregion // Initialize Controls
        }

        private void HandleMatch(object sender, EventArgs e)
        {
            SaveHistory();
            try
            {
                RunMatch();
            }
            catch (RegexMatchTimeoutException)
            {
                ShowTimeoutErrorMessage();
            }
        }

        private void HandleReplace(object sender, EventArgs e)
        {
            SaveHistory();
            try
            {
                RunReplace();
            }
            catch (RegexMatchTimeoutException)
            {
                ShowTimeoutErrorMessage();
            }
        }

        private void HandleValidate(object sender, EventArgs e)
        {
            SaveHistory();
            try
            {
                RunMatch();
            }
            catch (RegexMatchTimeoutException)
            {
                ShowTimeoutErrorMessage();
            }
        }

        private void HandleSplit(object sender, EventArgs e)
        {
            SaveHistory();
            try
            {
                RunMatch();
            }
            catch (RegexMatchTimeoutException)
            {
                ShowTimeoutErrorMessage();
            }
        }

        private static void ShowTimeoutErrorMessage()
        {
            MessageBox.Show("The pattern matching ran longer than the maximum allowed time.\n" +
                "Check the Timeout setting (by default it is 2 seconds).", "Regex Timeout",
                MessageBoxButtons.OK,
                MessageBoxType.Error,
                MessageBoxDefaultButton.OK);
        }

        private void HandleResultExplorerSelectedItemChanged(object sender, EventArgs e)
        {
            if (!((tvwResultExplorer.SelectedItem as TreeGridItem)?.Tag is Capture capture))
                return;

            txtMatchValue.Text = capture.Value;
            txtInputText.Selection = new Range<int>(capture.Index, capture.Index + capture.Length - 1);
            txtInputText.Focus();
            lblPosition.Text = $"Position: {capture.Index} Length: {capture.Length}";
        }

        private void HandlePatternHistorySelectedIndexChanged(object sender, EventArgs e)
        {
            if (lboPatternHistory.SelectedIndex < 0)
                return;

            RegexPattern pattern = (lboPatternHistory.Items[lboPatternHistory.SelectedIndex] as ListItem).Tag as RegexPattern;
            txtRegexPattern.Text = pattern.Pattern;
            txtReplacementString.Text = pattern.ReplacementText;
            chkCompiled.Checked = pattern.IsCompiled;
            chkCultureInvariant.Checked = pattern.IsCultureInvariant;
            chkEcmaScript.Checked = pattern.IsEcmaScript;
            chkExplicitCapture.Checked = pattern.IsExplicitCapture;
            chkIgnoreCase.Checked = pattern.IsIgnoreCase;
            chkIgnoreWhite.Checked = pattern.IsIgnoreWhite;
            chkMultiline.Checked = pattern.IsMultiline;
            chkRightToLeft.Checked = pattern.IsRightToLeft;
            chkSingleLine.Checked = pattern.IsSingleLine;
            nudTimeout.Value = pattern.Timeout;
        }

        private void HandleInputHistorySelectedIndexChanged(object sender, EventArgs e)
        {
            if (lboInputHistory.SelectedIndex < 0)
                return;

            string inputText = (lboInputHistory.Items[lboInputHistory.SelectedIndex] as ListItem).Tag as string;
            txtInputText.Text = inputText;
            txtInputText.Selection = new Range<int>(0, 0);
        }

        private void RunMatch()
        {
            var groupNames = CurrentRegex.GetGroupNames();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MatchCollection matches = CurrentRegex.Matches(txtInputText.Text);
            stopwatch.Stop();
            string match = matches.Count == 1 ? "Match" : "Matches";
            lblStatusMessage.Text = $"{matches.Count} {match}, {stopwatch.Elapsed.TotalMilliseconds} ms";
            PopulateResultExplorerMatch(matches, groupNames);
        }

        private void RunReplace()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string replacedText = CurrentRegex.Replace(txtInputText.Text, txtReplacementString.Text);
            stopwatch.Stop();
            lblStatusMessage.Text = $"{stopwatch.Elapsed.TotalMilliseconds} ms";
            PopulateResultExplorerReplace(replacedText);
        }

        private void SaveHistory()
        {
            if (!patternHistory.Any(ph => ph.IsSame(CurrentPattern)))
            {
                patternHistory.AddFirst(CurrentPattern);
                UpdatePatternHistoryListBox();
            }

            if (!inputHistory.Any(ih => ih == txtInputText.Text))
            {
                inputHistory.AddFirst(txtInputText.Text);
                UpdateInputHistoryListBox();
            }
        }

        private void UpdatePatternHistoryListBox()
        {
            lboPatternHistory.SuspendLayout();
            lboPatternHistory.Items.Clear();
            foreach (RegexPattern item in patternHistory)
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
            foreach (string item in inputHistory)
            {
                lboInputHistory.Items.Add(
                    new ListItem()
                    {
                        Tag = item,
                        Text = new string(item.Take(20).TakeWhile(c => c != '\r' && c != '\n').ToArray())
                    });
            }
            lboInputHistory.ResumeLayout();
        }

        private void PopulateResultExplorerMatch(MatchCollection matches, string[] groupNames)
        {
            txtMatchValue.Text = string.Empty;
            splResultExplorer.Position = splResultExplorer.Height - 30;
            tvwResultExplorer.SuspendLayout();
            tvwResultExplorer.ShowHeader = false;
            tvwResultExplorer.Columns.Clear();
            tvwResultExplorer.Columns.Add(new GridColumn()
            {
                HeaderText = "Match",
                DataCell = new TextBoxCell(0)
            });

            var treeGridItemCollection = new TreeGridItemCollection();
            foreach (Match match in matches)
            {
                var item = new TreeGridItem()
                {
                    Values = new string[] { match.Value },
                    Tag = match
                };

                foreach (string groupName in groupNames.Where(g => g != "0"))
                {
                    Group group = match.Groups[groupName];
                    item.Children.Add(new TreeGridItem()
                    {
                        Values = new string[] { $"{groupName}: {group.Value}" },
                        Tag = group
                    });
                }

                treeGridItemCollection.Add(item);
            }

            tvwResultExplorer.DataStore = treeGridItemCollection;
            tvwResultExplorer.ResumeLayout();
        }

        private void PopulateResultExplorerReplace(string replacedText)
        {
            tvwResultExplorer.DataStore = new TreeGridItemCollection();
            lblPosition.Text = string.Empty;
            txtMatchValue.Text = replacedText;
            splResultExplorer.Position = 0;
        }
    }
}
