namespace CodePlex.Tools.TraceMyNet
{
    using System.Windows.Forms;

    partial class TraceMyNet
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
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuMainFile = new System.Windows.Forms.MenuItem();
            this.menuOpen = new System.Windows.Forms.MenuItem();
            this.menuSave = new System.Windows.Forms.MenuItem();
            this.menuSaveAs = new System.Windows.Forms.MenuItem();
            this.menuSeparator1 = new System.Windows.Forms.MenuItem();
            this.menuExit = new System.Windows.Forms.MenuItem();
            this.menuMainEdit = new System.Windows.Forms.MenuItem();
            this.menuUndo = new System.Windows.Forms.MenuItem();
            this.menuRedo = new System.Windows.Forms.MenuItem();
            this.menuSeparator2 = new System.Windows.Forms.MenuItem();
            this.menuCut = new System.Windows.Forms.MenuItem();
            this.menuCopy = new System.Windows.Forms.MenuItem();
            this.menuPaste = new System.Windows.Forms.MenuItem();
            this.menuDelete = new System.Windows.Forms.MenuItem();
            this.menuSeparator3 = new System.Windows.Forms.MenuItem();
            this.menuSelectAll = new System.Windows.Forms.MenuItem();
            this.menuSeparator5 = new System.Windows.Forms.MenuItem();
            this.menuClearConnections = new System.Windows.Forms.MenuItem();
            this.menuClearConnections_Selected = new System.Windows.Forms.MenuItem();
            this.menuClearConnections_Inactive = new System.Windows.Forms.MenuItem();
            this.menuClearConnections_All = new System.Windows.Forms.MenuItem();
            this.menuSeparator4 = new System.Windows.Forms.MenuItem();
            this.menuFind = new System.Windows.Forms.MenuItem();
            this.menuMainCapture = new System.Windows.Forms.MenuItem();
            this.menuStartCapture = new System.Windows.Forms.MenuItem();
            this.menuStopCapture = new System.Windows.Forms.MenuItem();
            this.menuPauseCapture = new System.Windows.Forms.MenuItem();
            this.menuContinueCapture = new System.Windows.Forms.MenuItem();
            this.menuSeparator6 = new System.Windows.Forms.MenuItem();
            this.menuOptions = new System.Windows.Forms.MenuItem();
            this.menuMainFormat = new System.Windows.Forms.MenuItem();
            this.menuDefaultHandler = new System.Windows.Forms.MenuItem();
            this.menuCustomHandler = new System.Windows.Forms.MenuItem();
            this.menuFont = new System.Windows.Forms.MenuItem();
            this.menuMainView = new System.Windows.Forms.MenuItem();
            this.menuWordWrap = new System.Windows.Forms.MenuItem();
            this.menuBinary = new System.Windows.Forms.MenuItem();
            this.menuRelativeTime = new System.Windows.Forms.MenuItem();
            this.menuSeparator8 = new System.Windows.Forms.MenuItem();
            this.menuShowData = new System.Windows.Forms.MenuItem();
            this.menuStatusBar = new System.Windows.Forms.MenuItem();
            this.menuMainHelp = new System.Windows.Forms.MenuItem();
            this.menuHelp = new System.Windows.Forms.MenuItem();
            this.menuSeparator12 = new System.Windows.Forms.MenuItem();
            this.menuAbout = new System.Windows.Forms.MenuItem();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.sbpAppStatus = new System.Windows.Forms.StatusBarPanel();
            this.sbpListeningPort = new System.Windows.Forms.StatusBarPanel();
            this.sbpDestinationPort = new System.Windows.Forms.StatusBarPanel();
            this.sbpDataStats = new System.Windows.Forms.StatusBarPanel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.transferredData = new System.Windows.Forms.RichTextBox();
            this.transferredDataMenu = new System.Windows.Forms.ContextMenu();
            this.menuTransferredDataUndo = new System.Windows.Forms.MenuItem();
            this.menuSeparator15 = new System.Windows.Forms.MenuItem();
            this.menuTransferredDataCut = new System.Windows.Forms.MenuItem();
            this.menuTransferredDataCopy = new System.Windows.Forms.MenuItem();
            this.menuTransferredDataPaste = new System.Windows.Forms.MenuItem();
            this.menuSeparator16 = new System.Windows.Forms.MenuItem();
            this.menuTransferredDataSelectAll = new System.Windows.Forms.MenuItem();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.connectionList = new System.Windows.Forms.TreeView();
            this.connectionMenu = new System.Windows.Forms.ContextMenu();
            this.menuConnectionUndo = new System.Windows.Forms.MenuItem();
            this.menuSeparator13 = new System.Windows.Forms.MenuItem();
            this.menuConnectionCut = new System.Windows.Forms.MenuItem();
            this.menuConnectionCopy = new System.Windows.Forms.MenuItem();
            this.menuConnectionPaste = new System.Windows.Forms.MenuItem();
            this.menuSeparator14 = new System.Windows.Forms.MenuItem();
            this.connectionProperties = new System.Windows.Forms.MenuItem();
            this.splitter = new System.Windows.Forms.Splitter();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            ((System.ComponentModel.ISupportInitialize)(this.sbpAppStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpListeningPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpDestinationPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpDataStats)).BeginInit();
            this.panelRight.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuMainFile,
            this.menuMainEdit,
            this.menuMainCapture,
            this.menuMainFormat,
            this.menuMainView,
            this.menuMainHelp});
            // 
            // menuMainFile
            // 
            this.menuMainFile.Index = 0;
            this.menuMainFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuOpen,
            this.menuSave,
            this.menuSaveAs,
            this.menuSeparator1,
            this.menuExit});
            this.menuMainFile.Text = "&File";
            // 
            // menuOpen
            // 
            this.menuOpen.Index = 0;
            this.menuOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.menuOpen.Text = "&Open...";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuSave
            // 
            this.menuSave.Enabled = false;
            this.menuSave.Index = 1;
            this.menuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuSave.Text = "&Save";
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Index = 2;
            this.menuSaveAs.Text = "Save &As...";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // menuSeparator1
            // 
            this.menuSeparator1.Index = 3;
            this.menuSeparator1.Text = "-";
            // 
            // menuExit
            // 
            this.menuExit.Index = 4;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuQuit_Click);
            // 
            // menuMainEdit
            // 
            this.menuMainEdit.Index = 1;
            this.menuMainEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuUndo,
            this.menuRedo,
            this.menuSeparator2,
            this.menuCut,
            this.menuCopy,
            this.menuPaste,
            this.menuDelete,
            this.menuSeparator3,
            this.menuSelectAll,
            this.menuSeparator5,
            this.menuClearConnections,
            this.menuSeparator4,
            this.menuFind});
            this.menuMainEdit.Text = "&Edit";
            // 
            // menuUndo
            // 
            this.menuUndo.Enabled = false;
            this.menuUndo.Index = 0;
            this.menuUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.menuUndo.Text = "&Undo";
            // 
            // menuRedo
            // 
            this.menuRedo.Enabled = false;
            this.menuRedo.Index = 1;
            this.menuRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
            this.menuRedo.Text = "&Redo";
            // 
            // menuSeparator2
            // 
            this.menuSeparator2.Index = 2;
            this.menuSeparator2.Text = "-";
            // 
            // menuCut
            // 
            this.menuCut.Enabled = false;
            this.menuCut.Index = 3;
            this.menuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.menuCut.Text = "Cu&t";
            // 
            // menuCopy
            // 
            this.menuCopy.Index = 4;
            this.menuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuCopy.Text = "&Copy";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuPaste
            // 
            this.menuPaste.Enabled = false;
            this.menuPaste.Index = 5;
            this.menuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.menuPaste.Text = "&Paste";
            // 
            // menuDelete
            // 
            this.menuDelete.Enabled = false;
            this.menuDelete.Index = 6;
            this.menuDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.menuDelete.Text = "&Delete";
            // 
            // menuSeparator3
            // 
            this.menuSeparator3.Index = 7;
            this.menuSeparator3.Text = "-";
            // 
            // menuSelectAll
            // 
            this.menuSelectAll.Index = 8;
            this.menuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.menuSelectAll.Text = "Select &All";
            this.menuSelectAll.Click += new System.EventHandler(this.menuSelectAll_Click);
            // 
            // menuSeparator5
            // 
            this.menuSeparator5.Index = 9;
            this.menuSeparator5.Text = "-";
            // 
            // menuClearConnections
            // 
            this.menuClearConnections.Index = 10;
            this.menuClearConnections.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuClearConnections_Selected,
            this.menuClearConnections_Inactive,
            this.menuClearConnections_All});
            this.menuClearConnections.Text = "Clear Co&nnections";
            // 
            // menuClearConnections_Selected
            // 
            this.menuClearConnections_Selected.Index = 0;
            this.menuClearConnections_Selected.Text = "&Selected";
            this.menuClearConnections_Selected.Click += new System.EventHandler(this.menuClearConnections_Selected_Click);
            // 
            // menuClearConnections_Inactive
            // 
            this.menuClearConnections_Inactive.Index = 1;
            this.menuClearConnections_Inactive.Text = "&Inactive";
            this.menuClearConnections_Inactive.Click += new System.EventHandler(this.menuClearConnections_Inactive_Click);
            // 
            // menuClearConnections_All
            // 
            this.menuClearConnections_All.Index = 2;
            this.menuClearConnections_All.Text = "&All";
            this.menuClearConnections_All.Click += new System.EventHandler(this.menuClearConnections_All_Click);
            // 
            // menuSeparator4
            // 
            this.menuSeparator4.Index = 11;
            this.menuSeparator4.Text = "-";
            // 
            // menuFind
            // 
            this.menuFind.Index = 12;
            this.menuFind.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.menuFind.Text = "&Find...";
            this.menuFind.Click += new System.EventHandler(this.menuFind_Click);
            // 
            // menuMainCapture
            // 
            this.menuMainCapture.Index = 2;
            this.menuMainCapture.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuStartCapture,
            this.menuStopCapture,
            this.menuPauseCapture,
            this.menuContinueCapture,
            this.menuSeparator6,
            this.menuOptions});
            this.menuMainCapture.Text = "&Capture";
            // 
            // menuStartCapture
            // 
            this.menuStartCapture.Index = 0;
            this.menuStartCapture.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuStartCapture.Text = "&Start";
            this.menuStartCapture.Click += new System.EventHandler(this.menuStartCapture_Click);
            // 
            // menuStopCapture
            // 
            this.menuStopCapture.Index = 1;
            this.menuStopCapture.Shortcut = System.Windows.Forms.Shortcut.ShiftF5;
            this.menuStopCapture.Text = "S&top";
            this.menuStopCapture.Click += new System.EventHandler(this.menuStopCapture_Click);
            // 
            // menuPauseCapture
            // 
            this.menuPauseCapture.Enabled = false;
            this.menuPauseCapture.Index = 2;
            this.menuPauseCapture.Text = "&Pause";
            // 
            // menuContinueCapture
            // 
            this.menuContinueCapture.Enabled = false;
            this.menuContinueCapture.Index = 3;
            this.menuContinueCapture.Text = "&Continue";
            // 
            // menuSeparator6
            // 
            this.menuSeparator6.Index = 4;
            this.menuSeparator6.Text = "-";
            // 
            // menuOptions
            // 
            this.menuOptions.Index = 5;
            this.menuOptions.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
            this.menuOptions.Text = "&Options...";
            this.menuOptions.Click += new System.EventHandler(this.menuOptions_Click);
            // 
            // menuMainFormat
            // 
            this.menuMainFormat.Index = 3;
            this.menuMainFormat.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuDefaultHandler,
            this.menuCustomHandler,
            this.menuFont});
            this.menuMainFormat.Text = "F&ormat";
            // 
            // menuDefaultHandler
            // 
            this.menuDefaultHandler.Index = 0;
            this.menuDefaultHandler.Text = "&Default Handler";
            this.menuDefaultHandler.Click += new System.EventHandler(this.menuDefaultHandler_Click);
            // 
            // menuCustomHandler
            // 
            this.menuCustomHandler.Index = 1;
            this.menuCustomHandler.Text = "&Custom Handler...";
            this.menuCustomHandler.Click += new System.EventHandler(this.menuCustomHandler_Click);
            // 
            // menuFont
            // 
            this.menuFont.Index = 2;
            this.menuFont.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.menuFont.Text = "&Font...";
            this.menuFont.Click += new System.EventHandler(this.menuFont_Click);
            // 
            // menuMainView
            // 
            this.menuMainView.Index = 4;
            this.menuMainView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuWordWrap,
            this.menuBinary,
            this.menuRelativeTime,
            this.menuSeparator8,
            this.menuShowData,
            this.menuStatusBar});
            this.menuMainView.Text = "&View";
            // 
            // menuWordWrap
            // 
            this.menuWordWrap.Index = 0;
            this.menuWordWrap.Text = "&Word Wrap";
            this.menuWordWrap.Click += new System.EventHandler(this.menuWordWrap_Click);
            // 
            // menuBinary
            // 
            this.menuBinary.Index = 1;
            this.menuBinary.Text = "&Binary Format";
            this.menuBinary.Click += new System.EventHandler(this.menuBinary_Click);
            // 
            // menuRelativeTime
            // 
            this.menuRelativeTime.Index = 2;
            this.menuRelativeTime.Text = "&Relative Times";
            this.menuRelativeTime.Click += new System.EventHandler(this.menuRelativeTime_Click);
            // 
            // menuSeparator8
            // 
            this.menuSeparator8.Index = 3;
            this.menuSeparator8.Text = "-";
            // 
            // menuShowData
            // 
            this.menuShowData.Index = 4;
            this.menuShowData.Text = "&Show Network Data";
            this.menuShowData.Click += new System.EventHandler(this.menuShowHide_Click);
            // 
            // menuStatusBar
            // 
            this.menuStatusBar.Index = 5;
            this.menuStatusBar.Text = "&Status Bar";
            this.menuStatusBar.Click += new System.EventHandler(this.menuStatusBar_Click);
            // 
            // menuMainHelp
            // 
            this.menuMainHelp.Index = 5;
            this.menuMainHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuHelp,
            this.menuSeparator12,
            this.menuAbout});
            this.menuMainHelp.Text = "&Help";
            // 
            // menuHelp
            // 
            this.menuHelp.Index = 0;
            this.menuHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.menuHelp.Text = "&Help";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // menuSeparator12
            // 
            this.menuSeparator12.Index = 1;
            this.menuSeparator12.Text = "-";
            // 
            // menuAbout
            // 
            this.menuAbout.Index = 2;
            this.menuAbout.Text = "&About...";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 408);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sbpAppStatus,
            this.sbpListeningPort,
            this.sbpDestinationPort,
            this.sbpDataStats});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(592, 20);
            this.statusBar.TabIndex = 3;
            // 
            // sbpAppStatus
            // 
            this.sbpAppStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpAppStatus.MinWidth = 50;
            this.sbpAppStatus.Name = "sbpAppStatus";
            this.sbpAppStatus.Width = 50;
            // 
            // sbpListeningPort
            // 
            this.sbpListeningPort.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpListeningPort.MinWidth = 50;
            this.sbpListeningPort.Name = "sbpListeningPort";
            this.sbpListeningPort.Width = 50;
            // 
            // sbpDestinationPort
            // 
            this.sbpDestinationPort.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpDestinationPort.MinWidth = 50;
            this.sbpDestinationPort.Name = "sbpDestinationPort";
            this.sbpDestinationPort.Width = 50;
            // 
            // sbpDataStats
            // 
            this.sbpDataStats.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpDataStats.MinWidth = 50;
            this.sbpDataStats.Name = "sbpDataStats";
            this.sbpDataStats.Width = 50;
            // 
            // panelRight
            // 
            this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelRight.Controls.Add(this.transferredData);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(208, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(384, 408);
            this.panelRight.TabIndex = 9;
            // 
            // transferredData
            // 
            this.transferredData.AcceptsTab = true;
            this.transferredData.BackColor = System.Drawing.SystemColors.Window;
            this.transferredData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.transferredData.ContextMenu = this.transferredDataMenu;
            this.transferredData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transferredData.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transferredData.HideSelection = false;
            this.transferredData.Location = new System.Drawing.Point(0, 0);
            this.transferredData.Name = "transferredData";
            this.transferredData.ReadOnly = true;
            this.transferredData.Size = new System.Drawing.Size(380, 404);
            this.transferredData.TabIndex = 6;
            this.transferredData.Text = "";
            this.transferredData.WordWrap = false;
            // 
            // transferredDataMenu
            // 
            this.transferredDataMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTransferredDataUndo,
            this.menuSeparator15,
            this.menuTransferredDataCut,
            this.menuTransferredDataCopy,
            this.menuTransferredDataPaste,
            this.menuSeparator16,
            this.menuTransferredDataSelectAll});
            this.transferredDataMenu.Popup += new System.EventHandler(this.transferredDataMenu_Popup);
            // 
            // menuTransferredDataUndo
            // 
            this.menuTransferredDataUndo.Enabled = false;
            this.menuTransferredDataUndo.Index = 0;
            this.menuTransferredDataUndo.Text = "Undo";
            // 
            // menuSeparator15
            // 
            this.menuSeparator15.Index = 1;
            this.menuSeparator15.Text = "-";
            // 
            // menuTransferredDataCut
            // 
            this.menuTransferredDataCut.Enabled = false;
            this.menuTransferredDataCut.Index = 2;
            this.menuTransferredDataCut.Text = "Cut";
            // 
            // menuTransferredDataCopy
            // 
            this.menuTransferredDataCopy.Index = 3;
            this.menuTransferredDataCopy.Text = "Copy";
            this.menuTransferredDataCopy.Click += new System.EventHandler(this.menuTransferredDataCopy_Click);
            // 
            // menuTransferredDataPaste
            // 
            this.menuTransferredDataPaste.Enabled = false;
            this.menuTransferredDataPaste.Index = 4;
            this.menuTransferredDataPaste.Text = "Paste";
            // 
            // menuSeparator16
            // 
            this.menuSeparator16.Index = 5;
            this.menuSeparator16.Text = "-";
            // 
            // menuTransferredDataSelectAll
            // 
            this.menuTransferredDataSelectAll.Index = 6;
            this.menuTransferredDataSelectAll.Text = "Select All";
            this.menuTransferredDataSelectAll.Click += new System.EventHandler(this.menuTransferredDataSelectAll_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Controls.Add(this.connectionList);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(208, 408);
            this.panelLeft.TabIndex = 10;
            // 
            // connectionList
            // 
            this.connectionList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.connectionList.ContextMenu = this.connectionMenu;
            this.connectionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionList.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionList.ItemHeight = 18;
            this.connectionList.Location = new System.Drawing.Point(0, 0);
            this.connectionList.Name = "connectionList";
            this.connectionList.Size = new System.Drawing.Size(204, 404);
            this.connectionList.TabIndex = 8;
            this.connectionList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.connectionList_AfterSelect);
            // 
            // connectionMenu
            // 
            this.connectionMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuConnectionUndo,
            this.menuSeparator13,
            this.menuConnectionCut,
            this.menuConnectionCopy,
            this.menuConnectionPaste,
            this.menuSeparator14,
            this.connectionProperties});
            // 
            // menuConnectionUndo
            // 
            this.menuConnectionUndo.Enabled = false;
            this.menuConnectionUndo.Index = 0;
            this.menuConnectionUndo.Text = "Undo";
            // 
            // menuSeparator13
            // 
            this.menuSeparator13.Index = 1;
            this.menuSeparator13.Text = "-";
            // 
            // menuConnectionCut
            // 
            this.menuConnectionCut.Index = 2;
            this.menuConnectionCut.Text = "Cut";
            // 
            // menuConnectionCopy
            // 
            this.menuConnectionCopy.Index = 3;
            this.menuConnectionCopy.Text = "Copy";
            // 
            // menuConnectionPaste
            // 
            this.menuConnectionPaste.Enabled = false;
            this.menuConnectionPaste.Index = 4;
            this.menuConnectionPaste.Text = "Paste";
            // 
            // menuSeparator14
            // 
            this.menuSeparator14.Index = 5;
            this.menuSeparator14.Text = "-";
            // 
            // connectionProperties
            // 
            this.connectionProperties.Index = 6;
            this.connectionProperties.Text = "Properties";
            this.connectionProperties.Click += new System.EventHandler(this.connectionProperties_Click);
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(208, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(3, 408);
            this.splitter.TabIndex = 9;
            this.splitter.TabStop = false;
            // 
            // TraceMyNet
            // 
            this.ClientSize = new System.Drawing.Size(592, 428);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.statusBar);
            this.Menu = this.mainMenu;
            this.Name = "TraceMyNet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.TraceMyNet_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TraceMyNet_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.sbpAppStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpListeningPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpDestinationPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpDataStats)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private StatusBar statusBar;
        private Splitter splitter;

        private Panel panelLeft;
        private Panel panelRight;

        private MainMenu mainMenu;
        private MenuItem menuMainFile;
        private MenuItem menuOptions;
        private MenuItem menuOpen;
        private MenuItem menuMainHelp;
        private MenuItem menuHelp;
        private MenuItem menuAbout;
        private MenuItem menuMainView;
        private MenuItem menuBinary;
        private MenuItem menuWordWrap;
        private MenuItem menuDefaultHandler;
        private MenuItem menuCustomHandler;
        private MenuItem menuRelativeTime;

        private ContextMenu connectionMenu;
        private MenuItem connectionProperties;

        private ToolTip toolTip;
        private StatusBarPanel sbpAppStatus;
        private StatusBarPanel sbpListeningPort;
        private StatusBarPanel sbpDestinationPort;
        private StatusBarPanel sbpDataStats;
        private MenuItem menuSave;
        private MenuItem menuSaveAs;
        private MenuItem menuExit;
        private MenuItem menuMainEdit;
        private MenuItem menuUndo;
        private MenuItem menuRedo;
        private MenuItem menuCut;
        private MenuItem menuCopy;
        private MenuItem menuPaste;
        private MenuItem menuDelete;
        private MenuItem menuSelectAll;
        private MenuItem menuFind;
        private MenuItem menuMainCapture;
        private MenuItem menuStartCapture;
        private MenuItem menuStopCapture;
        private MenuItem menuPauseCapture;
        private MenuItem menuContinueCapture;
        private MenuItem menuMainFormat;
        private MenuItem menuFont;
        private MenuItem menuConnectionCopy;
        private ContextMenu transferredDataMenu;
        private MenuItem menuTransferredDataCopy;
        private MenuItem menuTransferredDataSelectAll;
        private FontDialog fontDialog1;
        private MenuItem menuStatusBar;
        private MenuItem menuTransferredDataUndo;
        private MenuItem menuTransferredDataCut;
        private MenuItem menuTransferredDataPaste;
        private MenuItem menuConnectionUndo;
        private MenuItem menuConnectionCut;
        private MenuItem menuConnectionPaste;
        private TreeView connectionList;
        private RichTextBox transferredData;
        private MenuItem menuSeparator1;
        private MenuItem menuSeparator2;
        private MenuItem menuSeparator3;
        private MenuItem menuSeparator4;
        private MenuItem menuSeparator6;
        private MenuItem menuSeparator8;
        private MenuItem menuSeparator12;
        private MenuItem menuSeparator15;
        private MenuItem menuSeparator13;
        private MenuItem menuSeparator16;
        private MenuItem menuSeparator14;
        private MenuItem menuClearConnections;
        private MenuItem menuClearConnections_Selected;
        private MenuItem menuClearConnections_Inactive;
        private MenuItem menuClearConnections_All;
        private MenuItem menuSeparator5;
        internal MenuItem menuShowData;

        #endregion
    }
}
