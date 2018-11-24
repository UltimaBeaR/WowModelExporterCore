using System.Collections.Generic;

namespace WowheadModelLoader
{
    /// <summary>
    /// Набор всяких глобальных констант и т.д.
    /// </summary>
    public static class WhGlobal
    {
        public static int VertexSize32 => 10;

        public static readonly IReadOnlyDictionary<WhCharVariationType, int[]> CharVariationMap =
            new Dictionary<WhCharVariationType, int[]>()
            {
                { WhCharVariationType.Skin, new int[] { 0, 5 } },
                { WhCharVariationType.Face, new int[] { 1, 6 } },
                { WhCharVariationType.FacialHair, new int[] { 2, 7 } },
                { WhCharVariationType.Hair, new int[] { 3, 8 } },
                { WhCharVariationType.Underwear, new int[] { 4, 9 } },
                { WhCharVariationType.Custom1, new int[] { 10, 11 } },
                { WhCharVariationType.Custom2, new int[] { 12, 13 } },
                { WhCharVariationType.Custom3, new int[] { 14, 15 } }
            };

        public static readonly IReadOnlyDictionary<WhSlot, WhSlot> UniqueSlots =
            new Dictionary<WhSlot, WhSlot>()
            {
                { (WhSlot)0, (WhSlot)0 },
                { (WhSlot)1, (WhSlot)1 },
                { (WhSlot)2, (WhSlot)0 },
                { (WhSlot)3, (WhSlot)3 },
                { (WhSlot)4, (WhSlot)4 },
                { (WhSlot)5, (WhSlot)5 },
                { (WhSlot)6, (WhSlot)6 },
                { (WhSlot)7, (WhSlot)7 },
                { (WhSlot)8, (WhSlot)8 },
                { (WhSlot)9, (WhSlot)9 },
                { (WhSlot)10, (WhSlot)10 },
                { (WhSlot)11, (WhSlot)0 },
                { (WhSlot)12, (WhSlot)0 },
                { (WhSlot)13, (WhSlot)21 },
                { (WhSlot)14, (WhSlot)22 },
                { (WhSlot)15, (WhSlot)22 },
                { (WhSlot)16, (WhSlot)16 },
                { (WhSlot)17, (WhSlot)21 },
                { (WhSlot)18, (WhSlot)0 },
                { (WhSlot)19, (WhSlot)19 },
                { (WhSlot)20, (WhSlot)5 },
                { (WhSlot)21, (WhSlot)21 },
                { (WhSlot)22, (WhSlot)22 },
                { (WhSlot)23, (WhSlot)22 },
                { (WhSlot)24, (WhSlot)0 },
                { (WhSlot)25, (WhSlot)21 },
                { (WhSlot)26, (WhSlot)21 },
                { (WhSlot)27, (WhSlot)27 }
            };

        public static readonly IReadOnlyDictionary<WhSlot, WhSlot> SlotAlternate =
            new Dictionary<WhSlot, WhSlot>()
            {
                { (WhSlot)0, (WhSlot)0 },
                { (WhSlot)1, (WhSlot)0 },
                { (WhSlot)2, (WhSlot)0 },
                { (WhSlot)3, (WhSlot)0 },
                { (WhSlot)4, (WhSlot)0 },
                { (WhSlot)5, (WhSlot)0 },
                { (WhSlot)6, (WhSlot)0 },
                { (WhSlot)7, (WhSlot)0 },
                { (WhSlot)8, (WhSlot)0 },
                { (WhSlot)9, (WhSlot)0 },
                { (WhSlot)10, (WhSlot)0 },
                { (WhSlot)11, (WhSlot)0 },
                { (WhSlot)12, (WhSlot)0 },
                { (WhSlot)13, (WhSlot)22 },
                { (WhSlot)14, (WhSlot)0 },
                { (WhSlot)15, (WhSlot)0 },
                { (WhSlot)16, (WhSlot)0 },
                { (WhSlot)17, (WhSlot)22 },
                { (WhSlot)18, (WhSlot)0 },
                { (WhSlot)19, (WhSlot)0 },
                { (WhSlot)20, (WhSlot)0 },
                { (WhSlot)21, (WhSlot)0 },
                { (WhSlot)22, (WhSlot)0 },
                { (WhSlot)23, (WhSlot)0 },
                { (WhSlot)24, (WhSlot)0 },
                { (WhSlot)25, (WhSlot)0 },
                { (WhSlot)26, (WhSlot)0 },
                { (WhSlot)27, (WhSlot)0 }
            };

