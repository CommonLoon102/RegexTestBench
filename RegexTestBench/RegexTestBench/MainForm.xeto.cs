using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegexTestBench
{
    public partial class MainForm : Form
    {
        private readonly LinkedList<RegexPattern> _patternHistory = new();
        private readonly LinkedList<string> _inputHistory = new();

        private Regex CurrentRegex => CurrentPattern.Regex;

        private RegexPattern CurrentPattern =>
            new RegexPattern(txtRegexPattern.Text)
            {
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

        public MainForm() : this(initializeControls: true) { }

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxType.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxType.Error);
            }
        }

        private void HandleSplit(object sender, EventArgs e)
        {
            SaveHistory();
            try
            {
                RunSplit();
            }
            catch (RegexMatchTimeoutException)
            {
                ShowTimeoutErrorMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxType.Error);
            }
        }

        private static void ShowTimeoutErrorMessage()
        {
            MessageBox.Show("The pattern matching ran longer than the maximum allowed time.\n" +
                "Check the Timeout setting (by default it is 2 seconds).", "Regex Timeout",
                MessageBoxType.Error);
        }

        private void HandleResultExplorerSelectedItemChanged(object sender, EventArgs e)
        {
            switch ((tvwResultExplorer.SelectedItem as TreeGridItem)?.Tag)
            {
                case Capture capture:
                    txtMatchValue.Text = capture.Value;
                    txtInputText.Selection = new(capture.Index, capture.Index + capture.Length - 1);
                    txtInputText.Focus();
                    lblPosition.Text = $"Position: {capture.Index} Length: {capture.Length}";
                    break;
                case string splitText:
                    txtMatchValue.Text = splitText;
                    break;
                default:
                    break;
                case null:
                    break;
            }
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

            var inputText = (lboInputHistory.Items[lboInputHistory.SelectedIndex] as ListItem).Tag as string;
            txtInputText.Text = inputText;
            txtInputText.Selection = new(0, 0);
        }

        private void RunMatch()
        {
            string[] groupNames = CurrentRegex.GetGroupNames();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            MatchCollection matches = CurrentRegex.Matches(txtInputText.Text);
            stopwatch.Stop();
            var match = matches.Count == 1 ? "Match" : "Matches";
            lblStatusMessage.Text = $"{matches.Count} {match}, {stopwatch.Elapsed.TotalMilliseconds} ms";
            PopulateResultExplorerMatch(matches, groupNames);
        }

        private void RunReplace()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string replacedText = CurrentRegex.Replace(txtInputText.Text, txtReplacementString.Text);
            stopwatch.Stop();
            lblStatusMessage.Text = $"{stopwatch.Elapsed.TotalMilliseconds} ms";
            lblPosition.Text = string.Empty;
            PopulateResultExplorerReplace(replacedText);
        }

        private void RunSplit()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string[] splitTexts = CurrentRegex.Split(txtInputText.Text);
            stopwatch.Stop();
            var part = splitTexts.Length == 1 ? "Part" : "Parts";
            lblStatusMessage.Text = $"{splitTexts.Length} {part}, {stopwatch.Elapsed.TotalMilliseconds} ms";
            lblPosition.Text = string.Empty;
            PopulateResultExplorerSplit(splitTexts);
        }

        private void SaveHistory()
        {
            if (!_patternHistory.Any(ph => ph.IsSame(CurrentPattern)))
            {
                _patternHistory.AddFirst(CurrentPattern);
                UpdatePatternHistoryListBox();
            }

            if (!_inputHistory.Any(ih => ih == txtInputText.Text))
            {
                _inputHistory.AddFirst(txtInputText.Text);
                UpdateInputHistoryListBox();
            }
        }

        private void UpdatePatternHistoryListBox()
        {
            lboPatternHistory.SuspendLayout();
            lboPatternHistory.Items.Clear();
            foreach (RegexPattern item in _patternHistory)
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
            foreach (string item in _inputHistory)
            {
                lboInputHistory.Items.Add(
                    new ListItem()
                    {
                        Tag = item,
                        Text = new(item.Take(20).TakeWhile(c => c != '\r' && c != '\n').ToArray())
                    });
            }

            lboInputHistory.ResumeLayout();
        }

        private void PopulateResultExplorerMatch(MatchCollection matches, string[] groupNames)
        {
            tvwResultExplorer.SuspendLayout();
            ResetResultExplorer();

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
            txtMatchValue.Text = replacedText;
            splResultExplorer.Position = 0;
        }

        private void PopulateResultExplorerSplit(string[] splitTexts)
        {
            tvwResultExplorer.SuspendLayout();
            ResetResultExplorer();

            var treeGridItemCollection = new TreeGridItemCollection();
            foreach (string splitText in splitTexts)
            {
                var item = new TreeGridItem()
                {
                    Values = new string[] { splitText },
                    Tag = splitText
                };

                treeGridItemCollection.Add(item);
            }

            tvwResultExplorer.DataStore = treeGridItemCollection;
            tvwResultExplorer.ResumeLayout();
        }

        private void ResetResultExplorer()
        {
            txtMatchValue.Text = string.Empty;
            splResultExplorer.Position = splResultExplorer.Height - 30;
            tvwResultExplorer.ShowHeader = false;
            tvwResultExplorer.AllowMultipleSelection = false;
            tvwResultExplorer.Columns.Clear();
            tvwResultExplorer.Columns.Add(new GridColumn()
            {
                DataCell = new TextBoxCell(0)
            });
        }
    }
}
