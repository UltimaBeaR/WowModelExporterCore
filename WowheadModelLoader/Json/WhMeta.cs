using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace WowheadModelLoader.Json
{
    public class WhJsonMeta
    {
        /// <summary>
        /// id обычной модели (mo3)
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        /// id HD версии модели (mo3)
        /// </summary>
        public int HDModel { get; set; }

        public Dictionary<int, uint> Textures { get; set; }
        public Dictionary<int, uint> Textures2 { get; set; }

        // ToDo: Ключи возможно что числа, т.к. при исопльзовании там id текстуры какой то юзается в ключе
        // Я не знаю что приходит - по исползьованию похоже что может быть ModelFile, так как есть испльзование FileDataId
        public Dictionary<string, ModelFile[]> TextureFiles { get; set; }

        public Dictionary<int, ModelFile[]> ModelFiles { get; set; }

        public ItemStruct Item { get; set; }

        public CreatureStruct Creature { get; set; }

        public int RaceFlags { get; set; }

        public WhRace Race { get; set; }
        public WhGender Gender { get; set; }

        public JToken Equipment { get; set; }

        public WhViewerOptions.Item[] GetEquipmentAsItemObjects()
        {
            return Equipment.ToObject<WhViewerOptions.Item[]>();
        }

        public string[] GetEquipmentAsStringIds()
        {
            return Equipment.ToObject<string[]>();
        }

        // ToDo: смотрел тип из использования, а не json-а. ключ регион (похоже что int так как идет parseint),
        // значение это textureId, скорее всего тоже int, но на всякий случай пусть пока будет строкой 
        public Dictionary<WhRegion, string> ComponentTextures { get; set; }

        public Dictionary<string, int> ComponentModels { get; set; }

        public float Scale { get; set; }

        public class ModelFile
        {
            public uint FileDataId { get; set; }
            public WhGender Gender { get; set; }
            public WhClass Class { get; set; }
            public WhRace Race { get; set; }
        }

        public class ItemStruct
        {
            public int Flags { get; set; }
            public WhSlot InventoryType { get; set; }
            public int ItemClass { get; set; }
            public int ItemSubClass { get; set; }
            public int[] HideGeosetMale { get; set; }
            public int[] HideGeosetFemale { get; set; }
            public int[] GeosetGroup { get; set; }

            // ToDo: узнать тип
            public object ParticleColor { get; set; }
        }

        public class CreatureStruct
        {
            public int CreatureGeosetData { get; set; }
            public uint Texture { get; set; }
            public uint HDTexture { get; set; }
            public int SkinColor { get; set; }
            public int HairStyle { get; set; }
            public int HairColor { get; set; }
            public int FaceType { get; set; }
            public int FacialHair { get; set; }
        }
    }
}
