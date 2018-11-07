namespace WowheadModelLoader
{
    public struct Vec2
    {
        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public float this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return X;
                    case 1: return Y;
                }

                throw new System.ArgumentOutOfRangeException();
            }

            set
            {
                switch (idx)
                {
                    case 0: X = value; return;
                    case 1: Y = value; return;
                }

                throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}
