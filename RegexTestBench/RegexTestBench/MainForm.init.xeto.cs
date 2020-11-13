using Eto.Forms;
using Eto.Serialization.Xaml;

namespace RegexTestBench
{
    public partial class MainForm : Form
    {
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

        private MainForm(bool initializeControls)
        {
            XamlReader.Load(this);
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
        }
    }
}
