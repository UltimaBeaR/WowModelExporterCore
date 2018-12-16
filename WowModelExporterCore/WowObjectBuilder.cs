using System;
using System.Collections.Generic;
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

            var characterObject = new WowObject();

            // Добавляем MainMesh
            characterObject.Meshes.Add(MakeMeshFromWhModel(whCharacterModel));

            characterObject.Bones = MakeBoneHierarchyFromWhModel(whCharacterModel);
            characterObject.OptimizeBones();
            TranslateBonePositionsFromGlobalToLocal(characterObject);

            // Рога

            if (whCharacterModel.HornsModel != null)
                AddAdditionalMeshToCharacterObject(characterObject, whCharacterModel.HornsModel);

            // Маунт

            if (whCharacterModel.Mount != null)
            {
                // ToDo: хз че тут  делать, создать его вообще отдельно?

                //var mountObject = new WowObject()
                //{
                //    Parent = characterObject
                //};

                //// MainMesh (не факт что я вообще тут правильно делаю, т.к. еще не тестил)
                //mountObject.Meshes.Add(MakeMeshFromWhModel(whCharacterModel.Mount));

                //characterObject.Children.Add(mountObject);
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

                    if (whItemModel.Bone > -1 && whItemModel.Bone < whCharacterModel.Bones.Length)
                    {
                        var itemPosition = Vec3.ConvertPositionFromWh(whItemModel.Attachment.Position);

                        var itemObject = new WowObject()
                        {
                            Parent = characterObject,
                            GlobalPosition = itemPosition,
                        };

                        // MainMesh - по сути он использоваться будет тлолько в AttachedWowObjects кости
                        itemObject.Meshes.Add(MakeMeshFromWhModel(whItemModel.Model));

                        // Если у итема есть кость, к которой можно прикрепиться
                        if (itemObject.Parent != null && whItemModel.Attachment.Bone >= 0)
                        {
                            var parentAttachmentBone = itemObject.Parent.Bones[whItemModel.Attachment.Bone];
                            var whParentAttachmentBone = whItemModel.Model.Parent.Bones[whItemModel.Attachment.Bone];

                            // записываем объект этого итема в кость, к которой крепимся(у родительского объекта)
                            parentAttachmentBone.AttachedWowObjects.Add(itemObject);

                            // меняем данные о костях в вершинах так итема так, чтобы при привязке к скелету родителя этот итем двигался вместе с костью
                            // (при этом в иерархии объектов он не будет в ноде кости, а будет на том же уровне что и родительский объект, к скелету которого мы привязываем итем)

                            itemObject.MainMesh.ApplyTransform(
                                whParentAttachmentBone.LastUpdatedTranslation,
                                whParentAttachmentBone.LastUpdatedRotation,
                                whParentAttachmentBone.LastUpdatedScale);

                            var eachVertexItemBoneIndexes = new ByteVec4((byte)parentAttachmentBone.Index, 0, 0, 0);
                            var eachVertexItemBoneWeights = new Vec4(1, 0, 0, 0);

                            foreach (var vertex in itemObject.MainMesh.Vertices)
                            {
                                vertex.BoneIndexes = eachVertexItemBoneIndexes;
                                vertex.BoneWeights = eachVertexItemBoneWeights;
                            }
                        }

                        if (whItem.Visual?.Models != null && whItemModel.Model.Loaded)
                        {
                            foreach (var visual in whItem.Visual.Models)
                            {
                                if (visual != null)
                                {
                                    var visualPosition = Vec3.ConvertPositionFromWh(visual.Attachment.Position);

                                    var visualObject = new WowObject()
                                    {
                                        Parent = characterObject,
                                        GlobalPosition = visualPosition
                                    };

                                    // MainMesh (не факт что я вообще тут правильно делаю, т.к. еще не тестил)
                                    visualObject.Meshes.Add(MakeMeshFromWhModel(visual.Model));
                                }
                            }
                        }
                    }
                    else if (whItemModel.Bone == -1)
                        AddAdditionalMeshToCharacterObject(characterObject, whItemModel.Model);
                }
            }

            foreach (var collection in whCharacterModel.CollectionModels.Values)
            {
                if (collection == null)
                    continue;

                AddAdditionalMeshToCharacterObject(characterObject, collection);
            }

            return characterObject;
        }

        private void AddAdditionalMeshToCharacterObject(WowObject characterObject, WhModel whModel)
        {
            var itemObject = new WowObject()
            {
                Parent = characterObject,
                GlobalPosition = new Vec3(0, 0, 0),
            };

            // На этот раз добавляем не в MainMesh а делаем дополнительный меш, таким образом этот меш будет исползовать те же кости что и основной
            characterObject.Meshes.Add(MakeMeshFromWhModel(whModel));
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

                var triangles = ConvertTrianglesFromWh(whModel.Indices
                    .Skip(whTexUnit.Mesh.IndexStart)
                    .Take(whTexUnit.Mesh.IndexCount)
                    .ToArray());

                var material = MakeMaterialFromWhTexUnit(whTexUnit);

                var submesh = new WowSubmeshWithMaterials(mesh, triangles, material);

                mesh.Submeshes.Add(submesh);
            }

            mesh.RemoveUnusedVertices();

            return mesh;
        }

        private WowBone[] MakeBoneHierarchyFromWhModel(WhModel whModel)
        {
            if (whModel.Bones == null || whModel.Bones.Length == 0)
                return null;

            // Формируем начальный массив всех костей и задаем абсолютные позиции для всех костей (прописываем их временно в локальную позицию)
            var wowBones = whModel.Bones
                .Select(x => new WowBone()
                {
                    Index = Convert.ToByte(x.Index),
                    Id = x.Id,
                    LocalPosition = Vec3.ConvertPositionFromWh(x.Pivot)
                })
                .ToArray();

            // Прописываем иерархию родители/дети для всех костей

            for (int boneIdx = 0; boneIdx < wowBones.Length; boneIdx++)
            {
                var wowBone = wowBones[boneIdx];
                var whBone = whModel.Bones[boneIdx];

                // Если у текущей кости есть родительская кость
                if (whBone.Parent >= 0)
                {
                    wowBone.ParentBone = wowBones[whBone.Parent];
                    wowBone.ParentBone.ChildBones.Add(wowBone);
                }
            }

            return wowBones;
        }

        private void TranslateBonePositionsFromGlobalToLocal(WowObject wowObject)
        {
            var rootBone = wowObject.GetRootBone();
            TranslateBoneChildrenPositionsFromGlobalToLocal(rootBone);
        }

        private void TranslateBoneChildrenPositionsFromGlobalToLocal(WowBone wowBone)
        {
            if (wowBone == null)
                return;

            foreach (var childBone in wowBone.ChildBones)
            {
                TranslateBoneChildrenPositionsFromGlobalToLocal(childBone);

                childBone.LocalPosition = new Vec3(
                    childBone.LocalPosition.X - wowBone.LocalPosition.X,
                    childBone.LocalPosition.Y - wowBone.LocalPosition.Y,
                    childBone.LocalPosition.Z - wowBone.LocalPosition.Z);
            }
        }

        private WowMaterial MakeMaterialFromWhTexUnit(WhTexUnit whTexUnit)
        {
            var material = new WowMaterial()
            {
                BothSides = !whTexUnit.Cull,
                Type = GetMaterialTypeFromBlendFlag(whTexUnit.RenderFlag.Blend)
            };

            var whTextures = whTexUnit.GetTextures();

            var images = new TextureImage[]
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

        private static WowMaterial.MaterialType GetMaterialTypeFromBlendFlag(ushort renderFlagBlend)
        {
            // ToDo: посмотрел по примерам как че отображается. Вообще есть в TexUnit setBlendMode() который ставит режим наложения в зависимости от этого флага.
            // так же этот флаг передается в шейдер в атрибутах

            switch (renderFlagBlend)
            {
                default:
                case 0: return WowMaterial.MaterialType.Opaque;
                case 1: return WowMaterial.MaterialType.Cutout;
                case 2: return WowMaterial.MaterialType.Transparent;
            }
        }

        private TextureImage GetImageFromWhTextureInfo(WhTexUnit.TextureInfo whTextureInfo)
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
                WhPosition = whVertex.Position,

                WhNormal = new Vec3(whVertex.Normal.X, whVertex.Normal.Y, whVertex.Normal.Z),

                UV1 = ConvertUVFromWh(whVertex.U, whVertex.V),
                UV2 = ConvertUVFromWh(whVertex.U2, whVertex.V2),

                BoneIndexes = whVertex.Bones,

                BoneWeights = new Vec4(
                    ConvertBoneWeightFromWh(whVertex.Weights[0]),
                    ConvertBoneWeightFromWh(whVertex.Weights[1]),
                    ConvertBoneWeightFromWh(whVertex.Weights[2]),
                    ConvertBoneWeightFromWh(whVertex.Weights[3])
                )
            };
        }

        public ushort[] ConvertTrianglesFromWh(ushort[] triangles)
        {
            // меняем порядок отрисовки индексов, из-за того что ставим негативный X в вершинах

            var newTriangles = new ushort[triangles.Length];

            for (int i = 0; i < triangles.Length; i += 3)
            {
                newTriangles[i] = triangles[i];
                newTriangles[i + 1] = triangles[i + 2];
                newTriangles[i + 2] = triangles[i + 1];
            }

            return newTriangles;
        }

        private Vec2 ConvertUVFromWh(float u, float v)
        {
            return new Vec2(u, 1f - v);
        }

        private float ConvertBoneWeightFromWh(byte boneWeight)
        {
            return boneWeight / 255f;
        }
    }
}
