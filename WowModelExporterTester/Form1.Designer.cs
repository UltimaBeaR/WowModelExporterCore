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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.webControl = new EO.WinForm.WebControl();
            this.webView = new EO.WebBrowser.WebView();
            this.tabs = new System.Windows.Forms.TabControl();
            this.browserTab = new System.Windows.Forms.TabPage();
            this.browserContentPanel = new System.Windows.Forms.Panel();
            this.addressPanel = new System.Windows.Forms.Panel();
            this.urlButtonsPanel = new System.Windows.Forms.Panel();
            this.consoleLogModelsButton = new System.Windows.Forms.Button();
            this.navigateToCharacterSearchButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.navigateToDressroomButton = new System.Windows.Forms.Button();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.devToolsTab = new System.Windows.Forms.TabPage();
            this.devToolsContent = new System.Windows.Forms.Panel();
            this.devToolsHeader = new System.Windows.Forms.Panel();
            this.showDevToolsCheckbox = new System.Windows.Forms.CheckBox();
            this.utilityTab = new System.Windows.Forms.TabPage();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDevToolsTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabs.SuspendLayout();
            this.browserTab.SuspendLayout();
            this.browserContentPanel.SuspendLayout();
            this.addressPanel.SuspendLayout();
            this.urlButtonsPanel.SuspendLayout();
            this.devToolsTab.SuspendLayout();
            this.devToolsHeader.SuspendLayout();
            this.utilityTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 35);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(156, 145);
            this.textBox1.TabIndex = 1;
            // 
            // webControl
            // 
            this.webControl.BackColor = System.Drawing.Color.Gray;
            this.webControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl.Location = new System.Drawing.Point(0, 0);
            this.webControl.Name = "webControl";
            this.webControl.Size = new System.Drawing.Size(786, 361);
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
            this.browserContentPanel.Location = new System.Drawing.Point(3, 60);
            this.browserContentPanel.Name = "browserContentPanel";
            this.browserContentPanel.Size = new System.Drawing.Size(786, 361);
            this.browserContentPanel.TabIndex = 4;
            // 
            // addressPanel
            // 
            this.addressPanel.Controls.Add(this.urlButtonsPanel);
            this.addressPanel.Controls.Add(this.addressTextBox);
            this.addressPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.addressPanel.Location = new System.Drawing.Point(3, 3);
            this.addressPanel.Name = "addressPanel";
            this.addressPanel.Size = new System.Drawing.Size(786, 57);
            this.addressPanel.TabIndex = 3;
            // 
            // urlButtonsPanel
            // 
            this.urlButtonsPanel.BackColor = System.Drawing.Color.Transparent;
            this.urlButtonsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.urlButtonsPanel.Controls.Add(this.consoleLogModelsButton);
            this.urlButtonsPanel.Controls.Add(this.navigateToCharacterSearchButton);
            this.urlButtonsPanel.Controls.Add(this.exportButton);
            this.urlButtonsPanel.Controls.Add(this.navigateToDressroomButton);
            this.urlButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlButtonsPanel.Location = new System.Drawing.Point(0, 20);
            this.urlButtonsPanel.Name = "urlButtonsPanel";
            this.urlButtonsPanel.Size = new System.Drawing.Size(786, 37);
            this.urlButtonsPanel.TabIndex = 1;
            // 
            // consoleLogModelsButton
            // 
            this.consoleLogModelsButton.Location = new System.Drawing.Point(607, 5);
            this.consoleLogModelsButton.Name = "consoleLogModelsButton";
            this.consoleLogModelsButton.Size = new System.Drawing.Size(129, 23);
            this.consoleLogModelsButton.TabIndex = 3;
            this.consoleLogModelsButton.Text = "console.log() models";
            this.consoleLogModelsButton.UseVisualStyleBackColor = true;
            this.consoleLogModelsButton.Click += new System.EventHandler(this.getModelsButton_Click);
            // 
            // navigateToCharacterSearchButton
            // 
            this.navigateToCharacterSearchButton.Location = new System.Drawing.Point(224, 5);
            this.navigateToCharacterSearchButton.Name = "navigateToCharacterSearchButton";
            this.navigateToCharacterSearchButton.Size = new System.Drawing.Size(105, 23);
            this.navigateToCharacterSearchButton.TabIndex = 2;
            this.navigateToCharacterSearchButton.Text = "character search";
            this.navigateToCharacterSearchButton.UseVisualStyleBackColor = true;
            this.navigateToCharacterSearchButton.Click += new System.EventHandler(this.navigateToCharacterSearchButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(4, 5);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 1;
            this.exportButton.Text = "export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // navigateToDressroomButton
            // 
            this.navigateToDressroomButton.Location = new System.Drawing.Point(143, 5);
            this.navigateToDressroomButton.Name = "navigateToDressroomButton";
            this.navigateToDressroomButton.Size = new System.Drawing.Size(75, 23);
            this.navigateToDressroomButton.TabIndex = 0;
            this.navigateToDressroomButton.Text = "dressroom";
            this.navigateToDressroomButton.UseVisualStyleBackColor = true;
            this.navigateToDressroomButton.Click += new System.EventHandler(this.navigateToDressroomButton_Click);
            // 
            // addressTextBox
            // 
            this.addressTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.addressTextBox.Location = new System.Drawing.Point(0, 0);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(786, 20);
            this.addressTextBox.TabIndex = 0;
            this.addressTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.addressTextBox_KeyPress);
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
            this.utilityTab.Controls.Add(this.textBox1);
            this.utilityTab.Controls.Add(this.button1);
            this.utilityTab.Location = new System.Drawing.Point(4, 22);
            this.utilityTab.Name = "utilityTab";
            this.utilityTab.Padding = new System.Windows.Forms.Padding(3);
            this.utilityTab.Size = new System.Drawing.Size(792, 424);
            this.utilityTab.TabIndex = 1;
            this.utilityTab.Text = "utility";
            this.utilityTab.UseVisualStyleBackColor = true;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabs);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabs.ResumeLayout(false);
            this.browserTab.ResumeLayout(false);
            this.browserContentPanel.ResumeLayout(false);
            this.addressPanel.ResumeLayout(false);
            this.addressPanel.PerformLayout();
            this.urlButtonsPanel.ResumeLayout(false);
            this.devToolsTab.ResumeLayout(false);
            this.devToolsHeader.ResumeLayout(false);
            this.devToolsHeader.PerformLayout();
            this.utilityTab.ResumeLayout(false);
            this.utilityTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private EO.WinForm.WebControl webControl;
        private EO.WebBrowser.WebView webView;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage browserTab;
        private System.Windows.Forms.Panel browserContentPanel;
        private System.Windows.Forms.Panel addressPanel;
        private System.Windows.Forms.TabPage utilityTab;
        private System.Windows.Forms.Panel urlButtonsPanel;
        private System.Windows.Forms.Button navigateToDressroomButton;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button navigateToCharacterSearchButton;
        private System.Windows.Forms.Button consoleLogModelsButton;
        private System.Windows.Forms.TabPage devToolsTab;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDevToolsTabToolStripMenuItem;
        private System.Windows.Forms.Panel devToolsContent;
        private System.Windows.Forms.Panel devToolsHeader;
        private System.Windows.Forms.CheckBox showDevToolsCheckbox;
    }
}

