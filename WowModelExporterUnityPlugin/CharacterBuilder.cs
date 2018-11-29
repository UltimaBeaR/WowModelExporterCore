using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using WowheadModelLoader;
using WowModelExporterCore;

namespace WowModelExporterUnityPlugin
{
    public class CharacterBuilder
    {
        public CharacterBuilder()
        {
            _exporter = new WowModelExporter();
        }

        public void Build(WhRace race, WhGender gender)
        {
            var characterWowObject = _exporter.LoadCharacter(race, gender, null);
            CreateCharacterGameObjects(characterWowObject, race.ToString(), gender.ToString());
        }

        private void CreateCharacterGameObjects(WowObject characterWowObject, string race, string gender)
        {
            //PrepareForVRChatUtility.PrepareObject(characterWowObject, true, true);

            var containerGo = new GameObject((race ?? "unknown_race") + " " + (gender ?? "unknown_gender"));
            containerGo.transform.position = Vector3.zero;

            var characterGo = CreateGameObjectForCharacterWowObject("character", containerGo.transform, characterWowObject, out var rootBoneGo);
        }

        private GameObject CreateGameObjectForCharacterWowObject(string name, Transform parent, WowObject characterWowObject, out GameObject rootBoneGo)
        {
            var mesh = CreateMeshFromWowMeshWithMaterials(characterWowObject.Mesh);
            var materials = CreateMaterialsFromWowMeshWithMaterials(characterWowObject.Mesh);

            var go = new GameObject(name);
            go.transform.position = new Vector3(characterWowObject.GlobalPosition.X, characterWowObject.GlobalPosition.Y, characterWowObject.GlobalPosition.Z);
            go.transform.parent = parent;

            var skinnedMeshRenderer = go.AddComponent<SkinnedMeshRenderer>();

            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.materials = materials;

            rootBoneGo = CreateSkeletonForWowObject(parent, characterWowObject, out var boneTransforms);

            ApplyMeshBindposesFromBoneHierarchy(mesh, boneTransforms, rootBoneGo.transform);

            skinnedMeshRenderer.bones = boneTransforms;

            return go;
        }

        private GameObject CreateSkeletonForWowObject(Transform parent, WowObject wowObject, out Transform[] boneTransforms)
        {
            var wowRootBone = wowObject.GetRootBone();

            if (wowRootBone == null)
            {
                boneTransforms = new Transform[0];
                return null;
            }

            boneTransforms = new Transform[wowObject.Bones.Length];
            return CreateSkeletonElementsForWowBoneAndItsChildren(parent, wowRootBone, boneTransforms);
        }

        private GameObject CreateSkeletonElementsForWowBoneAndItsChildren(Transform parent, WowBone wowBone, Transform[] boneTransformsToFill)
        {
            var boneGo = CreateSkeletonElementsForWowBone(parent, wowBone);
            boneTransformsToFill[wowBone.Index] = boneGo.transform;

            foreach (var childWowBone in wowBone.ChildBones)
                CreateSkeletonElementsForWowBoneAndItsChildren(boneGo.transform, childWowBone, boneTransformsToFill);

            return boneGo;
        }

        private GameObject CreateSkeletonElementsForWowBone(Transform parent, WowBone wowBone)
        {
            var boneGo = new GameObject(wowBone.GetName() ?? ("bone " + wowBone.Id));

            boneGo.transform.parent = parent;
            boneGo.transform.localPosition = new Vector3(wowBone.LocalPosition.X, wowBone.LocalPosition.Y, wowBone.LocalPosition.Z);

            return boneGo;
        }

        private Mesh CreateMeshFromWowMeshWithMaterials(WowMeshWithMaterials wowMesh)
        {
            var mesh = new Mesh();

            var vertices = wowMesh.Vertices
                .Select(x => new Vector3(x.Position.X, x.Position.Y, x.Position.Z))
                .ToList();

            var normals = wowMesh.Vertices
                .Select(x => new Vector3(x.Normal.X, x.Normal.Y, x.Normal.Z))
                .ToList();

            var uv1 = wowMesh.Vertices
                .Select(x => new Vector2(x.UV1.X, x.UV1.Y))
                .ToList();

            var uv2 = wowMesh.Vertices
                .Select(x => new Vector2(x.UV2.X, x.UV2.Y))
                .ToList();

            var boneWeights = wowMesh.Vertices
                .Select(x => new BoneWeight()
                {
                    boneIndex0 = x.BoneIndexes[0],
                    weight0 = x.BoneWeights[0],

                    boneIndex1 = x.BoneIndexes[1],
                    weight1 = x.BoneWeights[1],

                    boneIndex2 = x.BoneIndexes[2],
                    weight2 = x.BoneWeights[2],

                    boneIndex3 = x.BoneIndexes[3],
                    weight3 = x.BoneWeights[3],
                })
                .ToArray();

            mesh.SetVertices(vertices);

            mesh.SetNormals(normals);

            mesh.SetUVs(0, uv1);
            mesh.SetUVs(1, uv2);

            mesh.boneWeights = boneWeights;

            mesh.subMeshCount = wowMesh.Submeshes.Count;
            for (int submeshIdx = 0; submeshIdx < wowMesh.Submeshes.Count; submeshIdx++)
                mesh.SetTriangles(wowMesh.Submeshes[submeshIdx].Triangles.Select(x => (int)x).ToList(), submeshIdx);

            mesh.RecalculateBounds();
            //mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Записывает <see cref="Mesh.bindposes"/> от текущего расположения костей в просранстве. Кости передаются в порядке следования индексов
        /// </summary>
        private void ApplyMeshBindposesFromBoneHierarchy(Mesh mesh, Transform[] boneTransforms, Transform rootTransorm)
        {
            mesh.bindposes = boneTransforms
                .Select(boneTransform =>
                {
                    if (boneTransform == null)
                        return Matrix4x4.identity;

                    // Хз че тут не так, но если так делать и рутовая кость не в 0ле, то глючит
                    // return boneTransform.worldToLocalMatrix * rootTransorm.localToWorldMatrix;

                    return boneTransform.worldToLocalMatrix;
                })
                .ToArray();
        }

        private Material[] CreateMaterialsFromWowMeshWithMaterials(WowMeshWithMaterials wowMesh)
        {
            var materials = new Material[wowMesh.Submeshes.Count];

            for (int submeshIdx = 0; submeshIdx < wowMesh.Submeshes.Count; submeshIdx++)
            {
                var whMaterial = wowMesh.Submeshes[submeshIdx].Material;

                var material = new Material(Shader.Find("Standard"));
                
                // Smoothness
                material.SetFloat("_Glossiness", 0);

                material.SetTexture("_MainTex", CreateTextureFromBitmap(whMaterial.MainImage.Bitmap));

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
