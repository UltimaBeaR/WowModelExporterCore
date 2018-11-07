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
    }
}
