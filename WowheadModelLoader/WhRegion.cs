using System.Collections.Generic;

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

        public static readonly Dictionary<WhRegion, WhRegionOldNew> Old = new Dictionary<WhRegion, WhRegionOldNew>()
        {
            { (WhRegion)0, new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 0.5f,
                H= 0.25f
            } },
            { (WhRegion)1, new WhRegionOldNew
            {
                X= 0,
                Y= 0.25f,
                W= 0.5f,
                H= 0.25f
            } },
            { (WhRegion)2, new WhRegionOldNew
            {
                X= 0,
                Y= 0.5f,
                W= 0.5f,
                H= 0.125f
            } },
            { (WhRegion)3, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0,
                W= 0.5f,
                H= 0.25f
            } },
            { (WhRegion)4, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.25f,
                W= 0.5f,
                H= 0.125f
            } },
            { (WhRegion)5, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.375f,
                W= 0.5f,
                H= 0.25f
            } },
            { (WhRegion)6, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.625f,
                W= 0.5f,
                H= 0.25f
            } },
            { (WhRegion)7, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0.875f,
                W= 0.5f,
                H= 0.125f
            } },
            { (WhRegion)8, new WhRegionOldNew {} },
            { (WhRegion)9, new WhRegionOldNew
            {
                X= 0,
                Y= 0.625f,
                W= 0.5f,
                H= 0.125f
            } },
            { (WhRegion)10, new WhRegionOldNew
            {
                X= 0,
                Y= 0.75f,
                W= 0.5f,
                H= 0.25f
            } },
            { (WhRegion)11, new WhRegionOldNew {} },
            { (WhRegion)12, new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 1,
                H= 1
            } },
            { (WhRegion)13, new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 1,
                H= 1
            } }
        };

        public static readonly Dictionary<WhRegion, WhRegionOldNew> New = new Dictionary<WhRegion, WhRegionOldNew>()
        {
            { (WhRegion)0, new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 0.25f,
                H= 0.25f
            } },
            { (WhRegion)1, new WhRegionOldNew
            {
                X= 0,
                Y= 0.25f,
                W= 0.25f,
                H= 0.25f
            } },
            { (WhRegion)2, new WhRegionOldNew
            {
                X= 0,
                Y= 0.5f,
                W= 0.25f,
                H= 0.125f
            } },
            { (WhRegion)3, new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0,
                W= 0.25f,
                H= 0.25f
            } },
            { (WhRegion)4, new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.25f,
                W= 0.25f,
                H= 0.125f
            } },
            { (WhRegion)5, new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.375f,
                W= 0.25f,
                H= 0.25f
            } },
            { (WhRegion)6, new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.625f,
                W= 0.25f,
                H= 0.25f
            } },
            { (WhRegion)7, new WhRegionOldNew
            {
                X= 0.25f,
                Y= 0.875f,
                W= 0.25f,
                H= 0.125f
            } },
            { (WhRegion)8, new WhRegionOldNew
            {
                X= 0.75f,
                Y= 0.75f,
                W= 0.25f,
                H= 0.25f
            } },
            { (WhRegion)9, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0,
                W= 0.5f,
                H= 1
            } },
            { (WhRegion)10, new WhRegionOldNew
            {
                X= 0.5f,
                Y= 0,
                W= 0.5f,
                H= 1
            } },
            { (WhRegion)11, new WhRegionOldNew {} },
            { (WhRegion)12, new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 0.5f,
                H= 1
            } },
            { (WhRegion)13, new WhRegionOldNew
            {
                X= 0,
                Y= 0,
                W= 1,
                H= 1
            } }
        };
    }
}
