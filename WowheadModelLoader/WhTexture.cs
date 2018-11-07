using System.Drawing;

namespace WowheadModelLoader
{
    public class WhTexture
    {
        // ToDo: так как тут идет ссылка на Bitmap, который IDispossable, возможно стоит этот класс тоже сделать IDispossable (у js варианта есть destroy метод)

        // ToDo: незнаю пока какого типа index, пока будет int (возможно он не используется, тогда убрать его вообще)
        // Иногда сюда присваивается значение из енума Region, а иногда 0 или 1 (которые по энаму региона явно ничего не значат), то есть видмо разное может означать этот индекс
        public WhTexture(WhModel model, int index, uint file)
        {
            Model = model;
            Index = index;

            // url формировать и сохранять тут не буду, т.к. он нафиг не нужен
            // также не буду добавлять alphaimg, mergedimg и текстуры, так как по сути это все одно и то же (= Img)

            Img = null;

            Loaded = false;

            LoadAndHandle_Texture(file);
        }

        public WhModel Model { get; set; }
        public int Index { get; set; }
        public Bitmap Img { get; set; }
        public bool ImgHasAlpha { get; set; }
        public bool Loaded { get; set; }

        public bool Ready()
        {
            return Loaded;
        }

        public void SplitImages()
        {
            ImgHasAlpha = CheckIfImgHasAlpha();
            Loaded = true;
        }

        private bool CheckIfImgHasAlpha()
        {
            // ToDo: это очень медленно, чтобы было быстрее нужно грузить в виде сырых пикселей (причем не png а битмапа обычного) и проверять на прозрачность пиксели
            for (int xPixel = 0; xPixel <= (Img.Width - 1); xPixel++)
            {
                for (int yPixel = 0; yPixel <= (Img.Height - 1); yPixel++)
                {
                    if (Img.GetPixel(xPixel, yPixel).A != 255)
                        return true;
                }
            }

            return false;
        }

        private void LoadAndHandle_Texture(uint file)
        {
            try
            {
                Img = WhDataLoader.LoadTexture(file);

                SplitImages();
            }
            catch
            {
                Img = null;
            }
        }
    }
}
