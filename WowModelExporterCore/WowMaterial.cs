using System.Drawing;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowMaterial
    {
        //public TextureImage DiffuseImage { get; set; }

        // ToDo: сделал временно (это ссылка на 1ин из 4х картинок)
        public TextureImage MainImage { get; set; }

        public TextureImage Image1 { get; set; }
        public TextureImage Image2 { get; set; }
        public TextureImage Image3 { get; set; }
        public TextureImage Image4 { get; set; }

        /// <summary>
        /// Рисовать ли с двух сторон (лицевая и обратная стороны). Если false то только с лицевой.
        /// </summary>
        public bool BothSides { get; set; }

        public MaterialType Type { get; set; }

        public enum MaterialType
        {
            Opaque,
            Cutout,
            Transparent
        }
    }
}
