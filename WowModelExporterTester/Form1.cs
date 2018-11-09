using System;
using System.Windows.Forms;
using WowModelExporterCore;

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

            var model = exporter.LoadModel("humanmale", new string[]
            {
                // шлем
                "161600",

                // плечи
                "161621",

                // плащ
                "163365",

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


            var texture = exporter.GetFirstTexture(model);


            textBox1.Text = model.Vertices.Length.ToString();
        }
    }
}
