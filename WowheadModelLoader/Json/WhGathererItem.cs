using Newtonsoft.Json;

namespace WowheadModelLoader.Json
{
    public class WhJsonGathererItem
    {
        [JsonProperty("name_enus")]
        public string Name { get; set; }

        [JsonProperty("jsonequip")]
        public WhJsonGathererItemOtherData OtherData { get; set; }
    }

    public class WhJsonGathererItemOtherData
    {
        [JsonProperty("displayid")]
        public int DisplayId { get; set; }

        [JsonProperty("slotbak")]
        public WhSlot SlotBak { get; set; }
    }
}
