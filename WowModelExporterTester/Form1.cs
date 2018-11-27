using System;
using System.Windows.Forms;
using WowModelExporterCore;
using WowheadModelLoader;
using WowModelExporterFbx;
using System.Collections.Generic;
using WebViewJsModifier;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WowModelExporterTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            OptsJsonForExport = null;
            ConsoleLogModelsFunction = null;
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

            InitJsModifier();
        }

        private void InitJsModifier()
        {
            var jsModifyActions = new List<JsModifyAction>();

            jsModifyActions.Add(CreateJsModifyAction_ZamModelViewerContructor());
            jsModifyActions.Add(CreateJsModifyAction_WebGlDrawFunction());
            jsModifyActions.Add(CreateJsModifyAction_TestTextReplace());
            jsModifyActions.Add(CreateJsModifyAction_RenderTexUnitsByMeshIndex());

            new JsModifier(webView, jsModifyActions);
        }

        private InterceptDataJsModifyAction CreateJsModifyAction_ZamModelViewerContructor()
        {
            var jsModifyAction = new InterceptDataJsModifyAction(
                "/modelviewer/viewer/viewer.min.js$",
                new[] { "function ZamModelViewer(opts){" },
                "opts"
            );

            jsModifyAction.Intercepted += (sender, getData) =>
            {
                OptsJsonForExport = getData();
            };

            return jsModifyAction;
        }

        private InterceptDataJsModifyAction CreateJsModifyAction_WebGlDrawFunction()
        {
            var jsModifyAction = new InterceptDataJsModifyAction(
                "/modelviewer/viewer/viewer.min.js$",
                new[] { "draw:function(){var self=this,gl=self.context,i;var time=self.getTime();self.delta=(time-self.time)*.001;self.time=time;self.updateCamera();gl.viewport(0,0,self.width,self.height);gl.clear(gl.COLOR_BUFFER_BIT|gl.DEPTH_BUFFER_BIT);" },
                @"(function () {
                    console.log(self.models);
                    return null;
                })(this)"
            );

            jsModifyAction.Intercepted += (sender, getData) =>
            {
                ConsoleLogModelsFunction = getData;
            };

            return jsModifyAction;
        }

        private TextReplaceJsModifyAction CreateJsModifyAction_TestTextReplace()
        {
            var jsModifyAction = new TextReplaceJsModifyAction(
                "/modelviewer/viewer/viewer.min.js$",
                new[] { "Wow.AnimatedQuat.getValue(self.rotation,anim.index,time,self.tmpQuat);" },
                "mat4.fromQuat(self.tmpMat,self.tmpQuat);mat4.transpose(self.tmpMat,self.tmpMat);",
                "quat.invert(self.tmpQuat, self.tmpQuat);    mat4.fromQuat(self.tmpMat,self.tmpQuat);"
            );

            return jsModifyAction;
        }

        private TextReplaceJsModifyAction CreateJsModifyAction_RenderTexUnitsByMeshIndex()
        {
            var jsModifyAction = new TextReplaceJsModifyAction(
                "/modelviewer/viewer/viewer.min.js$",
                new[] { "ZamModelViewer.Wow.TexUnit=function(r)", "draw:function(){" },
                "",
                "if (window.g___visibleMeshIndex != undefined && this.meshIndex != window.g___visibleMeshIndex) return;"
            );

            return jsModifyAction;
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

            var dialog = new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                ShowPlacesList = true,
                Title = "Choose directory to export .fbx and it's dependencies",
                InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath),
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                fbxExporter.ExportWowObject(wowObject, dialog.FileName).ToString();

                System.Diagnostics.Process.Start("explorer.exe", dialog.FileName);
            }
            else
            {
                MessageBox.Show("Export canceled");
            }
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

        private Func<string> ConsoleLogModelsFunction
        {
            get => _consoleLogModelsFunction;

            set
            {
                _consoleLogModelsFunction = value;

                consoleLogModelsButton.Enabled = value != null;
            }
        }

        private string _optsJsonForExport;
        private Func<string> _consoleLogModelsFunction;

        private void getModelsButton_Click(object sender, EventArgs e)
        {
            ConsoleLogModelsFunction();
        }

        private void showDevToolsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            webView.HideDevTools();

            if (showDevToolsCheckbox.Checked)
                webView.ShowDevTools(devToolsContent.Handle);
        }

        private void openCacheDirectoryButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", DataLoaderBase.CacheDirectory);
        }

        private void drawOnlySelectedSumeshCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVisibleMesh();
        }

        private void submeshIndexTextbox_TextChanged(object sender, EventArgs e)
        {
            UpdateVisibleMesh();
        }

        private void UpdateVisibleMesh()
        {
            if (drawOnlySelectedSumeshCheckbox.Checked)
                webView.EvalScript("window.g___visibleMeshIndex = " + submeshIndexTextbox.Text + ";");
            else
                webView.EvalScript("window.g___visibleMeshIndex = undefined;");
        }
    }
}
