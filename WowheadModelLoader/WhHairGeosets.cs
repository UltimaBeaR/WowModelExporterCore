using System.Collections.Generic;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    public class WhHairGeosets
    {
        public WhHairGeosets(WhJsonHairGeosets data)
        {
            HairGeosets = data.hairGeosets;
        }

        public Dictionary<WhCharVariationType, WhJsonHairGeoset[]> HairGeosets { get; set; }

        public WhJsonHairGeoset GetHairGeosetByVariationId(WhCharVariationType variationType, int variationID)
        {
            var hairGeosets = HairGeosets.GetOrDefault(variationType);
            if (hairGeosets == null)
                return null;

            for (var i = 0; i < hairGeosets.Length; i++)
            {
                if (hairGeosets[i].variationID == variationID)
                    return hairGeosets[i];
            }

            return null;
        }

        public WhJsonHairGeoset GetHairGeosetByColorIndex(WhCharVariationType variationType, int colorIndex)
        {
            var hairGeosets = HairGeosets.GetOrDefault(variationType);
            if (hairGeosets == null)
                return null;

            for (var i = 0; i < hairGeosets.Length; i++)
            {
                if (hairGeosets[i].colorIndex == colorIndex)
                    return hairGeosets[i];
            }

            return null;
        }

        public int GetHairGeosetConditional(WhCharVariationType variationType, int colorIndex, int geosetType)
        {
            var hairGeosets = HairGeosets.GetOrDefault(variationType);
            if (hairGeosets == null)
                return -1;

            for (var i = 0; i < hairGeosets.Length; i++)
            {
                if (hairGeosets[i].colorIndex == colorIndex && hairGeosets[i].geosetType == geosetType)
                    return hairGeosets[i].geosetID;
            }

            return -1;
        }
    }
}
