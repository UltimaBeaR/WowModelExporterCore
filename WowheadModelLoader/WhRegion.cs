namespace WowheadModelLoader
{
    public enum WhRegion
    {
        ArmUpper = 0,
        ArmLower = 1,
        Hand = 2,
        TorsoUpper = 3,
        TorsoLower = 4,
        LegUpper = 5,
        LegLower = 6,
        Foot = 7,
        AccessorY = 8,
        FaceUpper = 9,
        FaceLower = 10,
        Unused = 11,
        Base = 12,
        Unknown735 = 13,
    }

    public class WhRegionOldNew
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }

        private static readonly WhRegionOldNew[] Old = new WhRegionOldNew[]
        {
            new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 0.5f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0.25f,
                W= 0.5f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0.5f,
                W= 0.5f,
                H= 0.125f
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0,
                W= 0.5f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.25f,
                W= 0.5f,
                H= 0.125f
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.375f,
                W= 0.5f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.625f,
                W= 0.5f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.875f,
                W= 0.5f,
                H= 0.125f
            },
            new WhRegionOldNew {},
            new WhRegionOldNew
            {
                X= 0,
                Y= 0.625f,
                W= 0.5f,
                H= 0.125f
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0.75f,
                W= 0.5f,
                H= 0.25f
            },
            new WhRegionOldNew {},
            new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 1,
                H= 1
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 1,
                H= 1
            }
        };

        private static readonly WhRegionOldNew[] New = new WhRegionOldNew[]
        {
            new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 0.25f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0.25f,
                W= 0.25f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0.5f,
                W= 0.25f,
                H= 0.125f
            },
            new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0,
                W= 0.25f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.25f,
                W= 0.25f,
                H= 0.125f
            },
            new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.375f,
                W= 0.25f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.625f,
                W= 0.25f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.875f,
                W= 0.25f,
                H= 0.125f
            },
            new WhRegionOldNew
            {
                X= 0.75f,
                Y= 0.75f,
                W= 0.25f,
                H= 0.25f
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0,
                W= 0.5f,
                H= 1
            },
            new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0,
                W= 0.5f,
                H= 1
            },
            new WhRegionOldNew {},
            new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 0.5f,
                H= 1
            },
            new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 1,
                H= 1
            }
        };
    }
}
