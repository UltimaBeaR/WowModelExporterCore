namespace WowheadModelLoader
{
    public struct Mat3
    {
        public float a00;
        public float a01;
        public float a02;

        public float a10;
        public float a11;
        public float a12;

        public float a20;
        public float a21;
        public float a22;

        public float a30;
        public float a31;
        public float a32;

        public float this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return a00;
                    case 1: return a01;
                    case 2: return a02;
                    case 3: return a10;
                    case 4: return a11;
                    case 5: return a12;
                    case 6: return a20;
                    case 7: return a21;
                    case 8: return a22;
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

                    case 3: a10 = value; return;
                    case 4: a11 = value; return;
                    case 5: a12 = value; return;

                    case 6: a20 = value; return;
                    case 7: a21 = value; return;
                    case 8: a22 = value; return;
                }

                throw new System.ArgumentOutOfRangeException();
            }
        }

        public static Mat3 Identity()
        {
            var res = new Mat3();

            res[0] = 1;
            res[1] = 0;
            res[2] = 0;
            res[3] = 0;
            res[4] = 1;
            res[5] = 0;
            res[6] = 0;
            res[7] = 0;
            res[8] = 1;

            return res;
        }

        public static Mat3 FromMat4(Mat4 a)
        {
            var res = new Mat3();

            res[0] = a[0];
            res[1] = a[1];
            res[2] = a[2];
            res[3] = a[4];
            res[4] = a[5];
            res[5] = a[6];
            res[6] = a[8];
            res[7] = a[9];
            res[8] = a[10];

            return res;
        }
    }
}
