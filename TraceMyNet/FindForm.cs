namespace CodePlex.Tools.TraceMyNet
{
    using System.Windows.Forms;

    public partial class FindForm : Form
    {
        private int iFound;
        private int iLastValidFound;
        private int iStartPos;
        private int iEndPos;

        public FindForm()
        {
            InitializeComponent();
        }

        public FindForm(ref RichTextBox myTB)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            formTB = myTB;
            iFound = 0;
            iLastValidFound = 0;
            iStartPos = 0;
            iEndPos = formTB.Text.Length;
        }

        private void radioButtonDirectionUp_CheckedChanged(object sender, System.EventArgs e)
        {
            iStartPos = 0;
            iEndPos = iLastValidFound - 1;
        }

        private void radioButtonDirectionDown_CheckedChanged(object sender, System.EventArgs e)
        {
            iStartPos = iLastValidFound + 1;
            iEndPos = formTB.Text.Length;
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void MyFindForm_Load(object sender, System.EventArgs e)
        {
            bool bFocused = textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                buttonFindNext.Enabled = true;
            }
            else
            {
                buttonFindNext.Enabled = false;
            }
        }

        private void buttonFindNext_Click(object sender, System.EventArgs e)
        {
            RichTextBoxFinds rtbf = RichTextBoxFinds.None;

            if (checkBoxMatchCase.Checked == true)
            {
                rtbf = RichTextBoxFinds.MatchCase;
            }

            if (checkBoxMatchWholeWord.Checked == true)
            {
                rtbf |= RichTextBoxFinds.WholeWord;
            }

            if (radioButtonDirectionUp.Checked == true)
            {
                rtbf |= RichTextBoxFinds.Reverse;
            }

            iFound = formTB.Find(textBox1.Text, iStartPos, iEndPos, rtbf);

            if (iFound == -1)
            {
                MessageBox.Show("Cannot find '" + textBox1.Text + "'.", "TraceMyNet .Net", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                iLastValidFound = iFound;

                if (radioButtonDirectionUp.Checked == false)
                {
                    iStartPos = iFound + 1;
                    iEndPos = formTB.Text.Length;
                }
                else
                {
                    iStartPos = 0;
                    iEndPos = iFound - 1;
                }
            }
        }

        private void MyFindForm_Activated(object sender, System.EventArgs e)
        {
            bool bFocused = textBox1.Focus();
        }
    }
}
