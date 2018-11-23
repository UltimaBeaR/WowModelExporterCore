using System.IO;

namespace WowheadModelLoader
{
    // ToDo: реализовал пока только минимум, необходимый для считывания из файла
    public class WhAnimatedQuat : WhAnimated<Vec4>
    {
        public WhAnimatedQuat(BinaryReader r)
        {
            Read(r);
        }

        public static WhAnimatedQuat[] ReadSet(BinaryReader r)
        {
            var count = r.ReadInt32();
            var data = new WhAnimatedQuat[count];
            for (var i = 0; i < count; i++)
                data[i] = new WhAnimatedQuat(r);

            return data;
        }

        public static Vec4 GetValue(WhAnimatedQuat[] dataset, ushort anim, int time)
        {
            return GetValueBase(() => new Vec4(), dataset, anim, time);
        }

        public override Vec4 ReadValue(BinaryReader r)
        {
            return new Vec4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }

        public override Vec4 Interpolate(Vec4 v1, Vec4 v2, float r)
        {
            return Quat.Slerp(v1, v2, r);
        }
    }
}
