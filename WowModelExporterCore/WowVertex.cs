using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowVertex
    {
        public Vec3 Position { get; set; }
        public Vec4 Normal { get; set; }
        public Vec2 UV1 { get; set; }
        public Vec2 UV2 { get; set; }
        public ByteVec4 Weights { get; set; }
        public ByteVec4 Bones { get; set; }
    }
}
