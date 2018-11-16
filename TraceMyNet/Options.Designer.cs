namespace CodePlex.Tools.TraceMyNet
{
    using System.Windows.Forms;
    using System.ComponentModel;

    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.resolve = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.customIPAddresses = new System.Windows.Forms.CheckBox();
            this.ipv4Any = new System.Windows.Forms.CheckBox();
            this.ipv6Any = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.localIPAddresses = new System.Windows.Forms.ListBox();
            this.sslS = new System.Windows.Forms.CheckBox();
            this.currentCertLabelS = new System.Windows.Forms.Label();
            this.addCertFromStoreS = new System.Windows.Forms.Button();
            this.addCertFromFileS = new System.Windows.Forms.Button();
            this.localPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ipv6Allowed = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.sslC = new System.Windows.Forms.CheckBox();
            this.currentCertLabelC = new System.Windows.Forms.Label();
            this.addCertFromStoreC = new System.Windows.Forms.Button();
            this.addCertFromFileC = new System.Windows.Forms.Button();
            this.remoteIPAddress = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.remotePort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.remoteHostName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ipv4Allowed = new System.Windows.Forms.CheckBox();
            this.socketProxy = new System.Windows.Forms.CheckBox();
            this.save = new System.Windows.Forms.Button();
            this.load = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(218, 341);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(88, 23);
            this.ok.TabIndex = 18;
            this.ok.Text = "OK";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(313, 341);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(88, 23);
            this.cancel.TabIndex = 19;
            this.cancel.Text = "Cancel";
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // resolve
            // 
            this.resolve.Location = new System.Drawing.Point(305, 44);
            this.resolve.Name = "resolve";
            this.resolve.Size = new System.Drawing.Size(88, 23);
            this.resolve.TabIndex = 11;
            this.resolve.Text = "Resolve";
            this.toolTip.SetToolTip(this.resolve, "Resolves the Destination Server Name (or Address)");
            this.resolve.Click += new System.EventHandler(this.resolve_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.customIPAddresses);
            this.groupBox1.Controls.Add(this.ipv4Any);
            this.groupBox1.Controls.Add(this.ipv6Any);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.localIPAddresses);
            this.groupBox1.Controls.Add(this.sslS);
            this.groupBox1.Controls.Add(this.currentCertLabelS);
            this.groupBox1.Controls.Add(this.addCertFromStoreS);
            this.groupBox1.Controls.Add(this.addCertFromFileS);
            this.groupBox1.Controls.Add(this.localPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 152);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local End Point";
            // 
            // customIPAddresses
            // 
            this.customIPAddresses.Location = new System.Drawing.Point(111, 19);
            this.customIPAddresses.Name = "customIPAddresses";
            this.customIPAddresses.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.customIPAddresses.Size = new System.Drawing.Size(67, 43);
            this.customIPAddresses.TabIndex = 4;
            this.customIPAddresses.Text = "Custom List";
            this.customIPAddresses.CheckedChanged += new System.EventHandler(this.customIPAddresses_CheckedChanged);
            // 
            // ipv4Any
            // 
            this.ipv4Any.Checked = true;
            this.ipv4Any.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ipv4Any.Location = new System.Drawing.Point(13, 40);
            this.ipv4Any.Name = "ipv4Any";
            this.ipv4Any.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ipv4Any.Size = new System.Drawing.Size(73, 22);
            this.ipv4Any.TabIndex = 3;
            this.ipv4Any.Text = "IPv6 Any";
            // 
            // ipv6Any
            // 
            this.ipv6Any.Checked = true;
            this.ipv6Any.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ipv6Any.Location = new System.Drawing.Point(13, 21);
            this.ipv6Any.Name = "ipv6Any";
            this.ipv6Any.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ipv6Any.Size = new System.Drawing.Size(73, 22);
            this.ipv6Any.TabIndex = 2;
            this.ipv6Any.Text = "IPv4 Any";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(119, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 16);
            this.label8.TabIndex = 117;
            this.label8.Text = "Add Server Cert:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // localIPAddresses
            // 
            this.localIPAddresses.Enabled = false;
            this.localIPAddresses.Location = new System.Drawing.Point(184, 19);
            this.localIPAddresses.Name = "localIPAddresses";
            this.localIPAddresses.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.localIPAddresses.Size = new System.Drawing.Size(209, 69);
            this.localIPAddresses.Sorted = true;
            this.localIPAddresses.TabIndex = 5;
            // 
            // sslS
            // 
            this.sslS.Location = new System.Drawing.Point(11, 94);
            this.sslS.Name = "sslS";
            this.sslS.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.sslS.Size = new System.Drawing.Size(75, 24);
            this.sslS.TabIndex = 113;
            this.sslS.Text = "SSL";
            this.sslS.CheckedChanged += new System.EventHandler(this.sslS_CheckedChanged);
            // 
            // currentCertLabelS
            // 
            this.currentCertLabelS.Location = new System.Drawing.Point(8, 121);
            this.currentCertLabelS.Name = "currentCertLabelS";
            this.currentCertLabelS.Size = new System.Drawing.Size(385, 18);
            this.currentCertLabelS.TabIndex = 116;
            this.currentCertLabelS.Text = "Current Cert: ***No Cert Selection***";
            // 
            // addCertFromStoreS
            // 
            this.addCertFromStoreS.Location = new System.Drawing.Point(305, 93);
            this.addCertFromStoreS.Name = "addCertFromStoreS";
            this.addCertFromStoreS.Size = new System.Drawing.Size(88, 23);
            this.addCertFromStoreS.TabIndex = 8;
            this.addCertFromStoreS.Text = "From Store";
            this.addCertFromStoreS.Click += new System.EventHandler(this.addCertFromStoreS_Click);
            // 
            // addCertFromFileS
            // 
            this.addCertFromFileS.Location = new System.Drawing.Point(210, 93);
            this.addCertFromFileS.Name = "addCertFromFileS";
            this.addCertFromFileS.Size = new System.Drawing.Size(88, 23);
            this.addCertFromFileS.TabIndex = 7;
            this.addCertFromFileS.Text = "From File";
            this.addCertFromFileS.Click += new System.EventHandler(this.addCertFromFileS_Click);
            // 
            // localPort
            // 
            this.localPort.Location = new System.Drawing.Point(89, 68);
            this.localPort.Name = "localPort";
            this.localPort.Size = new System.Drawing.Size(65, 20);
            this.localPort.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 68);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 105;
            this.label2.Text = "Port #";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ipv6Allowed);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.sslC);
            this.groupBox2.Controls.Add(this.currentCertLabelC);
            this.groupBox2.Controls.Add(this.addCertFromStoreC);
            this.groupBox2.Controls.Add(this.addCertFromFileC);
            this.groupBox2.Controls.Add(this.remoteIPAddress);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.resolve);
            this.groupBox2.Controls.Add(this.remotePort);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.remoteHostName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.ipv4Allowed);
            this.groupBox2.Location = new System.Drawing.Point(8, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(404, 153);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination End Point";
            // 
            // ipv6Allowed
            // 
            this.ipv6Allowed.Checked = true;
            this.ipv6Allowed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ipv6Allowed.Location = new System.Drawing.Point(330, 69);
            this.ipv6Allowed.Name = "ipv6Allowed";
            this.ipv6Allowed.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ipv6Allowed.Size = new System.Drawing.Size(63, 24);
            this.ipv6Allowed.TabIndex = 14;
            this.ipv6Allowed.Text = "IPv6";
            this.ipv6Allowed.CheckedChanged += new System.EventHandler(this.ipv6Allowed_CheckedChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(119, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 16);
            this.label7.TabIndex = 112;
            this.label7.Text = "Add Client Cert:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sslC
            // 
            this.sslC.Location = new System.Drawing.Point(7, 100);
            this.sslC.Name = "sslC";
            this.sslC.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.sslC.Size = new System.Drawing.Size(75, 24);
            this.sslC.TabIndex = 15;
            this.sslC.Text = "SSL";
            this.sslC.CheckedChanged += new System.EventHandler(this.sslC_CheckedChanged);
            // 
            // currentCertLabelC
            // 
            this.currentCertLabelC.Location = new System.Drawing.Point(8, 127);
            this.currentCertLabelC.Name = "currentCertLabelC";
            this.currentCertLabelC.Size = new System.Drawing.Size(385, 18);
            this.currentCertLabelC.TabIndex = 111;
            this.currentCertLabelC.Text = "Current Cert: ***No Cert Selection***";
            // 
            // addCertFromStoreC
            // 
            this.addCertFromStoreC.Location = new System.Drawing.Point(305, 99);
            this.addCertFromStoreC.Name = "addCertFromStoreC";
            this.addCertFromStoreC.Size = new System.Drawing.Size(88, 23);
            this.addCertFromStoreC.TabIndex = 17;
            this.addCertFromStoreC.Text = "From Store";
            this.addCertFromStoreC.Click += new System.EventHandler(this.addCertFromStoreC_Click);
            // 
            // addCertFromFileC
            // 
            this.addCertFromFileC.Location = new System.Drawing.Point(210, 99);
            this.addCertFromFileC.Name = "addCertFromFileC";
            this.addCertFromFileC.Size = new System.Drawing.Size(88, 23);
            this.addCertFromFileC.TabIndex = 16;
            this.addCertFromFileC.Text = "From File";
            this.addCertFromFileC.Click += new System.EventHandler(this.addCertFromFileC_Click);
            // 
            // remoteIPAddress
            // 
            this.remoteIPAddress.Location = new System.Drawing.Point(89, 44);
            this.remoteIPAddress.Name = "remoteIPAddress";
            this.remoteIPAddress.Size = new System.Drawing.Size(152, 21);
            this.remoteIPAddress.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(7, 46);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(75, 16);
            this.label6.TabIndex = 106;
            this.label6.Text = "IP Address";
            // 
            // remotePort
            // 
            this.remotePort.Location = new System.Drawing.Point(89, 68);
            this.remotePort.Name = "remotePort";
            this.remotePort.Size = new System.Drawing.Size(65, 20);
            this.remotePort.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(7, 72);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(75, 16);
            this.label4.TabIndex = 104;
            this.label4.Text = "Port #";
            // 
            // remoteHostName
            // 
            this.remoteHostName.Location = new System.Drawing.Point(89, 20);
            this.remoteHostName.Name = "remoteHostName";
            this.remoteHostName.Size = new System.Drawing.Size(304, 20);
            this.remoteHostName.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(75, 16);
            this.label3.TabIndex = 103;
            this.label3.Text = "Server Name";
            // 
            // ipv4Allowed
            // 
            this.ipv4Allowed.Checked = true;
            this.ipv4Allowed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ipv4Allowed.Location = new System.Drawing.Point(261, 69);
            this.ipv4Allowed.Name = "ipv4Allowed";
            this.ipv4Allowed.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ipv4Allowed.Size = new System.Drawing.Size(63, 24);
            this.ipv4Allowed.TabIndex = 13;
            this.ipv4Allowed.Text = "IPv4";
            this.ipv4Allowed.CheckedChanged += new System.EventHandler(this.ipv4Allowed_CheckedChanged);
            // 
            // socketProxy
            // 
            this.socketProxy.Location = new System.Drawing.Point(8, 1);
            this.socketProxy.Name = "socketProxy";
            this.socketProxy.Size = new System.Drawing.Size(404, 24);
            this.socketProxy.TabIndex = 1;
            this.socketProxy.Text = "Act as a Socket Proxy (Otherwise will listen on raw IP socket)";
            this.socketProxy.CheckedChanged += new System.EventHandler(this.socketProxy_CheckedChanged);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(103, 341);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(88, 23);
            this.save.TabIndex = 21;
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(8, 341);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(88, 23);
            this.load.TabIndex = 20;
            this.load.Text = "Load";
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // Options
            // 
            this.AcceptButton = this.ok;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(421, 375);
            this.Controls.Add(this.save);
            this.Controls.Add(this.load);
            this.Controls.Add(this.socketProxy);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.Text = "TraceMyNet Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private Button ok;
        private Button cancel;
        private ToolTip toolTip;
        private CheckBox socketProxy;
        private GroupBox groupBox1;
        private TextBox localPort;
        private GroupBox groupBox2;
        private TextBox remotePort;
        private TextBox remoteHostName;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button resolve;
        private ListBox localIPAddresses;
        private Label label6;
        private ComboBox remoteIPAddress;
        private CheckBox sslC;
        private Button addCertFromStoreC;
        private Button addCertFromFileC;
        private Label currentCertLabelC;
        private Label label7;
        private Label label8;
        private CheckBox sslS;
        private Label currentCertLabelS;
        private Button addCertFromStoreS;
        private Button addCertFromFileS;
        private CheckBox ipv4Allowed;
        private CheckBox ipv6Allowed;
        private CheckBox ipv4Any;
        private CheckBox ipv6Any;
        private CheckBox customIPAddresses;
        private Button save;
        private Button load;
    }
}