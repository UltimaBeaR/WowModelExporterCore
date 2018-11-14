using System;

namespace WowheadModelLoader
{
    public struct Vec3
    {
        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                }

                throw new System.ArgumentOutOfRangeException();
            }

            set
            {
                switch (idx)
                {
                    case 0: X = value; return;
                    case 1: Y = value; return;
                    case 2: Z = value; return;
                }

                throw new System.ArgumentOutOfRangeException();
            }
        }

        public void RotateAroundX(float angleRad)
        {
            var sin = (float)Math.Sin(angleRad);
            var cos = (float)Math.Cos(angleRad);

            var y = Y * cos - Z * sin;
            var z = Y * sin + Z * cos;

            Y = y;
            Z = z;
        }

        public void RotateAroundY(float angleRad)
        {
            var sin = (float)Math.Sin(angleRad);
            var cos = (float)Math.Cos(angleRad);

            var x = X * cos + Z * sin;
            var z = -X * sin + Z * cos;

            X = x;
            Z = z;
        }

        public void RotateAroundZ(float angleRad)
        {
            var sin = (float)Math.Sin(angleRad);
            var cos = (float)Math.Cos(angleRad);

            var x = X * cos - Y * sin;
            var y = X * sin + Y * cos;

            X = X;
            Y = y;
        }
    }
}