        public static readonly IReadOnlyDictionary<WhSlot, WhType> SlotType =
            new Dictionary<WhSlot, WhType>()
            {
                { (WhSlot)0, (WhType)0 },
                { (WhSlot)1, (WhType)2 },
                { (WhSlot)2, (WhType)0 },
                { (WhSlot)3, (WhType)4 },
                { (WhSlot)4, (WhType)128 },
                { (WhSlot)5, (WhType)128 },
                { (WhSlot)6, (WhType)128 },
                { (WhSlot)7, (WhType)128 },
                { (WhSlot)8, (WhType)128 },
                { (WhSlot)9, (WhType)128 },
                { (WhSlot)10, (WhType)128 },
                { (WhSlot)11, (WhType)0 },
                { (WhSlot)12, (WhType)0 },
                { (WhSlot)13, (WhType)1 },
                { (WhSlot)14, (WhType)1 },
                { (WhSlot)15, (WhType)1 },
                { (WhSlot)16, (WhType)128 },
                { (WhSlot)17, (WhType)1 },
                { (WhSlot)18, (WhType)0 },
                { (WhSlot)19, (WhType)128 },
                { (WhSlot)20, (WhType)128 },
                { (WhSlot)21, (WhType)1 },
                { (WhSlot)22, (WhType)1 },
                { (WhSlot)23, (WhType)1 },
                { (WhSlot)24, (WhType)0 },
                { (WhSlot)25, (WhType)1 },
                { (WhSlot)26, (WhType)1 },
                { (WhSlot)27, (WhType)2 }
            };

        public static readonly IReadOnlyDictionary<WhSlot, int> SlotOrder =
            new Dictionary<WhSlot, int>()
            {
                { (WhSlot)0, 0 },
                { (WhSlot)1, 16 },
                { (WhSlot)2, 0 },
                { (WhSlot)3, 15 },
                { (WhSlot)4, 1 },
                { (WhSlot)5, 7 },
                { (WhSlot)6, 11 },
                { (WhSlot)7, 5 },
                { (WhSlot)8, 6 },
                { (WhSlot)9, 9 },
                { (WhSlot)10, 8 },
                { (WhSlot)11, 0 },
                { (WhSlot)12, 0 },
                { (WhSlot)13, 17 },
                { (WhSlot)14, 18 },
                { (WhSlot)15, 19 },
                { (WhSlot)16, 14 },
                { (WhSlot)17, 20 },
                { (WhSlot)18, 0 },
                { (WhSlot)19, 12 },
                { (WhSlot)20, 8 },
                { (WhSlot)21, 21 },
                { (WhSlot)22, 22 },
                { (WhSlot)23, 23 },
                { (WhSlot)24, 0 },
                { (WhSlot)25, 24 },
                { (WhSlot)26, 25 },
                { (WhSlot)27, 0 }
            };

        public static readonly IReadOnlyDictionary<WhRace, int[]> RaceFallbacks = new Dictionary<WhRace, int[]>()
        {
            { (WhRace)36, new int[] { 2, 0, 2, 1, 2, 0, 2, 1 } },
            { (WhRace)35, new int[] { 9, 0, 9, 1, 9, 0, 9, 1 } },
            { (WhRace)34, new int[] { 3, 0, 3, 1, 3, 0, 3, 1 } },
            { (WhRace)33, new int[] { 5, 1, 0, -1, 5, 0, 0, -1 } },
            { (WhRace)31, new int[] { 0, -1, 8, 1, 0, -1, 8, 1 } },
            { (WhRace)30, new int[] { 11, 0, 11, 1, 11, 0, 11, 1 } },
            { (WhRace)29, new int[] { 10, 0, 10, 1, 10, 0, 10, 1 } },
            { (WhRace)28, new int[] { 6, 0, 6, 1, 6, 0, 6, 1 } },
            { (WhRace)27, new int[] { 4, 0, 4, 1, 4, 0, 4, 1 } },
            { (WhRace)26, new int[] { 24, 0, 24, 1, 24, 0, 24, 1 } },
            { (WhRace)25, new int[] { 24, 0, 24, 1, 24, 0, 24, 1 } },
            { (WhRace)23, new int[] { 1, 0, 1, 1, 1, 0, 1, 1 } },
            { (WhRace)15, new int[] { 5, 0, 5, 1, 5, 0, 5, 1 } }
        };

        public static IReadOnlyDictionary<string, bool> StandingMounts = new Dictionary<string, bool>()
        {
            { "28060", true },
            { "28063", true },
            { "28082", true },
            { "41903", true },
            { "42147", true },
            { "44808", true },
            { "45271", true }
        };

        public static IReadOnlyDictionary<string, bool> ReversedItems = new Dictionary<string, bool>()
        {
            { "139260", true },
            { "45146", true }
        };

        public static IReadOnlyDictionary<string, bool> ReversedModels = new Dictionary<string, bool>()
        {
            { "147259", true }
        };
    }
}
