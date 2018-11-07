using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    /// <summary>
    /// ZamModelViewer.Wow.Model
    /// </summary>
    public class WhModel
    {
        public WhModel(WhViewerOptions viewerOptions, WhModelInfo model, int index, bool skipLoad = false)
        {
            // Это я делать не буду, так как оно ставится только тут, нафига мне это если я могу напрямую использовать WhTexture (ZamModelViewer.Wow.Texture)
            // в js файле (в методах LoadMeta и Setup этого класса) при этом есть использование через new self.Texture, по сути аналогично new ZamModelViewer.Wow.Texture
            //self.Texture = ZamModelViewer.Wow.Texture;

            //self.renderer = renderer;
            //self.viewer = viewer;

            Model = model;
            ModelIndex = index;
            ModelPath = null;

            Loaded = false;

            //self.particlesEnabled = false;
            //self.ribbonsEnabled = false;
            //self.deferredParentingMatrix = mat4.create();

            Opts = viewerOptions;

            //self.mount = null;
            //self.isMount = self.opts.mount && self.opts.mount.type == ZamModelViewer.Wow.Types.NPC && self.opts.mount.id == self.model.id;
            //if (self.model.type == ZamModelViewer.Wow.Types.CHARACTER) {
            //    if (self.opts.mount && self.opts.mount.type == ZamModelViewer.Wow.Types.NPC && self.opts.mount.id) {
            //        self.opts.mount.parent = self;
            //        self.mount = new ZamModelViewer.Wow.Model(renderer,viewer,self.opts.mount,0)
            //    }
            //}

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

            //self.HornsIndex = 0;
            //self.EyePatchIndex = 0;
            //self.TattoosIndex = 0;

            Parent = Model.Parent;

            Items = new Dictionary<WhSlot, WhItem>();

            NeedsCompositing = false;

            CustomFeatures = null;

            TextureOverrides = new Dictionary<int, WhTexture>();

            CompositeTexture = null;

            NpcTexture = null;
            SpecialTextures = new Dictionary<int, WhTexture>();

            //self.bakedTextures = [{}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}];

            IsHD = false;

            //self.sheathMain = -1;
            //self.sheathOff = -1;

            NumGeosets = 29;
            Geosets = new ushort[NumGeosets];
            GeosetDefaults = new ushort[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1 };
            CreatureGeosetData = 0;
            for (int i = 0; i < NumGeosets; i++)
                Geosets[i] = (ushort)(i * 100 + GeosetDefaults[i]);

            //self.time = 0;
            //self.frame = 0;
            //self.startAnimation = null;
            //self.currentAnimation = null;
            //self.animStartTime = 0;
            //self.animPaused = false;
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

            //self.animations = null;
            //self.animLookup = null;
            //self.bones = null;
            //self.boneLookup = null;
            //self.keyBoneLookup = null;

            Meshes = null;

            TexUnits = null;
            TexUnitLookup = null;

            //self.renderFlags = null;

            Materials = null;
            MaterialLookup = null;

            //self.textureAnims = null;
            //self.textureAnimLookup = null;
            //self.textureReplacements = null;
            //self.attachments = null;
            //self.attachmentLookup = null;
            //self.colors = null;
            //self.alphas = null;
            //self.alphaLookup = null;
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

        public WhRace Race { get; set; }
        public WhGender Gender { get; set; }
        public WhClass Class { get; set; }

        public WhJsonMeta Meta { get; set; }

        public int SkinIndex { get; set; }
        public int HairIndex { get; set; }
        public int HairColorIndex { get; set; }
        public int FaceIndex { get; set; }
        public int FeaturesIndex { get; set; }
        public int FeaturesColorIndex { get; set; }

        public WhModel Parent { get; set; }

        public Dictionary<WhSlot, WhItem> Items { get; set; }

        public bool NeedsCompositing { get; set; }

        public WhCustomFeatures CustomFeatures { get; set; }

        public Dictionary<int, WhTexture> TextureOverrides { get; set; }

        public WhTexture CompositeTexture { get; set; }

        public WhTexture NpcTexture { get; set; }
        public Dictionary<int, WhTexture> SpecialTextures { get; set; }

        public bool IsHD { get; set; }

        public int NumGeosets { get; set; }
        public ushort[] Geosets { get; set; }
        public ushort[] GeosetDefaults { get; set; }
        public int CreatureGeosetData { get; set; }

        public WhVertex[] Vertices { get; set; }
        public ushort[] Indices { get; set; }

        public WhMesh[] Meshes { get; set; }

        public WhTexUnit[] TexUnits { get; set; }
        public short[] TexUnitLookup { get; set; }

        public WhMaterial[] Materials { get; set; }
        public short[] MaterialLookup { get; set; }

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

                LoadAndHandle_MetaCharacterCustomization(meta.Race, meta.Gender);

                /*
                if (self.opts.sheathMain)
                    self.sheathMain = self.opts.sheathMain;
                if (self.opts.sheathOff)
                    self.sheathOff = self.opts.sheathOff;
                if (self.isHD && self.meta.Creature && self.meta.Creature.HDTexture) {
                    self.npcTexture = new self.Texture(self,-1,self.meta.Creature.HDTexture)
                } else if (self.meta.Creature && self.meta.Creature.Texture) {
                    self.npcTexture = new self.Texture(self,-1,self.meta.Creature.Texture)
                }
                */

                Race = meta.Race;
                Gender = meta.Gender;

                _Load(WhType.PATH, model.ToString());

                /*
                if (self.meta.Equipment) {
                    self.loadItems(self.meta.Equipment)
                }*/

                if (Opts.Items != null)
                    LoadItems(Opts.Items);

                /*if (self.model.type != Type.CHARACTER && self.meta.Race > 0) {
                    self.skinIndex = parseInt(self.meta.Creature.SkinColor);
                    self.hairIndex = parseInt(self.meta.Creature.HairStyle);
                    self.hairColorIndex = parseInt(self.meta.Creature.HairColor);
                    self.faceIndex = parseInt(self.meta.Creature.FaceType);
                    self.featuresIndex = parseInt(self.meta.Creature.FacialHair);
                    self.faceColorIndex = self.hairColorIndex;
                    self.HornsIndex = 0;
                    self.EyePatchIndex = 0;
                    self.TattoosIndex = 0
                } else {
                    if (self.opts.sk)
                        self.skinIndex = parseInt(self.opts.sk);
                    if (self.opts.ha)
                        self.hairIndex = parseInt(self.opts.ha);
                    if (self.opts.hc)
                        self.hairColorIndex = parseInt(self.opts.hc);
                    if (self.opts.fa)
                        self.faceIndex = parseInt(self.opts.fa);
                    if (self.opts.fh)
                        self.featuresIndex = parseInt(self.opts.fh);
                    if (self.opts.fc)
                        self.faceColorIndex = parseInt(self.opts.fc);
                    if (self.opts.ho)
                        self.HornsIndex = parseInt(self.opts.ho);
                    if (self.opts.ep)
                        self.EyePatchIndex = parseInt(self.opts.ep);
                    if (self.opts.ta)
                        self.TattoosIndex = parseInt(self.opts.ta)
                }
                 */
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
                // ToDo реализовать все остальное
                throw new NotImplementedException();
            }
            else if (type == WhType.ITEMVISUAL)
            {
                // ToDo реализовать все остальное
                throw new NotImplementedException();
            }
            else if (type == WhType.COLLECTION)
            {
                // ToDo реализовать все остальное
                throw new NotImplementedException();
            }
            else
            {
                // ToDo реализовать все остальное
                throw new NotImplementedException();
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

                //r.position = ofsAnimations;
                //var numAnims = r.getInt32();
                //if (numAnims > 0) {
                //    self.animations = new Array(numAnims);
                //    for (i = 0; i < numAnims; ++i) {
                //        self.animations[i] = new Wow.Animation(r)
                //    }
                //}
                //r.position = ofsAnimLookup;
                //var numAnimLookup = r.getInt32();
                //if (numAnimLookup > 0) {
                //    self.animLookup = new Array(numAnimLookup);
                //    for (i = 0; i < numAnimLookup; ++i) {
                //        self.animLookup[i] = r.getInt16()
                //    }
                //}

                //r.position = ofsBones;
                //var numBones = r.getInt32();
                //if (numBones > 0) {
                //    self.bones = new Array(numBones);
                //    for (i = 0; i < numBones; ++i) {
                //        self.bones[i] = new Wow.Bone(self,i,r)
                //    }
                //}
                //r.position = ofsBoneLookup;
                //var numBoneLookup = r.getInt32();
                //if (numBoneLookup > 0) {
                //    self.boneLookup = new Array(numBoneLookup);
                //    for (i = 0; i < numBoneLookup; ++i) {
                //        self.boneLookup[i] = r.getInt16()
                //    }
                //}
                //r.position = ofsKeyBoneLookup;
                //var numKeyBoneLookup = r.getInt32();
                //if (numKeyBoneLookup > 0) {
                //    self.keyBoneLookup = new Array(numKeyBoneLookup);
                //    for (i = 0; i < numKeyBoneLookup; ++i) {
                //        self.keyBoneLookup[i] = r.getInt16()
                //    }
                //}

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

                //r.position = ofsRenderFlags;
                //var numRenderFlags = r.getInt32();
                //if (numRenderFlags > 0) {
                //    self.renderFlags = new Array(numRenderFlags);
                //    for (i = 0; i < numRenderFlags; ++i) {
                //        self.renderFlags[i] = new Wow.RenderFlag(r)
                //    }
                //}

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
                //r.position = ofsTexAnimLookup;
                //var numTexAnimLookup = r.getInt32();
                //if (numTexAnimLookup > 0) {
                //    self.textureAnimLookup = new Array(numTexAnimLookup);
                //    for (i = 0; i < numTexAnimLookup; ++i) {
                //        self.textureAnimLookup[i] = r.getInt16()
                //    }
                //}

                //r.position = ofsTexReplacements;
                //var numTexReplacements = r.getInt32();
                //if (numTexReplacements > 0) {
                //    self.textureReplacements = new Array(numTexReplacements);
                //    for (i = 0; i < numTexReplacements; ++i) {
                //        self.textureReplacements[i] = r.getInt16()
                //    }
                //}

                //r.position = ofsAttachments;
                //var numAttachments = r.getInt32();
                //if (numAttachments > 0) {
                //    self.attachments = new Array(numAttachments);
                //    for (i = 0; i < numAttachments; ++i) {
                //        self.attachments[i] = new Wow.Attachment(r)
                //    }
                //}
                //r.position = ofsAttachmentLookup;
                //var numAttachmentLookup = r.getInt32();
                //if (numAttachmentLookup > 0) {
                //    self.attachmentLookup = new Array(numAttachmentLookup);
                //    for (i = 0; i < numAttachmentLookup; ++i) {
                //        self.attachmentLookup[i] = r.getInt16()
                //    }
                //}

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
                //r.position = ofsAlphaLookup;
                //var numAlphaLookup = r.getInt32();
                //if (numAlphaLookup > 0) {
                //    self.alphaLookup = new Array(numAlphaLookup);
                //    for (i = 0; i < numAlphaLookup; ++i) {
                //        self.alphaLookup[i] = r.getInt16()
                //    }
                //}

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

        public void OnLoaded()
        {
            //var self = this, i, Slots = ZamModelViewer.Wow.Slots;

            if (TexUnits != null) {
                for (int i = 0; i < TexUnits.Length; ++i)
                    TexUnits[i].Setup(this);

                //self.sortedTexUnits = self.texUnits.concat();
                //self.sortedTexUnits.sort(function(a, b) {
                //    if (a.meshId == b.meshId)
                //        return a.priorityPlane - b.priorityPlane;
                //    else
                //        return a.meshId - b.meshId
                //})
            }

            //if (self.mount) {
            //    if (ZamModelViewer.Wow.StandingMounts[self.mount.model.id]) {
            //        self.setAnimation("StealthStand")
            //    } else {
            //        self.setAnimation("Mount")
            //    }
            //} else {
            //    self.setAnimation("Stand")
            //}
            //self.updateBuffers(true);
            //self.updateBounds();

            Setup();

            Loaded = true;

            //if (self.isMount && self.parent.loaded) {
            //    self.parent.updateBounds()
            //}
            //if (self.parent && self.parent.loaded) {
            //    self.parent.updateMeshes()
            //}

            SetupWhenLoaded();
        }

        public int GetResolutionVariationType(WhCharVariationType variationType)
        {
            return WhGlobal.CharVariationMap[variationType][IsHD ? 1 : 0];
        }

        public void Setup()
        {
            if (Model.Type != WhType.CHARACTER && Model.Type != WhType.NPC && Model.Type != WhType.HUMANOIDNPC || (int)Race < 1)
            {
                //self.applyMonsterGeosets();
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
                skinFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Skin).ToString(), 0, SkinIndex);
                faceFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Face).ToString(), FaceIndex, SkinIndex);
                facialHairFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.FacialHair).ToString(), FeaturesIndex, FeaturesColorIndex);
                hairFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Hair).ToString(), HairIndex, HairColorIndex);
                underwearFeature = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Underwear).ToString(), 0, SkinIndex);

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

            //if (self.hairGeosets) {
            //    self.currentHairGeoset = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Hair, self.hairIndex)
            //}

            UpdateMeshes();

            //var Region = ZamModelViewer.Wow.Regions;

            if (NpcTexture == null) {
                NeedsCompositing = true;

                if (skinFeature != null && skinFeature.textures[0] != 0 && !SpecialTextures.ContainsKey(1))
                {
                    SpecialTextures[1] = new WhTexture(this, 1, skinFeature.textures[0]);
                }

                //if (underwearFeature) {
                //    if (underwearFeature.textures[0] != 0 && !self.bakedTextures[Region.LegUpper][1])
                //        self.bakedTextures[Region.LegUpper][1] = new self.Texture(self,Region.LegUpper,underwearFeature.textures[0]);
                //    if (underwearFeature.textures[1] != 0 && !self.bakedTextures[Region.TorsoUpper][1])
                //        self.bakedTextures[Region.TorsoUpper][1] = new self.Texture(self,Region.TorsoUpper,underwearFeature.textures[1])
                //}
                //if (tattooFeature) {
                //    var tattooRegion = self.getTattooRegion();
                //    if (tattooRegion && tattooFeature.textures[0] != 0 && !self.bakedTextures[tattooRegion][1]) {
                //        self.bakedTextures[tattooRegion][1] = new self.Texture(self,tattooRegion,tattooFeature.textures[0])
                //    }
                //}
                //if (faceFeature) {
                //    if (faceFeature.textures[0] != 0 && !self.bakedTextures[Region.FaceLower][1])
                //        self.bakedTextures[Region.FaceLower][1] = new self.Texture(self,Region.FaceLower,faceFeature.textures[0]);
                //    if (faceFeature.textures[1] != 0 && !self.bakedTextures[Region.FaceUpper][1])
                //        self.bakedTextures[Region.FaceUpper][1] = new self.Texture(self,Region.FaceUpper,faceFeature.textures[1])
                //}
                //if (facialHairFeature) {
                //    if (facialHairFeature.textures[0] != 0 && !self.bakedTextures[Region.FaceLower][2])
                //        self.bakedTextures[Region.FaceLower][2] = new self.Texture(self,Region.FaceLower,facialHairFeature.textures[0]);
                //    if (facialHairFeature.textures[1] != 0 && !self.bakedTextures[Region.FaceUpper][2])
                //        self.bakedTextures[Region.FaceUpper][2] = new self.Texture(self,Region.FaceUpper,facialHairFeature.textures[1])
                //}
                //if (self.currentHairGeoset) {
                //    if (self.currentHairGeoset.showscalp == 1) {
                //        hairFeature = self.customFeatures.getFeature(self.getResolutionVariationType(CharVariationType.Hair), 1, self.hairColorIndex)
                //    } else if (hairFeature && self.currentHairGeoset.showscalp == 0) {
                //        if (hairFeature.textures[1] != 0 && !self.bakedTextures[Region.FaceLower][3])
                //            self.bakedTextures[Region.FaceLower][3] = new self.Texture(self,Region.FaceLower,hairFeature.textures[1]);
                //        if (hairFeature.textures[2] != 0 && !self.bakedTextures[Region.FaceUpper][3])
                //            self.bakedTextures[Region.FaceUpper][3] = new self.Texture(self,Region.FaceUpper,hairFeature.textures[2])
                //    }
                //}
            }

            if (skinFeature != null && skinFeature.textures[1] != 0 && !SpecialTextures.ContainsKey(8))
            {
                SpecialTextures[8] = new WhTexture(this, 8, skinFeature.textures[1]);
            }

            //if (hairFeature && hairFeature.textures[0] != 0 && !self.specialTextures[6]) {
            //    self.specialTextures[6] = new self.Texture(self,6,hairFeature.textures[0])
            //}
        }

        private void SetupWhenLoaded()
        {
            // ToDo: этого метода нету в оригинальном js
            // тут я буду выносить все что нужно дополнительно сделать после загрузки модели
            // например мне нужны некоторые вещи из draw() метода (в Model, то есть текущем классе).
            // при этом самого draw() тут не реализовано, т.к. отрисовка тут не нужна но нужно сделать некую подготовку которая вызывается из него в оригинале
            // может быть тут еще что то будет
            // сначало наверно надо удостовериться что все текстуры и модели подгрузились (это наверно будет кейс, если я сделаю асинхронно подгрузку всего этого)
            // и только после этого выполнять основной код.



            // Далее код из draw()

            if (NeedsCompositing)
                CompositeTextures();
        }

        public void SetGeometryVisibleCustom(object data)
        {
            //if (data)
            //{
            //    this.geosets[data.geosetType] = data.geosetType * 100 + data.geosetID
            //}
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

        public void UpdateMeshes()
        {
            //var self = this, i, j, u, Wow = ZamModelViewer.Wow, Races = Wow.Races, Genders = Wow.Genders, Slots = Wow.Slots, CharVariationType = Wow.CharVariationType;
            
            if (TexUnits == null || TexUnits.Length == 0)
                return;

            Geosets[7] = 702;

            //if (self.hairGeosets) {
            //    if (self.race == Races.ZANDALARITROLL) {
            //        var earring = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Custom3, self.EyePatchIndex);
            //        self.setGeometryVisibleCustom(earring);
            //        var tusk = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Custom2, self.HornsIndex);
            //        self.setGeometryVisibleCustom(tusk)
            //    } else if (self.race == Races.VULPERA) {
            //        var nose = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Custom2, self.HornsIndex);
            //        self.setGeometryVisibleCustom(nose)
            //    } else if (self.race == Races.DARKIRONDWARF) {
            //        var earring = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Custom1, self.HornsIndex);
            //        self.setGeometryVisibleCustom(earring)
            //    } else if (self.race == Races.LIGHTFORGEDDRAENEI) {
            //        var rune = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Custom3, self.EyePatchIndex);
            //        self.setGeometryVisibleCustom(rune)
            //    }
            //    var skin = self.hairGeosets.getHairGeosetByColorIndex(CharVariationType.Skin, self.skinIndex);
            //    self.setGeometryVisibleCustom(skin);
            //    var face = self.hairGeosets.getHairGeosetByVariationId(CharVariationType.Face, self.faceIndex);
            //    self.setGeometryVisibleCustom(face)
            //}

            //self.setGeometryVisibleCustom(self.currentHairGeoset);

            if (Geosets[0] == 0)
                Geosets[0] = 1;

            //if (self.facialHairStyles) {
            //    var facialStyle = self.facialHairStyles.getStyle(self.featuresIndex);
            //    if (facialStyle) {
            //        if (facialStyle.geoset[0] != -1)
            //            self.geosets[1] = 100 + facialStyle.geoset[0];
            //        if (facialStyle.geoset[1] != -1)
            //            self.geosets[3] = 300 + facialStyle.geoset[1];
            //        if (facialStyle.geoset[2] != -1)
            //            self.geosets[2] = 200 + facialStyle.geoset[2];
            //        if (facialStyle.geoset[3] != -1)
            //            self.geosets[16] = 1600 + facialStyle.geoset[3];
            //        if (facialStyle.geoset[4] != -1)
            //            self.geosets[17] = 1700 + facialStyle.geoset[4]
            //    }
            //}

            bool eyeGlowFlag = false;
            if (Class == WhClass.DEATHKNIGHT)
                eyeGlowFlag = true;
            else
            {
                var faceSection = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Face).ToString(), FaceIndex, SkinIndex);
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
            var skinSection = CustomFeatures.GetFeature(GetResolutionVariationType(WhCharVariationType.Skin).ToString(), 0, SkinIndex);
            if (skinSection != null)
                tailFlag = (skinSection.flags & 256) != 0;

            //var tailGeoset = 1;
            //if (!tailFlag) {
            //    tailGeoset = self.hairGeosets.getHairGeosetConditional(CharVariationType.Skin, self.skinIndex, 19)
            //}
            //if (tailGeoset <= 0)
            //    tailGeoset = 1;
            //self.setGeometryVisible(1900 + tailGeoset, 1900 + tailGeoset, true);

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

                //var hideGeoset = gender == Genders.MALE ? helm.HideGeosetMale : helm.HideGeosetFemale;
                //if (hideGeoset) {
                //    var groups = [0, 1, 2, 3, 7, 16, 17, 24, 25];
                //    for (i = 0; i < hideGeoset.length; i++) {
                //        if ((1 << race & hideGeoset[i]) != 0) {
                //            var meshBase = groups[i] * 100;
                //            var meshID = meshBase + self.geosetDefaults[groups[i]];
                //            self.setGeometryVisible(meshBase + self.geosetDefaults[i], meshBase + 99, false);
                //            self.setGeometryVisible(meshID, meshID, true)
                //        }
                //    }
                //}
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

            //if (gloves && gloves.geosetGroup && gloves.geosetGroup[0]) {
            //    var glovesGeoset = 401 + gloves.geosetGroup[0];
            //    self.setGeometryVisible(401, 499, false);
            //    self.setGeometryVisible(glovesGeoset, glovesGeoset, true);
            //    gloves.sortValue += 2
            //} else {
            //    if (chest && chest.geosetGroup && chest.geosetGroup[0]) {
            //        var chesetGeoset = 801 + chest.geosetGroup[0];
            //        self.setGeometryVisible(chesetGeoset, chesetGeoset, true)
            //    }
            //}

            //var canShowShirt = !(chest || belt || wrist);
            //if (canShowShirt && shirt && shirt.geosetGroup && shirt.geosetGroup[0]) {
            //    var shirtGeoset = 801 + shirt.geosetGroup[0];
            //    self.setGeometryVisible(shirtGeoset, shirtGeoset, true)
            //}

            //if (tabard) {
            //    if ((tabard.flags & 1048576) == 0) {
            //        self.setGeometryVisible(2200, 2299, false);
            //        self.setGeometryVisible(2202, 2202, true)
            //    }
            //} else {
            //    if (chest && chest.geosetGroup && chest.geosetGroup[3]) {
            //        var chestGeoset = 2201 + chest.geosetGroup[3];
            //        self.setGeometryVisible(2200, 2299, false);
            //        self.setGeometryVisible(chestGeoset, chestGeoset, true)
            //    }
            //}

            bool largeBelt = false;
            //if (belt && belt.geosetGroup && belt.geosetGroup[0]) {
            //    largeBelt = (belt.flags & 512) != 0
            //}

            bool robePants = false;
            bool robeChest = false;
            //if (chest && chest.geosetGroup && chest.geosetGroup[2]) {
            //    robeChest = true;
            //    self.setGeometryVisible(501, 599, false);
            //    self.setGeometryVisible(902, 999, false);
            //    self.setGeometryVisible(1100, 1199, false);
            //    self.setGeometryVisible(1300, 1399, false);
            //    var chestGeoset = 1301 + chest.geosetGroup[2];
            //    self.setGeometryVisible(chestGeoset, chestGeoset, true)
            //} else if (pants && pants.geosetGroup && pants.geosetGroup[2]) {
            //    robePants = true;
            //    self.setGeometryVisible(501, 599, false);
            //    self.setGeometryVisible(902, 999, false);
            //    self.setGeometryVisible(1100, 1199, false);
            //    self.setGeometryVisible(1300, 1399, false);
            //    var pantsGeoset = 1301 + pants.geosetGroup[2];
            //    self.setGeometryVisible(pantsGeoset, pantsGeoset, true)
            //} else {
            //    if (boots && boots.geosetGroup && boots.geosetGroup[0]) {
            //        self.setGeometryVisible(501, 599, false);
            //        self.setGeometryVisible(901, 901, true);
            //        var bootsGeoset = 501 + boots.geosetGroup[0];
            //        self.setGeometryVisible(bootsGeoset, bootsGeoset, true)
            //    } else {
            //        var pantsGeoset;
            //        if (pants && pants.geosetGroup && pants.geosetGroup[1]) {
            //            pantsGeoset = 901 + pants.geosetGroup[1]
            //        } else {
            //            pantsGeoset = 901
            //        }
            //        self.setGeometryVisible(pantsGeoset, pantsGeoset, true)
            //    }
            //}

            //var bootsGeoset;
            //if (boots && boots.geosetGroup && boots.geosetGroup[1]) {
            //    bootsGeoset = 2e3 + boots.geosetGroup[1]
            //} else if (boots && (boots.flags & 1048576) == 0) {
            //    bootsGeoset = 2002
            //} else {
            //    bootsGeoset = 2001
            //}
            //self.setGeometryVisible(bootsGeoset, bootsGeoset, true);

            bool showTabard = false;
            //var hasDress = robeChest | robePants;
            //if (!hasDress && tabard && tabard.geosetGroup && tabard.geosetGroup[0]) {
            //    showTabard = false;
            //    var tabardGeoset;
            //    if (largeBelt) {
            //        showTabard = true;
            //        tabardGeoset = 1203
            //    } else {
            //        showTabard = true;
            //        tabardGeoset = 1201 + tabard.geosetGroup[0]
            //    }
            //    self.setGeometryVisible(tabardGeoset, tabardGeoset, true)
            //} else if (!(flags & 16)) {} else {
            //    self.setGeometryVisible(1201, 1201, true);
            //    if (!hasDress) {
            //        self.setGeometryVisible(1202, 1202, true);
            //        showTabard = true
            //    }
            //}

            if (!(showTabard || robeChest)) {
                //if (chest && chest.geosetGroup && chest.geosetGroup[1]) {
                //    var chestGeoset = 1001 + chest.geosetGroup[1];
                //    self.setGeometryVisible(chestGeoset, chestGeoset, true)
                //} else if (shirt && shirt.geosetGroup && shirt.geosetGroup[1]) {
                //    var shirtGeoset = 1001 + shirt.geosetGroup[1];
                //    self.setGeometryVisible(shirtGeoset, shirtGeoset, true)
                //}
            }

            if (!robeChest) {
                //if (pants && pants.geosetGroup && pants.geosetGroup[0]) {
                //    var geosetGroup = pants.geosetGroup[0];
                //    var pantsGeoset = 1101 + geosetGroup;
                //    if (geosetGroup > 2) {
                //        self.setGeometryVisible(1300, 1399, false);
                //        self.setGeometryVisible(pantsGeoset, pantsGeoset, true)
                //    } else if (!showTabard) {
                //        self.setGeometryVisible(pantsGeoset, pantsGeoset, true)
                //    }
                //}
            }

            //if (cloak && cloak.geosetGroup && cloak.geosetGroup[0]) {
            //    self.setGeometryVisible(1500, 1599, false);
            //    var cloakGeoset = 1501 + cloak.geosetGroup[0];
            //    self.setGeometryVisible(cloakGeoset, cloakGeoset, true)
            //}

            //if (belt && belt.geosetGroup && belt.geosetGroup[0]) {
            //    self.setGeometryVisible(1800, 1899, false);
            //    var beltGeoset = 1801 + belt.geosetGroup[0];
            //    self.setGeometryVisible(beltGeoset, beltGeoset, true)
            //}

            if (pants == null && !robeChest && !robePants && !showTabard && !tailFlag && !largeBelt)
                SetGeometryVisible(1401, 1401, true);
            else
                SetGeometryVisible(1400, 1499, false);

            //if (self.hornsModel) {
            //    self.hornsModel.setGeometryVisible(0, 3e3, false);
            //    if (!pants)
            //        self.hornsModel.setGeometryVisible(1401, 1401, true);
            //    self.hornsModel.setGeometryVisible(2400 + self.HornsIndex, 2400 + self.HornsIndex, true);
            //    self.hornsModel.setGeometryVisible(2500 + self.EyePatchIndex, 2500 + self.EyePatchIndex, true)
            //}

            //var attach;
            //for (slot in self.items) {
            //    item = self.items[slot];
            //    if (!item.models)
            //        continue;
            //    var slotAttachments = self.getSlotAttachments(slot, item);
            //    for (i = 0; i < item.models.length; ++i) {
            //        if (item.models[i] && !item.models[i].isCollection && slotAttachments.length > i) {
            //            attach = self.attachments[slotAttachments[i]];
            //            item.models[i].bone = attach.bone;
            //            item.models[i].attachment = attach;
            //            if (item.visual && item.visual.models) {
            //                var itemModel = item.models[i].model;
            //                for (var j = 0; j < item.visual.models.length; j++) {
            //                    if (!itemModel.attachments || !itemModel.attachments[j] || !item.visual.models[j])
            //                        continue;
            //                    attach = itemModel.attachments[j];
            //                    item.visual.models[j].bone = attach.bone;
            //                    item.visual.models[j].attachment = attach;
            //                    item.visual.models[j].model.bone = attach.bone;
            //                    item.visual.models[j].model.attachment = attach
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void CompositeTextures()
        {
            // Временно так, потом реализовать все что закоменчего, вместо этого
            CompositeTexture = SpecialTextures[1];







            //var self = this, i, j, Region = ZamModelViewer.Wow.Regions, Classes = ZamModelViewer.Wow.Classes, Slots = ZamModelViewer.Wow.Slots, Races = ZamModelViewer.Wow.Races, item, t;
            //for (i = 0; i < self.bakedTextures.length; ++i) {
            //    for (j in self.bakedTextures[i]) {
            //        if (!self.bakedTextures[i][j].ready())
            //            return
            //    }
            //}
            //for (i in self.items) {
            //    item = self.items[i];
            //    if (!item.loaded) {
            //        return
            //    } else if (item.textures) {
            //        for (j = 0; j < item.textures.length; ++j) {
            //            if (item.textures[j].texture && !item.textures[j].texture.ready())
            //                return
            //        }
            //    }
            //}
            //for (var i in self.specialTextures) {
            //    if (self.specialTextures[i] && !self.specialTextures[i].ready())
            //        return
            //}
            //if (!self.specialTextures[1])
            //    return;
            //if (!self.compositeImage) {
            //    self.compositeImage = document.createElement("canvas");
            //    self.compositeImage.width = self.specialTextures[1].mergedImg.width;
            //    self.compositeImage.height = self.specialTextures[1].mergedImg.height
            //}
            //var ctx = self.compositeImage.getContext("2d");
            //ctx.drawImage(self.specialTextures[1].mergedImg, 0, 0, self.compositeImage.width, self.compositeImage.height);
            //var w = self.compositeImage.width, h = self.compositeImage.height, regions = Region.old, r;
            //if (w != h)
            //    regions = Region.new;
            //for (i = 1; i <= 3; ++i) {
            //    if (self.bakedTextures[Region.FaceLower][i]) {
            //        if (!self.bakedTextures[Region.FaceLower][i].ready())
            //            return;
            //        r = regions[Region.FaceLower];
            //        ctx.drawImage(self.bakedTextures[Region.FaceLower][i].mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
            //    }
            //    if (self.bakedTextures[Region.FaceUpper][i]) {
            //        if (!self.bakedTextures[Region.FaceUpper][i].ready())
            //            return;
            //        r = regions[Region.FaceUpper];
            //        ctx.drawImage(self.bakedTextures[Region.FaceUpper][i].mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
            //    }
            //}
            //var drawBra = true, drawPanties = true, uniqueSlot;
            //for (i in self.items) {
            //    uniqueSlot = self.items[i].uniqueSlot;
            //    if (uniqueSlot == Slots.SHIRT || uniqueSlot == Slots.CHEST || uniqueSlot == Slots.TABARD)
            //        drawBra = false;
            //    if (uniqueSlot == Slots.PANTS)
            //        drawPanties = false
            //}
            //if (drawBra && self.bakedTextures[Region.TorsoUpper][1]) {
            //    if (!self.bakedTextures[Region.TorsoUpper][1].ready())
            //        return;
            //    r = regions[Region.TorsoUpper];
            //    ctx.drawImage(self.bakedTextures[Region.TorsoUpper][1].mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
            //}
            //if (drawPanties && self.bakedTextures[Region.LegUpper][1]) {
            //    if (!self.bakedTextures[Region.LegUpper][1].ready())
            //        return;
            //    r = regions[Region.LegUpper];
            //    ctx.drawImage(self.bakedTextures[Region.LegUpper][1].mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
            //}
            //if (self.TattoosIndex > 0) {
            //    var tattooRegion = self.getTattooRegion();
            //    if (tattooRegion && self.bakedTextures[tattooRegion][1]) {
            //        if (!self.bakedTextures[tattooRegion][1].ready())
            //            return;
            //        r = regions[tattooRegion];
            //        ctx.drawImage(self.bakedTextures[tattooRegion][1].mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
            //    }
            //}
            //var items = [];
            //for (i in self.items) {
            //    items.push(self.items[i])
            //}
            //items.sort(function(a, b) {
            //    return a.sortValue - b.sortValue
            //});
            //for (i = 0; i < items.length; ++i) {
            //    item = items[i];
            //    if (!item.textures)
            //        continue;
            //    for (j = 0; j < item.textures.length; ++j) {
            //        t = item.textures[j];
            //        if (t.gender != self.gender || !t.texture || !t.texture.ready())
            //            continue;
            //        if (t.region != Region.Base) {
            //            if ((self.meta.RaceFlags & 2) != 0 && t.region == Region.Foot)
            //                continue;
            //            r = regions[t.region];
            //            ctx.drawImage(t.texture.mergedImg, w * r.x, h * r.y, w * r.w, h * r.h)
            //        }
            //    }
            //}
            //var gl = self.renderer.context;
            //if (self.compositeTexture)
            //    gl.deleteTexture(self.compositeTexture);
            //self.compositeTexture = gl.createTexture();
            //gl.bindTexture(gl.TEXTURE_2D, self.compositeTexture);
            //gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, self.compositeImage);
            //gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);

            NeedsCompositing = false;
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
                LoadAndHandle_Meta(metaPath, type, id);
            }
            else if (type == WhType.PATH)
            {
                ModelPath = id;

                if (Meta == null)
                    Meta = new WhJsonMeta();

                LoadAndHandle_Mo3(id);
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

            //self.hairGeosets = new ZamModelViewer.Wow.HairGeosets(data.hairGeosets);
            //self.facialHairStyles = new ZamModelViewer.Wow.FacialHairStyles(data.facialHairStyles);

            if (data.HDCustomGeoFileDataID != 0 && Class == WhClass.DEMONHUNTER)
            {
                //    var modelInfo = {
                //        type: Type.PATH,
                //        id: data.HDCustomGeoFileDataID,
                //        parent: self
                //    };
                //    self.hornsModel = new ZamModelViewer.Wow.Model(self.renderer, self.viewer, modelInfo,0)
            }

            if (NeedsCompositing)
                Setup();
        }
    }
}