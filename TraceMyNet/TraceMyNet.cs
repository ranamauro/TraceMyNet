namespace CodePlex.Tools.TraceMyNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using Microsoft.Win32;
    using Ranamauro.Net;

    public partial class TraceMyNet : Form, IPacketStreamReceiver
    {
        internal static TraceMyNet MainForm;
        internal static ConfigurationData Configuration;
        internal static IList<Socket> Accepters = new List<Socket>();
        internal static CapFile.PacketSniffer Sniffer;
        internal static string RemoteHostName;
        internal static IPEndPoint[] LocalEndPoints;
        internal static IPEndPoint RemoteEndPoint;
        internal static bool NoUi = false;
        internal static X509CertificateCollection ClientCerts;
        internal static X509Certificate2 ServerCert;
        internal static string CurrentFileName;

        public static bool Debug = true;
        public static TextWriter LogFile = new DebugTextWriter();

        public class DebugTextWriter : StreamWriter
        {
            public DebugTextWriter()
                : base(new DebugOutStream(), Encoding.Unicode, 1024)
            {
                this.AutoFlush = true;
            }

            class DebugOutStream : Stream
            {
                public override void Write(byte[] buffer, int offset, int count)
                {
                    System.Diagnostics.Debug.Write(Encoding.Unicode.GetString(buffer, offset, count));
                }

                public override bool CanRead { get { return false; } }
                public override bool CanSeek { get { return false; } }
                public override bool CanWrite { get { return true; } }
                public override void Flush() { System.Diagnostics.Debug.Flush(); }
                public override long Length { get { throw new InvalidOperationException(); } }
                public override int Read(byte[] buffer, int offset, int count) { throw new InvalidOperationException(); }
                public override long Seek(long offset, SeekOrigin origin) { throw new InvalidOperationException(); }
                public override void SetLength(long value) { throw new InvalidOperationException(); }
                public override long Position
                {
                    get { throw new InvalidOperationException(); }
                    set { throw new InvalidOperationException(); }
                }
            };
        }

        AsyncCallback StaticAcceptCallback, StaticAcceptedReceiveCallback, StaticConnectedReceiveCallback;

        public TraceMyNet()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            Init();
        }

        public TraceMyNet(object dummy)
        {
            Init();

            TraceMyNet_Load(null, null);
        }

        public void Init()
        {
            Name = Constants.Name;
            Text = Constants.ProductName;
            StaticAcceptCallback = new AsyncCallback(AcceptCallback);
            StaticAcceptedReceiveCallback = new AsyncCallback(AcceptedReceiveCallback);
            StaticConnectedReceiveCallback = new AsyncCallback(ConnectedReceiveCallback);
            MainForm = this;
            if (!string.IsNullOrEmpty(TraceMyNet.CurrentFileName))
            {
                menuSave.Enabled = true;
            }
        }

        void menuQuit_Click(object sender, EventArgs e)
        {
            if (LogFile != null)
            {
                LogFile.Close();
            }
            Close();
        }

        void menuHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "TraceMyNet .NET is a Socket proxy.\r\nAll TCP traffic directed to the configured local endpoint,\r\nwill be redirected to the configured remote endpoint.", "TraceMyNet .NET  Help");
        }

        void menuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "TraceMyNet .NET\r\n\r\nVersion 2.0.", "TraceMyNet .NET About");
        }

        void menuOptions_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.ShowDialog();

            ConfigurationData.Save(Configuration);
            UpdateConfigurationData();

            if (Started)
            {
                // restart if it was started
                Started = false;
                StartCapturing();
            }
        }

        void RefreshContent()
        {
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("TraceMyNet#" + GetHashCode() + "::RefreshContent(): entering");
            connectionList.BeginUpdate();
            ConnectionNode connectionNode = null;
            for (int i = 0; i < connectionList.Nodes.Count; i++)
            {
                connectionNode = connectionList.Nodes[i] as ConnectionNode;
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("TraceMyNet#" + GetHashCode() + "::RefreshContent(): calling SetText() on connectionNode#" + connectionNode.GetHashCode());
                connectionNode.SetText();
                BufferNode bufferNode = null;
                for (int j = 0; j < connectionNode.Nodes.Count; j++)
                {
                    bufferNode = connectionNode.Nodes[j] as BufferNode;
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("TraceMyNet#" + GetHashCode() + "::RefreshContent(): calling SetText() on bufferNode#" + bufferNode.GetHashCode());
                    bufferNode.SetText();
                }
            }
            if (connectionList.SelectedNode == null)
            {
                transferredData.Text = string.Empty;
            }
            else
            {
                connectionList_AfterSelect(null, new TreeViewEventArgs(connectionList.SelectedNode));
            }
            connectionList.EndUpdate();
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("TraceMyNet#" + GetHashCode() + "::RefreshContent(): done");
        }

        void menuStartCapture_Click(object sender, EventArgs e)
        {
            StartCapturing();
        }

        void menuStopCapture_Click(object sender, EventArgs e)
        {
            StopCapturing();
        }

        void menuRelativeTime_Click(object sender, EventArgs e)
        {
            TraceMyNet.Configuration.RelativeTime = !TraceMyNet.Configuration.RelativeTime;
            menuRelativeTime.Checked = TraceMyNet.Configuration.RelativeTime;
            RefreshContent();
        }

        void menuBinary_Click(object sender, EventArgs e)
        {
            TraceMyNet.Configuration.BinaryFormat = !TraceMyNet.Configuration.BinaryFormat;
            menuBinary.Checked = TraceMyNet.Configuration.BinaryFormat;
            RefreshContent();
        }

        void menuWordWrap_Click(object sender, EventArgs e)
        {
            TraceMyNet.Configuration.WordWrap = !TraceMyNet.Configuration.WordWrap;
            menuWordWrap.Checked = TraceMyNet.Configuration.WordWrap;
            transferredData.WordWrap = TraceMyNet.Configuration.WordWrap;
            RefreshContent();
        }

        void menuStatusBar_Click(object sender, EventArgs e)
        {
            TraceMyNet.Configuration.ShowStatusBar = !TraceMyNet.Configuration.ShowStatusBar;
            menuStatusBar.Checked = TraceMyNet.Configuration.ShowStatusBar;
            ShowStatusBar();
        }

        void menuShowHide_Click(object sender, EventArgs e)
        {
            TraceMyNet.Configuration.ShowData = !TraceMyNet.Configuration.ShowData;
            menuShowData.Checked = TraceMyNet.Configuration.ShowStatusBar;
            ShowData();
        }

        void menuOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            // dialog.InitialDirectory = "c:\\";
            dialog.Filter = "Netmon cap files (*.cap)|*.cap|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CapFile.ReadFromFile(this, dialog.FileName);
            }
        }

        void menuSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            // dialog.InitialDirectory = "c:\\";
            // support for cap is pretty hard to implement, I might never get to it....
            // dialog.Filter = "Netmon cap file (*.cap)|*.cap|Text file (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CapFile.WriteToFile(connectionList, dialog.FileName);
                CurrentFileName = dialog.FileName;
                menuSave.Enabled = true;
            }
        }

        void connectionProperties_Click(object sender, EventArgs e)
        {
            ConnectionNode connectionNode = null;
            TreeNode node = connectionList.SelectedNode;
            if (node is BufferNode)
            {
                connectionNode = node.Parent as ConnectionNode;
            }
            else if (node is ConnectionNode)
            {
                connectionNode = node as ConnectionNode;
            }

            if (connectionNode != null)
            {
                MessageBox.Show(this, connectionNode.DnsInfo, "TraceMyNet .NET Connection Properties");
            }
        }

        public void UpdateConfigurationData()
        {
            LocalEndPoints = TraceMyNet.Configuration.LocalEndpoints;
            RemoteHostName = TraceMyNet.Configuration.RemoteHostName;
            RemoteEndPoint = TraceMyNet.Configuration.RemoteEndpoint;
            if (TraceMyNet.Configuration.CertS != null)
            {
                ServerCert = new X509Certificate2(TraceMyNet.Configuration.CertS);
            }
            if (TraceMyNet.Configuration.CertsC != null && TraceMyNet.Configuration.CertsC.Count > 0)
            {
                ClientCerts = new X509CertificateCollection();
                foreach (byte[] certC in TraceMyNet.Configuration.CertsC)
                {
                    ClientCerts.Add(new X509Certificate2(certC));
                }
            }
        }

        void TraceMyNet_Load(object sender, EventArgs e)
        {
            if (Configuration == null)
            {
                Configuration = ConfigurationData.Load();
            }
            if (!TraceMyNet.NoUi)
            {
                MainForm.Size = TraceMyNet.Configuration.FormSize;
                MainForm.Location = TraceMyNet.Configuration.FormLocation;
                transferredData.Font = new Font(TraceMyNet.Configuration.FontFamilyName, TraceMyNet.Configuration.FontSize, TraceMyNet.Configuration.FontStyle);
                MainForm.ShowStatusBar();
                MainForm.ShowData();
                menuBinary.Checked = TraceMyNet.Configuration.BinaryFormat;
                menuRelativeTime.Checked = TraceMyNet.Configuration.RelativeTime;
                menuWordWrap.Checked = TraceMyNet.Configuration.WordWrap;
                menuStatusBar.Checked = TraceMyNet.Configuration.ShowStatusBar;
                menuShowData.Checked = TraceMyNet.Configuration.ShowStatusBar;
                transferredData.WordWrap = TraceMyNet.Configuration.WordWrap;
            }
            UpdateConfigurationData();
            UpdateCustomHandler(false);
            if (CurrentFileName != null)
            {
                CapFile.ReadFromFile(this, TraceMyNet.CurrentFileName);
            }
            Started = false;
            StartCapturing();
        }

        void UpdateCustomHandler(bool withUi)
        {
            if (TraceMyNet.Configuration.CustomHandlerFile == null || !File.Exists(TraceMyNet.Configuration.CustomHandlerFile))
            {
                goto useDefault;
            }
            ArrayList handlers = new ArrayList();
            handlers.Add(DefaultHandler.Instance);
            Type[] types = null;
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(TraceMyNet.Configuration.CustomHandlerFile);
                types = assembly.GetTypes();
            }
            catch (Exception exception)
            {
                if (withUi)
                {
                    MessageBox.Show(this, "Exception caught while searching for ICustomHandler in this Assembly:\r\n\r\n" + exception.ToString(), "Error loading CustomHandler");
                }
                goto useDefault;
            }
            // now look for a type that implements ICustomHandler
            foreach (Type type in types)
            {
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine(assembly.FullName + ": " + type.Name);
                if (type.Equals(typeof(ICustomHandler)) || type.Equals(typeof(DefaultHandler)))
                {
                    continue;
                }
                if (type.IsInterface || type.IsAbstract)
                {
                    continue;
                }
                if (!typeof(ICustomHandler).IsAssignableFrom(type))
                {
                    continue;
                }
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("FOUND: " + type.FullName);
                try
                {
                    ICustomHandler customHandler = Activator.CreateInstance(type) as ICustomHandler;
                    handlers.Add(customHandler);
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("handlers: " + handlers.Count);
                }
                catch (Exception exception)
                {
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("CAUGHT: " + exception);
                }
                if (TraceMyNet.Debug) TraceMyNet.LogFile.Flush();
            }
            if (handlers.Count == 1)
            {
                if (withUi)
                {
                    MessageBox.Show(this, "No ICustomHandler found in this Assembly, will use the default one.", "CustomHandler Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                /*
                PickHandler ph = new PickHandler();
                bool first = true;
                foreach (object o in handlers)
                {
                    string text = o.GetType().FullName;
                    ph.ComboBox.Items.Add(text);
                    if (first)
                    {
                        first = false;
                        ph.ComboBox.SelectedIndex = 0;
                        ph.ComboBox.Text = text;
                    }
                }
                DialogResult dr = ph.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    ConnectionNode.CustomHandler = (ICustomHandler)handlers[ph.ComboBox.SelectedIndex];
                    return;
                }
                */
            }
        useDefault:
            TraceMyNet.Configuration.CustomHandlerFile = null;
            ConnectionNode.CustomHandler = DefaultHandler.Instance;
        }

        void TraceMyNet_Closing(object sender, CancelEventArgs e)
        {
            TraceMyNet.Configuration.FormSize = MainForm.Size;
            TraceMyNet.Configuration.FormLocation = MainForm.Location;
            TraceMyNet.Configuration.FontFamilyName = transferredData.Font.FontFamily.Name;
            TraceMyNet.Configuration.FontSize = transferredData.Font.Size;
            TraceMyNet.Configuration.FontStyle = transferredData.Font.Style;
            TraceMyNet.Configuration.WordWrap = menuWordWrap.Checked;
            ConfigurationData.Save(Configuration);
        }

        internal bool Started
        {
            get
            {
                return transferredData.Visible;
            }
            set
            {
                transferredData.Visible = value;
                connectionList.Visible = value;
                connectionList.HideSelection = false;
                if (value)
                {
                    if (TraceMyNet.Configuration.SocketProxy)
                    {
                        sbpAppStatus.Text = "Status: Listening";
                        if (LocalEndPoints != null && LocalEndPoints.Length > 0)
                        {
                            if (LocalEndPoints.Length == 1)
                            {
                                sbpListeningPort.Text = "Local: " + LocalEndPoints[0];
                            }
                            else
                            {
                                sbpListeningPort.Text = "Local Port: " + LocalEndPoints[0].Port;
                            }
                        }
                        else
                        {
                            sbpListeningPort.Text = "Local: none";
                        }
                        sbpDestinationPort.Text = "Destination: " + RemoteHostName + ":" + RemoteEndPoint;
                    }
                    else
                    {
                        sbpAppStatus.Text = "Status: Listening on all ports all destinations";
                        sbpListeningPort.Text = "Local Port: *";
                        sbpDestinationPort.Text = "Destination: *";
                    }
                    menuStartCapture.Enabled = false;
                    menuStopCapture.Enabled = true;
                }
                else
                {
                    sbpAppStatus.Text = "Status: Inactive";
                    menuStartCapture.Enabled = true;
                    menuStopCapture.Enabled = false;
                }
            }
        }

        void StopCapturing()
        {
            if (!Started)
            {
                return;
            }
            CloseSockets();
            ConnectionNode connectionNode = null;
            for (int i = 0; i < connectionList.Nodes.Count; i++)
            {
                connectionNode = connectionList.Nodes[i] as ConnectionNode;
                connectionNode.Cleanup(false);
            }
            Started = false;
        }

        void StartCapturing()
        {
            if (Started)
            {
                return;
            }
            CloseSockets();
            if (TraceMyNet.Configuration.SocketProxy)
            {
                Started = false;
                if (LocalEndPoints == null || LocalEndPoints.Length == 0 || RemoteEndPoint == null)
                {
                    MessageBox.Show("Selected options are invalid", "Invalid Options", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    foreach (IPEndPoint ep in LocalEndPoints)
                    {
                        Socket accepter = null;
                        try
                        {
                            if (!Socket.OSSupportsIPv6 && ep.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                continue;
                            }
                            accepter = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            accepter.Bind(ep);
                            accepter.Listen((int)SocketOptionName.MaxConnections);
                            accepter.BeginAccept(StaticAcceptCallback, accepter);
                            Accepters.Add(accepter);
                            Started = true;
                        }
                        catch (Exception exception)
                        {
                            if (accepter != null)
                            {
                                try
                                {
                                    accepter.Close();
                                }
                                catch { }
                            }
                            MessageBox.Show("Caught the following Exception tryng to Bind the Socket to: " + ep.ToString() + "\r\n\r\n" + exception.Message, "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            else
            {
                Sniffer = new CapFile.PacketSniffer(this);
                Sniffer.Start();

                Started = true;
            }
            RefreshContent();
        }

        static void CloseSockets()
        {
            if (Accepters != null)
            {
                foreach (Socket accepter in Accepters)
                {
                    try
                    {
                        accepter.Close();
                    }
                    catch
                    {
                    }
                }
                Accepters.Clear();
            }
            if (Sniffer != null)
            {
                try
                {
                    Sniffer.Stop();
                }
                catch
                {
                }
                Sniffer = null;
            }
        }

        void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket accepter = asyncResult.AsyncState as Socket;
            Socket connected = null;
            ConnectionNode node = null;
            Exception caughtException = null;
            Socket accepted = null;

            try
            {
                try
                {
                    accepted = accepter.EndAccept(asyncResult);
                    accepter.BeginAccept(StaticAcceptCallback, accepter);
                }
                catch (ObjectDisposedException ode)
                {
                    caughtException = ode;
                }

                try
                {
                    connected = new Socket(RemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    connected.Connect(RemoteEndPoint);
                }
                catch (Exception exception)
                {
                    connected.Close();
                    connected = null;
                    caughtException = exception;
                }

                node = new ConnectionNode(accepted, connected);
                AddConnectionNode(node);

                if (accepted != null && connected != null)
                {
                    node.accepted.BeginRead(node.acceptedBufferNode, 0, node.acceptedBufferNode.Length, StaticAcceptedReceiveCallback, node);
                    node.connected.BeginRead(node.connectedBufferNode, 0, node.connectedBufferNode.Length, StaticConnectedReceiveCallback, node);
                    return;
                }
            }
            catch (Exception exception)
            {
                caughtException = exception;
            }

            if (accepted != null)
            {
                accepted.Close();
            }
            if (connected != null)
            {
                connected.Close();
            }
            if (node != null)
            {
                node.Cleanup(caughtException != null);
            }
        }

        void AcceptedReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionNode node = asyncResult.AsyncState as ConnectionNode;
            Exception caughtException = null;
            int bytesTransferred;

            try
            {
                bytesTransferred = node.accepted.EndRead(asyncResult);
                string s = Encoding.ASCII.GetString(node.acceptedBufferNode, 0, bytesTransferred);
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("DATA:: acceptedBufferNode(" + bytesTransferred + "): " + s);
                Add(node, DataDirection.Sent, node.acceptedBufferNode, bytesTransferred, null);
                if (bytesTransferred > 0)
                {
                    try
                    {
                        node.connected.Write(node.acceptedBufferNode, 0, bytesTransferred);
                        node.accepted.BeginRead(node.acceptedBufferNode, 0, node.acceptedBufferNode.Length, StaticAcceptedReceiveCallback, node);
                        return;
                    }
                    catch (Exception exception)
                    {
                        // ignore this exception
                        if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("acceptedBufferNode(" + bytesTransferred + "): " + exception);
                    }
                }
            }
            catch (Exception exception)
            {
                caughtException = exception as SocketException;
                if (caughtException != null)
                {
                    Add(node, DataDirection.Sent, null, 0, caughtException);
                }
            }

            if (node != null)
            {
                node.Cleanup(caughtException != null);
            }
        }

        void ConnectedReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionNode node = asyncResult.AsyncState as ConnectionNode;
            Exception caughtException = null;
            int bytesTransferred;

            try
            {
                bytesTransferred = node.connected.EndRead(asyncResult);
                string s = Encoding.ASCII.GetString(node.connectedBufferNode, 0, bytesTransferred);
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("DATA:: connectedBufferNode(" + bytesTransferred + "): " + s);
                Add(node, DataDirection.Received, node.connectedBufferNode, bytesTransferred, null);
                if (bytesTransferred > 0)
                {
                    try
                    {
                        node.accepted.Write(node.connectedBufferNode, 0, bytesTransferred);
                        node.connected.BeginRead(node.connectedBufferNode, 0, node.connectedBufferNode.Length, StaticConnectedReceiveCallback, node);
                        return;
                    }
                    catch (Exception exception)
                    {
                        // ignore this exception
                        if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("connectedBufferNode(" + bytesTransferred + "): " + exception);
                    }
                }
            }
            catch (Exception exception)
            {
                caughtException = exception as SocketException;
                if (caughtException != null)
                {
                    Add(node, DataDirection.Received, null, 0, caughtException);
                }
            }

            if (node != null)
            {
                node.Cleanup(caughtException != null);
            }
        }

        void connectionList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e != null)
            {
                if (e.Node is ConnectionNode)
                {
                    ((ConnectionNode)e.Node).FormatData(transferredData);
                }
                else if (e.Node is BufferNode)
                {
                    ((BufferNode)e.Node).FormatData(transferredData);
                }
                else if (e.Node != null)
                {
                    transferredData.Text = e.Node.GetType().ToString();
                }
            }
        }

        // adds a child node to this one
        public void Add(ConnectionNode father, DataDirection direction, byte[] origin, int length, Exception exception)
        {
            BufferNode bufferNode = new BufferNode(father, direction, origin, length, exception);
            AddBufferNode(bufferNode);
        }

        public void Add(ConnectionNode father, DataDirection direction, byte[] origin, int length, int frameNumber, Exception exception, DateTime timeStamp)
        {
            BufferNode bufferNode = new BufferNode(father, direction, origin, length, frameNumber, exception, timeStamp);
            AddBufferNode(bufferNode);
        }

        delegate void AddConnectionDelegate(ConnectionNode node, TreeView connectionList);
        delegate void AddBufferDelegate(BufferNode node, TreeView connectionList);

        static readonly AddConnectionDelegate addConnection = new AddConnectionDelegate(AddConnection);
        static readonly AddBufferDelegate addBuffer = new AddBufferDelegate(AddBuffer);

        // adds itself to the main TreeView list
        public void AddConnectionNode(ConnectionNode node)
        {
            connectionList.BeginInvoke(addConnection, new object[] { node, connectionList });
        }

        // adds itself to the main TreeView list
        void AddBufferNode(BufferNode node)
        {
            connectionList.BeginInvoke(addBuffer, new object[] { node, connectionList });
        }

        static void AddBuffer(BufferNode node, TreeView connectionList)
        {
            connectionList.BeginUpdate();
            // can't really call connectionList.EndInvoke() 'cause I don't know where my IAsyncResult is.
            // the only thing I might be missing, though, is the exception on failure. 
            node.father.Nodes.Add(node);
            connectionList.EndUpdate();
        }

        static void AddConnection(ConnectionNode node, TreeView connectionList)
        {
            // can't really call connectionList.EndInvoke() 'cause I don't know where my IAsyncResult is.
            // the only thing I might be missing, though, is the exception on failure. 
            connectionList.Nodes.Add(node);
            connectionList.EndUpdate();
        }

        void menuCopy_Click(object sender, EventArgs e)
        {
            if (transferredData.SelectionLength > 0)
            {
                // Copy the selected text to the Clipboard.
                transferredData.Copy();
            }
        }

        void menuTransferredDataCopy_Click(object sender, EventArgs e)
        {
            if (transferredData.SelectionLength > 0)
            {
                // Copy the selected text to the Clipboard.
                transferredData.Copy();
            }
        }

        void menuTransferredDataSelectAll_Click(object sender, EventArgs e)
        {
            transferredData.SelectAll();
        }

        void transferredDataMenu_Popup(object sender, EventArgs e)
        {
            menuTransferredDataCopy.Enabled = (transferredData.SelectedText.Length > 0);
        }

        void menuFont_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = transferredData.Font;
            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                transferredData.Font = fontDialog1.Font;
            }
        }

        void menuDefaultHandler_Click(object sender, EventArgs e)
        {
            TraceMyNet.Configuration.CustomHandlerFile = null;
            ConnectionNode.CustomHandler = DefaultHandler.Instance;
            ConfigurationData.Save(Configuration);
        }
        void menuCustomHandler_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            // add support for loading and compiling source code on the fly
            dialog.Filter = "Custom Handler Assembly (*.dll; *.exe)|*.dll; *.exe|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TraceMyNet.Configuration.CustomHandlerFile = dialog.FileName;
                UpdateCustomHandler(true);
                ConfigurationData.Save(Configuration);
            }
        }
        void menuClearConnections_Selected_Click(object sender, EventArgs e)
        {
            connectionList.BeginUpdate();
            ConnectionNode connectionNode = connectionList.SelectedNode as ConnectionNode;
            if (connectionNode != null)
            {
                connectionNode.Cleanup(false);
                connectionNode.Remove();
            }
            connectionList.EndUpdate();
            RefreshContent();
        }

        void menuClearConnections_Inactive_Click(object sender, EventArgs e)
        {
            connectionList.BeginUpdate();
            bool modified;
            do
            {
                modified = false;
                ConnectionNode connectionNode = null;
                for (int i = 0; i < connectionList.Nodes.Count; i++)
                {
                    connectionNode = connectionList.Nodes[i] as ConnectionNode;
                    if (connectionNode.status != ConnectionStatus.Open)
                    {
                        connectionNode.Cleanup(false);
                        connectionNode.Remove();
                        modified = true;
                        break;
                    }
                }
            } while (modified);
            connectionList.EndUpdate();
            RefreshContent();
        }

        void menuClearConnections_All_Click(object sender, EventArgs e)
        {
            connectionList.BeginUpdate();
            while (connectionList.Nodes.Count > 0)
            {
                ConnectionNode connectionNode = connectionList.Nodes[connectionList.Nodes.Count - 1] as ConnectionNode;
                connectionNode.Cleanup(false);
                connectionNode.Remove();
            }
            connectionList.EndUpdate();
            RefreshContent();
        }

        void menuFind_Click(object sender, EventArgs e)
        {
            FindForm findForm = new FindForm(ref transferredData);
            findForm.Show();
        }

        void menuSelectAll_Click(object sender, EventArgs e)
        {
            transferredData.SelectAll();
        }

        void ShowStatusBar()
        {
            menuStatusBar.Checked = TraceMyNet.Configuration.ShowStatusBar;
            statusBar.Visible = TraceMyNet.Configuration.ShowStatusBar;
        }

        void ShowData()
        {
            menuShowData.Checked = TraceMyNet.Configuration.ShowData;
            splitter.Visible = TraceMyNet.Configuration.ShowData;
            panelRight.Visible = TraceMyNet.Configuration.ShowData;
            panelLeft.Dock = TraceMyNet.Configuration.ShowData ? DockStyle.Left : DockStyle.Fill;
        }

        Dictionary<TcpPairKey, ConnectionNode> connections = new Dictionary<TcpPairKey, ConnectionNode>();

        public void AddConversation(TcpPairKey tcpPairKey, DateTime timeStamp)
        {
            ConnectionNode connectionNode = new ConnectionNode(tcpPairKey.Source, tcpPairKey.Destination, timeStamp);
            connections[tcpPairKey] = connectionNode;
            this.AddConnectionNode(connectionNode);
        }

        public void MoveToClosedConversation(TcpPairKey tcpPairKey)
        {
        }

        public void AddData(TcpPairKey tcpPairKey, DataDirection dataDirection, byte[] buffer, int length, int packetsReceived, Exception exception, DateTime timeStamp)
        {
            ConnectionNode connectionNode = connections[tcpPairKey];
            this.Add(connectionNode, dataDirection, buffer, length, packetsReceived, exception, timeStamp);
        }
    }

    public enum ConnectionStatus
    {
        Open,
        Closed,
        Interrupted,
    }

    public struct Constants
    {
        public const string ProductName = "TraceMyNet .NET";
        public const string Name = "TraceMyNet";
        public const string EmailContact = "mauroot";
        public const string SupportUri = "http://toolbox/default.aspx?Page=details/details.aspx?ToolID=21650";
        public static Version ApplicationVersion = typeof(Constants).Assembly.GetName().Version;
        public static string MailToUri = "mailto:" + EmailContact + "?Subject=TraceMyNet .NET feedback (tool version = " + ApplicationVersion + ", runtime = " + Environment.Version + ")";
    }

    [Serializable]
    public class ConfigurationData
    {
        public string[] LocalEndpointsAddress;
        public int[] LocalEndpointsPort;
        public string RemoteEndpointAddress;
        public int RemoteEndpointPort;

        public string RemoteHostName;
        public Size FormSize;
        public Point FormLocation;
        public bool RelativeTime;
        public bool BinaryFormat;
        public bool WordWrap;
        public bool ShowStatusBar;
        public bool ShowData;
        public byte[] CertS;
        public List<byte[]> CertsC;
        public bool SslS;
        public bool SslC;
        public bool SocketProxy;
        public bool Saved;
        public string FontFamilyName;
        public float FontSize;
        public FontStyle FontStyle;
        public string CustomHandlerFile; // Must implement ICustomHandler and have default constructor

        internal IPEndPoint[] LocalEndpoints
        {
            get
            {
                List<IPEndPoint> list = new List<IPEndPoint>();
                for (int index = 0; index < LocalEndpointsAddress.Length; index++)
                {
                    list.Add(new IPEndPoint(IPAddress.Parse(this.LocalEndpointsAddress[index]), this.LocalEndpointsPort[index]));
                }
                return list.ToArray();
            }
            set
            {
                this.LocalEndpointsAddress = new string[value.Length];
                this.LocalEndpointsPort = new int[value.Length];
                for (int index = 0; index < LocalEndpointsAddress.Length; index++)
                {
                    this.LocalEndpointsAddress[index] = value[index].Address.ToString();
                    this.LocalEndpointsPort[index] = value[index].Port;
                }
            }
        }

        internal IPEndPoint RemoteEndpoint
        {
            get
            {
                return new IPEndPoint(IPAddress.Parse(this.RemoteEndpointAddress), this.RemoteEndpointPort);
            }
            set
            {
                this.RemoteEndpointAddress = value.Address.ToString();
                this.RemoteEndpointPort = value.Port;
            }
        }

        const string RegKeyPath = @"SOFTWARE\Microsoft\.NETFramework\Samples\TraceMyNet";

        public static void Save(ConfigurationData configurationData)
        {
            string blob = ToString(configurationData);
            // save options to the registry
            using (RegistryKey settings = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                settings.SetValue("Blob", blob);
            }
        }

        public static string ToString(ConfigurationData configurationData)
        {
            configurationData.Saved = true;
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationData));
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, configurationData);
            memoryStream.Seek(0, SeekOrigin.Begin);
            byte[] blobData = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(blobData);
        }

        public static ConfigurationData Load()
        {
            string blobString = null;
            using (RegistryKey settings = Registry.CurrentUser.CreateSubKey(RegKeyPath))
            {
                blobString = settings.GetValue("Blob") as string;
            }
            return Parse(blobString);
        }

        public static ConfigurationData Parse(string blobString)
        {
            ConfigurationData configurationData = null;

            try
            {
                if (blobString != null)
                {
                    byte[] blobData = Encoding.UTF8.GetBytes(blobString);
                    using (MemoryStream memoryStream = new MemoryStream(blobData))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationData));
                        configurationData = serializer.Deserialize(memoryStream) as ConfigurationData;
                    }
                }
            }
            catch
            {
            }

            // make sure this instance wasn't just crafted
            if (configurationData == null || !configurationData.Saved)
            {
                // make sure your registry is clean
                try
                {
                    using (RegistryKey settings = Registry.CurrentUser.CreateSubKey(RegKeyPath))
                    {
                        settings.DeleteValue("Blob", false);
                    }
                }
                catch
                {
                }

                // create default config
                configurationData = new ConfigurationData();
                if (Socket.OSSupportsIPv6)
                {
                    configurationData.LocalEndpoints = new IPEndPoint[]
                    {
                        new IPEndPoint(IPAddress.Any, 8080),
                        new IPEndPoint(IPAddress.IPv6Any, 8080)
                    };
                }
                else
                {
                    configurationData.LocalEndpoints = new IPEndPoint[]
                    {
                        new IPEndPoint(IPAddress.Any, 8080)
                    };
                }
                configurationData.RemoteHostName = Dns.GetHostName();
                configurationData.RemoteEndpoint = new IPEndPoint(IPAddress.Any, 80);

                configurationData.BinaryFormat = false;
                configurationData.RelativeTime = true;
                configurationData.FormSize.Height = 480;
                configurationData.FormSize.Width = 640;
                configurationData.FormLocation.X = 10;
                configurationData.FormLocation.Y = 10;
                configurationData.WordWrap = true;
                configurationData.ShowStatusBar = true;
                configurationData.ShowData = true;
                configurationData.CertS = null;
                configurationData.CertsC = null;
                configurationData.SslS = false;
                configurationData.SslC = false;
                configurationData.SocketProxy = true;
                configurationData.FontFamilyName = FontFamily.GenericSansSerif.Name;
                configurationData.FontSize = 8;
                configurationData.FontStyle = FontStyle.Regular;
            }
            else if (configurationData.FontFamilyName == null || configurationData.FontFamilyName.Length == 0)
            {
                configurationData.FontFamilyName = FontFamily.GenericSansSerif.Name;
                configurationData.FontSize = 8;
            }

            return configurationData;
        }
    }

    public interface ICustomHandler
    {
        string FormatLabel(IDataPacket dataPacket);
        void FormatData(IDataPacket dataPacket, RichTextBox richTextBox);
        void FormatData(IDataPacket[] dataPackets, RichTextBox richTextBox);
    }

    public interface ICustomHandlerTextBinary : ICustomHandler
    {
        string FormatLabel(IDataPacket dataPacket, out Color color);
        void FormatDataAsText(IDataPacket[] dataPackets, RichTextBox richTextBox);
        void FormatDataAsBinary(IDataPacket[] dataPackets, RichTextBox richTextBox);
        void FormatDataAsText(IDataPacket dataPacket, RichTextBox richTextBox);
        void FormatDataAsBinary(IDataPacket dataPacket, RichTextBox richTextBox);
    }

    public interface IDataPacket
    {
        byte[] Data { get; set; }
        DateTime TimeStamp { get; set; }
        DateTime RootTimeStamp { get; set; }
        DataDirection Direction { get; set; }
        int FrameNumber { get; set; }
        Exception Exception { get; set; }
    }

    public class DefaultHandler : ICustomHandlerTextBinary
    {
        public static readonly ICustomHandler Instance = new DefaultHandler();

        // ISO-8859-1 (aka 28591) maps U+0080-U+00ff to 0x80-0xff.
        static readonly Encoding Iso8859 = Encoding.GetEncoding(28591);

        public string FormatLabel(IDataPacket dataPacket)
        {
            Color color;
            return FormatLabel(dataPacket, out color);
        }

        public string FormatLabel(IDataPacket dataPacket, out Color color)
        {
            StringBuilder stringBuilder = new StringBuilder();
            color = dataPacket.Direction == DataDirection.Received ? Color.Purple : Color.Blue;
            if (dataPacket.Exception != null)
            {
                SocketException socketException = dataPacket.Exception as SocketException;
                if (socketException != null && socketException.NativeErrorCode == 10054)
                {
                    stringBuilder.Append("RST");
                    color = Color.FromArgb(255, 0, 0);
                }
                else
                {
                    stringBuilder.Append("EXC");
                    color = Color.FromArgb(100, 200, 0);
                }
            }
            else if (dataPacket.Data == null || dataPacket.Data.Length == 0)
            {
                stringBuilder.Append("SHT");
                color = Color.FromArgb(200, 100, 0);
            }
            else
            {
                stringBuilder.Append(dataPacket.Direction == DataDirection.Received ? "RCV" : "SND");
            }
            stringBuilder.Append(dataPacket.Direction == DataDirection.Received ? " < " : " > ");
            stringBuilder.Append(TraceMyNet.Configuration.RelativeTime ? AccurateTimer.FormatRelative(dataPacket.TimeStamp, dataPacket.RootTimeStamp) : AccurateTimer.FormatAbsolute(dataPacket.TimeStamp));
            if (dataPacket.FrameNumber >= 0)
            {
                stringBuilder.Append(" #" + dataPacket.FrameNumber);
            }
            if (dataPacket.Data != null && dataPacket.Data.Length > 0)
            {
                stringBuilder.Append(" (" + dataPacket.Data.Length.ToString() + " bytes)");
            }
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("DefaultHandler#" + GetHashCode() + "::FormatLabel() stringBuilder.ToString():[" + stringBuilder.ToString() + "]");
            return stringBuilder.ToString();
        }

        public void FormatData(IDataPacket dataPacket, RichTextBox richTextBox)
        {
            FormatData(dataPacket, richTextBox, TraceMyNet.Configuration.BinaryFormat);
            richTextBox.Select(0, 0);
            richTextBox.ScrollToCaret();
        }

        public void FormatData(IDataPacket[] dataPackets, RichTextBox richTextBox)
        {
            foreach (IDataPacket dataPacket in dataPackets)
            {
                FormatData(dataPacket, richTextBox, TraceMyNet.Configuration.BinaryFormat);
            }
            richTextBox.Select(0, 0);
            richTextBox.ScrollToCaret();
        }

        public void FormatDataAsText(IDataPacket[] dataPackets, RichTextBox richTextBox)
        {
            foreach (IDataPacket dataPacket in dataPackets)
            {
                FormatData(dataPacket, richTextBox, false);
            }
            richTextBox.Select(0, 0);
            richTextBox.ScrollToCaret();
        }

        public void FormatDataAsBinary(IDataPacket[] dataPackets, RichTextBox richTextBox)
        {
            foreach (IDataPacket dataPacket in dataPackets)
            {
                FormatData(dataPacket, richTextBox, true);
            }
            richTextBox.Select(0, 0);
            richTextBox.ScrollToCaret();
        }

        public void FormatDataAsText(IDataPacket dataPacket, RichTextBox richTextBox)
        {
            FormatData(dataPacket, richTextBox, false);
            richTextBox.Select(0, 0);
            richTextBox.ScrollToCaret();
        }

        public void FormatDataAsBinary(IDataPacket dataPacket, RichTextBox richTextBox)
        {
            FormatData(dataPacket, richTextBox, true);
            richTextBox.Select(0, 0);
            richTextBox.ScrollToCaret();
        }

        public void FormatData(IDataPacket dataPacket, RichTextBox richTextBox, bool binary)
        {
            richTextBox.SelectionColor = dataPacket.Direction == DataDirection.Received ? Color.Purple : Color.Blue;
            if (dataPacket.Exception != null)
            {
                richTextBox.AppendText(dataPacket.Exception.ToString() + "\r\n");
            }
            else if (dataPacket.Data == null || dataPacket.Data.Length == 0)
            {
                richTextBox.AppendText("(socket shutdown by remote peer)\r\n");
            }
            else
            {
                richTextBox.AppendText(binary ? FormatBinary(dataPacket.Data) + "\r\n" : FormatText(dataPacket.Data));
            }
        }

        static string FormatText(byte[] data)
        {
            return Iso8859.GetString(data, 0, data.Length);
        }

        public static string FormatBinary(byte[] data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int offset = 0;
            int length = data.Length;
            for (; ; )
            {
                int n = Math.Min(length, 16);
                stringBuilder.Append(string.Format("{0:X8} : ", offset));
                for (int i = 0; i < n; ++i)
                {
                    stringBuilder.Append(string.Format("{0:X2}", data[offset + i]) + ((i == 7) ? '-' : ' '));
                }
                for (int i = n; i < 16; ++i)
                {
                    stringBuilder.Append("   ");
                }
                stringBuilder.Append(": ");
                for (int i = 0; i < n; ++i)
                {
                    stringBuilder.Append(((data[offset + i] < 0x20) || (data[offset + i] > 0x7e)) ? '.' : (char)(data[offset + i]));
                }
                offset += n;
                length -= n;
                if (length <= 0)
                {
                    break;
                }
                stringBuilder.Append("\r\n");
            }
            return stringBuilder.ToString();
        }
    }

    public class ConnectionNode : TreeNode
    {
        public static ICustomHandler CustomHandler;
        public Stream accepted;
        public Stream connected;
        public byte[] acceptedBufferNode = new byte[4096];
        public byte[] connectedBufferNode = new byte[4096];

        internal IPEndPoint acceptedEndPoint;
        IPEndPoint connectedEndPoint;
        string info;
        internal DateTime timeStamp;
        internal ConnectionStatus status;

        public IPEndPoint AcceptedEndPoint
        {
            get
            {
                return acceptedEndPoint;
            }
        }

        public IPEndPoint ConnectedEndPoint
        {
            get
            {
                return connectedEndPoint;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }

        public ConnectionNode(IPEndPoint acceptedEndPoint, IPEndPoint connectedEndPoint, DateTime timeStamp)
        {
            this.timeStamp = timeStamp;
            this.connectedEndPoint = connectedEndPoint;
            this.acceptedEndPoint = acceptedEndPoint;
            status = ConnectionStatus.Closed;
            ForeColor = Color.DarkRed;
            SetText();
        }

        public bool MyCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("ConnectionNode#" + GetHashCode() + "::MyCertificateValidationCallback(): sslPolicyErrors:" + sslPolicyErrors);
            return true;
        }

        public ConnectionNode(Socket accepted, Socket connected)
        {
            timeStamp = AccurateTimer.GetTimeStamp();
            if (accepted != null)
            {
                acceptedEndPoint = accepted.RemoteEndPoint as IPEndPoint;
                this.accepted = new NetworkStream(accepted, true);
                if (TraceMyNet.Configuration.SslS)
                {
                    SslStream sslStream = new SslStream(this.accepted, false, new RemoteCertificateValidationCallback(MyCertificateValidationCallback));
                    sslStream.AuthenticateAsServer(TraceMyNet.ServerCert, false, SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls, false);
                    this.accepted = sslStream;
                }
            }
            if (connected != null)
            {
                connectedEndPoint = connected.RemoteEndPoint as IPEndPoint;
                this.connected = new NetworkStream(connected, true);
                if (TraceMyNet.Configuration.SslC)
                {
                    SslStream sslStream = new SslStream(this.connected, false, new RemoteCertificateValidationCallback(MyCertificateValidationCallback));
                    sslStream.AuthenticateAsClient(TraceMyNet.RemoteHostName, TraceMyNet.ClientCerts, SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls, false);
                    this.connected = sslStream;
                }
            }
            status = ConnectionStatus.Open;
            ForeColor = Color.Green;

            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("ConnectionNode#" + GetHashCode() + "::.ctor(): calling SetText()");
            SetText();
        }

        public string DnsInfo
        {
            get
            {
                if (info == null)
                {
                    info = string.Empty;
                    IPHostEntry ipHostEntry;
                    if (acceptedEndPoint != null)
                    {
                        try
                        {
                            ipHostEntry = Dns.GetHostEntry(acceptedEndPoint.Address);
                            info += "Source:\r\n" + Options.DumpIPHostEntry(ipHostEntry) + "\r\n";
                        }
                        catch (Exception exception)
                        {
                            info += "Source:\r\n" + exception.Message + "\r\n";
                        }
                    }
                    if (connectedEndPoint != null)
                    {
                        try
                        {
                            ipHostEntry = Dns.GetHostEntry(connectedEndPoint.Address);
                            info += "Destination:\r\n" + Options.DumpIPHostEntry(ipHostEntry) + "\r\n";
                        }
                        catch (Exception exception)
                        {
                            info += "Destination:\r\n" + exception.Message + "\r\n";
                        }
                    }
                }
                return info;
            }
        }

        public void Cleanup(bool onException)
        {
            if (status == ConnectionStatus.Open)
            {
                lock (this)
                {
                    // only cleanup once
                    if (status == ConnectionStatus.Open)
                    {
                        status = onException ? ConnectionStatus.Interrupted : ConnectionStatus.Closed;
                        ForeColor = onException ? Color.Red : Color.DarkRed;
                        if (accepted != null)
                        {
                            accepted.Close();
                        }
                        if (connected != null)
                        {
                            connected.Close();
                        }
                    }
                }
            }
        }

        public void SetText()
        {
            Text = "CONN " + AccurateTimer.FormatAbsolute(timeStamp) + " (" + (acceptedEndPoint != null ? acceptedEndPoint.ToString() : "<null>") + " -> " + (connectedEndPoint != null ? connectedEndPoint.ToString() : "<null>") + ")";
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("ConnectionNode#" + GetHashCode() + "::SetText() Text:[" + Text + "]");
        }

        public void FormatData(RichTextBox richTextBox)
        {
            richTextBox.Text = string.Empty;
            if (TraceMyNet.MainForm.menuShowData.Checked)
            {
                if (CustomHandler != null)
                {
                    IDataPacket[] dataPackets = new IDataPacket[Nodes.Count];
                    Nodes.CopyTo(dataPackets, 0);
                    if (CustomHandler is ICustomHandlerTextBinary)
                    {
                        if (TraceMyNet.Configuration.BinaryFormat)
                        {
                            ((ICustomHandlerTextBinary)CustomHandler).FormatDataAsBinary(dataPackets, richTextBox);
                        }
                        else
                        {
                            ((ICustomHandlerTextBinary)CustomHandler).FormatDataAsText(dataPackets, richTextBox);
                        }
                    }
                    else
                    {
                        CustomHandler.FormatData(dataPackets, richTextBox);
                    }
                }
            }
        }
    }

    public class BufferNode : TreeNode, IDataPacket
    {
        byte[] data;
        DateTime timeStamp;
        DataDirection direction;
        int frameNumber;
        Exception exception;

        internal ConnectionNode father;

        public byte[] Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                timeStamp = value;
            }
        }

        public DateTime RootTimeStamp
        {
            get
            {
                return father.timeStamp;
            }
            set
            {
                father.timeStamp = value;
            }
        }

        public DataDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        public int FrameNumber
        {
            get
            {
                return frameNumber;
            }
            set
            {
                frameNumber = value;
            }
        }

        public Exception Exception
        {
            get
            {
                return exception;
            }
            set
            {
                exception = value;
            }
        }

        public BufferNode(ConnectionNode father, DataDirection direction, byte[] origin, int dataLength, Exception exception)
            : this(father, direction, origin, dataLength, -1, exception, AccurateTimer.GetTimeStamp())
        {
        }

        public BufferNode(ConnectionNode father, DataDirection direction, byte[] origin, int dataLength, int frameNumber, Exception exception, DateTime timeStamp)
        {
            this.father = father;

            Exception = exception;
            TimeStamp = timeStamp;
            Direction = direction;
            FrameNumber = frameNumber;
            if (origin != null)
            {
                Data = new byte[dataLength];
                if (dataLength > 0)
                {
                    Buffer.BlockCopy(origin, 0, Data, 0, dataLength);
                }
            }
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("BufferNode#" + GetHashCode() + "::.ctor() calling SetText()");
            SetText();
        }

        public void SetText()
        {
            if (ConnectionNode.CustomHandler != null)
            {
                if (ConnectionNode.CustomHandler is ICustomHandlerTextBinary)
                {
                    Color color;
                    Text = ((ICustomHandlerTextBinary)ConnectionNode.CustomHandler).FormatLabel(this, out color);
                    ForeColor = color;
                }
                else
                {
                    Text = ConnectionNode.CustomHandler.FormatLabel(this);
                    ForeColor = Direction == DataDirection.Received ? Color.Purple : Color.Blue;
                }
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("BufferNode#" + GetHashCode() + "::SetText() Text:[" + Text + "]");
            }
        }

        public void FormatData(RichTextBox richTextBox)
        {
            richTextBox.Text = string.Empty;
            if (TraceMyNet.MainForm.menuShowData.Checked)
            {
                if (ConnectionNode.CustomHandler != null)
                {
                    if (ConnectionNode.CustomHandler is ICustomHandlerTextBinary)
                    {
                        if (TraceMyNet.Configuration.BinaryFormat)
                        {
                            ((ICustomHandlerTextBinary)ConnectionNode.CustomHandler).FormatDataAsBinary(this, richTextBox);
                        }
                        else
                        {
                            ((ICustomHandlerTextBinary)ConnectionNode.CustomHandler).FormatDataAsText(this, richTextBox);
                        }
                    }
                    else
                    {
                        ConnectionNode.CustomHandler.FormatData(this, richTextBox);
                    }
                }
            }
        }
    }
}

namespace Ranamauro.Net
{
    using System.IO;

    public class TraceMyNet
    {
        public static bool Debug { get { return CodePlex.Tools.TraceMyNet.TraceMyNet.Debug; } }
        public static TextWriter LogFile { get { return CodePlex.Tools.TraceMyNet.TraceMyNet.LogFile; } }
    }
}
