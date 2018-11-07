using System.IO;

namespace WowheadModelLoader
{
    public struct WhVertex
    {
        public WhVertex(BinaryReader r)
        {
            Position = new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            Normal = new Vec4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), 0);
            U = r.ReadSingle();
            V = r.ReadSingle();
            U2 = r.ReadSingle();
            V2 = r.ReadSingle();
            Weights = new ByteVec4(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte());
            Bones = new ByteVec4(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte());
            
            //self.transPosition = vec3.clone(self.position);
            //self.transNormal = vec4.clone(self.normal)
        }

        public Vec3 Position { get; set; }
        public Vec4 Normal { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public float U2 { get; set; }
        public float V2 { get; set; }
        public ByteVec4 Weights { get; set; }
        public ByteVec4 Bones { get; set; }
    }
}
