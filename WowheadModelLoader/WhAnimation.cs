using System.IO;

namespace WowheadModelLoader
{
    public class WhAnimation
    {
        public WhAnimation(BinaryReader r)
        {
            Id = r.ReadUInt16();
            SubId = r.ReadUInt16();
            Flags = r.ReadUInt32();
            Length = r.ReadUInt32();
            Next = r.ReadInt16();
            Index = r.ReadUInt16();

            var available = r.GetBool();
            if (available)
                Name = r.GetString();
        }

        public ushort Id { get; set; }
        public ushort SubId { get; set; }
        public uint Flags { get; set; }
        public uint Length { get; set; }
        public short Next { get; set; }
        public ushort Index { get; set; }
        public string Name { get; set; }
    }
}
