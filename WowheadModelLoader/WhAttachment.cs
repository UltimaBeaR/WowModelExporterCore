using System.IO;

namespace WowheadModelLoader
{
    public class WhAttachment
    {
        public WhAttachment(BinaryReader r)
        {
            Id = r.ReadInt32();
            Bone = r.ReadInt32();
            Position = new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            Slot = -1;
        }

        public int Id { get; set; }
        public int Bone { get; set; }
        public Vec3 Position { get; set; }
        public int Slot { get; set; }
    }
}
