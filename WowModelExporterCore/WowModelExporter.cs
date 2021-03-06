﻿using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowModelExporter
    {
        public WowObject LoadCharacter(WhRace race, WhGender gender, string[] itemIds, float scale = 1f)
        {
            var whCharacterModel = LoadWhCharacterModel(race, gender, itemIds);

            return new WowObjectBuilder(scale).BuildFromCharacterWhModel(whCharacterModel);
        }

        public WowObject LoadCharacter(WhViewerOptions opts, float scale = 1f)
        {
            var whCharacterModel = LoadWhCharacterModel(opts);

            return new WowObjectBuilder(scale).BuildFromCharacterWhModel(whCharacterModel);
        }

        private WhModel LoadWhCharacterModel(WhRace race, WhGender gender, string[] itemIds)
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
                WhModelInfo.CreateForCharacter(race, gender), 0);

            WhDefferedList.Execute();

            characterModel.EmulateDraw(false);

            return characterModel;
        }

        private WhModel LoadWhCharacterModel(WhViewerOptions opts)
        {
            var characterModel = new WhModel(
                opts,
                opts.Model, 0);

            WhDefferedList.Execute();

            characterModel.EmulateDraw(false);

            return characterModel;
        }
    }
}
