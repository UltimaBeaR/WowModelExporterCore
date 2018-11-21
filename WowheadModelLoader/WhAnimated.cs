using System;
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

        public abstract TValue Interpolate(TValue v1, TValue v2, float r);

        public TValue GetValue(int time, Func<TValue> createDefaultValue)
        {
            var result = createDefaultValue();

            if (Type != 0 || Data.Length > 1)
            {
                if (Times.Length > 1)
                {
                    var maxTime = Times[Times.Length - 1];
                    if (maxTime > 0 && time > maxTime)
                        time %= maxTime;

                    var idx = 0;
                    var numTimes = Times.Length;

                    for (var i = 0; i < numTimes; ++i)
                    {
                        if (time >= Times[i] && time < Times[i + 1])
                        {
                            idx = i;
                            break;
                        }
                    }

                    var t1 = Times[idx];
                    var t2 = Times[idx + 1];

                    float r = 0;

                    if (t1 != t2)
                        r = (time - t1) / (t2 - t1);

                    if (Type == 1)
                        return Interpolate(Data[idx], Data[idx + 1], r);
                    else
                    {
                        result = Data[idx];
                        return result;
                    }
                }
                else if (Data.Length > 0)
                {
                    result = Data[0];
                    return result;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                if (Data.Length == 0)
                    return result;
                else
                {
                    result = Data[0];
                    return result;
                }
            }
        }

        public static TValue GetValueBase(Func<TValue> createDefaultValue, WhAnimated<TValue>[] dataset, ushort anim, int time)
        {
            if (dataset == null || dataset.Length == 0)
                return createDefaultValue();

            if (anim >= dataset.Length)
                anim = 0;

            return dataset[anim].GetValue(time, createDefaultValue);
        }

        public static bool IsUsed(WhAnimated<TValue>[] dataset, ushort anim)
        {
            if (dataset == null || dataset.Length == 0)
                return false;

            if (anim >= dataset.Length)
                anim = 0;

            return dataset[anim].Used;
        }
    }
}
