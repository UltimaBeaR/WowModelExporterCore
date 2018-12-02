using Newtonsoft.Json;
using System.IO;
using WowheadModelLoader;
using WowheadModelLoader.Json;

namespace WowModelExporterCore
{
    public class WowVrcFile
    {
        public const string currentVersion = "unversioned";

        public const string fileDialogFilter = "wow -> vrc file|*.wowvrc";

        public WowVrcFile(string optsJson)
        {
            var opts = JsonConvert.DeserializeObject<WhOpts>(optsJson);

            _data = new WowVrcFileData()
            {
                Version = currentVersion,
                Header = new WowVrcFileData.HeaderData() { Opts = opts }
            };
        }

        public WowVrcFile(WhRace race, WhGender gender, string[] itemIds)
        {
            var manualHeaderData = new WowVrcFileData.HeaderData.ManualHeaderData()
            {
                Race = race,
                Gender = gender,
                ItemIds = itemIds
            };

            _data = new WowVrcFileData()
            {
                Version = currentVersion,
                Header = new WowVrcFileData.HeaderData() { ManualData = manualHeaderData }
            };
        }

        private WowVrcFile()
        {
        }

        public static WowVrcFile Open(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var fileData = JsonConvert.DeserializeObject<WowVrcFileData>(json);

            if (fileData.Version != currentVersion)
                throw new System.Exception("invalid wowvrc file version");

            return new WowVrcFile()
            {
                _data = fileData
            };
        }

        public void SaveTo(string fileName)
        {
            var json = JsonConvert.SerializeObject(_data, Formatting.Indented);

            File.WriteAllText(fileName, json);
        }

        public WhOpts GetOpts()
        {
            return _data.Header.Opts;
        }

        public WowVrcFileData.HeaderData.ManualHeaderData GetManualHeader()
        {
            return _data.Header.ManualData;
        }

        public WowVrcFileData.BlendshapeData[] Blendshapes { get { return _data.Blendshapes; } set { _data.Blendshapes = value; } }

        private WowVrcFileData _data { get; set; }
    }

    public class WowVrcFileData
    {
        public string Version { get; set; }

        public HeaderData Header { get; set; }

        public BlendshapeData[] Blendshapes { get; set; }

        public class HeaderData
        {
            // Тут должно быть задано одно либо другое (приоритет идет в Opts)

            // ToDo: сделать определение race / gender из WhOpts. Поидее там в поле models задается type/id. в type должно быть character, а в id в строковом виде race + gender
            // Это определение потом юзать для проверки при мерже визем и т.д. так как скелет будет совпадать только если эти поля (type/id) равны

            public WhOpts Opts { get; set; }
            public ManualHeaderData ManualData { get; set; }

            public class ManualHeaderData
            {
                public WhRace Race { get; set; }
                public WhGender Gender { get; set; }
                public string[] ItemIds { get; set; }
            }
        }

        public class BlendshapeData
        {
            public string Name { get; set; }

            public BoneData[] Bones { get; set; }

            public class BoneData
            {
                public string Name { get; set; }

                public Vec3 LocalPosition { get; set; }
                public Vec4 LocalRotation { get; set; }
                public Vec3 LocalScale { get; set; }
            }
        }
    }
}
