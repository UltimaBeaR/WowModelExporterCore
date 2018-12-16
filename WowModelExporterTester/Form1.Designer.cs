namespace WowModelExporterTester
{
    partial class Form1
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
            this.newFromBrowserStateButton = new System.Windows.Forms.Button();
            this.webControl = new EO.WinForm.WebControl();
            this.webView = new EO.WebBrowser.WebView();
            this.tabs = new System.Windows.Forms.TabControl();
            this.mainTab = new System.Windows.Forms.TabPage();
            this.openGroupbox = new System.Windows.Forms.GroupBox();
            this.openExistingButton = new System.Windows.Forms.Button();
            this.filenameTextbox = new System.Windows.Forms.TextBox();
            this.createNewGroupbox = new System.Windows.Forms.GroupBox();
            this.newFromRaceGenderButton = new System.Windows.Forms.Button();
            this.isMaleCheckbox = new System.Windows.Forms.CheckBox();
            this.raceCombobox = new System.Windows.Forms.ComboBox();
            this.newFromBrowserStateMayBeOldLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mergeGroupBox = new System.Windows.Forms.GroupBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.browserTab = new System.Windows.Forms.TabPage();
            this.browserContentPanel = new System.Windows.Forms.Panel();
            this.addressPanel = new System.Windows.Forms.Panel();
            this.urlButtonsPanel = new System.Windows.Forms.Panel();
            this.submeshIndexTextbox = new System.Windows.Forms.TextBox();
            this.drawOnlySelectedSumeshCheckbox = new System.Windows.Forms.CheckBox();
            this.consoleLogModelsButton = new System.Windows.Forms.Button();
            this.navigateToCharacterSearchButton = new System.Windows.Forms.Button();
            this.navigateToDressroomButton = new System.Windows.Forms.Button();
            this.addressTextboxPanel = new System.Windows.Forms.Panel();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.devToolsTab = new System.Windows.Forms.TabPage();
            this.devToolsContent = new System.Windows.Forms.Panel();
            this.devToolsHeader = new System.Windows.Forms.Panel();
            this.showDevToolsCheckbox = new System.Windows.Forms.CheckBox();
            this.utilityTab = new System.Windows.Forms.TabPage();
            this.openCacheDirectoryButton = new System.Windows.Forms.Button();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDevToolsTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prepareForVrchatCheckbox = new System.Windows.Forms.CheckBox();
            this.tabs.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.openGroupbox.SuspendLayout();
            this.createNewGroupbox.SuspendLayout();
            this.browserTab.SuspendLayout();
            this.browserContentPanel.SuspendLayout();
            this.addressPanel.SuspendLayout();
            this.urlButtonsPanel.SuspendLayout();
            this.addressTextboxPanel.SuspendLayout();
            this.devToolsTab.SuspendLayout();
            this.devToolsHeader.SuspendLayout();
            this.utilityTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // newFromBrowserStateButton
            // 
            this.newFromBrowserStateButton.Location = new System.Drawing.Point(211, 46);
            this.newFromBrowserStateButton.Name = "newFromBrowserStateButton";
            this.newFromBrowserStateButton.Size = new System.Drawing.Size(116, 23);
            this.newFromBrowserStateButton.TabIndex = 0;
            this.newFromBrowserStateButton.Text = "from browser state";
            this.newFromBrowserStateButton.UseVisualStyleBackColor = true;
            this.newFromBrowserStateButton.Click += new System.EventHandler(this.newFromBrowserStateButton_Click);
            // 
            // webControl
            // 
            this.webControl.BackColor = System.Drawing.Color.Gray;
            this.webControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl.Location = new System.Drawing.Point(0, 0);
            this.webControl.Name = "webControl";
            this.webControl.Size = new System.Drawing.Size(786, 363);
            this.webControl.TabIndex = 2;
            this.webControl.Text = "webControl";
            this.webControl.WebView = this.webView;
            // 
            // webView
            // 
            this.webView.ObjectForScripting = null;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.mainTab);
            this.tabs.Controls.Add(this.browserTab);
            this.tabs.Controls.Add(this.devToolsTab);
            this.tabs.Controls.Add(this.utilityTab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(800, 450);
            this.tabs.TabIndex = 4;
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.prepareForVrchatCheckbox);
            this.mainTab.Controls.Add(this.openGroupbox);
            this.mainTab.Controls.Add(this.filenameTextbox);
            this.mainTab.Controls.Add(this.createNewGroupbox);
            this.mainTab.Controls.Add(this.label1);
            this.mainTab.Controls.Add(this.mergeGroupBox);
            this.mainTab.Controls.Add(this.exportButton);
            this.mainTab.Location = new System.Drawing.Point(4, 22);
            this.mainTab.Name = "mainTab";
            this.mainTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainTab.Size = new System.Drawing.Size(792, 424);
            this.mainTab.TabIndex = 1;
            this.mainTab.Text = "main";
            this.mainTab.UseVisualStyleBackColor = true;
            // 
            // openGroupbox
            // 
            this.openGroupbox.Controls.Add(this.openExistingButton);
            this.openGroupbox.Location = new System.Drawing.Point(20, 95);
            this.openGroupbox.Name = "openGroupbox";
            this.openGroupbox.Size = new System.Drawing.Size(200, 80);
            this.openGroupbox.TabIndex = 7;
            this.openGroupbox.TabStop = false;
            this.openGroupbox.Text = "open existing .wowvrc";
            // 
            // openExistingButton
            // 
            this.openExistingButton.Location = new System.Drawing.Point(56, 33);
            this.openExistingButton.Name = "openExistingButton";
            this.openExistingButton.Size = new System.Drawing.Size(75, 23);
            this.openExistingButton.TabIndex = 0;
            this.openExistingButton.Text = "open";
            this.openExistingButton.UseVisualStyleBackColor = true;
            this.openExistingButton.Click += new System.EventHandler(this.openExistingButton_Click);
            // 
            // filenameTextbox
            // 
            this.filenameTextbox.Location = new System.Drawing.Point(20, 30);
            this.filenameTextbox.Multiline = true;
            this.filenameTextbox.Name = "filenameTextbox";
            this.filenameTextbox.ReadOnly = true;
            this.filenameTextbox.Size = new System.Drawing.Size(548, 59);
            this.filenameTextbox.TabIndex = 6;
            // 
            // createNewGroupbox
            // 
            this.createNewGroupbox.Controls.Add(this.newFromRaceGenderButton);
            this.createNewGroupbox.Controls.Add(this.isMaleCheckbox);
            this.createNewGroupbox.Controls.Add(this.raceCombobox);
            this.createNewGroupbox.Controls.Add(this.newFromBrowserStateButton);
            this.createNewGroupbox.Controls.Add(this.newFromBrowserStateMayBeOldLabel);
            this.createNewGroupbox.Location = new System.Drawing.Point(226, 95);
            this.createNewGroupbox.Name = "createNewGroupbox";
            this.createNewGroupbox.Size = new System.Drawing.Size(342, 80);
            this.createNewGroupbox.TabIndex = 5;
            this.createNewGroupbox.TabStop = false;
            this.createNewGroupbox.Text = "create new .wowvrc";
            // 
            // newFromRaceGenderButton
            // 
            this.newFromRaceGenderButton.Location = new System.Drawing.Point(6, 46);
            this.newFromRaceGenderButton.Name = "newFromRaceGenderButton";
            this.newFromRaceGenderButton.Size = new System.Drawing.Size(175, 23);
            this.newFromRaceGenderButton.TabIndex = 4;
            this.newFromRaceGenderButton.Text = "from race/gender";
            this.newFromRaceGenderButton.UseVisualStyleBackColor = true;
            this.newFromRaceGenderButton.Click += new System.EventHandler(this.newFromRaceGenderButton_Click);
            // 
            // isMaleCheckbox
            // 
            this.isMaleCheckbox.AutoSize = true;
            this.isMaleCheckbox.Checked = true;
            this.isMaleCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.isMaleCheckbox.Location = new System.Drawing.Point(133, 23);
            this.isMaleCheckbox.Name = "isMaleCheckbox";
            this.isMaleCheckbox.Size = new System.Drawing.Size(48, 17);
            this.isMaleCheckbox.TabIndex = 3;
            this.isMaleCheckbox.Text = "male";
            this.isMaleCheckbox.UseVisualStyleBackColor = true;
            // 
            // raceCombobox
            // 
            this.raceCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.raceCombobox.FormattingEnabled = true;
            this.raceCombobox.Location = new System.Drawing.Point(6, 19);
            this.raceCombobox.Name = "raceCombobox";
            this.raceCombobox.Size = new System.Drawing.Size(121, 21);
            this.raceCombobox.TabIndex = 2;
            // 
            // newFromBrowserStateMayBeOldLabel
            // 
            this.newFromBrowserStateMayBeOldLabel.AutoSize = true;
            this.newFromBrowserStateMayBeOldLabel.ForeColor = System.Drawing.Color.Red;
            this.newFromBrowserStateMayBeOldLabel.Location = new System.Drawing.Point(211, 17);
            this.newFromBrowserStateMayBeOldLabel.Name = "newFromBrowserStateMayBeOldLabel";
            this.newFromBrowserStateMayBeOldLabel.Size = new System.Drawing.Size(124, 26);
            this.newFromBrowserStateMayBeOldLabel.TabIndex = 1;
            this.newFromBrowserStateMayBeOldLabel.Text = "browser state may be old\r\n(page refresh needed)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Current opened .wowvrc path";
            // 
            // mergeGroupBox
            // 
            this.mergeGroupBox.Location = new System.Drawing.Point(20, 181);
            this.mergeGroupBox.Name = "mergeGroupBox";
            this.mergeGroupBox.Size = new System.Drawing.Size(548, 100);
            this.mergeGroupBox.TabIndex = 3;
            this.mergeGroupBox.TabStop = false;
            this.mergeGroupBox.Text = "merge .wowvrc";
            // 
            // exportButton
            // 
            this.exportButton.Enabled = false;
            this.exportButton.Location = new System.Drawing.Point(20, 317);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(548, 23);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "export .fbx and .fbx.wowvrc.meta from opened .wowvrc";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click_1);
            // 
            // browserTab
            // 
            this.browserTab.Controls.Add(this.browserContentPanel);
            this.browserTab.Controls.Add(this.addressPanel);
            this.browserTab.Location = new System.Drawing.Point(4, 22);
            this.browserTab.Name = "browserTab";
            this.browserTab.Padding = new System.Windows.Forms.Padding(3);
            this.browserTab.Size = new System.Drawing.Size(792, 424);
            this.browserTab.TabIndex = 0;
            this.browserTab.Text = "browser";
            this.browserTab.UseVisualStyleBackColor = true;
            // 
            // browserContentPanel
            // 
            this.browserContentPanel.Controls.Add(this.webControl);
            this.browserContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserContentPanel.Location = new System.Drawing.Point(3, 58);
            this.browserContentPanel.Name = "browserContentPanel";
            this.browserContentPanel.Size = new System.Drawing.Size(786, 363);
            this.browserContentPanel.TabIndex = 4;
            // 
            // addressPanel
            // 
            this.addressPanel.Controls.Add(this.urlButtonsPanel);
            this.addressPanel.Controls.Add(this.addressTextboxPanel);
            this.addressPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.addressPanel.Location = new System.Drawing.Point(3, 3);
            this.addressPanel.Name = "addressPanel";
            this.addressPanel.Size = new System.Drawing.Size(786, 55);
            this.addressPanel.TabIndex = 3;
            // 
            // urlButtonsPanel
            // 
            this.urlButtonsPanel.BackColor = System.Drawing.Color.Transparent;
            this.urlButtonsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.urlButtonsPanel.Controls.Add(this.submeshIndexTextbox);
            this.urlButtonsPanel.Controls.Add(this.drawOnlySelectedSumeshCheckbox);
            this.urlButtonsPanel.Controls.Add(this.consoleLogModelsButton);
            this.urlButtonsPanel.Controls.Add(this.navigateToCharacterSearchButton);
            this.urlButtonsPanel.Controls.Add(this.navigateToDressroomButton);
            this.urlButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlButtonsPanel.Location = new System.Drawing.Point(0, 22);
            this.urlButtonsPanel.Name = "urlButtonsPanel";
            this.urlButtonsPanel.Size = new System.Drawing.Size(786, 33);
            this.urlButtonsPanel.TabIndex = 1;
            // 
            // submeshIndexTextbox
            // 
            this.submeshIndexTextbox.Location = new System.Drawing.Point(350, 5);
            this.submeshIndexTextbox.Name = "submeshIndexTextbox";
            this.submeshIndexTextbox.Size = new System.Drawing.Size(46, 20);
            this.submeshIndexTextbox.TabIndex = 6;
            this.submeshIndexTextbox.TextChanged += new System.EventHandler(this.submeshIndexTextbox_TextChanged);
            // 
            // drawOnlySelectedSumeshCheckbox
            // 
            this.drawOnlySelectedSumeshCheckbox.AutoSize = true;
            this.drawOnlySelectedSumeshCheckbox.Location = new System.Drawing.Point(226, 8);
            this.drawOnlySelectedSumeshCheckbox.Name = "drawOnlySelectedSumeshCheckbox";
            this.drawOnlySelectedSumeshCheckbox.Size = new System.Drawing.Size(118, 17);
            this.drawOnlySelectedSumeshCheckbox.TabIndex = 5;
            this.drawOnlySelectedSumeshCheckbox.Text = "only submesh index";
            this.drawOnlySelectedSumeshCheckbox.UseVisualStyleBackColor = true;
            this.drawOnlySelectedSumeshCheckbox.CheckedChanged += new System.EventHandler(this.drawOnlySelectedSumeshCheckbox_CheckedChanged);
            // 
            // consoleLogModelsButton
            // 
            this.consoleLogModelsButton.Location = new System.Drawing.Point(411, 4);
            this.consoleLogModelsButton.Name = "consoleLogModelsButton";
            this.consoleLogModelsButton.Size = new System.Drawing.Size(129, 23);
            this.consoleLogModelsButton.TabIndex = 3;
            this.consoleLogModelsButton.Text = "console.log() models";
            this.consoleLogModelsButton.UseVisualStyleBackColor = true;
            this.consoleLogModelsButton.Click += new System.EventHandler(this.getModelsButton_Click);
            // 
            // navigateToCharacterSearchButton
            // 
            this.navigateToCharacterSearchButton.Location = new System.Drawing.Point(85, 3);
            this.navigateToCharacterSearchButton.Name = "navigateToCharacterSearchButton";
            this.navigateToCharacterSearchButton.Size = new System.Drawing.Size(105, 23);
            this.navigateToCharacterSearchButton.TabIndex = 2;
            this.navigateToCharacterSearchButton.Text = "character search";
            this.navigateToCharacterSearchButton.UseVisualStyleBackColor = true;
            this.navigateToCharacterSearchButton.Click += new System.EventHandler(this.navigateToCharacterSearchButton_Click);
            // 
            // navigateToDressroomButton
            // 
            this.navigateToDressroomButton.Location = new System.Drawing.Point(4, 3);
            this.navigateToDressroomButton.Name = "navigateToDressroomButton";
            this.navigateToDressroomButton.Size = new System.Drawing.Size(75, 23);
            this.navigateToDressroomButton.TabIndex = 0;
            this.navigateToDressroomButton.Text = "dressroom";
            this.navigateToDressroomButton.UseVisualStyleBackColor = true;
            this.navigateToDressroomButton.Click += new System.EventHandler(this.navigateToDressroomButton_Click);
            // 
            // addressTextboxPanel
            // 
            this.addressTextboxPanel.Controls.Add(this.addressTextBox);
            this.addressTextboxPanel.Controls.Add(this.goButton);
            this.addressTextboxPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.addressTextboxPanel.Location = new System.Drawing.Point(0, 0);
            this.addressTextboxPanel.Name = "addressTextboxPanel";
            this.addressTextboxPanel.Size = new System.Drawing.Size(786, 22);
            this.addressTextboxPanel.TabIndex = 7;
            // 
            // addressTextBox
            // 
            this.addressTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addressTextBox.Location = new System.Drawing.Point(0, 0);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(743, 20);
            this.addressTextBox.TabIndex = 0;
            this.addressTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.addressTextBox_KeyPress);
            // 
            // goButton
            // 
            this.goButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.goButton.Location = new System.Drawing.Point(743, 0);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(43, 22);
            this.goButton.TabIndex = 1;
            this.goButton.Text = "go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // devToolsTab
            // 
            this.devToolsTab.Controls.Add(this.devToolsContent);
            this.devToolsTab.Controls.Add(this.devToolsHeader);
            this.devToolsTab.Location = new System.Drawing.Point(4, 22);
            this.devToolsTab.Name = "devToolsTab";
            this.devToolsTab.Size = new System.Drawing.Size(792, 424);
            this.devToolsTab.TabIndex = 2;
            this.devToolsTab.Text = "dev tools";
            this.devToolsTab.UseVisualStyleBackColor = true;
            // 
            // devToolsContent
            // 
            this.devToolsContent.BackColor = System.Drawing.Color.DarkGray;
            this.devToolsContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devToolsContent.Location = new System.Drawing.Point(0, 39);
            this.devToolsContent.Name = "devToolsContent";
            this.devToolsContent.Size = new System.Drawing.Size(792, 385);
            this.devToolsContent.TabIndex = 1;
            // 
            // devToolsHeader
            // 
            this.devToolsHeader.Controls.Add(this.showDevToolsCheckbox);
            this.devToolsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.devToolsHeader.Location = new System.Drawing.Point(0, 0);
            this.devToolsHeader.Name = "devToolsHeader";
            this.devToolsHeader.Size = new System.Drawing.Size(792, 39);
            this.devToolsHeader.TabIndex = 0;
            // 
            // showDevToolsCheckbox
            // 
            this.showDevToolsCheckbox.AutoSize = true;
            this.showDevToolsCheckbox.Location = new System.Drawing.Point(8, 12);
            this.showDevToolsCheckbox.Name = "showDevToolsCheckbox";
            this.showDevToolsCheckbox.Size = new System.Drawing.Size(97, 17);
            this.showDevToolsCheckbox.TabIndex = 0;
            this.showDevToolsCheckbox.Text = "show dev tools";
            this.showDevToolsCheckbox.UseVisualStyleBackColor = true;
            this.showDevToolsCheckbox.CheckedChanged += new System.EventHandler(this.showDevToolsCheckbox_CheckedChanged);
            // 
            // utilityTab
            // 
            this.utilityTab.Controls.Add(this.openCacheDirectoryButton);
            this.utilityTab.Location = new System.Drawing.Point(4, 22);
            this.utilityTab.Name = "utilityTab";
            this.utilityTab.Size = new System.Drawing.Size(792, 424);
            this.utilityTab.TabIndex = 3;
            this.utilityTab.Text = "utility";
            this.utilityTab.UseVisualStyleBackColor = true;
            // 
            // openCacheDirectoryButton
            // 
            this.openCacheDirectoryButton.Location = new System.Drawing.Point(8, 16);
            this.openCacheDirectoryButton.Name = "openCacheDirectoryButton";
            this.openCacheDirectoryButton.Size = new System.Drawing.Size(148, 23);
            this.openCacheDirectoryButton.TabIndex = 4;
            this.openCacheDirectoryButton.Text = "open wh loader cache dir";
            this.openCacheDirectoryButton.UseVisualStyleBackColor = true;
            this.openCacheDirectoryButton.Click += new System.EventHandler(this.openCacheDirectoryButton_Click);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.browserToolStripMenuItem.Text = "Browser";
            // 
            // showDevToolsTabToolStripMenuItem
            // 
            this.showDevToolsTabToolStripMenuItem.Name = "showDevToolsTabToolStripMenuItem";
            this.showDevToolsTabToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showDevToolsTabToolStripMenuItem.Text = "Show dev tools tab";
            // 
            // prepareForVrchatCheckbox
            // 
            this.prepareForVrchatCheckbox.AutoSize = true;
            this.prepareForVrchatCheckbox.Checked = true;
            this.prepareForVrchatCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.prepareForVrchatCheckbox.Location = new System.Drawing.Point(20, 294);
            this.prepareForVrchatCheckbox.Name = "prepareForVrchatCheckbox";
            this.prepareForVrchatCheckbox.Size = new System.Drawing.Size(117, 17);
            this.prepareForVrchatCheckbox.TabIndex = 8;
            this.prepareForVrchatCheckbox.Text = "prepare for VRChat";
            this.prepareForVrchatCheckbox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabs);
            this.Name = "Form1";
            this.Text = "WoW -> VRChat";
            this.tabs.ResumeLayout(false);
            this.mainTab.ResumeLayout(false);
            this.mainTab.PerformLayout();
            this.openGroupbox.ResumeLayout(false);
            this.createNewGroupbox.ResumeLayout(false);
            this.createNewGroupbox.PerformLayout();
            this.browserTab.ResumeLayout(false);
            this.browserContentPanel.ResumeLayout(false);
            this.addressPanel.ResumeLayout(false);
            this.urlButtonsPanel.ResumeLayout(false);
            this.urlButtonsPanel.PerformLayout();
            this.addressTextboxPanel.ResumeLayout(false);
            this.addressTextboxPanel.PerformLayout();
            this.devToolsTab.ResumeLayout(false);
            this.devToolsHeader.ResumeLayout(false);
            this.devToolsHeader.PerformLayout();
            this.utilityTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button newFromBrowserStateButton;
        private EO.WinForm.WebControl webControl;
        private EO.WebBrowser.WebView webView;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage browserTab;
        private System.Windows.Forms.Panel browserContentPanel;
        private System.Windows.Forms.Panel addressPanel;
        private System.Windows.Forms.TabPage mainTab;
        private System.Windows.Forms.Panel urlButtonsPanel;
        private System.Windows.Forms.Button navigateToDressroomButton;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Button navigateToCharacterSearchButton;
        private System.Windows.Forms.Button consoleLogModelsButton;
        private System.Windows.Forms.TabPage devToolsTab;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDevToolsTabToolStripMenuItem;
        private System.Windows.Forms.Panel devToolsContent;
        private System.Windows.Forms.Panel devToolsHeader;
        private System.Windows.Forms.CheckBox showDevToolsCheckbox;
        private System.Windows.Forms.Button openCacheDirectoryButton;
        private System.Windows.Forms.TextBox submeshIndexTextbox;
        private System.Windows.Forms.CheckBox drawOnlySelectedSumeshCheckbox;
        private System.Windows.Forms.Label newFromBrowserStateMayBeOldLabel;
        private System.Windows.Forms.TabPage utilityTab;
        private System.Windows.Forms.GroupBox mergeGroupBox;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Panel addressTextboxPanel;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.GroupBox createNewGroupbox;
        private System.Windows.Forms.Button newFromRaceGenderButton;
        private System.Windows.Forms.CheckBox isMaleCheckbox;
        private System.Windows.Forms.ComboBox raceCombobox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox openGroupbox;
        private System.Windows.Forms.TextBox filenameTextbox;
        private System.Windows.Forms.Button openExistingButton;
        private System.Windows.Forms.CheckBox prepareForVrchatCheckbox;
    }
}

