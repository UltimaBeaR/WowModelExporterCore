using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using WowheadModelLoader;
using WowModelExporterCore;

namespace WowModelExporterUnityPlugin
{
    public class WowModelExporterUnityPlugin
    {
        public WowModelExporterUnityPlugin()
        {
            _exporter = new WowModelExporter();
        }

        public void DoStuff()
        {
            var model = _exporter.LoadModel("humanmale", new string[] {
                // шлем
                "161600",

                // плечи
                //"161621"
            });

            var texture = CreateTextureFromBitmap(model.CompositeTexture.Img);

            var mesh = CreateMesh(model);

            CreateGameObject("character", mesh, texture);

            foreach (var item in model.Items)
            {
                var itemModel = item.Value.Models.First().Model;
                var itemTexture = itemModel.Materials[1].Texture;
                var itemMesh = CreateMesh(itemModel);

                var unityItemTexture = CreateTextureFromBitmap(itemTexture.Img);
                CreateGameObject("item", itemMesh, unityItemTexture);
            }
        }

        private GameObject CreateGameObject(string name, Mesh mesh, Texture2D mainTexture)
        {
            var go = new GameObject(name);

            go.transform.position = Vector3.zero;

            var meshFilter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;


            // ToDo: по аналогии с тем как сделано в js варианте TexUnit, надо также получить текстуру по TexUnit-у (= сабмеш в юнити) и сделать для нее материал и поставить на соответствующий
            // сабмеш (ее еще надо видимо будет загрузить, ну скорее всего код для этого есть он просто закомменчен сейчас, в том же месте где идет получение текстуры для кожи)

            renderer.materials = new Material[mesh.subMeshCount];

            foreach (var material in renderer.materials)
            {
                //StandardShaderUtils.ChangeRenderMode(material, StandardShaderUtils.BlendMode.Transparent);
                //material.color = new UnityEngine.Color(Random.value, Random.value, Random.value, 0.5f);
                material.SetTexture("_MainTex", mainTexture);
            }




            return go;
        }

        private Mesh CreateMesh(WhModel model)
        {
            var mesh = new Mesh();

            mesh.SetVertices(model.Vertices
                .Select(x => MakeVector3PositionFromVec3Position(x.Position))
                .ToList());

            mesh.SetUVs(0, model.Vertices
                .Select(x => new Vector2(x.U, 1f - x.V))
                .ToList());
            mesh.SetUVs(1, model.Vertices
                .Select(x => new Vector2(x.U2, 1f - x.V2))
                .ToList());

            var submeshesTriangles = new List<List<int>>();

            foreach (var texUnit in model.TexUnits)
            {
                if (!texUnit.Show)
                    continue;

                var submesh = texUnit.Mesh;

                var triangles = model.Indices
                    .Skip(submesh.IndexStart)
                    .Take(submesh.IndexCount)
                    .Select(x => (int)x)
                    .ToList();

                submeshesTriangles.Add(triangles);
            }

            mesh.subMeshCount = submeshesTriangles.Count;
            for(int submeshIdx = 0; submeshIdx < submeshesTriangles.Count; submeshIdx++)
                mesh.SetTriangles(submeshesTriangles[submeshIdx], submeshIdx);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        private Texture2D CreateTextureFromBitmap(Bitmap bitmap)
        {
            byte[] pngBytes;

            using (var bmpStream = new MemoryStream())
            {
                bitmap.Save(bmpStream, System.Drawing.Imaging.ImageFormat.Png);
                pngBytes = bmpStream.GetBuffer();
            }

            var texture = new Texture2D(bitmap.Width, bitmap.Height);
            texture.LoadImage(pngBytes);

            return texture;
        }

        private Vector3 MakeVector3PositionFromVec3Position(Vec3 position)
        {
            return new Vector3(position.X, position.Z, -position.Y);
        }

        private WowModelExporter _exporter;
    }
}
