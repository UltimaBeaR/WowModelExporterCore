using System.Collections.Generic;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    public class WhCustomFeatures
    {
        public WhCustomFeatures(WhJsonCustomFeatures customFeatures)
        {
            Features = customFeatures.features;
        }

        public WhJsonCustomFeature[] GetFeatures(int baseSection)
        {
            if (Features.TryGetValue(baseSection, out var result))
                return result;

            return null;
        }

        public WhJsonCustomFeature GetFeature(int baseSection, int variationIndex, int colorIndex)
        {
            var numVariations = GetVariationsCount(baseSection, variationIndex);

            var features = GetFeatures(baseSection);

            if (features != null)
            {
                for (int i = 0; i < features.Length; i++)
                {
                    var feature = features[i];

                    if (feature.variationIndex == variationIndex && (numVariations > 1 ? feature.colorIndex == colorIndex : true))
                        return feature;
                }
            }

            return null;
        }

        public int GetVariationsCount(int baseSection, long variationIndex)
        {
            int count = 0;

            var features = GetFeatures(baseSection);

            if (features != null)
            {
                for (int i = 0; i < features.Length; i++)
                {
                    var feature = features[i];
                    if (feature.variationIndex == variationIndex)
                        count++;
                }
            }

            return count;
        }

        public Dictionary<int, WhJsonCustomFeature[]> Features { get; set; }
    }
}
