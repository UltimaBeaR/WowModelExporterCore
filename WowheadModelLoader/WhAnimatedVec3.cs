using System.IO;

namespace WowheadModelLoader
{
    // ToDo: реализовал пока только минимум, необходимый для считывания из файла
    public class WhAnimatedVec3 : WhAnimated<Vec3>
    {
        public WhAnimatedVec3(BinaryReader r)
        {
            Read(r);
        }

        public static WhAnimatedVec3[] ReadSet(BinaryReader r)
        {
            var count = r.ReadInt32();
            var data = new WhAnimatedVec3[count];
            for (var i = 0; i < count; i++)
                data[i] = new WhAnimatedVec3(r);

            return data;
        }

        public static Vec3 GetValue(WhAnimatedVec3[] dataset, ushort anim, int time)
        {
            return GetValueBase(() => new Vec3(), dataset, anim, time);
        }

        public override Vec3 ReadValue(BinaryReader r)
        {
            return new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }

        public override Vec3 Interpolate(Vec3 v1, Vec3 v2, float r)
        {
            return Vec3.Lerp(v1, v2, r);
        }
    }
}
