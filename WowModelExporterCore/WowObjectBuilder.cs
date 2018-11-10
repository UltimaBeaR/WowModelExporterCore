using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowObjectBuilder
    {
        public WowObject BuildFromCharacterWhModel(WhModel whCharacterModel)
        {
            // Сам перс

            var characterObject = new WowObject()
            {
                Children = new List<WowObject>()
            };

            characterObject.Mesh = MakeMeshFromWhModel(whCharacterModel);

            // Маунт

            // ToDo: незнаю проверять ли флаг IsMount
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

            foreach (var whItem in whCharacterModel.Items)
            {
                if (whItem.Value.Models == null)
                    continue;

                foreach (var whItemModel in whItem.Value.Models)
                {
                    if (whItemModel == null)
                        continue;

                    var itemObject = new WowObject()
                    {
                        Parent = characterObject,
                        Children = new List<WowObject>()
                    };

                    itemObject.Mesh = MakeMeshFromWhModel(whItemModel.Model);

                    characterObject.Children.Add(itemObject);
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
                Position = new Vec3(whVertex.Position.X, whVertex.Position.Z, -whVertex.Position.Y),
                Normal = whVertex.Normal,
                UV1 = new Vec2(whVertex.U, 1f - whVertex.V),
                UV2 = new Vec2(whVertex.U2, 1f - whVertex.V2),
                Weights = whVertex.Weights,
                Bones = whVertex.Bones
            };
        }
    }
}
