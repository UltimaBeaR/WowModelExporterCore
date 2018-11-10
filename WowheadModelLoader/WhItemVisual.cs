using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    // ToDo: эти штуки создаются только если при изначальном создании модели в опциях в итемах в месте с типом и id передаются itemvisual id, то есть пока я не передам, эту штуку не протестируешь
    public class WhItemVisual
    {
        public WhItemVisual(WhModel model, int id)
        {
            Model = model;
            Models = null;
            Loaded = false;

            if (id != 0)
                Load(id);
        }

        public WhModel Model { get; set; }
        public WhItemModel[] Models { get; set; }
        public bool Loaded { get; set; }
        public int Id { get; set; }

        public void Load(int id)
        {
            Id = id;

            LoadAndHandle_ItemVisual(Id);
        }

        // ToDo: я не уверен что тут тип меты такой - надо протестить (аналогично везде, в загрузке из json-а и тд)
        public void LoadMeta(WhJsonMeta meta)
        {
            Models = new WhItemModel[5];

            for (int i = 0; i < Models.Length; i++)
            {
                // ToDo: не факт что это правильно так как там jtoken (возможно он вообще null не может быть тут),
                // надо узнать какой вариант сериализации тут (там 2 возможных вариант, массив строк и массив объектов)
                // пока сделал через список строк, так как массив строк также используется где-то там где itemvisual хэндлит загрузку mo3
                if (meta.GetEquipmentAsStringIds()[i] != null)
                {
                    Models[i] = new WhItemModel
                    {
                        Race = WhRace.Undefined2,
                        Gender = WhGender.MALE,
                        Bone = -1,
                        Attachment = null,
                        Model = null
                    };

                    var modelInfo = new WhModelInfo
                    {
                        Type = WhType.ITEMVISUAL,
                        Id = Id.ToString(),
                        Parent = Model
                    };

                    Models[i].Model = new WhModel(Model.Opts, modelInfo, i);
                }
            }

            Loaded = true;
            Model.UpdateMeshes();
        }

        private void LoadAndHandle_ItemVisual(int id)
        {
            var data = WhDataLoader.LoadItemVisual(Id);

            LoadMeta(data);
        }
    }
}
