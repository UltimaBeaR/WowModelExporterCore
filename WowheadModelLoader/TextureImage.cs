using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WowheadModelLoader
{
    public class TextureImage
    {
        public string Hash { get; set; }

        public Bitmap Bitmap { get; set; }

        public static TextureImage FromByteArray(byte[] binary)
        {
            if (binary == null || binary.Length == 0)
                return null;

            var result = new TextureImage();

            using (var ms = new MemoryStream(binary))
                result.Bitmap = (Bitmap)Image.FromStream(ms);

            result.RecalculateHash();

            return result;
        }

        /// <summary>
        /// Начать изменять Bitmap. После вызова Dispose на позваращемом объекте битмап меняться не должен (в этот момент будет просчитан его хэш)
        /// </summary>
        /// <returns></returns>
        public IDisposable ChangeBitmap()
        {
            return new ChangeBitmapAndRecalculateHash(this);
        }

        public override int GetHashCode()
        {
            return Hash?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            var objTextureImage = obj as TextureImage;

            if (objTextureImage == null)
                return false;

            return objTextureImage.Hash == Hash;
        }

        private void RecalculateHash()
        {
            if (Bitmap == null)
            {
                Hash = null;
                return;
            }

            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                Bitmap.Save(ms, ImageFormat.Png);
                bytes = ms.ToArray();
            }

            var sha1 = new SHA1CryptoServiceProvider();
            byte[] hash = sha1.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }

            Hash = sb.ToString();
        }

        public class ChangeBitmapAndRecalculateHash : IDisposable
        {
            public ChangeBitmapAndRecalculateHash(TextureImage textureImage)
            {
                _textureImage = textureImage;
            }

            public void Dispose()
            {
                _textureImage.RecalculateHash();
            }

            private TextureImage _textureImage;
        }
    }
}
