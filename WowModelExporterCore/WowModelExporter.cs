using System.Drawing;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowModelExporter
    {
        public WhModel LoadModel(string characterId, string[] itemIds)
        {
            var gathererItems = WhDataLoader.LoadItemsFromGatherer(itemIds);

            var options = new WhViewerOptions() { Cls = WhClass.WARRIOR, Hd = true };

            if (gathererItems != null)
            {
                options.Items = gathererItems
                    .Select((x, i) => new WhViewerOptions.Item()
                    {
                        Slot = x.Value.OtherData.SlotBak,
                        Id = x.Value.OtherData.DisplayId,
                        // Пока так, когда узнать откуда взять этот id - надо тоже прописать
                        VisualId = null
                    })
                    .ToArray();
            }

            var characterModel = new WhModel(
                options,
                new WhModelInfo() { Type = WhType.CHARACTER, Id = characterId }, 0);

            return characterModel;
        }

        public Bitmap GetFirstTexture(WhModel model)
        {
            var texturesWithCompositeFirst = model.TexUnits.Where(x => x.Show).SelectMany(x => x.GetTextures().Values).Select(x => new { ddd = x, order = x.Img != null ? 0 : 1 }).OrderBy(x => x.order).Select(x => x.ddd).ToArray();

            foreach (var texture in texturesWithCompositeFirst)
            {
                if (texture.Img != null)
                    return texture.Img;

                if (texture.Texture != null)
                    return texture.Texture.Img;
            }

            return null;
        }
    }
}
