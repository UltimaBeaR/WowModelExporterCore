using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    // ToDo: неплохо было бы по всему этому проекту заменить get по индексу в dictionary на DictionaryExtensions.GetOrDefault()

    /// <summary>
    /// ZamModelViewer.Wow.Model
    /// </summary>
    public class WhModel
    {
        public WhModel(WhViewerOptions viewerOptions, WhModelInfo model, int index, bool skipLoad = false)
        {
            Model = model;
            ModelIndex = index;
            ModelPath = null;

            Loaded = false;

            //self.particlesEnabled = false;
            //self.ribbonsEnabled = false;
            //self.deferredParentingMatrix = mat4.create();

            Opts = viewerOptions;

            Mount = null;

            IsMount = Opts.Mount != null && Opts.Mount.Type == WhType.NPC && Opts.Mount.Id == Model.Id;

            if (Model.Type == WhType.CHARACTER)
            {
                if (Opts.Mount != null && Opts.Mount.Type == WhType.NPC && (Opts.Mount.Id != null && Opts.Mount.Id != "0"))
                {
                    Opts.Mount.Parent = this;
                    Mount = new WhModel(viewerOptions, Opts.Mount, 0);
                }
            }

            Race = WhRace.Undefined;
            Gender = WhGender.Undefined;
            Class = Enum.IsDefined(typeof(WhClass), Opts.Cls) ? Opts.Cls : WhClass.Undefined;

            Meta = null;

            SkinIndex = 0;
            HairIndex = 0;
            HairColorIndex = 0;
            FaceIndex = 0;
            FeaturesIndex = 0;
            FeaturesColorIndex = 0;
            HornsIndex = 0;
            EyePatchIndex = 0;
            TattoosIndex = 0;

            Parent = Model.Parent;

            Items = new Dictionary<WhSlot, WhItem>();

            NeedsCompositing = false;

            CustomFeatures = null;

            TextureOverrides = new Dictionary<int, WhTexture>();

            CompositeTexture = null;

            NpcTexture = null;
            SpecialTextures = new Dictionary<int, WhTexture>();

            BakedTextures = new Dictionary<WhRegion, Dictionary<int, WhTexture>>()
            {
                { (WhRegion)0, new Dictionary<int, WhTexture>() },
                { (WhRegion)1, new Dictionary<int, WhTexture>() },
                { (WhRegion)2, new Dictionary<int, WhTexture>() },
                { (WhRegion)3, new Dictionary<int, WhTexture>() },
                { (WhRegion)4, new Dictionary<int, WhTexture>() },
                { (WhRegion)5, new Dictionary<int, WhTexture>() },
                { (WhRegion)6, new Dictionary<int, WhTexture>() },
                { (WhRegion)7, new Dictionary<int, WhTexture>() },
                { (WhRegion)8, new Dictionary<int, WhTexture>() },
                { (WhRegion)9, new Dictionary<int, WhTexture>() },
                { (WhRegion)10, new Dictionary<int, WhTexture>() },
                { (WhRegion)11, new Dictionary<int, WhTexture>() },
                { (WhRegion)12, new Dictionary<int, WhTexture>() },
                { (WhRegion)13, new Dictionary<int, WhTexture>() }
            };

            IsHD = false;

            SheathMain = -1;
            SheathOff = -1;

            NumGeosets = 29;
            Geosets = new ushort[NumGeosets];
            GeosetDefaults = new ushort[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1 };
            CreatureGeosetData = 0;
            for (int i = 0; i < NumGeosets; i++)
                Geosets[i] = (ushort)(i * 100 + GeosetDefaults[i]);

            Time = 0;
            Frame = 0;

            StartAnimation = null;
            CurrentAnimation = null;
            AnimStartTime = 0;

            AnimPaused = false;

            //self.matrix = mat4.create();
            //self.vbData = null;
            //self.vb = null;
            //self.ib = null;
            //self.shaderReady = false;
            //self.vs = null;
            //self.fs = null;
            //self.program = null;
            //self.uniforms = null;
            //self.attribs = null;
            //self.ambientColor = [.35, .35, .35, 1];
            //self.primaryColor = [1, 1, 1, 1];
            //self.secondaryColor = [.35, .35, .35, 1];
            //self.lightDir1 = vec3.create();
            //self.lightDir2 = vec3.create();
            //self.lightDir3 = vec3.create();
            //vec3.normalize(self.lightDir1, [5, -3, 3]);
            //vec3.normalize(self.lightDir2, [5, 5, 5]);
            //vec3.normalize(self.lightDir3, [-5, -5, -5]);
            //self.boundsSet = false;
            //self.animBounds = false;
            //self.boundsMin = [0, 0, 0];
            //self.boundsMax = [0, 0, 0];
            //self.boundsCenter = [0, 0, 0];
            //self.boundsSize = [0, 0, 0];

            Vertices = null;
            Indices = null;

            Animations = null;

            AnimLookup = null;
            Bones = null;
            BoneLookup = null;
            KeyBoneLookup = null;

            Meshes = null;

            TexUnits = null;
            TexUnitLookup = null;

            RenderFlags = null;

            Materials = null;
            MaterialLookup = null;

            //self.textureAnims = null;
            TextureAnimLookup = null;
            TextureReplacements = null;

            Attachments = null;
            AttachmentLookup = null;

            //self.colors = null;
            //self.alphas = null;
            AlphaLookup = null;
            //self.particleEmitters = null;
            //self.ribbonEmitters = null;
            //self.tmpMat = mat4.create();
            //self.tmpVec = vec3.create();
            //self.tmpVec3 = vec3.create();
            //self.tmpVec4 = vec4.create();
            //self.mountMat = mat4.create(); 

            if (!skipLoad)
                Load();
        }

        public WhModelInfo Model { get; set; }
        public int ModelIndex { get; set; }
        public string ModelPath { get; set; }

        public bool Loaded { get; set; }

        public WhViewerOptions Opts { get; set; }

        public bool IsMount { get; set; }
        public WhModel Mount { get; set; }

        public WhRace Race { get; set; }
        public WhGender Gender { get; set; }
        public WhClass Class { get; set; }

        public WhJsonMeta Meta { get; set; }

        public int SkinIndex { get; set; }
        public int HairIndex { get; set; }
        public int HairColorIndex { get; set; }
        public int FaceIndex { get; set; }
        public int FeaturesIndex { get; set; }
        public int FaceColorIndex { get; set; }
        public int FeaturesColorIndex { get; set; }
        public int HornsIndex { get; set; }
        public int EyePatchIndex { get; set; }
        public int TattoosIndex { get; set; }

        public WhModel Parent { get; set; }

        public Dictionary<WhSlot, WhItem> Items { get; set; }

        public bool NeedsCompositing { get; set; }

        public WhCustomFeatures CustomFeatures { get; set; }

        public Dictionary<int, WhTexture> TextureOverrides { get; set; }

        public Bitmap CompositeTexture { get; set; }

        public WhTexture NpcTexture { get; set; }
        public Dictionary<int, WhTexture> SpecialTextures { get; set; }
        public Dictionary<WhRegion, Dictionary<int, WhTexture>> BakedTextures { get; set; }

        public bool IsHD { get; set; }

        public int SheathMain { get; set; }
        public int SheathOff { get; set; }

        public int NumGeosets { get; set; }
        public ushort[] Geosets { get; set; }
        public ushort[] GeosetDefaults { get; set; }
        public int CreatureGeosetData { get; set; }

        public int Time { get; set; }
        public int Frame { get; set; }

        public WhAnimation StartAnimation { get; set; }
        public WhAnimation CurrentAnimation { get; set; }

        public int AnimStartTime { get; set; }

        public bool AnimPaused { get; set; }

        public WhVertex[] Vertices { get; set; }
        public ushort[] Indices { get; set; }

        public WhAnimation[] Animations { get; set; }

        public short[] AnimLookup { get; set; }
        public WhBone[] Bones { get; set; }
        public short[] BoneLookup { get; set; }
        public short[] KeyBoneLookup { get; set; }

        public WhMesh[] Meshes { get; set; }

        public WhTexUnit[] TexUnits { get; set; }
        public short[] TexUnitLookup { get; set; }

        public WhRenderFlag[] RenderFlags { get; set; }

        public WhMaterial[] Materials { get; set; }
        public short[] MaterialLookup { get; set; }

        public short[] TextureAnimLookup { get; set; }
        public short[] TextureReplacements { get; set; }

        public WhAttachment[] Attachments { get; set; }
        public short[] AttachmentLookup { get; set; }

        public short[] AlphaLookup { get; set; }

        public WhHairGeosets HairGeosets { get; set; }
        public WhJsonHairGeoset CurrentHairGeoset { get; set; }

        public WhFacialHairStyles FacialHairStyles { get; set; }

        public WhModel HornsModel { get; set; }

        public List<WhTexUnit> SortedTexUnits { get; set; }

        public int Bone { get; set; }
        public WhAttachment Attachment { get; set; }

        public void Update()
        {
            if (!Loaded || TexUnits == null)
                return;

            Frame++;

            Time = Opts.CurrentTime;

            if (CurrentAnimation != null)
            {
                if (AnimStartTime == 0)
                    AnimStartTime = Time;
                if (Time - AnimStartTime >= CurrentAnimation.Length)
                {
                    var nextAnim = CurrentAnimation.Next;
                    if (CurrentAnimation.Id != 0 && nextAnim != -1)
                    {
                        CurrentAnimation = Animations[nextAnim];
                        AnimStartTime = Time;
                    }
                    else
                    {
                        CurrentAnimation = StartAnimation;
                        AnimStartTime = Time;
                    }
                }
            }

            if (Race == WhRace.GOBLIN && Gender == WhGender.MALE && CurrentAnimation != null && CurrentAnimation.Name.StartsWith("Emote"))
            {
                SortedTexUnits[0].Show = false;
                SortedTexUnits[6].Show = false;
            }

            var numUnits = TexUnits.Length;

            for (int i = 0; i < numUnits; i++)
            {
                var u = TexUnits[i];
                if (!u.Show)
                    continue;

                var count = u.Mesh.IndexCount;
                var start = u.Mesh.IndexStart;

                for (int j = 0; j < count; ++j)
                    Vertices[Indices[start + j]].Frame = Frame;
            }

            var numBones = Bones.Length;

            var animTime = Time - AnimStartTime;

            //vb = self.vbData;

            if (Bones != null && Animations != null)
            {
                for (int i = 0; i < numBones; i++)
                    Bones[i].Updated = false;

                for (int i = 0; i < numBones; i++)
                    Bones[i].Update(animTime);

                if (Vertices != null)
                {
                    var numVerts = Vertices.Length;

                    //v, b, w, idx, tmpVec3 = self.tmpVec3, tmpVec4 = self.tmpVec4;

                    for (int i = 0; i < numVerts; i++)
                    {
                        var v = Vertices[i];
                        if (v.Frame != Frame)
                            continue;

                        var vertexSizeFloat = WhGlobal.VertexSize32;
                        var idx = i * vertexSizeFloat;

                        //vb[idx] = vb[idx + 1] = vb[idx + 2] = vb[idx + 3] = vb[idx + 4] = vb[idx + 5] = 0;
                        //for (j = 0; j < 4; ++j)
                        //{
                        //    w = v.weights[j] / 255;
                        //    if (w > 0)
                        //    {
                        //        b = self.bones[v.bones[j]];
                        //        vec3.transformMat4(tmpVec3, v.position, b.matrix);
                        //        vec4.transformMat4(tmpVec4, v.normal, b.matrix);
                        //        vb[idx + 0] += tmpVec3[0] * w;
                        //        vb[idx + 1] += tmpVec3[1] * w;
                        //        vb[idx + 2] += tmpVec3[2] * w;
                        //        vb[idx + 3] += tmpVec4[0] * w;
                        //        vb[idx + 4] += tmpVec4[1] * w;
                        //        vb[idx + 5] += tmpVec4[2] * w
                        //    }
                        //}
                        //v.transPosition[0] = vb[idx + 0];
                        //v.transPosition[1] = vb[idx + 1];
                        //v.transPosition[2] = vb[idx + 2];
                        //v.transNormal[0] = vb[idx + 3];
                        //v.transNormal[1] = vb[idx + 4];
                        //v.transNormal[2] = vb[idx + 5]
                    }

                    //self.updateBuffers(false);
                    //if (!self.animBounds)
                    //{
                    //    self.animBounds = true;
                    //    self.updateBounds()
                    //}
                }
            }
        }

        public void SetAnimation(string name)
        {
            for (var i = 0; i < Animations.Length; i++)
            {
                var anim = Animations[i];

                if (anim.Name == null)
                    continue;

                if (anim.Name == name && anim.SubId == 0)
                {
                    AnimStartTime = 0;
                    StartAnimation = CurrentAnimation = anim;

                    //WH.debug("Set animation to", anim.id, anim.name);

                    break;
                }
            }

            if (name != "Stand" && CurrentAnimation == null)
                SetAnimation("Stand");
        }

        public void Load()
        {
            if (Model == null || !Enum.IsDefined(typeof(WhType), Model.Type) || string.IsNullOrEmpty(Model.Id))
                return;

            _Load(Model.Type, Model.Id);
        }

        public int[] GetFallbackRaceGender(WhGender gender, WhRace race, bool isTexture)
        {
            var fallbackData = WhGlobal.RaceFallbacks.GetOrDefault(race);
            if (fallbackData != null)
            {
                var offset = isTexture ? 4 : 0;
                return fallbackData.Skip((int)gender * 2 + offset).Take(2).ToArray();
            }

            return null;
        }

        public uint SelectBestModel(int modelid, WhGender gender, WhClass cls, WhRace race)
        {
            var models = Meta.ModelFiles[modelid];
            if (models.Length == 1)
                return models[0].FileDataId;

            var fallbackData = GetFallbackRaceGender(gender, race, false);
            if (fallbackData != null && fallbackData[0] != 0)
            {
                race = (WhRace)fallbackData[0];
                gender = (WhGender)fallbackData[1];
            }

            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.Race != 0 && model.Race == race && (model.Class != 0 && model.Class == cls) && (model.Gender == gender || model.Gender == WhGender.Undefined2))
                    return model.FileDataId;
            }

            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.Race != 0 && model.Race == race && (model.Gender == gender || model.Gender == WhGender.Undefined2))
                    return model.FileDataId;
            }

            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.Class != 0 && model.Class == cls && (model.Gender == gender || model.Gender == WhGender.Undefined2))
                    return model.FileDataId;
            }

            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                if (model.Gender == gender || model.Gender == WhGender.Undefined2)
                    return model.FileDataId;
            }

            throw new NotImplementedException("selectBestModel for " + modelid + ", gender " + gender + ", class " + cls + ", race " + race + " failed!");
        }

        public void LoadMeta(WhJsonMeta meta, WhType type, int componentIndex)
        {
            if (!Enum.IsDefined(typeof(WhType), type))
                type = Model.Type;

            if (Meta == null)
                Meta = meta;

            int model;

            if (type == WhType.CHARACTER)
            {
                model = meta.Model;
                if (Opts.Hd && meta.HDModel != 0)
                {
                    model = meta.HDModel;
                    IsHD = true;
                }

                if (Enum.IsDefined(typeof(WhClass), Opts.Cls))
                    Class = Opts.Cls;

                WhDefferedList.Add(() => LoadAndHandle_MetaCharacterCustomization(meta.Race, meta.Gender));

                if (Opts.SheathMain != 0)
                    SheathMain = Opts.SheathMain;
                if (Opts.SheathOff != 0)
                    SheathOff = Opts.SheathOff;

                if (IsHD && Meta.Creature != null && Meta.Creature.HDTexture != 0)
                    NpcTexture = new WhTexture(this, -1, Meta.Creature.HDTexture);
                else if (Meta.Creature != null && Meta.Creature.Texture != 0)
                    NpcTexture = new WhTexture(this, -1, Meta.Creature.Texture);

                Race = meta.Race;
                Gender = meta.Gender;

                _Load(WhType.PATH, model.ToString());

                if (Meta.Equipment != null)
                    LoadItems(Meta.GetEquipmentAsItemObjects());

                if (Opts.Items != null)
                    LoadItems(Opts.Items);

                if (Model.Type != WhType.CHARACTER && (int)Meta.Race > 0) {
                    SkinIndex = Meta.Creature.SkinColor;
                    HairIndex = Meta.Creature.HairStyle;
                    HairColorIndex = Meta.Creature.HairColor;
                    FaceIndex = Meta.Creature.FaceType;
                    FeaturesIndex = Meta.Creature.FacialHair;
                    FaceColorIndex = HairColorIndex;

                    HornsIndex = 0;
                    EyePatchIndex = 0;
                    TattoosIndex = 0;
                }
                else
                {
                    if (Opts.sk != 0)
                        SkinIndex = Opts.sk;
                    if (Opts.ha != 0)
                        HairIndex = Opts.ha;
                    if (Opts.hc != 0)
                        HairColorIndex = Opts.hc;
                    if (Opts.fa != 0)
                        FaceIndex = Opts.fa;
                    if (Opts.fh != 0)
                        FeaturesIndex = Opts.fh;
                    if (Opts.fc != 0)
                        FaceColorIndex = Opts.fc;
                    if (Opts.ho != 0)
                        HornsIndex = Opts.ho;
                    if (Opts.ep != 0)
                        EyePatchIndex = Opts.ep;
                    if (Opts.ta != 0)
                        TattoosIndex = Opts.ta;
                }
            }
            else if (type == WhType.HELM)
            {
                WhRace race = WhRace.HUMAN;
                WhGender gender = WhGender.MALE;
                WhClass cls = WhClass.WARRIOR;

                if (Parent != null)
                {
                    race = Parent.Race;
                    gender = Parent.Gender;
                    cls = Parent.Class;
                }

                if (meta.ComponentModels != null) {
                    var model1 = meta.ComponentModels["0"];
                    if (model1 != 0 && meta.ModelFiles[model1] != null) {
                        _Load(WhType.PATH, SelectBestModel(model1, gender, cls, race).ToString());
                    }
                }

                if (meta.Textures != null) {
                    foreach (var texture in meta.Textures)
                        TextureOverrides[texture.Key] = new WhTexture(this, texture.Key, texture.Value);
                }
            }
            else if (type == WhType.SHOULDER)
            {
                var model1 = meta.ComponentModels["0"];
                var model2 = meta.ComponentModels["1"];

                if (Model.Shoulder == 1 || !Model.Shoulder.HasValue && model1 != 0)
                {
                    if (model1 != 0) {
                        _Load(WhType.PATH, meta.ModelFiles[model1][0].FileDataId.ToString());
                    }

                    if (meta.Textures != null)
                    {
                        foreach (var texture in meta.Textures)
                            TextureOverrides[texture.Key] = new WhTexture(this, texture.Key, texture.Value);
                    }
                }
                else if (Model.Shoulder == 2 || !Model.Shoulder.HasValue && model2 != 0) {
                    if (model2 != 0) {
                        _Load(WhType.PATH, (meta.ModelFiles[model2].Length > 1 ? meta.ModelFiles[model2][1].FileDataId : meta.ModelFiles[model2][0].FileDataId).ToString());
                    }
                    if (meta.Textures2 != null)
                    {
                        foreach (var texture in meta.Textures2)
                            TextureOverrides[texture.Key] = new WhTexture(this, texture.Key, texture.Value);
                    }
                }
            }
            else if (type == WhType.ITEMVISUAL)
                _Load(WhType.PATH, meta.GetEquipmentAsStringIds()[ModelIndex]);
            else if (type == WhType.COLLECTION)
            {
                if (meta.ComponentModels.ContainsKey(componentIndex.ToString()))
                {
                    var race = WhRace.HUMAN;
                    var gender = WhGender.MALE;
                    var cls = WhClass.WARRIOR;

                    if (Parent != null)
                    {
                        race = Parent.Race;
                        gender = Parent.Gender;
                        cls = Parent.Class;
                    }

                    model = meta.ComponentModels[componentIndex.ToString()];

                    if (model != 0 && meta.ModelFiles[model] != null)
                        _Load(WhType.PATH, SelectBestModel(model, gender, cls, race).ToString());
                    if (meta.Textures != null) {
                        foreach (var textureInMetaTextures in meta.Textures)
                            TextureOverrides[textureInMetaTextures.Key] = new WhTexture(this, textureInMetaTextures.Key, textureInMetaTextures.Value);
                    }
                } else {
                    System.Diagnostics.Debug.WriteLine("Attempt to load collection without valid model");
                }
            }
            else
            {
                if (meta.Creature != null && meta.Creature.CreatureGeosetData != 0)
                    CreatureGeosetData = meta.Creature.CreatureGeosetData;

                if (meta.Textures != null)
                {
                    foreach (var textureInMetaTextures in meta.Textures)
                    {
                        if (textureInMetaTextures.Value != 0)
                            TextureOverrides[textureInMetaTextures.Key] = new WhTexture(this, textureInMetaTextures.Key, textureInMetaTextures.Value);
                    }
                }
                else if (meta.ComponentTextures != null && Parent != null)
                {
                    var g = Parent.Gender;
                    foreach (var componentTexture in meta.ComponentTextures)
                    {
                        var textures = meta.TextureFiles[componentTexture.Value];
                        for (var j = 0; j < textures.Length; j++)
                        {
                            var texture = textures[j];
                            if (texture.Gender == g || texture.Gender == WhGender.Undefined3)
                                TextureOverrides[(int)componentTexture.Key] = new WhTexture(this, (int)componentTexture.Key, texture.FileDataId);
                        }
                    }
                }

                if (Opts.Hd && meta.HDModel != 0)
                    _Load(WhType.PATH, meta.HDModel.ToString());
                else if (meta.Model != 0)
                    _Load(WhType.PATH, meta.Model.ToString());
                else if ((int)meta.Race > 0)
                {
                    Race = meta.Race;
                    Gender = meta.Gender;

                    var modelInfo = WhModelInfo.CreateForCharacter(meta.Race, meta.Gender);
                    _Load(modelInfo.Type, modelInfo.Id);
                }
            }
        }

        public void LoadMo3(byte[] buffer)
        {
            if (buffer == null)
            {
                System.Diagnostics.Debug.WriteLine("Bad buffer for DataView");
                return;
            }

            uint magic;
            uint version;

            uint ofsVertices;
            uint ofsIndices;
            uint ofsSequences;
            uint ofsAnimations;
            uint ofsAnimLookup;
            uint ofsBones;
            uint ofsBoneLookup;
            uint ofsKeyBoneLookup;
            uint ofsMeshes;
            uint ofsTexUnits;
            uint ofsTexUnitLookup;
            uint ofsRenderFlags;
            uint ofsMaterials;
            uint ofsMaterialLookup;
            uint ofsTextureAnims;
            uint ofsTexAnimLookup;
            uint ofsTexReplacements;
            uint ofsAttachments;
            uint ofsAttachmentLookup;
            uint ofsColors;
            uint ofsAlphas;
            uint ofsAlphaLookup;
            uint ofsParticleEmitters;
            uint ofsRibbonEmitters;
            uint uncompressedSize;

            var sourceStream = new MemoryStream(buffer);
            MemoryStream compressedDataStream;

            using (sourceStream)
            using (var r = new BinaryReader(sourceStream))
            {
                magic = r.ReadUInt32();
                if (magic != 604210112)
                {
                    System.Diagnostics.Debug.WriteLine("Bad magic value");
                    return;
                }

                version = r.ReadUInt32();
                if (version < 2000)
                {
                    System.Diagnostics.Debug.WriteLine("Bad version");
                    return;
                }

                ofsVertices = r.ReadUInt32();
                ofsIndices = r.ReadUInt32();
                ofsSequences = r.ReadUInt32();
                ofsAnimations = r.ReadUInt32();
                ofsAnimLookup = r.ReadUInt32();
                ofsBones = r.ReadUInt32();
                ofsBoneLookup = r.ReadUInt32();
                ofsKeyBoneLookup = r.ReadUInt32();
                ofsMeshes = r.ReadUInt32();
                ofsTexUnits = r.ReadUInt32();
                ofsTexUnitLookup = r.ReadUInt32();
                ofsRenderFlags = r.ReadUInt32();
                ofsMaterials = r.ReadUInt32();
                ofsMaterialLookup = r.ReadUInt32();
                ofsTextureAnims = r.ReadUInt32();
                ofsTexAnimLookup = r.ReadUInt32();
                ofsTexReplacements = r.ReadUInt32();
                ofsAttachments = r.ReadUInt32();
                ofsAttachmentLookup = r.ReadUInt32();
                ofsColors = r.ReadUInt32();
                ofsAlphas = r.ReadUInt32();
                ofsAlphaLookup = r.ReadUInt32();
                ofsParticleEmitters = r.ReadUInt32();
                ofsRibbonEmitters = r.ReadUInt32();
                uncompressedSize = r.ReadUInt32();

                /*
                self.program = {
                    name: "Wow.Generic",
                    config: {}
                };
                self.attributeState = new ZamModelViewer.Tools.AttributeState;
                */

                compressedDataStream = new MemoryStream((int)uncompressedSize);

                try
                {
                    using (var inflaterInputStream = new InflaterInputStream(sourceStream))
                        inflaterInputStream.CopyTo(compressedDataStream);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Decompression error: " + ex);
                    return;
                }
            }

            if (compressedDataStream.Length < uncompressedSize)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected data size, { compressedDataStream.Length }, { uncompressedSize }");
                return;
            }

            using (compressedDataStream)
            using (var r = new BinaryReader(compressedDataStream))
            {
                r.BaseStream.Seek(ofsVertices, SeekOrigin.Begin);
                var numVertices = r.ReadInt32();
                if (numVertices > 0)
                {
                    Vertices = new WhVertex[numVertices];

                    for (int i = 0; i < numVertices; i++)
                        Vertices[i] = new WhVertex(r);
                }

                r.BaseStream.Seek(ofsIndices, SeekOrigin.Begin);
                var numIndices = r.ReadInt32();
                if (numIndices > 0)
                {
                    Indices = new ushort[numIndices];

                    for (int i = 0; i < numIndices; i++)
                        Indices[i] = r.ReadUInt16();
                }

                r.BaseStream.Seek(ofsAnimations, SeekOrigin.Begin);
                var numAnims = r.ReadInt32();
                if (numAnims > 0)
                {
                    Animations = new WhAnimation[numAnims];

                    for (int i = 0; i < numAnims; i++)
                        Animations[i] = new WhAnimation(r);
                }

                r.BaseStream.Seek(ofsAnimLookup, SeekOrigin.Begin);
                var numAnimLookup = r.ReadInt32();
                if (numAnimLookup > 0)
                {
                    AnimLookup = new short[numAnimLookup];

                    for (int i = 0; i < numAnimLookup; i++)
                        AnimLookup[i] = r.ReadInt16();
                }

                r.BaseStream.Seek(ofsBones, SeekOrigin.Begin);
                var numBones = r.ReadInt32();
                if (numBones > 0)
                {
                    Bones = new WhBone[numBones];

                    for (int i = 0; i < numBones; i++)
                        Bones[i] = new WhBone(this, i, r);
                }

                r.BaseStream.Seek(ofsBoneLookup, SeekOrigin.Begin);
                var numBoneLookup = r.ReadInt32();
                if (numBoneLookup > 0)
                {
                    BoneLookup = new short[numBoneLookup];

                    for (int i = 0; i < numBoneLookup; i++)
                        BoneLookup[i] = r.ReadInt16();
                }

                r.BaseStream.Seek(ofsKeyBoneLookup, SeekOrigin.Begin);
                var numKeyBoneLookup = r.ReadInt32();
                if (numKeyBoneLookup > 0)
                {
                    KeyBoneLookup = new short[numKeyBoneLookup];

                    for (int i = 0; i < numKeyBoneLookup; i++)
                        KeyBoneLookup[i] = r.ReadInt16();
                }

                r.BaseStream.Seek(ofsMeshes, SeekOrigin.Begin);
                var numMeshes = r.ReadInt32();
                if (numMeshes > 0)
                {
                    Meshes = new WhMesh[numMeshes];

                    for (int i = 0; i < numMeshes; i++)
                        Meshes[i] = new WhMesh(r);
                }

                r.BaseStream.Seek(ofsTexUnits, SeekOrigin.Begin);
                var numTexUnits = r.ReadInt32();
                if (numTexUnits > 0)
                {
                    TexUnits = new WhTexUnit[numTexUnits];

                    for (int i = 0; i < numTexUnits; i++)
                        TexUnits[i] = new WhTexUnit(r);
                }

                r.BaseStream.Seek(ofsTexUnitLookup, SeekOrigin.Begin);
                var numTexUnitLookup = r.ReadInt32();
                if (numTexUnitLookup > 0)
                {
                    TexUnitLookup = new short[numTexUnitLookup];

                    for (int i = 0; i < numTexUnitLookup; i++)
                        TexUnitLookup[i] = r.ReadInt16();
                }

                r.BaseStream.Seek(ofsRenderFlags, SeekOrigin.Begin);
                var numRenderFlags = r.ReadInt32();
                if (numRenderFlags > 0)
                {
                    RenderFlags = new WhRenderFlag[numRenderFlags];

                    for (int i = 0; i < numRenderFlags; i++)
                        RenderFlags[i] = new WhRenderFlag(r);
                }

                r.BaseStream.Seek(ofsMaterials, SeekOrigin.Begin);
                var numMaterials = r.ReadInt32();
                if (numMaterials > 0)
                {
                    Materials = new WhMaterial[numMaterials];

                    for (int i = 0; i < numMaterials; i++)
                        Materials[i] = new WhMaterial(this, i, r);
                }

                r.BaseStream.Seek(ofsMaterialLookup, SeekOrigin.Begin);
                var numMaterialLookup = r.ReadInt32();
                if (numMaterialLookup > 0)
                {
                    MaterialLookup = new short[numMaterialLookup];

                    for (int i = 0; i < numMaterialLookup; i++)
                        MaterialLookup[i] = r.ReadInt16();
                }

                //r.position = ofsTextureAnims;
                //var numTextureAnims = r.getInt32();
                //if (numTextureAnims > 0) {
                //    self.textureAnims = new Array(numTextureAnims);
                //    for (i = 0; i < numTextureAnims; ++i) {
                //        self.textureAnims[i] = new Wow.TextureAnimation(r)
                //    }
                //}

                r.BaseStream.Seek(ofsTexAnimLookup, SeekOrigin.Begin);
                var numTexAnimLookup = r.ReadInt32();
                if (numTexAnimLookup > 0)
                {
                    TextureAnimLookup = new short[numTexAnimLookup];

                    for (int i = 0; i < numTexAnimLookup; i++)
                        TextureAnimLookup[i] = r.ReadInt16();
                }

                r.BaseStream.Seek(ofsTexReplacements, SeekOrigin.Begin);
                var numTexReplacements = r.ReadInt32();
                if (numTexReplacements > 0)
                {
                    TextureReplacements = new short[numTexReplacements];

                    for (int i = 0; i < numTexReplacements; i++)
                        TextureReplacements[i] = r.ReadInt16();
                }

                r.BaseStream.Seek(ofsAttachments, SeekOrigin.Begin);
                var numAttachments = r.ReadInt32();
                if (numAttachments > 0)
                {
                    Attachments = new WhAttachment[numAttachments];

                    for (int i = 0; i < numAttachments; i++)
                        Attachments[i] = new WhAttachment(r);
                }

                r.BaseStream.Seek(ofsAttachmentLookup, SeekOrigin.Begin);
                var numAttachmentLookup = r.ReadInt32();
                if (numAttachmentLookup > 0)
                {
                    AttachmentLookup = new short[numAttachmentLookup];

                    for (int i = 0; i < numAttachmentLookup; i++)
                        AttachmentLookup[i] = r.ReadInt16();
                }

                //r.position = ofsColors;
                //var numColors = r.getInt32();
                //if (numColors > 0) {
                //    self.colors = new Array(numColors);
                //    for (i = 0; i < numColors; ++i) {
                //        self.colors[i] = new Wow.Color(r)
                //    }
                //}

                //r.position = ofsAlphas;
                //var numAlphas = r.getInt32();
                //if (numAlphas > 0) {
                //    self.alphas = new Array(numAlphas);
                //    for (i = 0; i < numAlphas; ++i) {
                //        self.alphas[i] = new Wow.Alpha(r)
                //    }
                //}

                r.BaseStream.Seek(ofsAlphaLookup, SeekOrigin.Begin);
                var numAlphaLookup = r.ReadInt32();
                if (numAlphaLookup > 0)
                {
                    AlphaLookup = new short[numAlphaLookup];

                    for (int i = 0; i < numAlphaLookup; i++)
                        AlphaLookup[i] = r.ReadInt16();
                }

                //r.position = ofsParticleEmitters;
                //var numParticleEmitters = r.getInt32();
                //if (numParticleEmitters > 0) {
                //    self.particleEmitters = new Array(numParticleEmitters);
                //    for (i = 0; i < numParticleEmitters; ++i) {
                //        self.particleEmitters[i] = new Wow.ParticleEmitter(self,r)
                //    }
                //}

                //r.position = ofsRibbonEmitters;
                //var numRibbonEmitters = r.getInt32();
                //if (numRibbonEmitters > 0) {
                //    self.ribbonEmitters = new Array(numRibbonEmitters);
                //    for (i = 0; i < numRibbonEmitters; ++i) {
                //        self.ribbonEmitters[i] = new Wow.RibbonEmitter(self,r)
                //    }
                //}
            }

            OnLoaded();
        }

        public List<short> GetSlotAttachments(WhSlot slot, WhItem item)
        {
            var slotAttachments = new List<short>();

            if (Attachments != null && AttachmentLookup != null)
            {
                var slotMap = new Dictionary<WhSlot, Func<WhItem, int[]>>()
                {
                    { (WhSlot)1, itm => new int[] { 11 } },
                    { (WhSlot)3, itm => new int[] { 6, 5 } },
                    { (WhSlot)22,
                        itm =>
                        {
                            if (itm != null && itm.Slot == WhSlot.SHIELD)
                                return new int[] { 0 };

                            return new int[] { 2 };
                        }
                    },
                    { (WhSlot)21, itm => new int[] { 1 } },
                    { (WhSlot)17, itm => new int[] { 1 } },
                    { (WhSlot)15, itm => new int[] { 2 } },
                    { (WhSlot)25, itm => new int[] { 1 } },
                    { (WhSlot)13, itm => new int[] { 1 } },
                    { (WhSlot)14, itm => new int[] { 0 } },
                    { (WhSlot)23, itm => new int[] { 2 } },
                    { (WhSlot)6, itm => new int[] { 53 } },
                    { (WhSlot)26, itm => new int[] { 1 } },
                    { (WhSlot)27, itm => new int[] { 55 } }
                };

                var sheathStandardOverrides = new Dictionary<WhSlot, int>()
                {
                    { (WhSlot)21, 26},
                    { (WhSlot)22, 27},
                    { (WhSlot)15, 28},
                    { (WhSlot)17, 26},
                    { (WhSlot)25, 32},
                    { (WhSlot)13, 32},
                    { (WhSlot)23, 33},
                    { (WhSlot)14, 28},
                    { (WhSlot)26, 26 }
                };

                var sheathWeaponOverrides = new Dictionary<int, Dictionary<WhSlot, int>>()
                {
                    { 0, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26 },
                            { (WhSlot)22, 27 }
                        }
                    },
                    { 1, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26 },
                            { (WhSlot)22, 27 }
                        }
                    },
                    { 2, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 30 },
                            { (WhSlot)22, 31 }
                        }
                    },
                    { 3, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 33 },
                            { (WhSlot)22, 32 }
                        }
                    },
                    { 4, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26 },
                            { (WhSlot)22, 27 },
                            { (WhSlot)15, 28 }
                        }
                    },
                    { 5, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26}
                        }
                    },
                    { 6, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26 },
                            { (WhSlot)22, 27 }
                        }
                    },
                    { 7, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26 },
                            { (WhSlot)22, 27 }
                        }
                    },
                    { 8, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 26 },
                            { (WhSlot)22, 27 }
                        }
                    },
                    { 9, new Dictionary<WhSlot, int>()
                        {
                            { (WhSlot)21, 33 },
                            { (WhSlot)22, 28 }
                        }
                    },
                };

                if (slotMap.ContainsKey(slot))
                {
                    var data = slotMap[slot](item);
                    for (int i = 0; i < data.Length; i++)
                    {
                        var att = data[i];

                        if ((SheathMain >= 0 || SheathOff >= 0 || Mount != null) && sheathStandardOverrides.ContainsKey(slot))
                            att = sheathStandardOverrides[slot];

                        if (SheathMain >= 0 && slot == (WhSlot)21 && sheathWeaponOverrides[SheathMain].ContainsKey(slot))
                            att = sheathWeaponOverrides[SheathMain][slot];

                        if (SheathOff >= 0 && slot == (WhSlot)22 && sheathWeaponOverrides[SheathOff].ContainsKey(slot))
                            att = sheathWeaponOverrides[SheathOff][slot];

                        if (att >= AttachmentLookup.Length || AttachmentLookup[att] == -1)
                            continue;

                        slotAttachments.Add(AttachmentLookup[att]);
                    }
                }
            }

            return slotAttachments;
        }

        public void OnLoaded()
        {
            if (TexUnits != null) {
                for (int i = 0; i < TexUnits.Length; ++i)
                    TexUnits[i].Setup(this);

                SortedTexUnits = TexUnits.ToList();
                SortedTexUnits.Sort((a, b) => {
                    if (a.MeshId == b.MeshId)
                        return a.PriorityPlane - b.PriorityPlane;
                    else
                        return a.MeshId - b.MeshId;
                });
            }

            if (Mount != null)
            {
                if (WhGlobal.StandingMounts.GetOrDefault(Mount.Model.Id))
                    SetAnimation("StealthStand");
                else
                    SetAnimation("Mount");
            }
            else
                SetAnimation("Stand");

            //self.updateBuffers(true);
            //self.updateBounds();

            Setup();

            Loaded = true;

            //if (self.isMount && self.parent.loaded) {
            //    self.parent.updateBounds()
            //}

            if (Parent != null && Parent.Loaded)
                Parent.UpdateMeshes();
        }

        public int GetResolutionVariationType(WhCharVariationType variationType)
        {
            return WhGlobal.CharVariationMap[variationType][IsHD ? 1 : 0];
        }

        public void Setup()
        {
            if (Model.Type != WhType.CHARACTER && Model.Type != WhType.NPC && Model.Type != WhType.HUMANOIDNPC || (int)Race < 1)
            {
                ApplyMonsterGeosets();
                return;
            }

            WhJsonCustomFeature skinFeature = null;
            WhJsonCustomFeature faceFeature = null;
            WhJsonCustomFeature facialHairFeature = null;
            WhJsonCustomFeature hairFeature = null;
            WhJsonCustomFeature underwearFeature = null;
            WhJsonCustomFeature tattooFeature = null;

            if (CustomFeatures != null)
            {
                skinFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Skin), 0, SkinIndex);
                faceFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Face), FaceIndex, SkinIndex);
                facialHairFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.FacialHair), FeaturesIndex, FeaturesColorIndex);
                hairFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Hair), HairIndex, HairColorIndex);
                underwearFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Underwear), 0, SkinIndex);

                //var tattooData = Wow.TattooDataForRace[self.race];
                //if (tattooData)
                //{
                //    var tattooVariationType = tattooData[self.gender];
                //    if (tattooData[3] != 0 && self.class != tattooData[3]) {
                //        tattooVariationType = null
                //    }
                //    if (tattooVariationType) {
                //        tattooFeature = self.customFeatures.getFeature(self.getResolutionVariationType(tattooVariationType), self.TattoosIndex, self.skinIndex)
                //    }
                //}
            }

            if (HairGeosets != null)
            {
                CurrentHairGeoset = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Hair, HairIndex);
            }

            UpdateMeshes();

            if (NpcTexture == null) {
                NeedsCompositing = true;

                if (skinFeature != null && skinFeature.textures[0] != 0 && !SpecialTextures.ContainsKey(1))
                {
                    SpecialTextures[1] = new WhTexture(this, 1, skinFeature.textures[0]);
                }

                if (underwearFeature != null)
                {
                    if (underwearFeature.textures[0] != 0 && !BakedTextures[WhRegion.LegUpper].ContainsKey(1))
                        BakedTextures[WhRegion.LegUpper][1] = new WhTexture(this, (int)WhRegion.LegUpper, underwearFeature.textures[0]);
                    if (underwearFeature.textures[1] != 0 && !BakedTextures[WhRegion.TorsoUpper].ContainsKey(1))
                        BakedTextures[WhRegion.TorsoUpper][1] = new WhTexture(this, (int)WhRegion.TorsoUpper, underwearFeature.textures[1]);
                }

                //if (tattooFeature) {
                //    var tattooRegion = self.getTattooRegion();
                //    if (tattooRegion && tattooFeature.textures[0] != 0 && !self.bakedTextures[tattooRegion][1]) {
                //        self.bakedTextures[tattooRegion][1] = new self.Texture(self,tattooRegion,tattooFeature.textures[0])
                //    }
                //}

                if (faceFeature != null)
                {
                    if (faceFeature.textures[0] != 0 && !BakedTextures[WhRegion.FaceLower].ContainsKey(1))
                        BakedTextures[WhRegion.FaceLower][1] = new WhTexture(this, (int)WhRegion.FaceLower, faceFeature.textures[0]);
                    if (faceFeature.textures[1] != 0 && !BakedTextures[WhRegion.FaceUpper].ContainsKey(1))
                        BakedTextures[WhRegion.FaceUpper][1] = new WhTexture(this, (int)WhRegion.FaceUpper, faceFeature.textures[1]);
                }

                if (facialHairFeature != null)
                {
                    if (facialHairFeature.textures[0] != 0 && !BakedTextures[WhRegion.FaceLower].ContainsKey(2))
                        BakedTextures[WhRegion.FaceLower][2] = new WhTexture(this, (int)WhRegion.FaceLower, facialHairFeature.textures[0]);
                    if (facialHairFeature.textures[1] != 0 && !BakedTextures[WhRegion.FaceUpper].ContainsKey(2))
                        BakedTextures[WhRegion.FaceUpper][2] = new WhTexture(this, (int)WhRegion.FaceUpper, facialHairFeature.textures[1]);
                }

                if (CurrentHairGeoset != null)
                {
                    if (CurrentHairGeoset.showscalp == 1)
                        hairFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Hair), 1, HairColorIndex);
                    else if (hairFeature != null && CurrentHairGeoset.showscalp == 0)
                    {
                        if (hairFeature.textures[1] != 0 && !BakedTextures[WhRegion.FaceLower].ContainsKey(3))
                            BakedTextures[WhRegion.FaceLower][3] = new WhTexture(this, (int)WhRegion.FaceLower, hairFeature.textures[1]);
                        if (hairFeature.textures[2] != 0 && !BakedTextures[WhRegion.FaceUpper].ContainsKey(3))
                            BakedTextures[WhRegion.FaceUpper][3] = new WhTexture(this, (int)WhRegion.FaceUpper, hairFeature.textures[2]);
                    }
                }
            }

            if (skinFeature != null && skinFeature.textures[1] != 0 && !SpecialTextures.ContainsKey(8))
                SpecialTextures[8] = new WhTexture(this, 8, skinFeature.textures[1]);

            if (hairFeature != null && hairFeature.textures[0] != 0 && !SpecialTextures.ContainsKey(6))
                SpecialTextures[6] = new WhTexture(this, 6, hairFeature.textures[0]);
        }

        // этот метод я добавил сам. то что тут делается в оригинале происходит на отрисовке, но я делаю это тут один раз после создания модели
        // По сути это метод ZamModelViewer.Wow.Model.prototype.draw = function(flipWinding)
        public void EmulateDraw(bool flipWinding)
        {
            // Код из оригинального draw() частично тут, частично в WowObjectBuilder.BuildFromCharacterWhModel
            // тут происходит обновление состояния (апдейты всякие, которые они засунули в draw), в билдере же происходит уже экспорт на основе этого состояния

            if (!Loaded)
                return;

            Update();

            if (NeedsCompositing)
                CompositeTextures();

            if (HornsModel != null)
            {
                var hornsModel = HornsModel;
                var boneMap = new Dictionary<uint, int>();
                for (var i = 0; i < Bones.Length; i++)
                {
                    boneMap[Bones[i].Id] = i;
                }

                var ibones = hornsModel.Bones;
                if (ibones != null)
                {
                    for (var i = 0; i < ibones.Length; i++)
                    {
                        var bone = ibones[i];
                        if(!boneMap.TryGetValue(bone.Id, out var pi))
                            continue;

                        //var dst = ibones[i].matrix;
                        //var src = self.bones[pi].matrix;

                        ibones[i].SkipUpdate = true;

                        //mat4.copy(dst, src)
                    }

                    //mat4.identity(self.tmpMat);
                    //hornsModel.setMatrix(self.matrix, self.tmpMat);
                    hornsModel.Update();
                    //hornsModel.draw(false)
                }
            }

            foreach (var item in Items.Values)
            {
                if (item == null || item.Models == null)
                    continue;

                for (int j = 0; j < item.Models.Count; j++)
                {
                    if (item.Models[j] != null && item.Models[j].Model != null && item.Models[j].Bone > -1 && item.Models[j].Bone < Bones.Length)
                    {
                        var winding = false;
                        var reversed = item.Models[j].Model.Model.Type == WhType.ITEM && WhGlobal.ReversedItems.GetOrDefault(item.Models[j].Model.Model.Id);
                        var upsideDown = WhGlobal.ReversedModels.GetOrDefault(item.Models[j].Model.Model.Id);

                        //mat4.identity(self.tmpMat);

                        if (upsideDown)
                        {
                            //vec3.set(self.tmpVec, 1, 1, -1);
                            //mat4.scale(self.tmpMat, self.tmpMat, self.tmpVec);

                            winding = true;
                        }
                        else if (reversed && item.Slot != WhSlot.LEFTHAND || !reversed && item.Slot == WhSlot.LEFTHAND)
                        {
                            //vec3.set(self.tmpVec, 1, -1, 1);
                            //mat4.scale(self.tmpMat, self.tmpMat, self.tmpVec);

                            winding = true;
                        }

                        if (item.Slot == (WhSlot)27)
                        {
                            //var scale = item.models[j].model.meta.Scale;
                            //vec3.set(self.tmpVec, scale, scale, scale);
                            //mat4.scale(self.tmpMat, self.tmpMat, self.tmpVec)
                        }

                        //item.models[j].model.setMatrix(self.matrix, self.bones[item.models[j].bone].matrix, item.models[j].attachment.position, self.tmpMat);

                        item.Models[j].Model.Update();
                        item.Models[j].Model.EmulateDraw(winding);

                        if (item.Visual != null && item.Visual.Models != null && item.Models[j].Model.Loaded)
                        {
                            for (var k = 0; k < item.Visual.Models.Length; k++)
                            {
                                var visual = item.Visual.Models[k];
                                if (visual != null)
                                {
                                    //mat4.identity(self.tmpMat);
                                    //mat4.rotateY(self.tmpMat, self.tmpMat, Math.PI / 2);
                                    //var transPos = visual.model.particleEmitters[0].position;
                                    //vec3.set(self.tmpVec, -transPos[0], -transPos[1], -transPos[2]);
                                    //mat4.translate(self.tmpMat, self.tmpMat, self.tmpVec);
                                    //visual.model.setMatrix(self.matrix, self.bones[item.models[j].bone].matrix, visual.attachment.position, self.tmpMat);

                                    item.Visual.Models[k].Model.Update();
                                    item.Visual.Models[k].Model.EmulateDraw(winding);
                                }
                            }
                        }
                    }
                    else if (item.Models[j] != null && item.Models[j].Model != null && item.Models[j].Bone == -1)
                    {
                        var boneMap = new Dictionary<uint, int>();

                        for (var i = 0; i < Bones.Length; i++)
                        {
                            boneMap[Bones[i].Id] = i;
                        }

                        var ibones = item.Models[j].Model.Bones;

                        if (ibones != null)
                        {
                            for (var i = 0; i < ibones.Length; i++)
                            {
                                var bone = ibones[i];
                                if (!boneMap.TryGetValue(bone.Id, out var pi))
                                    continue;

                                //var dst = ibones[i].matrix;
                                //var src = self.bones[pi].matrix;

                                ibones[i].SkipUpdate = true;

                                //mat4.copy(dst, src);
                            }

                            //mat4.identity(self.tmpMat);
                            //item.models[j].model.setMatrix(self.matrix, self.tmpMat);

                            item.Models[j].Model.Update();
                            item.Models[j].Model.EmulateDraw(false);
                        }
                    }
                }
            }
        }

        public void SetGeometryVisibleCustom(WhJsonHairGeoset data)
        {
            if (data != null)
                Geosets[data.geosetType] = (ushort)(data.geosetType * 100 + data.geosetID);
        }

        public void SetGeometryVisible(ushort minId, ushort maxId, bool visible)
        {
            if (TexUnits == null)
                return;

            foreach (var texUnit in TexUnits)
            {
                if (texUnit.MeshId >= minId && texUnit.MeshId <= maxId)
                    texUnit.Show = visible;
            }
        }

        public void ApplyMonsterGeosets()
        {
            SetGeometryVisible(0, 0, true);

            if (CreatureGeosetData != 0)
            {
                SetGeometryVisible(1, 899, false);
                for (int i = 0; i < 8; ++i)
                {
                    var geoset = CreatureGeosetData >> i * 4 & 15;
                    if (geoset != 0)
                    {
                        ushort meshId = (ushort)(100 * (i + 1));
                        SetGeometryVisible(meshId, (ushort)(meshId + 99), false);
                        SetGeometryVisible((ushort)(meshId + geoset), (ushort)(meshId + geoset), true);
                    }
                }
            }
        }

        public void UpdateMeshes()
        {
            if (TexUnits == null || TexUnits.Length == 0)
                return;

            Geosets[7] = 702;

            if (HairGeosets != null)
            {
                if (Race == WhRace.ZANDALARITROLL)
                {
                    var earring = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Custom3, EyePatchIndex);
                    SetGeometryVisibleCustom(earring);
                    var tusk = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Custom2, HornsIndex);
                    SetGeometryVisibleCustom(tusk);
                }
                else if (Race == WhRace.VULPERA)
                {
                    var nose = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Custom2, HornsIndex);
                    SetGeometryVisibleCustom(nose);
                }
                else if (Race == WhRace.DARKIRONDWARF)
                {
                    var earring = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Custom1, HornsIndex);
                    SetGeometryVisibleCustom(earring);
                }
                else if (Race == WhRace.LIGHTFORGEDDRAENEI)
                {
                    var rune = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Custom3, EyePatchIndex);
                    SetGeometryVisibleCustom(rune);
                }

                var skin = HairGeosets.GetHairGeosetByColorIndex(WhCharVariationType.Skin, SkinIndex);
                SetGeometryVisibleCustom(skin);

                var face = HairGeosets.GetHairGeosetByVariationId(WhCharVariationType.Face, FaceIndex);
                SetGeometryVisibleCustom(face);
            }

            SetGeometryVisibleCustom(CurrentHairGeoset);

            if (Geosets[0] == 0)
                Geosets[0] = 1;

            if (FacialHairStyles != null)
            {
                var facialStyle = FacialHairStyles.GetStyle(FeaturesIndex);
                if (facialStyle != null)
                {
                    if (facialStyle.geoset[0] != -1)
                        Geosets[1] = (ushort)(100 + facialStyle.geoset[0]);
                    if (facialStyle.geoset[1] != -1)
                        Geosets[3] = (ushort)(300 + facialStyle.geoset[1]);
                    if (facialStyle.geoset[2] != -1)
                        Geosets[2] = (ushort)(200 + facialStyle.geoset[2]);
                    if (facialStyle.geoset[3] != -1)
                        Geosets[16] = (ushort)(1600 + facialStyle.geoset[3]);
                    if (facialStyle.geoset[4] != -1)
                        Geosets[17] = (ushort)(1700 + facialStyle.geoset[4]);
                }
            }

            bool eyeGlowFlag = false;
            if (Class == WhClass.DEATHKNIGHT)
                eyeGlowFlag = true;
            else
            {
                var faceSection = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Face), FaceIndex, SkinIndex);
                if (faceSection != null)
                    eyeGlowFlag = (faceSection.flags & 4) != 0;
                else
                    eyeGlowFlag = false;
            }

            SetGeometryVisible(0, 3000, false);

            SetGeometryVisible(0, 0, true);

            for (int i = 0; i < Geosets.Length; i++)
            {
                if (eyeGlowFlag && i == 17)
                    SetGeometryVisible(1703, 1703, true);
                else
                    SetGeometryVisible(Geosets[i], Geosets[i], true);
            }

            bool tailFlag = false;
            var skinSection = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Skin), 0, SkinIndex);
            if (skinSection != null)
                tailFlag = (skinSection.flags & 256) != 0;

            int tailGeoset = 1;
            if (!tailFlag)
                tailGeoset = HairGeosets.GetHairGeosetConditional(WhCharVariationType.Skin, SkinIndex, 19);

            if (tailGeoset <= 0)
                tailGeoset = 1;

            SetGeometryVisible((ushort)(1900 + tailGeoset), (ushort)(1900 + tailGeoset), true);

            foreach (var item in Items.Values)
            {
                if (item.Models != null && item.Models.Count > 0)
                {
                    if (item.Slot == WhSlot.BOOTS) {
                        item.Models[0].Model.SetGeometryVisible(0, 3000, false);
                        item.Models[0].Model.SetGeometryVisible(501, 501, true);
                        item.Models[0].Model.SetGeometryVisible(2001, 2001, true);
                    }
                    else if (item.Slot == WhSlot.HANDS) {
                        item.Models[0].Model.SetGeometryVisible(0, 3000, false);
                        item.Models[0].Model.SetGeometryVisible(401, 401, true);
                    }
                    else if (item.Slot == WhSlot.PANTS) {
                        item.Models[0].Model.SetGeometryVisible(0, 3000, false);
                        item.Models[0].Model.SetGeometryVisible(901, 901, true);
                    }
                    else if (item.Slot == WhSlot.CHEST || item.Slot == WhSlot.ROBE) {
                        item.Models[0].Model.SetGeometryVisible(0, 3000, false);
                        item.Models[0].Model.SetGeometryVisible(1001, 1001, true);
                        item.Models[0].Model.SetGeometryVisible(1101, 1101, true);
                        item.Models[0].Model.SetGeometryVisible(1301, 1301, true);
                    }
                    else if (item.Slot == WhSlot.HEAD) {
                        if (item.Models.Count > 1) {
                            item.Models[1].Model.SetGeometryVisible(0, 3000, false);
                            item.Models[1].Model.SetGeometryVisible(2701, 2701, true);
                        }
                    }
                    else if (item.Slot == WhSlot.BELT) {
                        for (int i = 0; i < item.Models.Count; i++) {
                            if (item.Models[i].IsCollection) {
                                item.Models[i].Model.SetGeometryVisible(0, 3000, false);
                                item.Models[i].Model.SetGeometryVisible(1801, 1801, true);
                            }
                        }
                    }
                }
            }

            var helm = Items.GetOrDefault(WhSlot.HEAD);
            if (helm != null)
            {
                var race = Race;
                var gender = Gender;

                var hideGeoset = gender == WhGender.MALE ? helm.HideGeosetMale : helm.HideGeosetFemale;
                if (hideGeoset != null)
                {
                    var groups = new int[] { 0, 1, 2, 3, 7, 16, 17, 24, 25 };
                    for (int i = 0; i < hideGeoset.Length; i++)
                    {
                        if ((1 << (int)race & hideGeoset[i]) != 0)
                        {
                            var meshBase = groups[i] * 100;
                            var meshID = meshBase + GeosetDefaults[groups[i]];

                            SetGeometryVisible((ushort)(meshBase + GeosetDefaults[i]), (ushort)(meshBase + 99), false);
                            SetGeometryVisible((ushort)meshID, (ushort)meshID, true);
                        }
                    }
                }
            }

            var shirt = Items.GetOrDefault(WhSlot.SHIRT);
            var chest = Items.GetOrDefault(WhSlot.CHEST);
            var belt = Items.GetOrDefault(WhSlot.BELT);
            var pants = Items.GetOrDefault(WhSlot.PANTS);
            var boots = Items.GetOrDefault(WhSlot.BOOTS);
            var wrist = Items.GetOrDefault(WhSlot.BRACERS);
            var gloves = Items.GetOrDefault(WhSlot.HANDS);
            var tabard = Items.GetOrDefault(WhSlot.TABARD);
            var cloak = Items.GetOrDefault(WhSlot.CAPE);

            int flags = 0;
            if (tabard != null)
                flags |= 16;

            if (gloves != null && gloves.GeosetGroup != null && gloves.GeosetGroup[0] != 0)
            {
                var glovesGeoset = 401 + gloves.GeosetGroup[0];
                SetGeometryVisible(401, 499, false);
                SetGeometryVisible((ushort)glovesGeoset, (ushort)glovesGeoset, true);
                gloves.SortValue += 2;
            }
            else
            {
                if (chest != null && chest.GeosetGroup != null && chest.GeosetGroup[0] != 0)
                {
                    var chesetGeoset = 801 + chest.GeosetGroup[0];
                    SetGeometryVisible((ushort)chesetGeoset, (ushort)chesetGeoset, true);
                }
            }

            var canShowShirt = !(chest != null || belt != null || wrist != null);
            if (canShowShirt && shirt != null && shirt.GeosetGroup != null && shirt.GeosetGroup[0] != 0)
            {
                var shirtGeoset = 801 + shirt.GeosetGroup[0];
                SetGeometryVisible((ushort)shirtGeoset, (ushort)shirtGeoset, true);
            }

            if (tabard != null)
            {
                if ((tabard.Flags & 1048576) == 0)
                {
                    SetGeometryVisible(2200, 2299, false);
                    SetGeometryVisible(2202, 2202, true);
                }
            }
            else
            {
                if (chest != null && chest.GeosetGroup != null && chest.GeosetGroup[3] != 0)
                {
                    var chestGeoset = 2201 + chest.GeosetGroup[3];
                    SetGeometryVisible(2200, 2299, false);
                    SetGeometryVisible((ushort)chestGeoset, (ushort)chestGeoset, true);
                }
            }

            bool largeBelt = false;
            if (belt != null && belt.GeosetGroup != null && belt.GeosetGroup[0] != 0)
                largeBelt = (belt.Flags & 512) != 0;

            bool robePants = false;
            bool robeChest = false;

            if (chest != null && chest.GeosetGroup != null && chest.GeosetGroup[2] != 0)
            {
                robeChest = true;
                SetGeometryVisible(501, 599, false);
                SetGeometryVisible(902, 999, false);
                SetGeometryVisible(1100, 1199, false);
                SetGeometryVisible(1300, 1399, false);
                var chestGeoset = 1301 + chest.GeosetGroup[2];
                SetGeometryVisible((ushort)chestGeoset, (ushort)chestGeoset, true);
            }
            else if (pants != null && pants.GeosetGroup != null && pants.GeosetGroup[2] != 0)
            {
                robePants = true;
                SetGeometryVisible(501, 599, false);
                SetGeometryVisible(902, 999, false);
                SetGeometryVisible(1100, 1199, false);
                SetGeometryVisible(1300, 1399, false);
                var pantsGeoset = 1301 + pants.GeosetGroup[2];
                SetGeometryVisible((ushort)pantsGeoset, (ushort)pantsGeoset, true);
            }
            else
            {
                if (boots != null && boots.GeosetGroup != null && boots.GeosetGroup[0] != 0)
                {
                    SetGeometryVisible(501, 599, false);
                    SetGeometryVisible(901, 901, true);
                    var bootsGeoset = 501 + boots.GeosetGroup[0];
                    SetGeometryVisible((ushort)bootsGeoset, (ushort)bootsGeoset, true);
                }
                else
                {
                    int pantsGeoset;
                    if (pants != null && pants.GeosetGroup != null && pants.GeosetGroup[1] != 0)
                        pantsGeoset = 901 + pants.GeosetGroup[1];
                    else
                        pantsGeoset = 901;

                    SetGeometryVisible((ushort)pantsGeoset, (ushort)pantsGeoset, true);
                }
            }

            {
                int bootsGeoset;
                if (boots != null && boots.GeosetGroup != null && boots.GeosetGroup[1] != 0)
                    bootsGeoset = 2000 + boots.GeosetGroup[1];
                else if (boots != null && (boots.Flags & 1048576) == 0)
                    bootsGeoset = 2002;
                else
                    bootsGeoset = 2001;

                SetGeometryVisible((ushort)bootsGeoset, (ushort)bootsGeoset, true);
            }

            bool showTabard = false;

            var hasDress = robeChest | robePants;
            if (!hasDress && tabard != null && tabard.GeosetGroup != null && tabard.GeosetGroup[0] != 0)
            {
                showTabard = false;

                int tabardGeoset;
                if (largeBelt)
                {
                    showTabard = true;
                    tabardGeoset = 1203;
                }
                else
                {
                    showTabard = true;
                    tabardGeoset = 1201 + tabard.GeosetGroup[0];
                }

                SetGeometryVisible((ushort)tabardGeoset, (ushort)tabardGeoset, true);
            }
            else if ((flags & 16) == 0)
            {
            }
            else
            {
                SetGeometryVisible(1201, 1201, true);

                if (!hasDress)
                {
                    SetGeometryVisible(1202, 1202, true);
                    showTabard = true;
                }
            }

            if (!(showTabard || robeChest))
            {
                if (chest != null && chest.GeosetGroup != null && chest.GeosetGroup[1] != 0)
                {
                    var chestGeoset = 1001 + chest.GeosetGroup[1];
                    SetGeometryVisible((ushort)chestGeoset, (ushort)chestGeoset, true);
                }
                else if (shirt != null && shirt.GeosetGroup != null && shirt.GeosetGroup[1] != 0)
                {
                    var shirtGeoset = 1001 + shirt.GeosetGroup[1];
                    SetGeometryVisible((ushort)shirtGeoset, (ushort)shirtGeoset, true);
                }
            }

            if (!robeChest)
            {
                if (pants != null && pants.GeosetGroup != null && pants.GeosetGroup[0] != 0)
                {
                    var geosetGroup = pants.GeosetGroup[0];
                    var pantsGeoset = 1101 + geosetGroup;
                    if (geosetGroup > 2)
                    {
                        SetGeometryVisible(1300, 1399, false);
                        SetGeometryVisible((ushort)pantsGeoset, (ushort)pantsGeoset, true);
                    }
                    else if (!showTabard)
                        SetGeometryVisible((ushort)pantsGeoset, (ushort)pantsGeoset, true);
                }
            }

            if (cloak != null && cloak.GeosetGroup != null && cloak.GeosetGroup[0] != 0)
            {
                SetGeometryVisible(1500, 1599, false);
                var cloakGeoset = 1501 + cloak.GeosetGroup[0];
                SetGeometryVisible((ushort)cloakGeoset, (ushort)cloakGeoset, true);
            }

            if (belt != null && belt.GeosetGroup != null && belt.GeosetGroup[0] != 0)
            {
                SetGeometryVisible(1800, 1899, false);
                var beltGeoset = 1801 + belt.GeosetGroup[0];
                SetGeometryVisible((ushort)beltGeoset, (ushort)beltGeoset, true);
            }

            if (pants == null && !robeChest && !robePants && !showTabard && !tailFlag && !largeBelt)
                SetGeometryVisible(1401, 1401, true);
            else
                SetGeometryVisible(1400, 1499, false);

            if (HornsModel != null)
            {
                HornsModel.SetGeometryVisible(0, 3000, false);

                if (pants == null)
                    HornsModel.SetGeometryVisible(1401, 1401, true);

                HornsModel.SetGeometryVisible((ushort)(2400 + HornsIndex), (ushort)(2400 + HornsIndex), true);
                HornsModel.SetGeometryVisible((ushort)(2500 + EyePatchIndex), (ushort)(2500 + EyePatchIndex), true);
            }

            foreach (var itemInSlot in Items)
            {
                var slot = itemInSlot.Key;
                var item = itemInSlot.Value;

                if (item.Models == null)
                    continue;

                var slotAttachments = GetSlotAttachments(slot, item);
                for (int i = 0; i < item.Models.Count; i++)
                {
                    if (item.Models[i] != null && !item.Models[i].IsCollection && slotAttachments.Count > i)
                    {
                        var attach = Attachments[slotAttachments[i]];
                        item.Models[i].Bone = attach.Bone;
                        item.Models[i].Attachment = attach;

                        if (item.Visual != null && item.Visual.Models != null)
                        {
                            var itemModel = item.Models[i].Model;

                            for (var j = 0; j < item.Visual.Models.Length; j++)
                            {
                                if (itemModel.Attachments == null || itemModel.Attachments[j] == null || item.Visual.Models[j] == null)
                                    continue;

                                attach = itemModel.Attachments[j];

                                item.Visual.Models[j].Bone = attach.Bone;
                                item.Visual.Models[j].Attachment = attach;
                                item.Visual.Models[j].Model.Bone = attach.Bone;
                                item.Visual.Models[j].Model.Attachment = attach;
                            }
                        }
                    }
                }
            }
        }

        private void CompositeTextures()
        {
            for (int i = 0; i < BakedTextures.Count; i++)
            {
                foreach (var bakedTexture in BakedTextures[(WhRegion)i].Values)
                {
                    if (!bakedTexture.Ready())
                        return;
                }
            }

            foreach (var item in Items.Values)
            {
                if (!item.Loaded)
                    return;
                else if (item.Textures != null)
                {
                    for (int j = 0; j < item.Textures.Count; j++)
                    {
                        if (item.Textures[j].Texture != null && !item.Textures[j].Texture.Ready())
                            return;
                    }
                }
            }

            foreach (var specialTexture in SpecialTextures.Values)
            {
                if (specialTexture != null && !specialTexture.Ready())
                    return;
            }

            if (SpecialTextures[1] == null)
                return;

            if (CompositeTexture == null)
                CompositeTexture = new Bitmap(SpecialTextures[1].Img.Width, SpecialTextures[1].Img.Height);

            using (var graphics = Graphics.FromImage(CompositeTexture))
            {
                graphics.DrawImage(SpecialTextures[1].Img, new PointF(0, 0));

                var w = CompositeTexture.Width;
                var h = CompositeTexture.Height;
                var regions = WhRegionOldNew.Old;

                if (w != h)
                    regions = WhRegionOldNew.New;

                for (int i = 1; i <= 3; ++i)
                {
                    if (BakedTextures[WhRegion.FaceLower].ContainsKey(i))
                    {
                        if (!BakedTextures[WhRegion.FaceLower][i].Ready())
                            return;

                        DrawImage(graphics, BakedTextures[WhRegion.FaceLower][i].Img, w, h, regions[WhRegion.FaceLower]);
                    }

                    if (BakedTextures[WhRegion.FaceUpper].ContainsKey(i))
                    {
                        if (!BakedTextures[WhRegion.FaceUpper][i].Ready())
                            return;

                        DrawImage(graphics, BakedTextures[WhRegion.FaceUpper][i].Img, w, h, regions[WhRegion.FaceUpper]);
                    }
                }

                var drawBra = true;
                var drawPanties = true;
                WhSlot uniqueSlot;

                foreach (var item in Items.Values)
                {
                    uniqueSlot = item.UniqueSlot;
                    if (uniqueSlot == WhSlot.SHIRT || uniqueSlot == WhSlot.CHEST || uniqueSlot == WhSlot.TABARD)
                        drawBra = false;
                    if (uniqueSlot == WhSlot.PANTS)
                        drawPanties = false;
                }

                if (drawBra && BakedTextures[WhRegion.TorsoUpper].ContainsKey(1))
                {
                    if (!BakedTextures[WhRegion.TorsoUpper][1].Ready())
                        return;

                    DrawImage(graphics, BakedTextures[WhRegion.TorsoUpper][1].Img, w, h, regions[WhRegion.TorsoUpper]);
                }

                if (drawPanties && BakedTextures[WhRegion.LegUpper].ContainsKey(1))
                {
                    if (!BakedTextures[WhRegion.LegUpper][1].Ready())
                        return;

                    DrawImage(graphics, BakedTextures[WhRegion.LegUpper][1].Img, w, h, regions[WhRegion.LegUpper]);
                }

                //if (self.TattoosIndex > 0) {
                //    var tattooRegion = self.getTattooRegion();
                //    if (tattooRegion && self.bakedTextures[tattooRegion][1]) {
                //        if (!self.bakedTextures[tattooRegion][1].ready())
                //            return;
                //        r = regions[tattooRegion];
                //        ctx.drawImage(self.bakedTextures[tattooRegion][1].mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
                //    }
                //}

                var items = Items.Values.ToList();
                items.Sort((a, b) => a.SortValue - b.SortValue);

                foreach (var item in items)
                {
                    if (item.Textures == null)
                        continue;

                    foreach (var t in item.Textures)
                    {
                        if (t.Gender != Gender || t.Texture == null || !t.Texture.Ready())
                            continue;

                        if (t.Region != WhRegion.Base)
                        {
                            if ((Meta.RaceFlags & 2) != 0 && t.Region == WhRegion.Foot)
                                continue;

                            DrawImage(graphics, t.Texture.Img, w, h, regions[t.Region]);
                        }
                    }
                }
            }

            NeedsCompositing = false;
        }

        private static void DrawImage(Graphics graphics, Bitmap image, int sourceWidth, int sourceHeight, WhRegionOldNew region)
        {
            graphics.DrawImage(image, new RectangleF()
            {
                X = sourceWidth * region.X,
                Y = sourceHeight * region.Y,
                Width = sourceWidth * region.W,
                Height = sourceHeight * region.H
            });
        }

        public void LoadItems(WhViewerOptions.Item[] items)
        {
            if (items == null)
                return;

            for (var i = 0; i < items.Length; ++i)
                AddItem(items[i].Slot, items[i].Id, items[i].VisualId);
        }

        public void AddItem(WhSlot slot, int id, int? visualid)
        {
            var item = new WhItem(this, slot, id, Race, Gender);

            if (visualid.HasValue)
                item.SetVisual(visualid.Value);

            var uniqueSlot = item.UniqueSlot;
            var altSlot = WhGlobal.SlotAlternate[slot];

            if (!Items.ContainsKey(uniqueSlot) || altSlot == 0)
                Items[uniqueSlot] = item;
            else
            {
                item.UniqueSlot = altSlot;
                Items[altSlot] = item;
            }
        }

        private void _Load(WhType type, string id)
        {
            string metaPath = null;

            if (type == WhType.ITEM)
                metaPath = "item";
            else if (type == WhType.HELM)
                metaPath = $"armor/{(int)WhSlot.HEAD}";
            else if (type == WhType.SHOULDER)
                metaPath = $"armor/{(int)WhSlot.SHOULDER}";
            else if (type == WhType.NPC || type == WhType.HUMANOIDNPC)
                metaPath = "npc";
            else if (type == WhType.OBJECT)
                metaPath = "object";
            else if (type == WhType.CHARACTER)
                metaPath = "character";
            else if (type == WhType.ITEMVISUAL)
                metaPath = "itemvisual";

            if (metaPath != null)
            {
                WhDefferedList.Add(() => LoadAndHandle_Meta(metaPath, type, id));
            }
            else if (type == WhType.PATH)
            {
                ModelPath = id;

                if (Meta == null)
                    Meta = new WhJsonMeta();

                WhDefferedList.Add(() => LoadAndHandle_Mo3(id));
            }
        }

        private void LoadAndHandle_Meta(string metaPath, WhType type, string id)
        {
            var data = WhDataLoader.LoadMeta(metaPath, id);

            LoadMeta(data, type, 0);
        }

        private void LoadAndHandle_Mo3(string id)
        {
            var data = WhDataLoader.LoadMo3(id);

            LoadMo3(data);
        }

        private void LoadAndHandle_MetaCharacterCustomization(WhRace race, WhGender gender)
        {
            var data = WhDataLoader.LoadMetaCharacterCustomization(race, gender);

            CustomFeatures = new WhCustomFeatures(data.customFeatures);
            HairGeosets = new WhHairGeosets(data.hairGeosets);
            FacialHairStyles = new WhFacialHairStyles(data.facialHairStyles);

            if (data.HDCustomGeoFileDataID != 0 && Class == WhClass.DEMONHUNTER)
            {
                var modelInfo = new WhModelInfo()
                {
                    Type = WhType.PATH,
                    Id = data.HDCustomGeoFileDataID.ToString(),
                    Parent = this
                };

                HornsModel = new WhModel(Opts, modelInfo, 0);
            }

            if (NeedsCompositing)
                Setup();
        }
    }
}