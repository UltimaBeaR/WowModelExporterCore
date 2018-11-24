using System;
using System.Windows.Forms;
using WowModelExporterCore;
using WowheadModelLoader;
using WowModelExporterFbx;
using System.Collections.Generic;

namespace WowModelExporterTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            OptsJsonForExport = null;
            GetModels = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            InitBrowser();
        }

        private void InitBrowser()
        {
            webView.UrlChanged += (s, e) =>
            {
                //OptsJsonForExport = null;
                addressTextBox.Text = webView.Url;
            };

            webView.NewWindow += (s, e) => { e.Accepted = true; };

            InitInterceptor();
        }

        private void InitInterceptor()
        {
            var injections = new List<WebViewJsInjection>();

            injections.Add(CreateInjection_ZamModelViewerContructor());
            injections.Add(CreateInjection_WebGlDrawFunction());

            var interceptor = new WebViewJsInterceptor(webView, injections);
        }

        private WebViewJsInjection CreateInjection_ZamModelViewerContructor()
        {
            var injection = new WebViewJsInjection(
                "/modelviewer/viewer/viewer.min.js$",
                "function ZamModelViewer(opts){",
                "opts"
            );

            injection.Intercepted += (sender, getData) =>
            {
                OptsJsonForExport = getData();
            };

            return injection;
        }

        private WebViewJsInjection CreateInjection_WebGlDrawFunction()
        {
            var injection = new WebViewJsInjection(
                "/modelviewer/viewer/viewer.min.js$",
                "draw:function(){var self=this,gl=self.context,i;var time=self.getTime();self.delta=(time-self.time)*.001;self.time=time;self.updateCamera();gl.viewport(0,0,self.width,self.height);gl.clear(gl.COLOR_BUFFER_BIT|gl.DEPTH_BUFFER_BIT);",
                @"(function () {
                    console.log(self.models);
                    //self.models
                    return 'test';
                })(this)"
            );

            injection.Intercepted += (sender, getData) =>
            {
                GetModels = getData;
            };

            return injection;
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

        private Func<string> GetModels
        {
            get => _getModels;

            set
            {
                _getModels = value;

                getModelsButton.Enabled = value != null;
            }
        }

        private string _optsJsonForExport;
        private Func<string> _getModels;

        private void getModelsButton_Click(object sender, EventArgs e)
        {
            var json = GetModels();
        }

        private void showDevToolsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            webView.HideDevTools();

            if (showDevToolsCheckbox.Checked)
                webView.ShowDevTools(devToolsContent.Handle);
        }
    }
}
