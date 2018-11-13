using System;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var exporter = new WowModelExporter();

            var wowObject = exporter.LoadCharacter(WhRace.HUMAN, WhGender.MALE, new string[]
            {
                //// шлем
                //"161600",
                //// плечи
                //"161621",
                //// плащ
                //"163355",
                //// чест
                //"161602",
                //// брасы
                //"161629",
                //// руки
                //"161610",
                //// пояс
                //"161624",
                //// ноги
                //"161616",
                //// ступни
                //"161605"
            });

            var test = new Exporter();

            textBox1.Text = test.ExportWowObject(wowObject, "tessst.fbx").ToString();
        }
    }
}
