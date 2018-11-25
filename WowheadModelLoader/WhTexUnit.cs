using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WowheadModelLoader
{
    public class WhTexUnit
    {
        public WhTexUnit(BinaryReader r)
        {
            Flags = r.ReadByte();
            PriorityPlane = r.ReadByte();
            ShaderId = r.ReadUInt16();
            MeshIndex = r.ReadUInt16();
            GeosetIndex = r.ReadUInt16();
            ColorIndex = r.ReadInt16();
            RenderFlagIndex = r.ReadUInt16();
            MaterialLayer = r.ReadUInt16();
            Opcount = r.ReadUInt16();
            MaterialIndex = r.ReadInt16();
            TexUnitIndex = r.ReadUInt16();
            AlphaIndex = r.ReadInt16();
            TextureAnimIndex = r.ReadInt16();
            Show = true;
            Model = null;
            Mesh = null;
            MeshId = 0;
            RenderFlag = null;
            Material = new List<WhMaterial>();
            TextureAnim = new List<object>();

            //self.textureMatrix = mat4.create();

            Color = null;
            Alpha = null;

            Unlit = false;
            Cull = false;
            NoZWrite = false;

            //self.tmpColor = vec4.create();
            //self.tmpVec = vec3.create();
            //self.tmpQuat = quat.create();
        }

        public byte Flags { get; set; }
        public byte PriorityPlane { get; set; }
        public ushort ShaderId { get; set; }
        public ushort MeshIndex { get; set; }
        public ushort GeosetIndex { get; set; }
        public short ColorIndex { get; set; }
        public ushort RenderFlagIndex { get; set; }
        public ushort MaterialLayer { get; set; }
        public ushort Opcount { get; set; }
        public short MaterialIndex { get; set; }
        public ushort TexUnitIndex { get; set; }
        public short AlphaIndex { get; set; }
        public short TextureAnimIndex { get; set; }

        public bool Show { get; set; }
        public WhModel Model { get; set; }
        public WhMesh Mesh { get; set; }
        public ushort MeshId { get; set; }
        public WhRenderFlag RenderFlag { get; set; }
        public List<WhMaterial> Material { get; set; }
        public List<object> TextureAnim { get; set; }
        //textureMatrix
        public object Color { get; set; }
        public object Alpha { get; set; }

        public bool Flip { get; set; }
        public bool Unlit { get; set; }
        public bool Cull { get; set; }
        public bool NoZWrite { get; set; }
        //tmpColor
        //tmpVec
        //tmpQuat

        public void Setup(WhModel model)
        {
            Model = model;
            Mesh = model.Meshes[MeshIndex];
            MeshId = Mesh.Id;

            WhRenderFlag.ComputeFlags(this);

            // На момент теста эта штука была undefined, так что думаю можно это не использовать
            //window.MeshLoadFilter && window.MeshLoadFilter(self);

            //var program = ZamModelViewer.Wow.ShaderTool.GetWowProgram(self.shaderId, self.opcount, self.renderFlag);
            //WH.debug(self.shaderId, program);
            //self.program = program;

            for (int i = 0; i < Opcount; i++)
            {
                if (MaterialIndex > -1 && MaterialIndex < model.MaterialLookup.Length)
                {
                    var matIdx = model.MaterialLookup[MaterialIndex + i];
                    if (matIdx > -1 && matIdx < model.Materials.Length)
                        Material.Insert(i, model.Materials[matIdx]);
                }

                //    if (self.textureAnimIndex > -1 && self.textureAnimIndex < model.textureAnimLookup.length) {
                //        var animIdx = model.textureAnimLookup[self.textureAnimIndex + i];
                //        if (animIdx > -1 && model.textureAnims && animIdx < model.textureAnims.length) {
                //            self.textureAnim.splice(i, 0, model.textureAnims[animIdx])
                //        } else {
                //            self.textureAnim.splice(i, 0, null)
                //        }
                //    }
            }

            //if (self.flip) {
            //    self.material = self.material.reverse();
            //    self.textureAnim = self.textureAnim.reverse()
            //}
            //if (model.colors && self.colorIndex > -1 && self.colorIndex < model.colors.length) {
            //    self.color = model.colors[self.colorIndex]
            //}
            //if (self.alphaIndex > -1 && self.alphaIndex < model.alphaLookup.length) {
            //    var alphaIdx = model.alphaLookup[self.alphaIndex];
            //    if (alphaIdx > -1 && alphaIdx < model.alphas.length) {
            //        self.alpha = model.alphas[alphaIdx]
            //    }
            //}
        }

        public Dictionary<string, TextureInfo> GetTextures()
        {
            int count = 0;
            object texture1 = null;
            object texture2 = null;
            object texture3 = null;
            object texture4 = null;

            for (int i = 0; i < Material.Count; i++) {
                object texture = null;

                if (Material[i] != null)
                {
                    if (Material[i].Type == 1)
                    {
                        if (Model.NpcTexture != null)
                            texture = Model.NpcTexture;
                        else if (Model.CompositeTexture != null)
                            texture = Model.CompositeTexture;
                    }
                    else if (Material[i].Texture != null)
                        texture = Material[i].Texture;
                    else if (
                        (
                            ((int)Model.Model.Type < 8 || (int)Model.Model.Type > 32)
                            && Material[i].Type == 2 || Material[i].Type >= 11
                        )
                        && Model.TextureOverrides.ContainsKey(Material[i].Index)
                    )
                    {
                        texture = Model.TextureOverrides[Material[i].Index];
                    }
                    else if (Material[i].Type != -1 && Model.TextureOverrides.ContainsKey(Material[i].Type))
                        texture = Model.TextureOverrides[Material[i].Type];
                    else if (Material[i].Type != -1 && Model.SpecialTextures.ContainsKey(Material[i].Type))
                        texture = Model.SpecialTextures[Material[i].Type];
                    else if (Material[i].Filename == 0)
                    {
                        var mat = Model.Materials[MaterialIndex + count];
                        if (mat != null && mat.Texture != null)
                            texture = mat.Texture;
                    }
                }

                if (i == 0)
                    texture1 = texture;
                if (i == 1)
                    texture2 = texture;
                if (i == 2)
                    texture3 = texture;
                if (i == 3)
                    texture4 = texture;

                count++;
            }

            var textures = new Dictionary<string, TextureInfo>() {
                { "Texture1", new TextureInfo {
                    Texture = texture1 as WhTexture,
                    Img = texture1 as TextureImage,
                    Location = 0
                } },
                { "Texture2", new TextureInfo {
                    Texture = texture2 as WhTexture,
                    Img = texture2 as TextureImage,
                    Location = 1
                }},
                { "Texture3", new TextureInfo {
                    Texture = texture3 as WhTexture,
                    Img = texture3 as TextureImage,
                    Location= 2
                }},
                { "Texture4", new TextureInfo {
                    Texture = texture4 as WhTexture,
                    Img = texture4 as TextureImage,
                    Location= 3
                }}
            };

            foreach (var tex in textures)
            {
                var td = tex.Value;
                td.Uniform = "u" + tex.Key;
                td.Unit = "TEXTURE" + td.Location;
            }

            return textures;
        }

        public class TextureInfo
        {
            // Всегда будет что-то одно либо Img, либо Texture, оба одновременно заданными быть не могут

            public WhTexture Texture { get; set; }
            public TextureImage Img { get; set; }

            public int Location { get; set; }
            public string Uniform { get; set; }
            public string Unit { get; set; }
        }
    }
}
