using System;

namespace WowheadModelLoader
{
    public static class Quat
    {
        // По сути это identity
        public static Vec4 Create()
        {
            return new Vec4(0, 0, 0, 1);
        }

        public static Vec4 Slerp(Vec4 a, Vec4 b, float t)
        {
            var cosom = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

            if (cosom < 0)
            {
                cosom = -cosom;
                b.X = -b.X;
                b.Y = -b.Y;
                b.Z = -b.Z;
                b.W = -b.W;
            }

            float scale0;
            float scale1;

            if (1f - cosom > 1e-6f)
            {
                var omega = (float)Math.Acos(cosom);
                var sinom = (float)Math.Sin(omega);
                scale0 = (float)Math.Sin((1f - t) * omega) / sinom;
                scale1 = (float)Math.Sin(t * omega) / sinom;
            }
            else
            {
                scale0 = 1f - t;
                scale1 = t;
            }

            return new Vec4(
                scale0 * a.X + scale1 * b.X,
                scale0 * a.Y + scale1 * b.Y,
                scale0 * a.Z + scale1 * b.Z,
                scale0 * a.W + scale1 * b.W
            );
        }
    }
}
