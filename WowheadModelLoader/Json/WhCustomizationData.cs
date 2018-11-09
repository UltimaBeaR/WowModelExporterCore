using System.Collections.Generic;

namespace WowheadModelLoader.Json
{
    public class WhJsonCustomizationData
    {
        public WhJsonCustomFeatures customFeatures { get; set; }
        public WhJsonHairGeosets hairGeosets { get; set; }
        public WhJsonFacialHairStyles facialHairStyles { get; set; }

        public int CustomGeoFileDataID { get; set; }
        public int HDCustomGeoFileDataID { get; set; }
    }

    public class WhJsonCustomFeatures
    {
        public Dictionary<int, WhJsonCustomFeature[]> features { get; set; }
    }

    public class WhJsonCustomFeature
    {
        public uint[] textures { get; set; }
        public int flags { get; set; }
        public int variationIndex { get; set; }
        public int colorIndex { get; set; }
    }

    public class WhJsonHairGeosets
    {
        public Dictionary<WhCharVariationType, WhJsonHairGeoset[]> hairGeosets { get; set; }
    }

    public class WhJsonHairGeoset
    {
        public int variationID { get; set; }
        public int geosetID { get; set; }
        public int geosetType { get; set; }
        public int showscalp { get; set; }
        public int colorIndex { get; set; }
    }

    public class WhJsonFacialHairStyles
    {
        public WhJsonFacialHairStyle[] styles { get; set; }
    }

    public class WhJsonFacialHairStyle
    {
        public int[] geoset { get; set; }
        public int variationID { get; set; }
    }
}
