using System.IO;

namespace WowheadModelLoader
{
    public class WhMesh
    {
        public WhMesh(BinaryReader r)
        {
            Id = r.ReadUInt16();
            IndexWrap = r.ReadUInt16();
            VertexStart = r.ReadUInt16();
            VertexCount = r.ReadUInt16();
            IndexStart = r.ReadUInt16() + IndexWrap * 65536;
            IndexCount = r.ReadUInt16();
            CenterOfMass = new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            CenterBounds = new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            Radius = r.ReadSingle();
        }

        public ushort Id { get; set; }
        public ushort IndexWrap { get; set; }
        public ushort VertexStart { get; set; }
        public ushort VertexCount { get; set; }
        public int IndexStart { get; set; }
        public ushort IndexCount { get; set; }
        public Vec3 CenterOfMass { get; set; }
        public Vec3 CenterBounds { get; set; }
        public float Radius { get; set; }
    }
}
