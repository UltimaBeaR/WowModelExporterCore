using System.Collections.Generic;
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
        public object RenderFlag { get; set; }
        public List<WhMaterial> Material { get; set; }
        public List<object> TextureAnim { get; set; }
        //textureMatrix
        public object Color { get; set; }
        public object Alpha { get; set; }
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

            //ZamModelViewer.Wow.RenderFlag.ComputeFlags(self);

            // На момент теста эта штука была undefined, так что думаю можно это не использовать
            //window.MeshLoadFilter && window.MeshLoadFilter(self);

            //var program = ZamModelViewer.Wow.ShaderTool.GetWowProgram(self.shaderId, self.opcount, self.renderFlag);
            //WH.debug(self.shaderId, program);
            //self.program = program;
            //for (var i = 0; i < self.opcount; i++) {
            //    if (self.materialIndex > -1 && self.materialIndex < model.materialLookup.length) {
            //        var matIdx = model.materialLookup[self.materialIndex + i];
            //        if (matIdx > -1 && matIdx < model.materials.length) {
            //            self.material.splice(i, 0, model.materials[matIdx])
            //        }
            //    }
            //    if (self.textureAnimIndex > -1 && self.textureAnimIndex < model.textureAnimLookup.length) {
            //        var animIdx = model.textureAnimLookup[self.textureAnimIndex + i];
            //        if (animIdx > -1 && model.textureAnims && animIdx < model.textureAnims.length) {
            //            self.textureAnim.splice(i, 0, model.textureAnims[animIdx])
            //        } else {
            //            self.textureAnim.splice(i, 0, null)
            //        }
            //    }
            //}
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
    }
}
