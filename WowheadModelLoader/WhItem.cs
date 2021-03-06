﻿using System;
using System.Collections.Generic;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    public class WhItem
    {
        public WhItem(WhModel model, WhSlot slot, int id, WhRace race, WhGender gender)
        {
            Model = model;
            Slot = slot;
            UniqueSlot = WhGlobal.UniqueSlots[slot];
            SortValue = WhGlobal.SlotOrder[slot];
            Models = null;
            Textures = null;
            GeosetGroup = null;
            Flags = 0;
            Loaded = false;
            Visual = null;
            Visualid = 0;

            if (id != 0)
                Load(id, race, gender);
        }

        public WhModel Model { get; set; }
        public WhSlot Slot { get; set; }
        public WhSlot UniqueSlot { get; set; }
        public int SortValue { get; set; }
        public List<WhItemModel> Models { get; set; }
        public List<WhItemTexture> Textures { get; set; }
        public int[] GeosetGroup { get; set; }
        public int Flags { get; set; }
        public bool Loaded { get; set; }
        public WhItemVisual Visual { get; set; }
        public int Visualid { get; set; }

        public int Id { get; set; }
        public WhRace Race { get; set; }
        public WhGender Gender { get; set; }

        public int[] HideGeosetMale { get; set; }
        public int[] HideGeosetFemale { get; set; }

        public int ItemClass { get; set; }
        public int ItemSubClass { get; set; }

        public WhModel CollectionModel { get; set; }

        public void Load(int id, WhRace race, WhGender gender)
        {
            Id = id;
            Race = race;
            Gender = gender;

            var metaPath = "item";
            if (Slot == WhSlot.HEAD || Slot == WhSlot.SHOULDER || Slot == WhSlot.SHIRT ||
                Slot == WhSlot.CHEST || Slot == WhSlot.BELT || Slot == WhSlot.PANTS ||
                Slot == WhSlot.BOOTS || Slot == WhSlot.BRACERS || Slot == WhSlot.HANDS ||
                Slot == WhSlot.CAPE || Slot == WhSlot.TABARD || Slot == WhSlot.ROBE)
            {
                metaPath = $"armor/{(int)Slot}";
            }

            WhDefferedList.Add(() => LoadAndHandle_Meta(metaPath, id));
        }

        private uint SelectBestTexture(WhJsonMeta meta, string textureid, WhGender gender, WhClass cls, WhRace race) {
            var textures = meta.TextureFiles[textureid];

            for (int i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                if (texture.Race != 0 && texture.Race == race && (texture.Class != 0 && texture.Class == cls) && (texture.Gender == gender || texture.Gender == WhGender.Undefined3))
                    return texture.FileDataId;
            }

            for (int i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                if (texture.Race != 0 && texture.Race == race && (texture.Gender == gender || texture.Gender == WhGender.Undefined3))
                    return texture.FileDataId;
            }

            for (int i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                if (texture.Class != 0 && texture.Class == cls && (texture.Gender == gender || texture.Gender == WhGender.Undefined3))
                    return texture.FileDataId;
            }

            for (int i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                if (texture.Gender == gender || texture.Gender == WhGender.Undefined3)
                    return texture.FileDataId;
            }

            throw new System.NotImplementedException("selectBestTexture for " + textureid + ", gender " + gender + ", class " + cls + ", race " + race + " failed!");
        }

        public void LoadMeta(WhJsonMeta meta)
        {
            Flags = meta.Item.Flags;
            Slot = meta.Item.InventoryType;
            ItemClass = meta.Item.ItemClass;
            ItemSubClass = meta.Item.ItemSubClass;

            if (meta.ComponentTextures != null) {
                Textures = new List<WhItemTexture>();
                foreach (var componentTexture in meta.ComponentTextures) {
                    var region = componentTexture.Key;
                    var texFile = SelectBestTexture(meta, componentTexture.Value, Model.Gender, Model.Class, Model.Race);

                    WhItemTexture texture = null;
                    if (texFile != 0)
                    {
                        texture = new WhItemTexture()
                        {
                            Region = region,
                            Gender = Model.Gender,
                            File = texFile,
                            Texture = null
                        };

                        if (region != WhRegion.Base)
                            texture.Texture = new WhTexture(Model, (int)region, texFile);
                        else if (Slot == WhSlot.CAPE)
                            Model.TextureOverrides[2] = new WhTexture(Model, 2, texFile);
                    }

                    Textures.Add(texture);
                }
            }

            GeosetGroup = meta.Item.GeosetGroup;
            if (Slot == WhSlot.HEAD)
            {
                var gender = Model.Gender;
                var hideGeoset = gender == WhGender.MALE ? meta.Item.HideGeosetMale : meta.Item.HideGeosetFemale;
                if (gender == WhGender.MALE)
                    HideGeosetMale = meta.Item.HideGeosetMale;
                else
                    HideGeosetFemale = meta.Item.HideGeosetFemale;
            }

            if (Slot == WhSlot.SHOULDER)
                Models = new List<WhItemModel>() { null, null };
            else if (WhGlobal.SlotType[Slot] != WhType.ARMOR)
                Models = new List<WhItemModel>() { null };

            if (Models != null) {
                for (int i = 0; i < Models.Count; i++) {
                    var model = new WhItemModel()
                    {
                        Race = Race,
                        Gender = Gender,
                        Bone = -1,
                        Attachment = null,
                        Model = null
                    };

                    var modelInfo = new WhModelInfo() {
                        Type = WhGlobal.SlotType[Slot],
                        Id = Id.ToString(),
                        Parent = Model
                    };

                    if (Slot == WhSlot.SHOULDER)
                        modelInfo.Shoulder = i + 1;

                    model.Model = new WhModel(Model.Opts, modelInfo, i, true);
                    model.Model.LoadMeta(meta, modelInfo.Type);
                    Models[i] = model;
                }
            }

            if (Slot == WhSlot.BELT && meta.Model != 0) {
                var model = new WhItemModel()
                {
                    Race = WhRace.Undefined2,
                    Gender = WhGender.MALE,
                    Bone = -1,
                    Attachment = null,
                    Model = null
                };

                var modelInfo = new WhModelInfo()
                {
                    Type = WhGlobal.SlotType[Slot],
                    Id = Id.ToString(),
                    Parent = Model
                };

                model.Model = new WhModel(Model.Opts, modelInfo, 0, true);
                model.Model.LoadMeta(meta, WhType.ARMOR);
                Models = new List<WhItemModel>() { model };
            }

            if (Slot == WhSlot.SHIRT || Slot == WhSlot.CHEST || Slot == WhSlot.ROBE || Slot == WhSlot.BELT || Slot == WhSlot.PANTS || Slot == WhSlot.HANDS || Slot == WhSlot.BOOTS || Slot == WhSlot.HEAD)
            {
                int componentIndex = 0;
                if (Slot == WhSlot.HEAD)
                    componentIndex = 1;

                if (meta.ComponentModels != null && meta.ComponentModels.ContainsKey(componentIndex.ToString()))
                {
                    var modelId = meta.ComponentModels[componentIndex.ToString()];

                    if (modelId != 0 && meta.ModelFiles.ContainsKey(modelId)) {
                        var modelInfo = new WhModelInfo() {
                            Type = WhGlobal.SlotType[Slot],
                            Id = Id.ToString(),
                            Parent = Model
                        };

                        var model = new WhModel(Model.Opts, modelInfo, 0, true);

                        model.Meta = meta;
                        model.IsDirty = true;

                        var race = WhRace.HUMAN;
                        var gender = WhGender.MALE;
                        var cls = WhClass.WARRIOR;

                        if (Model != null)
                        {
                            race = Model.Race;
                            gender = Model.Gender;
                            cls = Model.Class;
                        }

                        var modelFile = model.SelectBestModel(modelId, gender, cls, race);
                        if (modelFile != 0)
                        {
                            if (Model != null) {
                                if (!Model.CollectionModels.ContainsKey(modelFile))
                                {
                                    Model.CollectionModels[modelFile] = model;
                                    CollectionModel = model;
                                    model._Load(WhType.PATH, modelFile.ToString());
                                }
                                else
                                    CollectionModel = Model.CollectionModels[modelFile];
                            }
                            else
                                model._Load(WhType.PATH, modelFile.ToString());

                            if (meta.Textures != null)
                            {
                                foreach (var texturePair in meta.Textures)
                                    model.TextureOverrides[texturePair.Key] = new WhTexture(model, texturePair.Key, texturePair.Value);
                            }
                        }
                    }
                }
            }

            if (Slot == WhSlot.PANTS && GeosetGroup[2] > 0)
                SortValue += 2;

            if (Visualid != 0)
                Visual = new WhItemVisual(Models[0].Model, Visualid);

            Loaded = true;

            Model.UpdateMeshes();
        }

        public void SetVisual(int id)
        {
            //if (Visual != null)
            //{
            //    // было так, обнуления нет
            //    //self.visual.destroy()
            //}

            Visualid = id;
        }

        private void LoadAndHandle_Meta(string metaPath, int id)
        {
            var data = WhDataLoader.LoadMeta(metaPath, id.ToString());

            LoadMeta(data);
        }
    }

    public class WhItemModel
    {
        public WhRace Race { get; set; }
        public WhGender Gender { get; set; }
        public int Bone { get; set; }
        public WhAttachment Attachment { get; set; }
        public WhModel Model { get; set; }
        public bool IsCollection { get; set; }
    }

    public class WhItemTexture
    {
        public WhRegion Region { get; set; }
        public WhGender Gender { get; set; }
        public uint File { get; set; }
        public WhTexture Texture { get; set; }
    }
}
