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

        public override Vec4 ReadValue(BinaryReader r)
        {
            return new Vec4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }
    }
}
