namespace WowheadModelLoader
{
    public struct ByteVec4
    {
        public ByteVec4(byte element0, byte element1, byte element2, byte element3)
        {
            _element0 = element0;
            _element1 = element1;
            _element2 = element2;
            _element3 = element3;
        }

        public byte this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return _element0;
                    case 1: return _element1;
                    case 2: return _element2;
                    case 3: return _element3;
                }

                throw new System.ArgumentOutOfRangeException();
            }

            set
            {
                switch (idx)
                {
                    case 0: _element0 = value; return;
                    case 1: _element1 = value; return;
                    case 2: _element2 = value; return;
                    case 3: _element3 = value; return;
                }

                throw new System.ArgumentOutOfRangeException();
            }
        }

        private byte _element0 { get; set; }
        private byte _element1 { get; set; }
        private byte _element2 { get; set; }
        private byte _element3 { get; set; }
    }
}
