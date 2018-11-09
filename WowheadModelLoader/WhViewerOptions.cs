namespace WowheadModelLoader
{
    public class WhViewerOptions
    {
        public WhClass Cls { get; set; }
        public bool Hd { get; set; }
        public Item[] Items { get; set; }

        public int SheathMain { get; set; }
        public int SheathOff { get; set; }

        public int sk { get; set; }
        public int ha { get; set; }
        public int hc { get; set; }
        public int fa { get; set; }
        public int fh { get; set; }
        public int fc { get; set; }
        public int ho { get; set; }
        public int ep { get; set; }
        public int ta { get; set; }

        public class Item
        {
            public WhSlot Slot { get; set; }
            public int Id { get; set; }
            // ToDo: хз пока откуда это брать, в примере undefined (нет этого элемента массива)
            public int? VisualId { get; set; }
        }
    }
}
