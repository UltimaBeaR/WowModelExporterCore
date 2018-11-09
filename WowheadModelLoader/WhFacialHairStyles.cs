using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    public class WhFacialHairStyles
    {
        public WhFacialHairStyles(WhJsonFacialHairStyles data)
        {
            Styles = data.styles;
        }

        public WhJsonFacialHairStyle[] Styles { get; set; }

        public WhJsonFacialHairStyle GetStyle(int variationID)
        {
            for (var i = 0; i < Styles.Length; i++)
            {
                if (Styles[i].variationID == variationID)
                    return Styles[i];
            }

            return null;
        }
    }
}
