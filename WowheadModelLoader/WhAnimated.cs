using System.IO;

namespace WowheadModelLoader
{
    // ToDo: реализовал пока только минимум, необходимый для считывания из файла
    public abstract class WhAnimated<TValue>
    {
        public void Read(BinaryReader r)
        {
            Type = r.ReadInt16();
            Seq = r.ReadInt16();
            Used = r.GetBool();

            var numTimes = r.ReadInt32();
            Times = new int[numTimes];
            for (int i = 0; i < numTimes; i++)
                Times[i] = r.ReadInt32();

            var numData = r.ReadInt32();
            Data = new TValue[numData];
            for (int i = 0; i < numData; i++)
                Data[i] = ReadValue(r);
        }

        public short Type { get; set; }
        public short Seq { get; set; }
        public bool Used { get; set; }
        public int[] Times { get; set; }
        public TValue[] Data { get; set; }

        public abstract TValue ReadValue(BinaryReader r);
    }
}
