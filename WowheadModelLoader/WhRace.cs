using System.Collections.Generic;

namespace WowheadModelLoader
{
    /// <summary>
    /// ZamModelViewer.Wow.Races
    /// </summary>
    public enum WhRace
    {
        Undefined = -1,
        Undefined2 = 0,
        HUMAN = 1,
        ORC = 2,
        DWARF = 3,
        NIGHTELF = 4,
        SCOURGE = 5,
        TAUREN = 6,
        GNOME = 7,
        TROLL = 8,
        GOBLIN = 9,
        BLOODELF = 10,
        DRAENEI = 11,
        FELORC = 12,
        NAGA = 13,
        BROKEN = 14,
        SKELETON = 15,
        VRYKUL = 16,
        TUSKARR = 17,
        FORESTTROLL = 18,
        TAUNKA = 19,
        NORTHRENDSKELETON = 20,
        ICETROLL = 21,
        WORGEN = 22,
        WORGENHUMAN = 23,
        PANDAREN = 24,
        PANDAREN_A = 25,
        PANDAREN_H = 26,
        NIGHTBORNE = 27,
        HIGHMOUNTAINTAUREN = 28,
        VOIDELF = 29,
        LIGHTFORGEDDRAENEI = 30,
        ZANDALARITROLL = 31,
        KULTIRAN = 32,
        THINHUMAN = 33,
        DARKIRONDWARF = 34,
        VULPERA = 35,
        MAGHARORC = 36,
        UPRIGHTORC = 37
    }

    public static class WhRaceExtensions
    {
        /// <summary>
        /// Продолжение ZamModelViewer.Wow.Races но для ключей = самим значениям энама
        /// </summary>
        public static string GetStringIdentifier(this WhRace race)
        {
            return _stringIdentifiers[race];
        }

        private static readonly Dictionary<WhRace, string> _stringIdentifiers = new Dictionary<WhRace, string>()
        {
            { WhRace.HUMAN, "human" },
            { WhRace.ORC, "orc" },
            { WhRace.DWARF, "dwarf" },
            { WhRace.NIGHTELF, "nightelf" },
            { WhRace.SCOURGE, "scourge" },
            { WhRace.TAUREN, "tauren" },
            { WhRace.GNOME, "gnome" },
            { WhRace.TROLL, "troll" },
            { WhRace.GOBLIN, "goblin" },
            { WhRace.BLOODELF, "bloodelf" },
            { WhRace.DRAENEI, "draenei" },
            { WhRace.FELORC, "felorc" },
            { WhRace.NAGA, "naga_" },
            { WhRace.BROKEN, "broken" },
            { WhRace.SKELETON, "skeleton" },
            { WhRace.VRYKUL, "vrykul" },
            { WhRace.TUSKARR, "tuskarr" },
            { WhRace.FORESTTROLL, "foresttroll" },
            { WhRace.TAUNKA, "taunka" },
            { WhRace.NORTHRENDSKELETON, "northrendskeleton" },
            { WhRace.ICETROLL, "icetroll" },
            { WhRace.WORGEN, "worgen" },
            { WhRace.WORGENHUMAN, "gilnean" },
            { WhRace.PANDAREN, "pandaren" },
            { WhRace.PANDAREN_A, "pandarena" },
            { WhRace.PANDAREN_H, "pandarenh" },
            { WhRace.NIGHTBORNE, "nightborne" },
            { WhRace.HIGHMOUNTAINTAUREN, "highmountaintauren" },
            { WhRace.VOIDELF, "voidelf" },
            { WhRace.LIGHTFORGEDDRAENEI, "lightforgeddraenei" },
            { WhRace.ZANDALARITROLL, "zandalaritroll" },
            { WhRace.KULTIRAN, "kultiran" },
            { WhRace.THINHUMAN, "thinhuman" },
            { WhRace.DARKIRONDWARF, "darkirondwarf" },
            { WhRace.VULPERA, "vulpera" },
            { WhRace.MAGHARORC, "magharorc" }
        };
    }
}
