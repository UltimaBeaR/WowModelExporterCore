using System.IO;

namespace WowheadModelLoader
{
    public class WhMaterial
    {
        public WhMaterial(WhModel model, int index, BinaryReader r)
        {
            Model = model;
            Index = index;

            Type = r.ReadInt32();
            Flags = r.ReadUInt32();
            Filename = r.ReadUInt32();

            Texture = null;

            Load();
        }

        public WhModel Model { get; set; }
        public int Index { get; set; }
        public int Type { get; set; }
        public uint Flags { get; set; }
        public uint Filename { get; set; }
        public WhTexture Texture { get; set; }

        public void Load()
        {
            if (Filename == 0)
                return;

            Texture = new WhTexture(Model, 0, Filename);
        }
    }
}
