namespace WowheadModelLoader
{
    public class WhViewerOptions
    {
        public WhClass Cls { get; set; }
        public bool Hd { get; set; }
        public Item[] Items { get; set; }

        public class Item
        {
            public WhSlot Slot { get; set; }
            public int Id { get; set; }
            // ToDo: хз пока откуда это брать, в примере undefined (нет этого элемента массива)
            public int? VisualId { get; set; }
        }
    }
}
