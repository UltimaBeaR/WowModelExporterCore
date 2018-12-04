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

        public static Vec3 Lerp(Vec3 a, Vec3 b, float t)
        {
            return new Vec3() {
                X = a.X + t * (b.X - a.X),
                Y = a.Y + t * (b.Y - a.Y),
                Z = a.Z + t * (b.Z - a.Z)
            };
        }

        // q это кватернион
        public static Vec3 TransformQuat(Vec3 a, Vec4 q)
        {
            var ix = q.W * a.X + q.Y * a.Z - q.Z * a.Y;
            var iy = q.W * a.Y + q.Z * a.X - q.X * a.Z;
            var iz = q.W * a.Z + q.X * a.Y - q.Y * a.X;
            var iw = -q.X * a.X - q.Y * a.Y - q.Z * a.Z;

            return new Vec3(
                ix * q.W + iw * -q.X + iy * -q.Z - iz * -q.Y,
                iy * q.W + iw * -q.Y + iz * -q.X - ix * -q.Z,
                iz * q.W + iw * -q.Z + ix * -q.Y - iy * -q.X
            );
        }

        public static Vec3 TransformMat4(Vec3 a, Mat4 m)
        {
            var x = a[0];
            var y = a[1];
            var z = a[2];

            var res = new Vec3();
            res[0] = m[0] * x + m[4] * y + m[8] * z + m[12];
            res[1] = m[1] * x + m[5] * y + m[9] * z + m[13];
            res[2] = m[2] * x + m[6] * y + m[10] * z + m[14];
            return res;
        }

    #region я добавил

        public static Vec3 ConvertPositionFromWh(Vec3 position)
        {
            var res = new Vec3(-position.X, position.Z, -position.Y);
            res.RotateAroundY((float)Math.PI / 2);
            return res;
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

        #endregion
    }
}