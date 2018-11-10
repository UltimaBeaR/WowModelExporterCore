﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowObjectBuilder
    {
        public WowObject BuildFromCharacterWhModel(WhModel whCharacterModel)
        {
            // ToDo: Смотреть как работает, Wow.Model.draw,
            // по сути в зависимости от алгоритма отрисовки идет и алгоритм построения объектов для дальнейшей отрисовки (то что тут происодит)

            // Сам перс

            var characterObject = new WowObject()
            {
                Children = new List<WowObject>()
            };

            characterObject.Mesh = MakeMeshFromWhModel(whCharacterModel);

            // Рога

            if (whCharacterModel.HornsModel != null)
            {
                var hornsObject = new WowObject()
                {
                    Parent = characterObject,
                    Children = new List<WowObject>()
                };

                hornsObject.Mesh = MakeMeshFromWhModel(whCharacterModel.HornsModel);

                characterObject.Children.Add(hornsObject);
            }

            // Маунт

            if (whCharacterModel.Mount != null)
            {
                var mountObject = new WowObject()
                {
                    Parent = characterObject,
                    Children = new List<WowObject>()
                };

                mountObject.Mesh = MakeMeshFromWhModel(whCharacterModel.Mount);

                characterObject.Children.Add(mountObject);
            }

            // Итемы

            foreach (var whItemInSlot in whCharacterModel.Items)
            {
                var whItem = whItemInSlot.Value;

                if (whItem?.Models == null)
                    continue;

                foreach (var whItemModel in whItem.Models)
                {
                    if (whItemModel?.Model == null)
                        continue;

                    var itemPosition = ConvertPositionFromWh(whItemModel.Attachment.Position);

                    var itemObject = new WowObject()
                    {
                        Parent = characterObject,
                        Children = new List<WowObject>(),
                        Position = itemPosition
                    };

                    itemObject.Mesh = MakeMeshFromWhModel(whItemModel.Model);

                    characterObject.Children.Add(itemObject);

                    if (whItem.Visual?.Models != null && whItemModel.Model.Loaded)
                    {
                        foreach (var visual in whItem.Visual.Models)
                        {
                            if (visual != null)
                            {
                                var visualPosition = ConvertPositionFromWh(visual.Attachment.Position);

                                var visualObject = new WowObject()
                                {
                                    Parent = characterObject,
                                    Children = new List<WowObject>(),
                                    Position = visualPosition
                                };

                                visualObject.Mesh = MakeMeshFromWhModel(visual.Model);

                                characterObject.Children.Add(visualObject);
                            }
                        }
                    }
                }
            }

            return characterObject;
        }

        private WowMeshWithMaterials MakeMeshFromWhModel(WhModel whModel)
        {
            var mesh = new WowMeshWithMaterials();

            mesh.Vertices = whModel.Vertices.Select(x => MakeVertexFromWhVertex(x)).ToArray();

            mesh.Submeshes = new List<WowSubmeshWithMaterials>();

            foreach (var whTexUnit in whModel.SortedTexUnits)
            {
                if (!whTexUnit.Show)
                    continue;

                var triangles = whModel.Indices
                    .Skip(whTexUnit.Mesh.IndexStart)
                    .Take(whTexUnit.Mesh.IndexCount)
                    .ToArray();

                var material = MakeMaterialFromWhTexUnit(whTexUnit);

                var submesh = new WowSubmeshWithMaterials(mesh, triangles, material);

                mesh.Submeshes.Add(submesh);
            }

            return mesh;
        }

        private WowMaterial MakeMaterialFromWhTexUnit(WhTexUnit whTexUnit)
        {
            var material = new WowMaterial();

            var whTextures = whTexUnit.GetTextures();

            var images = new Bitmap[]
            {
                GetImageFromWhTextureInfo(whTextures["Texture1"]),
                GetImageFromWhTextureInfo(whTextures["Texture2"]),
                GetImageFromWhTextureInfo(whTextures["Texture3"]),
                GetImageFromWhTextureInfo(whTextures["Texture4"])
            };

            material.MainImage = images.FirstOrDefault(x => x != null);

            material.Image1 = images[0];
            material.Image2 = images[1];
            material.Image3 = images[2];
            material.Image4 = images[3];

            return material;
        }

        private Bitmap GetImageFromWhTextureInfo(WhTexUnit.TextureInfo whTextureInfo)
        {
            if (whTextureInfo.Img != null)
                return whTextureInfo.Img;

            if (whTextureInfo.Texture?.Img != null)
                return whTextureInfo.Texture.Img;

            return null;
        }

        private WowVertex MakeVertexFromWhVertex(WhVertex whVertex)
        {
            return new WowVertex()
            {
                Position = ConvertPositionFromWh(whVertex.Position),
                Normal = whVertex.Normal,
                UV1 = ConvertUVFromWh(whVertex.U, whVertex.V),
                UV2 = ConvertUVFromWh(whVertex.U2, whVertex.V2),
                Weights = whVertex.Weights,
                Bones = whVertex.Bones
            };
        }

        private Vec3 ConvertPositionFromWh(Vec3 position)
        {
            return new Vec3(position.X, position.Z, -position.Y);
        }

        private Vec2 ConvertUVFromWh(float u, float v)
        {
            return new Vec2(u, 1f - v);
        }
    }
}