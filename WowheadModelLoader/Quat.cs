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

        public static Vec4 Invert(Vec4 a)
        {
            var dot = a.X * a.X + a.Y * a.Y + a.Z * a.Z + a.W * a.W;
            var invDot = dot != 0 ? 1f / dot : 0f;
            return new Vec4(
                -a.X * invDot,
                -a.Y * invDot,
                -a.Z * invDot,
                a.W * invDot
            );
        }

        public static Vec4 Multiply(Vec4 a, Vec4 b)
        {
            var ax = a[0];
            var ay = a[1];
            var az = a[2];
            var aw = a[3];
            var bx = b[0];
            var by = b[1];
            var bz = b[2];
            var bw = b[3];

            var res = new Vec4();

            res[0] = ax * bw + aw * bx + ay * bz - az * by;
            res[1] = ay * bw + aw * by + az * bx - ax * bz;
            res[2] = az * bw + aw * bz + ax * by - ay * bx;
            res[3] = aw * bw - ax * bx - ay * by - az * bz;

            return res;
        }

        public static Vec4 FromMat3(Mat3 m)
        {
            var fTrace = m[0] + m[4] + m[8];

            float fRoot;

            var res = new Vec4();

            if (fTrace > 0) {
                fRoot = (float)Math.Sqrt(fTrace + 1);
                res[3] = 0.5f * fRoot;
                fRoot = 0.5f / fRoot;

                res[0] = (m[7] - m[5]) * fRoot;
                res[1] = (m[2] - m[6]) * fRoot;
                res[2] = (m[3] - m[1]) * fRoot;
            }
            else
            {
                var i = 0;

                if (m[4] > m[0])
                    i = 1;

                if (m[8] > m[i * 3 + i])
                    i = 2;

                var j = (i + 1) % 3;
                var k = (i + 2) % 3;

                fRoot = (float)Math.Sqrt(m[i * 3 + i] - m[j * 3 + j] - m[k * 3 + k] + 1);
                res[i] = 0.5f * fRoot;
                fRoot = 0.5f / fRoot;

                res[3] = (m[k * 3 + j] - m[j * 3 + k]) * fRoot;
                res[j] = (m[j * 3 + i] + m[i * 3 + j]) * fRoot;
                res[k] = (m[k * 3 + i] + m[i * 3 + k]) * fRoot;
            }

            return res;
        }

        public static Vec4 RotateX(Vec4 a, float rad)
        {
            rad *= 0.5f;

            var ax = a[0];
            var ay = a[1];
            var az = a[2];
            var aw = a[3];
            var bx = (float)Math.Sin(rad);
            var bw = (float)Math.Cos(rad);

            var res = new Vec4();

            res[0] = ax * bw + aw * bx;
            res[1] = ay * bw + az * bx;
            res[2] = az * bw - ay * bx;
            res[3] = aw * bw - ax * bx;

            return res;
        }

        public static Vec4 RotateY(Vec4 a, float rad)
        {
            rad *= 0.5f;
            var ax = a[0];
            var ay = a[1];
            var az = a[2];
            var aw = a[3];
            var by = (float)Math.Sin(rad);
            var bw = (float)Math.Cos(rad);

            var res = new Vec4();

            res[0] = ax * bw - az * by;
            res[1] = ay * bw + aw * by;
            res[2] = az * bw + ax * by;
            res[3] = aw * bw - ay * by;
            return res;
        }

        public static Vec4 RotateZ(Vec4 a, float rad)
        {
            rad *= 0.5f;
            var ax = a[0];
            var ay = a[1];
            var az = a[2];
            var aw = a[3];
            var bz = (float)Math.Sin(rad);
            var bw = (float)Math.Cos(rad);

            var res = new Vec4();

            res[0] = ax * bw + ay * bz;
            res[1] = ay * bw - ax * bz;
            res[2] = az * bw + aw * bz;
            res[3] = aw * bw - az * bz;
            return res;
        }

        public static Vec4 QuaternionFromMatrix(Mat4 m)
        {
            var q = new Vec4();

            q.W = (float)Math.Sqrt(Math.Max(0, 1 + m.a00 + m.a11 + m.a22)) / 2f;
            q.X = (float)Math.Sqrt(Math.Max(0, 1 + m.a00 - m.a11 - m.a22)) / 2f;
            q.Y = (float)Math.Sqrt(Math.Max(0, 1 - m.a00 + m.a11 - m.a22)) / 2f;
            q.Z = (float)Math.Sqrt(Math.Max(0, 1 - m.a00 - m.a11 + m.a22)) / 2f;
            q.X *= Math.Sign(q.X * (m.a21 - m.a12));
            q.Y *= Math.Sign(q.Y * (m.a02 - m.a20));
            q.Z *= Math.Sign(q.Z * (m.a10 - m.a01));

            return q;
        }
    }
}
