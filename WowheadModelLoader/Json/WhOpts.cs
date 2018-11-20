namespace WowheadModelLoader.Json
{
    public class WhOpts
    {
        public int type { get; set; }
        public string contentPath { get; set; }

        // ToDo: { "0": {}, "context": {}, "length": 1 }
        public object container { get; set; }

        public float aspect { get; set; }
        public string background { get; set; }

        public int sheathMain { get; set; }
        public int sheathOff { get; set; }

        public int sk { get; set; }
        public int ha { get; set; }
        public int hc { get; set; }
        public int fa { get; set; }
        public int fh { get; set; }
        public int fc { get; set; }
        public int ep { get; set; }
        public int ho { get; set; }
        public int ta { get; set; }
        public int cls { get; set; }
        public int[][] items { get; set; }
        public Model mount { get; set; }
        public Model models { get; set; } 
        public bool hd { get; set; }

        public class Model
        {
            public int type { get; set; }
            public string id { get; set; }
        }
    }
}
