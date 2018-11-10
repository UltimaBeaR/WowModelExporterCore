using System.Drawing;

namespace WowModelExporterCore
{
    public class WowMaterial
    {
        //public Bitmap DiffuseImage { get; set; }

        // ToDo: сделал временно (это ссылка на 1ин из 4х картинок)
        public Bitmap MainImage { get; set; }

        public Bitmap Image1 { get; set; }
        public Bitmap Image2 { get; set; }
        public Bitmap Image3 { get; set; }
        public Bitmap Image4 { get; set; }

        //public enum Kind
        //{
        //    Opaque,
        //    Cutout,
        //    Transparent
        //}
    }
}
