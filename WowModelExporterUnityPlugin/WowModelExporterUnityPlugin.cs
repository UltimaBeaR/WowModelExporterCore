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
        // ToDo: основная модель - перс, у него внутренние модели - итемы.
        // у каждой модели есть несколько texunits (брать только те у которых show = true)
        // по сути это материалы из юнити, а модели это игровые объекты(либо меши).
        // То есть можно на каждую модель создать объект с мешем, внутри этого меша сделать сабмеши по используемым в show(видимых) texunits.
        // то есть создаем материалы на основе [show] texunits (текстуры исопльзуемые в материале получаются через texunit.gettextures()) и из них же вытаскиваем сабмеши.
        // далее на каждую модель создаем игровой объект с мешем, пихаем в сабмеши полученные сабмеши с материалом. при этом сабмеши видимо могут шарится, там надо будет
        // перестраивать меш(вертексы и индексы), так чтобы удалить неиспользуемые куски. то есть должны остаться только те вершины и индексы,
        // которые используются в видимых texunits (они определяют границы исползуемых индексов, которые определяют используемые вершины)



        public WowModelExporterUnityPlugin()
        {
            _exporter = new WowModelExporter();
        }

        private Texture2D CreateTextureFromModel(WhModel model)
        {
            return CreateTextureFromBitmap(_exporter.GetFirstTexture(model));
        }

        public void DoStuff()
        {
            var model = _exporter.LoadModel("humanmale", new string[] {
                // шлем
                "161600",

                // плечи
                "161621",

                // плащ
                "163365",

                // чест
                "161602",

                // брасы
                "161629",

                // руки
                "161610",

                // пояс
                "161624",

                // ноги
                "161616",

                // ступни
                "161605"
            });

            var texture = CreateTextureFromModel(model);

            var mesh = CreateMesh(model);

            CreateGameObject("character", mesh, texture);

            foreach (var item in model.Items)
            {
                if (item.Value.Models == null || !item.Value.Models.Any())
                    continue;

                var itemModel = item.Value.Models.First().Model;
                var itemMesh = CreateMesh(itemModel);

                var unityItemTexture = CreateTextureFromModel(itemModel);
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
                // Smoothness
                material.SetFloat("_Glossiness", 0);

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
