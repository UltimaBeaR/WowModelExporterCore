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

        public void CreateCharacterGameObjects(WhRace race, WhGender gender, string[] items)
        {
            var characterWowObject = _exporter.LoadCharacter(race, gender, items);

            var containerGo = new GameObject();
            containerGo.transform.position = Vector3.zero;

            var characterGo = CreateGameObjectForWowObject(
                "character_" + race.ToString() + "_" + gender.ToString() + "_" + (items?.Count().ToString() ?? "0"),
                containerGo.transform,
                characterWowObject);

            foreach (var childWowObject in characterWowObject.Children)
                CreateGameObjectForWowObject("item", characterGo.transform, childWowObject);

            CreateVisibleSkeletonForWowObject(containerGo.transform, characterWowObject);
        }

        public GameObject CreateGameObjectForWowObject(string name, Transform parent, WowObject wowObject)
        {
            var mesh = CreateMeshFromWowMeshWithMaterials(wowObject.Mesh);
            var materials = CreateMaterialsFromWowMeshWithMaterials(wowObject.Mesh);

            var go = new GameObject(name);
            go.transform.position = new Vector3(wowObject.Position.X, wowObject.Position.Y, wowObject.Position.Z);
            go.transform.parent = parent;

            var meshFilter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            renderer.materials = materials;

            return go;
        }

        public GameObject CreateVisibleSkeletonForWowObject(Transform parent, WowObject wowObject)
        {
            if (wowObject.Bones == null)
                return null;

            var skeletonRootGo = new GameObject("skeleton");
            skeletonRootGo.transform.position = new Vector3(wowObject.Position.X, wowObject.Position.Y);
            skeletonRootGo.transform.parent = parent;

            foreach (var wowBone in wowObject.Bones)
            {
                var boneGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                const float boneSphereScale = 0.05f;
                boneGo.transform.localScale = new Vector3(boneSphereScale, boneSphereScale, boneSphereScale);
                boneGo.transform.position = new Vector3(wowBone.X, wowBone.Y, wowBone.Z);
                boneGo.transform.parent = skeletonRootGo.transform;
            }

            return skeletonRootGo;
        }

        public Mesh CreateMeshFromWowMeshWithMaterials(WowMeshWithMaterials wowMesh)
        {
            var mesh = new Mesh();

            var vertices = wowMesh.Vertices
                .Select(x => new Vector3(x.Position.X, x.Position.Y, x.Position.Z))
                .ToList();

            var uv1 = wowMesh.Vertices
                .Select(x => new Vector2(x.UV1.X, x.UV1.Y))
                .ToList();

            var uv2 = wowMesh.Vertices
                .Select(x => new Vector2(x.UV2.X, x.UV2.Y))
                .ToList();

            mesh.SetVertices(vertices);

            mesh.SetUVs(0, uv1);
            mesh.SetUVs(1, uv2);

            mesh.subMeshCount = wowMesh.Submeshes.Count;
            for (int submeshIdx = 0; submeshIdx < wowMesh.Submeshes.Count; submeshIdx++)
                mesh.SetTriangles(wowMesh.Submeshes[submeshIdx].Triangles.Select(x => (int)x).ToList(), submeshIdx);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        public Material[] CreateMaterialsFromWowMeshWithMaterials(WowMeshWithMaterials wowMesh)
        {
            var materials = new Material[wowMesh.Submeshes.Count];

            for (int submeshIdx = 0; submeshIdx < wowMesh.Submeshes.Count; submeshIdx++)
            {
                var whMaterial = wowMesh.Submeshes[submeshIdx].Material;

                var material = new Material(Shader.Find("Standard"));
                
                // Smoothness
                material.SetFloat("_Glossiness", 0);

                material.SetTexture("_MainTex", CreateTextureFromBitmap(whMaterial.MainImage));

                materials[submeshIdx] = material;
            }

            return materials;
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

        private WowModelExporter _exporter;
    }
}
