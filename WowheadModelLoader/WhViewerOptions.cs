using Newtonsoft.Json;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    public class WhViewerOptions
    {
        // Это я добавил. Это должно быть в rendered.time, но так как я передаю в модели только этот объект, я решил записать это сюда.
        public int CurrentTime { get; set; }

        public int Type { get; set; }
        public string ContentPath { get; set; }

        // ToDo: { "0": {}, "context": {}, "length": 1 }
        public object Container { get; set; }

        public int SheathMain { get; set; }
        public int SheathOff { get; set; }

        public int sk { get; set; }
        public int ha { get; set; }
        public int hc { get; set; }
        public int fa { get; set; }
        public int fh { get; set; }
        public int fc { get; set; }
        public int ep { get; set; }
        public int ho { get; set; }
        public int ta { get; set; }

        public WhClass Cls { get; set; }

        public Item[] Items { get; set; }
        public WhModelInfo Model { get; set; }
        public WhModelInfo Mount { get; set; }
        public bool Hd { get; set; }

        public class Item
        {
            public WhSlot Slot { get; set; }
            public int Id { get; set; }
            // ToDo: хз пока откуда это брать, в примере undefined (нет этого элемента массива)
            public int? VisualId { get; set; }
        }

        public static WhViewerOptions FromJson(string json)
        {
            var whOpts = JsonConvert.DeserializeObject<WhOpts>(json);

            var result = new WhViewerOptions()
            {
                Container = whOpts.container,

                ContentPath = whOpts.contentPath,

                Type = whOpts.type,
                Cls = (WhClass)whOpts.cls,

                Hd = whOpts.hd,

                SheathMain = whOpts.sheathMain,
                SheathOff = whOpts.sheathOff,

                sk = whOpts.sk,
                ha = whOpts.ha,
                hc = whOpts.hc,
                fa = whOpts.fa,
                fh = whOpts.fh,
                fc = whOpts.fc,
                ep = whOpts.ep,
                ho = whOpts.ho,
                ta = whOpts.ta
            };

            if (whOpts.mount != null)
            {
                result.Mount = new WhModelInfo()
                {
                    Id = whOpts.mount.id,
                    Type = (WhType)whOpts.mount.type
                };
            }

            if (whOpts.models != null)
            {
                result.Model = new WhModelInfo()
                {
                    Id = whOpts.models.id,
                    Type = (WhType)whOpts.models.type
                };
            }

            if (whOpts.items != null)
            {
                result.Items = new Item[whOpts.items.Length];
                for (int i = 0; i < whOpts.items.Length; i++)
                {
                    result.Items[i] = new Item()
                    {
                        Slot = (WhSlot)whOpts.items[i][0],
                        Id = whOpts.items[i][1]
                    };

                    // Не уверен, но вроде так
                    if (whOpts.items[i].Length >= 3)
                        result.Items[i].VisualId = whOpts.items[i][2];
                }
            }

            return result;
        }
    }
}
