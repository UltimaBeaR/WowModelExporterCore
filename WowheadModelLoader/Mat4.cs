namespace WowheadModelLoader
{
    public struct Mat4
    {
        public float a00;
        public float a01;
        public float a02;
        public float a03;

        public float a10;
        public float a11;
        public float a12;
        public float a13;

        public float a20;
        public float a21;
        public float a22;
        public float a23;

        public float a30;
        public float a31;
        public float a32;
        public float a33;

        public float this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return a00;
                    case 1: return a01;
                    case 2: return a02;
                    case 3: return a03;
                    case 4: return a10;
                    case 5: return a11;
                    case 6: return a12;
                    case 7: return a13;
                    case 8: return a20;
                    case 9: return a21;
                    case 10: return a22;
                    case 11: return a23;
                    case 12: return a30;
                    case 13: return a31;
                    case 14: return a32;
                    case 15: return a33;
                }

                throw new System.ArgumentOutOfRangeException();
            }

            set
            {
                switch (idx)
                {
                    case 0: a00 = value; return;
                    case 1: a01 = value; return;
                    case 2: a02 = value; return;
                    case 3: a03 = value; return;

                    case 4: a10 = value; return;
                    case 5: a11 = value; return;
                    case 6: a12 = value; return;
                    case 7: a13 = value; return;

                    case 8: a20 = value; return;
                    case 9: a21 = value; return;
                    case 10: a22 = value; return;
                    case 11: a23 = value; return;

                    case 12: a30 = value; return;
                    case 13: a31 = value; return;
                    case 14: a32 = value; return;
                    case 15: a33 = value; return;
                }

                throw new System.ArgumentOutOfRangeException();
            }
        }

        public static Mat4 Identity()
        {
            var res = new Mat4();

            res[0] = 1;
            res[1] = 0;
            res[2] = 0;
            res[3] = 0;
            res[4] = 0;
            res[5] = 1;
            res[6] = 0;
            res[7] = 0;
            res[8] = 0;
            res[9] = 0;
            res[10] = 1;
            res[11] = 0;
            res[12] = 0;
            res[13] = 0;
            res[14] = 0;
            res[15] = 1;

            return res;
        }

        public static Mat4 FromRotationTranslation(Vec4 q, Vec3 v)
        {
            var x = q[0];
            var y = q[1];
            var z = q[2];
            var w = q[3];
            var x2 = x + x;
            var y2 = y + y;
            var z2 = z + z;
            var xx = x * x2;
            var xy = x * y2;
            var xz = x * z2;
            var yy = y * y2;
            var yz = y * z2;
            var zz = z * z2;
            var wx = w * x2;
            var wy = w * y2;
            var wz = w * z2;

            var res = new Mat4();

            res[0] = 1 - (yy + zz);
            res[1] = xy + wz;
            res[2] = xz - wy;
            res[3] = 0;
            res[4] = xy - wz;
            res[5] = 1 - (xx + zz);
            res[6] = yz + wx;
            res[7] = 0;
            res[8] = xz + wy;
            res[9] = yz - wx;
            res[10] = 1 - (xx + yy);
            res[11] = 0;
            res[12] = v[0];
            res[13] = v[1];
            res[14] = v[2];
            res[15] = 1;

            return res;
        }

        public static Mat4 Multiply(Mat4 a, Mat4 b)
        {
            var a00 = a[0];
            var a01 = a[1];
            var a02 = a[2];
            var a03 = a[3];
            var a10 = a[4];
            var a11 = a[5];
            var a12 = a[6];
            var a13 = a[7];
            var a20 = a[8];
            var a21 = a[9];
            var a22 = a[10];
            var a23 = a[11];
            var a30 = a[12];
            var a31 = a[13];
            var a32 = a[14];
            var a33 = a[15];
            var b0 = b[0];
            var b1 = b[1];
            var b2 = b[2];
            var b3 = b[3];

            var res = new Mat4();

            res[0] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            res[1] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            res[2] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            res[3] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
            b0 = b[4];
            b1 = b[5];
            b2 = b[6];
            b3 = b[7];
            res[4] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            res[5] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            res[6] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            res[7] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
            b0 = b[8];
            b1 = b[9];
            b2 = b[10];
            b3 = b[11];
            res[8] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            res[9] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            res[10] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            res[11] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
            b0 = b[12];
            b1 = b[13];
            b2 = b[14];
            b3 = b[15];
            res[12] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            res[13] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            res[14] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            res[15] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;

            return res;
        }

        public static Mat4 Invert(Mat4 a)
        {
            var a00 = a[0];
            var a01 = a[1];
            var a02 = a[2];
            var a03 = a[3];
            var a10 = a[4];
            var a11 = a[5];
            var a12 = a[6];
            var a13 = a[7];
            var a20 = a[8];
            var a21 = a[9];
            var a22 = a[10];
            var a23 = a[11];
            var a30 = a[12];
            var a31 = a[13];
            var a32 = a[14];
            var a33 = a[15];
            var b00 = a00 * a11 - a01 * a10;
            var b01 = a00 * a12 - a02 * a10;
            var b02 = a00 * a13 - a03 * a10;
            var b03 = a01 * a12 - a02 * a11;
            var b04 = a01 * a13 - a03 * a11;
            var b05 = a02 * a13 - a03 * a12;
            var b06 = a20 * a31 - a21 * a30;
            var b07 = a20 * a32 - a22 * a30;
            var b08 = a20 * a33 - a23 * a30;
            var b09 = a21 * a32 - a22 * a31;
            var b10 = a21 * a33 - a23 * a31;
            var b11 = a22 * a33 - a23 * a32;
            var det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;

            if (det == 0)
                throw new System.DivideByZeroException();

            det = 1 / det;

            var res = new Mat4();

            res[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
            res[1] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
            res[2] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
            res[3] = (a22 * b04 - a21 * b05 - a23 * b03) * det;
            res[4] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
            res[5] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
            res[6] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
            res[7] = (a20 * b05 - a22 * b02 + a23 * b01) * det;
            res[8] = (a10 * b10 - a11 * b08 + a13 * b06) * det;
            res[9] = (a01 * b08 - a00 * b10 - a03 * b06) * det;
            res[10] = (a30 * b04 - a31 * b02 + a33 * b00) * det;
            res[11] = (a21 * b02 - a20 * b04 - a23 * b00) * det;
            res[12] = (a11 * b07 - a10 * b09 - a12 * b06) * det;
            res[13] = (a00 * b09 - a01 * b07 + a02 * b06) * det;
            res[14] = (a31 * b01 - a30 * b03 - a32 * b00) * det;
            res[15] = (a20 * b03 - a21 * b01 + a22 * b00) * det;

            return res;
        }



        public static Mat4 FromQuat(Vec4 q)
        {
            var x = q[0];
            var y = q[1];
            var z = q[2];
            var w = q[3];

            var x2 = x + x;
            var y2 = y + y;
            var z2 = z + z;
            var xx = x * x2;
            var yx = y * x2;
            var yy = y * y2;
            var zx = z * x2;
            var zy = z * y2;
            var zz = z * z2;
            var wx = w * x2;
            var wy = w * y2;
            var wz = w * z2;

            var res = new Mat4();

            res[0] = 1 - yy - zz;
            res[1] = yx + wz;
            res[2] = zx - wy;
            res[3] = 0;
            res[4] = yx - wz;
            res[5] = 1 - xx - zz;
            res[6] = zy + wx;
            res[7] = 0;
            res[8] = zx + wy;
            res[9] = zy - wx;
            res[10] = 1 - xx - yy;
            res[11] = 0;
            res[12] = 0;
            res[13] = 0;
            res[14] = 0;
            res[15] = 1;

            return res;
        }



        public static Mat4 Translate(Mat4 a, Vec3 v)
        {
            var x = v[0];
            var y = v[1];
            var z = v[2];

            var a00 = a[0];
            var a01 = a[1];
            var a02 = a[2];
            var a03 = a[3];
            var a10 = a[4];
            var a11 = a[5];
            var a12 = a[6];
            var a13 = a[7];
            var a20 = a[8];
            var a21 = a[9];
            var a22 = a[10];
            var a23 = a[11];

            var res = new Mat4();

            res[0] = a00;
            res[1] = a01;
            res[2] = a02;
            res[3] = a03;
            res[4] = a10;
            res[5] = a11;
            res[6] = a12;
            res[7] = a13;
            res[8] = a20;
            res[9] = a21;
            res[10] = a22;
            res[11] = a23;
            res[12] = a00 * x + a10 * y + a20 * z + a[12];
            res[13] = a01 * x + a11 * y + a21 * z + a[13];
            res[14] = a02 * x + a12 * y + a22 * z + a[14];
            res[15] = a03 * x + a13 * y + a23 * z + a[15];

            return res;
        }

        public static Mat4 Scale(Mat4 a, Vec3 v)
        {
            var x = v[0];
            var y = v[1];
            var z = v[2];

            var res = new Mat4();

            res[0] = a[0] * x;
            res[1] = a[1] * x;
            res[2] = a[2] * x;
            res[3] = a[3] * x;
            res[4] = a[4] * y;
            res[5] = a[5] * y;
            res[6] = a[6] * y;
            res[7] = a[7] * y;
            res[8] = a[8] * z;
            res[9] = a[9] * z;
            res[10] = a[10] * z;
            res[11] = a[11] * z;
            res[12] = a[12];
            res[13] = a[13];
            res[14] = a[14];
            res[15] = a[15];

            return res;
        }
    }
}
