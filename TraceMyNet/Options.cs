namespace CodePlex.Tools.TraceMyNet
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Windows.Forms;

    public partial class Options : Form
    {
        IPHostEntry remoteHostEntry;
        List<IPAddress> localAddresses;

        public Options()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            UpdateEnabled();
        }

        void Options_Load(object sender, EventArgs e)
        {
            sslC.Checked = TraceMyNet.Configuration.SslC;
            sslS.Checked = TraceMyNet.Configuration.SslS;
            socketProxy.Checked = TraceMyNet.Configuration.SocketProxy;
            localPort.Text = TraceMyNet.Configuration.LocalEndpoints.Length > 1 ? TraceMyNet.Configuration.LocalEndpoints[0].Port.ToString() : "8080";
            remoteHostName.Text = TraceMyNet.Configuration.RemoteHostName;
            remotePort.Text = TraceMyNet.Configuration.RemoteEndpoint.Port.ToString();
            resolve_Click(null, null);
            UpdateEnabled();
        }

        void resolve_Click(object sender, EventArgs e)
        {
            string tip;
            resolve.Enabled = false;
            remoteHostName.Enabled = false;

            if (localAddresses == null)
            {
                localAddresses = new List<IPAddress>();
                try
                {
                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        IPInterfaceProperties properties = ni.GetIPProperties();
                        foreach (UnicastIPAddressInformation dns in properties.UnicastAddresses)
                        {
                            if (!localAddresses.Contains(dns.Address))
                            {
                                localAddresses.Add(dns.Address);
                            }
                        }
                    }
                    Options.SetToolTip(this, localIPAddresses, DumpIPAddresses(localAddresses));
                }
                catch (Exception exception)
                {
                    // if validation fails try other ways to get the local computer name and always fallback
                    // gracefully since this setting can't be changed and won't affect the application.
                    // also, for the same reason, such settings don't need to be validated
                    localAddresses.Add(IPAddress.Any);
                    localAddresses.Add(IPAddress.IPv6Any);
                    Options.SetToolTip(this, localIPAddresses, exception.ToString());
                }
            }
            localIPAddresses.Items.Clear();
            foreach (IPAddress a in localAddresses)
            {
                localIPAddresses.Items.Add(a);
            }
            try
            {
                IPAddress ip;
                if (IPAddress.TryParse(remoteHostName.Text, out ip))
                {
                    this.remoteHostEntry = new IPHostEntry();
                    this.remoteHostEntry.HostName = ip.ToString();
                    this.remoteHostEntry.Aliases = new string[] { ip.ToString() };
                    this.remoteHostEntry.AddressList = new IPAddress[] { ip };
                }
                else
                {
                    this.remoteHostEntry = Dns.GetHostEntry(remoteHostName.Text);
                }
                remoteHostName.Text = this.remoteHostEntry.HostName;
                remoteHostName.BackColor = Color.White;
                tip = DumpIPHostEntry(this.remoteHostEntry);
            }
            catch (Exception exception)
            {
                remoteHostName.Text = "localhost";
                remoteHostName.Text += " (" + exception.Message + ")";
                remoteHostName.BackColor = Color.Gray;
                remoteIPAddress.Items.Add(IPAddress.Loopback);
                remoteIPAddress.Items.Add(IPAddress.IPv6Loopback);
                tip = exception.ToString();
            }

            resolve.Enabled = true;
            remoteHostName.Enabled = true;
            FilterRemote();

            Options.SetToolTip(this, remoteHostName, tip);
            UpdateConfig();
        }

        void UpdateConfig()
        {
            List<IPEndPoint> l = new List<IPEndPoint>();
            if (customIPAddresses.Checked)
            {
                foreach (IPAddress a in localIPAddresses.SelectedItems)
                {
                    l.Add(new IPEndPoint(a, int.Parse(localPort.Text)));
                }
            }
            else
            {
                if (ipv4Any.Checked)
                {
                    l.Add(new IPEndPoint(IPAddress.Any, int.Parse(localPort.Text)));
                }
                if (ipv6Any.Checked)
                {
                    l.Add(new IPEndPoint(IPAddress.IPv6Any, int.Parse(localPort.Text)));
                }
            }
            TraceMyNet.Configuration.LocalEndpoints = l.ToArray();
            TraceMyNet.Configuration.RemoteHostName = remoteHostName.Text;
            if (remoteIPAddress.Items.Count > 0)
            {
                TraceMyNet.Configuration.RemoteEndpoint = new IPEndPoint((IPAddress)remoteIPAddress.SelectedItem, Int32.Parse(remotePort.Text));
            }
            TraceMyNet.Configuration.SocketProxy = socketProxy.Checked;
            TraceMyNet.Configuration.SslC = sslC.Checked;
            TraceMyNet.Configuration.SslS = sslS.Checked;
        }

        void ok_Click(object sender, EventArgs e)
        {
            UpdateConfig();
            Close();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static string DumpIPHostEntry(IPHostEntry ipHostEntry)
        {
            StringBuilder result = new StringBuilder();
            result.Append("HostName:");
            result.Append(ipHostEntry.HostName);
            result.Append("\n");
            for (int i = 0; i < ipHostEntry.Aliases.Length; i++)
            {
                result.Append("Alias[");
                result.Append(i.ToString());
                result.Append("]:");
                result.Append(ipHostEntry.Aliases[i]);
                result.Append("\n");
            }
            for (int i = 0; i < ipHostEntry.AddressList.Length; i++)
            {
                result.Append("Address[");
                result.Append(i.ToString());
                result.Append("]:");
                result.Append(ipHostEntry.AddressList[i].ToString());
                result.Append("\n");
            }
            return result.ToString();
        }

        public static string DumpIPAddresses(IEnumerable<IPAddress> addresses)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            foreach (IPAddress a in addresses)
            {
                result.Append("Endpoint[");
                result.Append(i.ToString());
                result.Append("]:");
                result.Append(a.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }

        delegate void SetToolTipDelegate(Options thisOptions, Control hostName, string tip);

        static readonly SetToolTipDelegate setToolTip = new SetToolTipDelegate(SetToolTipCallback);

        static void SetToolTip(Options thisOptions, Control control, string tip)
        {
            control.BeginInvoke(setToolTip, new object[] { thisOptions, control, tip });
        }

        static void SetToolTipCallback(Options thisOptions, Control hostName, string tip)
        {
            thisOptions.toolTip.SetToolTip(hostName, tip);
        }

        void sslC_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        void sslS_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        void socketProxy_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        void UpdateEnabled()
        {
            localIPAddresses.Enabled = customIPAddresses.Checked;
            ipv4Allowed.Enabled = !customIPAddresses.Checked;
            ipv6Allowed.Enabled = !customIPAddresses.Checked;
            groupBox1.Enabled = socketProxy.Checked;
            localPort.Enabled = socketProxy.Checked;
            groupBox2.Enabled = socketProxy.Checked;
            remotePort.Enabled = socketProxy.Checked;
            remoteHostName.Enabled = socketProxy.Checked;
            label2.Enabled = socketProxy.Checked;
            label3.Enabled = socketProxy.Checked;
            label4.Enabled = socketProxy.Checked;
            resolve.Enabled = socketProxy.Checked;
            label6.Enabled = socketProxy.Checked;
            sslS.Enabled = socketProxy.Checked;
            addCertFromFileS.Enabled = sslS.Enabled && sslS.Checked;
            addCertFromStoreS.Enabled = sslS.Enabled && sslS.Checked;
            currentCertLabelS.Enabled = sslS.Enabled && sslS.Checked;
            sslC.Enabled = socketProxy.Checked;
            addCertFromFileC.Enabled = sslC.Enabled && sslC.Checked;
            addCertFromStoreC.Enabled = sslC.Enabled && sslC.Checked;
            currentCertLabelC.Enabled = sslC.Enabled && sslC.Checked;
        }

        void addCertFromFileC_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "X509 Certificate files (*.cer)|*.cer|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            X509Certificate2 certificate = null;
            string fileName = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
            }
            try
            {
                if (fileName != null && File.Exists(fileName))
                {
                    try
                    {
                        certificate = new X509Certificate2(fileName);
                    }
                    catch (CryptographicException)
                    {
                        PasswordForm pwd = new PasswordForm();
                        if (pwd.ShowDialog() == DialogResult.OK)
                        {
                            certificate = new X509Certificate2(fileName, pwd.textPassword.Text);
                        }
                    }
                    AddCertC(certificate);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Exception caught while creating an X509Certificate.\r\n\tFileName: " + fileName + "\r\n\tException:\r\n" + exception.ToString(), "Error loading CustomHandler");
            }
        }

        void addCertFromStoreC_Click(object sender, EventArgs e)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = store.Certificates;
            DataTable dt = new DataTable("Certificates");
            dt.Columns.Add("FriendlyName");
            dt.Columns.Add("IssuerName");
            dt.Columns.Add("SerialNumber");
            dt.Columns.Add("SubjectName");
            dt.Columns.Add("Version");
            dt.Columns.Add("Archived");
            // dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
            foreach (X509Certificate2 cert in certs)
            {
                dt.Rows.Add(
                    new object[]{
                        cert.FriendlyName.ToString(),
                        cert.IssuerName.Name,
                        cert.SerialNumber,
                        cert.SubjectName.Name,
                        cert.Version.ToString(),
                        cert.Archived.ToString()
                    });
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            DataGrid dg = new DataGrid();
            dg.SetDataBinding(ds, "Certificates");
            Form f = new Form();
            f.Controls.Add(dg);
            dg.Dock = DockStyle.Fill;
            f.ShowDialog();
            X509Certificate2Collection selected = new X509Certificate2Collection();
            for (int i = 0; i < certs.Count; i++)
            {
                if (dg.IsSelected(i))
                {
                    AddCertC(certs[i]);
                }
            }
        }

        void AddCertC(X509Certificate2 cert)
        {
            if (cert == null)
            {
                return;
            }
            if (TraceMyNet.ClientCerts == null)
            {
                TraceMyNet.ClientCerts = new X509CertificateCollection();
            }
            if (!TraceMyNet.ClientCerts.Contains(cert))
            {
                TraceMyNet.ClientCerts.Add(cert);
                TraceMyNet.Configuration.CertsC.Add(cert.Export(X509ContentType.Pfx));
            }
            if (TraceMyNet.ClientCerts.Count == 0)
            {
                currentCertLabelC.Text = "Current Cert: ***No Cert Selection***";
            }
            else if (TraceMyNet.ClientCerts.Count == 1)
            {
                currentCertLabelC.Text = "Current Cert: " + TraceMyNet.ClientCerts[0].Subject;
            }
            else
            {
                currentCertLabelC.Text = "Current Cert: (List of " + TraceMyNet.ClientCerts.Count + ")";
            }
        }

        void addCertFromFileS_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "X509 Certificate files (*.pfx)|*.pfx|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            X509Certificate2 certificate = null;
            string fileName = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
            }
            try
            {
                if (fileName != null && File.Exists(fileName))
                {
                    try
                    {
                        certificate = new X509Certificate2(fileName);
                    }
                    catch (CryptographicException)
                    {
                        PasswordForm pwd = new PasswordForm();
                        if (pwd.ShowDialog() == DialogResult.OK)
                        {
                            certificate = new X509Certificate2(fileName, pwd.textPassword.Text);
                        }
                    }
                    AddCertS(certificate);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Exception caught while creating an X509Certificate.\r\n\tFileName: " + fileName + "\r\n\tException:\r\n" + exception.ToString(), "Error loading CustomHandler");
            }
        }

        void addCertFromStoreS_Click(object sender, EventArgs e)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = store.Certificates;
            DataTable dt = new DataTable("Certificates");
            dt.Columns.Add("FriendlyName");
            dt.Columns.Add("IssuerName");
            dt.Columns.Add("SerialNumber");
            dt.Columns.Add("SubjectName");
            dt.Columns.Add("Version");
            dt.Columns.Add("Archived");
            // dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
            foreach (X509Certificate2 cert in certs)
            {
                dt.Rows.Add(
                    new object[]{
                        cert.FriendlyName.ToString(),
                        cert.IssuerName.Name,
                        cert.SerialNumber,
                        cert.SubjectName.Name,
                        cert.Version.ToString(),
                        cert.Archived.ToString()
                    });
            }
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            DataGrid dg = new DataGrid();
            dg.SetDataBinding(ds, "Certificates");
            Form f = new Form();
            f.Controls.Add(dg);
            dg.Dock = DockStyle.Fill;
            f.ShowDialog();
            X509Certificate2Collection selected = new X509Certificate2Collection();
            for (int i = 0; i < certs.Count; i++)
            {
                if (dg.IsSelected(i))
                {
                    AddCertS(certs[i]);
                }
            }
        }

        void AddCertS(X509Certificate2 cert)
        {
            if (cert == null)
            {
                return;
            }
            TraceMyNet.ServerCert = cert;
            TraceMyNet.Configuration.CertS = cert.Export(X509ContentType.Pfx);
            currentCertLabelS.Text = "Current Cert: " + TraceMyNet.ServerCert.Subject;
        }

        void ipv4Allowed_CheckedChanged(object sender, EventArgs e)
        {
            FilterRemote();
        }

        void ipv6Allowed_CheckedChanged(object sender, EventArgs e)
        {
            FilterRemote();
        }

        void FilterRemote()
        {
            remoteIPAddress.Items.Clear();
            if (remoteHostEntry != null)
            {
                foreach (IPAddress a in remoteHostEntry.AddressList)
                {
                    if (ipv4Allowed.CheckState == CheckState.Checked && a.AddressFamily == AddressFamily.InterNetwork ||
                        ipv6Allowed.CheckState == CheckState.Checked && a.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIPAddress.Items.Add(a);
                    }
                }
            }
            if (remoteIPAddress.Items.Count > 0)
            {
                remoteIPAddress.SelectedIndex = 0;
            }
            else
            {
                remoteIPAddress.Text = null;
            }
        }

        void customIPAddresses_CheckedChanged(object sender, EventArgs e)
        {
            localIPAddresses.Enabled = customIPAddresses.Checked;
            ipv4Any.Enabled = !customIPAddresses.Checked;
            ipv6Any.Enabled = !customIPAddresses.Checked;
        }

        void load_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Xml config file (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader r = new StreamReader(dialog.FileName))
                {
                    string data = r.ReadToEnd();
                    TraceMyNet.Configuration = ConfigurationData.Parse(data);
                    Options_Load(null, null);
                    UpdateConfig();
                }
            }
        }

        void save_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Xml config file (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                UpdateConfig();
                string data = ConfigurationData.ToString(TraceMyNet.Configuration);
                using (StreamWriter w = new StreamWriter(dialog.FileName))
                {
                    w.Write(data);
                }
            }
        }
    }
}
