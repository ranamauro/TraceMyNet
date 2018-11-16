namespace CodePlex.Tools.TraceMyNet
{
    using System.Windows.Forms;

    partial class FindForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelFindWhat = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonDirectionDown = new System.Windows.Forms.RadioButton();
            this.radioButtonDirectionUp = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelFindWhat
            // 
            this.labelFindWhat.Location = new System.Drawing.Point(8, 16);
            this.labelFindWhat.Name = "labelFindWhat";
            this.labelFindWhat.Size = new System.Drawing.Size(56, 16);
            this.labelFindWhat.TabIndex = 0;
            this.labelFindWhat.Text = "Fi&nd what:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(249, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Location = new System.Drawing.Point(328, 14);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(88, 24);
            this.buttonFindNext.TabIndex = 8;
            this.buttonFindNext.Text = "&Find Next";
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(328, 48);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(88, 24);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxMatchWholeWord
            // 
            this.checkBoxMatchWholeWord.Location = new System.Drawing.Point(8, 43);
            this.checkBoxMatchWholeWord.Name = "checkBoxMatchWholeWord";
            this.checkBoxMatchWholeWord.Size = new System.Drawing.Size(152, 16);
            this.checkBoxMatchWholeWord.TabIndex = 2;
            this.checkBoxMatchWholeWord.Text = "Match &whole word only";
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.Location = new System.Drawing.Point(8, 65);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(152, 16);
            this.checkBoxMatchCase.TabIndex = 3;
            this.checkBoxMatchCase.Text = "Match &case";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonDirectionDown);
            this.groupBox1.Controls.Add(this.radioButtonDirectionUp);
            this.groupBox1.Location = new System.Drawing.Point(171, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(144, 37);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Direction";
            // 
            // radioButtonDirectionDown
            // 
            this.radioButtonDirectionDown.Checked = true;
            this.radioButtonDirectionDown.Location = new System.Drawing.Point(80, 16);
            this.radioButtonDirectionDown.Name = "radioButtonDirectionDown";
            this.radioButtonDirectionDown.Size = new System.Drawing.Size(56, 16);
            this.radioButtonDirectionDown.TabIndex = 1;
            this.radioButtonDirectionDown.TabStop = true;
            this.radioButtonDirectionDown.Text = "&Down";
            this.radioButtonDirectionDown.CheckedChanged += new System.EventHandler(this.radioButtonDirectionDown_CheckedChanged);
            // 
            // radioButtonDirectionUp
            // 
            this.radioButtonDirectionUp.Location = new System.Drawing.Point(16, 16);
            this.radioButtonDirectionUp.Name = "radioButtonDirectionUp";
            this.radioButtonDirectionUp.Size = new System.Drawing.Size(56, 16);
            this.radioButtonDirectionUp.TabIndex = 0;
            this.radioButtonDirectionUp.Text = "&Up";
            this.radioButtonDirectionUp.CheckedChanged += new System.EventHandler(this.radioButtonDirectionUp_CheckedChanged);
            // 
            // FindForm
            // 
            this.AcceptButton = this.buttonFindNext;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(424, 88);
            this.Controls.Add(this.labelFindWhat);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxMatchCase);
            this.Controls.Add(this.checkBoxMatchWholeWord);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonFindNext);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.ShowInTaskbar = false;
            this.Text = "Find";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MyFindForm_Load);
            this.Activated += new System.EventHandler(this.MyFindForm_Activated);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label labelFindWhat;
        private TextBox textBox1;
        private Button buttonFindNext;
        private Button buttonCancel;
        private GroupBox groupBox1;
        private CheckBox checkBoxMatchWholeWord;
        private CheckBox checkBoxMatchCase;
        private RadioButton radioButtonDirectionUp;
        private RadioButton radioButtonDirectionDown;
        private RichTextBox formTB;

        #endregion
    }
}