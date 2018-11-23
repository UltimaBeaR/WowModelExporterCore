﻿using System;
using System.Windows.Forms;
using WowModelExporterCore;
using WowheadModelLoader;
using WowModelExporterFbx;

namespace WowModelExporterTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            OptsJsonForExport = null;

            InitBrowser();
        }

        private void InitBrowser()
        {
            webView.UrlChanged += (obj, ev) =>
            {
                //OptsJsonForExport = null;
                addressTextBox.Text = webView.Url;
            };

            webView.NewWindow += (obj, ev) => { ev.Accepted = true; };

            var interceptor = new WebViewOptsInterceptor(webView);
            interceptor.OptsIntercepted += (json) =>
            {
                OptsJsonForExport = json;
            };
        }

        private void navigateToDressroomButton_Click(object sender, EventArgs e)
        {
            webView.Url = "https://www.wowhead.com/dressing-room";
        }

        private void navigateToCharacterSearchButton_Click(object sender, EventArgs e)
        {
            webView.Url = "https://www.wowhead.com/list";
        }

        private void addressTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                webView.Url = addressTextBox.Text;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            var exporter = new WowModelExporter();

            var wowObject = exporter.LoadCharacter(OptsJsonForExport);

            PrepareForVRChatUtility.PrepareObject(wowObject, true, true);

            var fbxExporter = new Exporter();

            fbxExporter.ExportWowObject(wowObject, "newtest").ToString();

            MessageBox.Show("Готово");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var exporter = new WowModelExporter();

            var wowObject = exporter.LoadCharacter(WhRace.HUMAN, WhGender.MALE, new string[]
            {
                // шлем
                "161600",
                // плечи
                "161621",
                // плащ
                "163355",
                // чест
                "161602",
                // брасы
                "161629",
                // руки
                "161610",
                // пояс
                "161624",
                // ноги
                "161616",
                // ступни
                "161605"
            });

            PrepareForVRChatUtility.PrepareObject(wowObject, true, true);

            var test = new Exporter();

            textBox1.Text = test.ExportWowObject(wowObject, "newtest").ToString();
        }

        private string OptsJsonForExport
        {
            get => _optsJsonForExport;

            set
            {
                _optsJsonForExport = value;

                exportButton.Enabled = value != null;
            }
        }

        private string _optsJsonForExport;
    }
}
