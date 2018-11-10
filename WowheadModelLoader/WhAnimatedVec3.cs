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

        public override Vec3 ReadValue(BinaryReader r)
        {
            return new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }
    }
}
