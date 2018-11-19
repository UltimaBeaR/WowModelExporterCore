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
            this.utilityTab = new System.Windows.Forms.TabPage();
            this.addressPanel = new System.Windows.Forms.Panel();
            this.browserContentPanel = new System.Windows.Forms.Panel();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.urlButtonsPanel = new System.Windows.Forms.Panel();
            this.navigateToDressroomButton = new System.Windows.Forms.Button();
            this.tabs.SuspendLayout();
            this.browserTab.SuspendLayout();
            this.utilityTab.SuspendLayout();
            this.addressPanel.SuspendLayout();
            this.browserContentPanel.SuspendLayout();
            this.urlButtonsPanel.SuspendLayout();
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
            this.textBox1.Size = new System.Drawing.Size(611, 307);
            this.textBox1.TabIndex = 1;
            // 
            // webControl
            // 
            this.webControl.BackColor = System.Drawing.Color.White;
            this.webControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl.Location = new System.Drawing.Point(0, 0);
            this.webControl.Name = "webControl";
            this.webControl.Size = new System.Drawing.Size(786, 356);
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
            // addressPanel
            // 
            this.addressPanel.Controls.Add(this.urlButtonsPanel);
            this.addressPanel.Controls.Add(this.addressTextBox);
            this.addressPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.addressPanel.Location = new System.Drawing.Point(3, 3);
            this.addressPanel.Name = "addressPanel";
            this.addressPanel.Size = new System.Drawing.Size(786, 62);
            this.addressPanel.TabIndex = 3;
            // 
            // browserContentPanel
            // 
            this.browserContentPanel.Controls.Add(this.webControl);
            this.browserContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserContentPanel.Location = new System.Drawing.Point(3, 65);
            this.browserContentPanel.Name = "browserContentPanel";
            this.browserContentPanel.Size = new System.Drawing.Size(786, 356);
            this.browserContentPanel.TabIndex = 4;
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
            // urlButtonsPanel
            // 
            this.urlButtonsPanel.Controls.Add(this.navigateToDressroomButton);
            this.urlButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlButtonsPanel.Location = new System.Drawing.Point(0, 20);
            this.urlButtonsPanel.Name = "urlButtonsPanel";
            this.urlButtonsPanel.Size = new System.Drawing.Size(786, 42);
            this.urlButtonsPanel.TabIndex = 1;
            // 
            // navigateToDressroomButton
            // 
            this.navigateToDressroomButton.Location = new System.Drawing.Point(3, 3);
            this.navigateToDressroomButton.Name = "navigateToDressroomButton";
            this.navigateToDressroomButton.Size = new System.Drawing.Size(75, 23);
            this.navigateToDressroomButton.TabIndex = 0;
            this.navigateToDressroomButton.Text = "dressroom";
            this.navigateToDressroomButton.UseVisualStyleBackColor = true;
            this.navigateToDressroomButton.Click += new System.EventHandler(this.navigateToDressroomButton_Click);
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
            this.utilityTab.ResumeLayout(false);
            this.utilityTab.PerformLayout();
            this.addressPanel.ResumeLayout(false);
            this.addressPanel.PerformLayout();
            this.browserContentPanel.ResumeLayout(false);
            this.urlButtonsPanel.ResumeLayout(false);
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
    }
}

