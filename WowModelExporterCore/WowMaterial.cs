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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var objWowMaterial = obj as WowMaterial;

            if (objWowMaterial == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            // MainImage не учитываем потому что он сейчас всегда равен одной из 4х image-ей

            if (!Equals(Image1, objWowMaterial.Image1))
                return false;
            if (!Equals(Image2, objWowMaterial.Image2))
                return false;
            if (!Equals(Image3, objWowMaterial.Image3))
                return false;
            if (!Equals(Image4, objWowMaterial.Image4))
                return false;

            if (!Equals(BothSides, objWowMaterial.BothSides))
                return false;

            if (!Equals(Type, objWowMaterial.Type))
                return false;

            // ToDo: когда появятся еще поля (свойства материала) - добавить их сюда тоже

            return true;
        }

        public string GetUniqueName()
        {
            var texturesHash = string.Format("{0:X}",
                (
                    (Image1?.Hash ?? "null") +
                    (Image2?.Hash ?? "null") +
                    (Image3?.Hash ?? "null") +
                    (Image4?.Hash ?? "null")
                ).GetHashCode());

            return Type.ToString() + "_" + (BothSides ? "both" : "cull") + "_" + texturesHash;
        }

        public enum MaterialType
        {
            Opaque,
            Cutout,
            Transparent
        }
    }
}
