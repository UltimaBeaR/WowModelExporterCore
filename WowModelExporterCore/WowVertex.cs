using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowVertex
    {
        public Vec3 Position
        {
            get
            {
                return Vec3.ConvertPositionFromWh(WhPosition);
            }
            set
            {
                WhPosition = Vec3.ConvertPositionToWh(value);
            }
        }

        public Vec3 Normal
        {
            get
            {
                return Vec3.ConvertPositionFromWh(WhNormal);
            }
            set
            {
                WhNormal = Vec3.ConvertPositionToWh(value);
            }
        }

        public Vec3 WhPosition { get; set; }
        public Vec3 WhNormal { get; set; }

        public Vec2 UV1 { get; set; }
        public Vec2 UV2 { get; set; }

        public ByteVec4 BoneIndexes { get; set; }
        public Vec4 BoneWeights { get; set; }
    }
}
