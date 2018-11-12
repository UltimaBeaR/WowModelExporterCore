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

        private Dictionary<string, string> _normalToHumanoidBoneNamesMapping = new Dictionary<string, string>()
        {
            { "ROOT", "ROOT" },

            { "body", "Hips" },

            { "leg_upper_l", "LeftUpperLeg" },
            { "leg_upper_r", "RightUpperLeg" },
            { "leg_lower_l", "LeftLowerLeg" },
            { "leg_lower_r", "RightLowerLeg" },
            { "foot_l", "LeftFoot" },
            { "foot_r", "RightFoot" },

            { "spine", "Spine" },
            { "chest", "Chest" },
            { "neck_lower", "Neck" },
            { "head", "Head" },

            { "shoulder_l", "LeftShoulder" },
            { "shoulder_r", "RightShoulder" },
            { "arm_upper_l", "LeftUpperArm" },
            { "arm_upper_r", "RightUpperArm" },
            { "arm_lower_l", "LeftLowerArm" },
            { "arm_lower_r", "RightLowerArm" },
            { "hand_l", "LeftHand" },
            { "hand_r", "RightHand" },

            { "toe_l", "LeftToes" },
            { "toe_r", "RightToes" },

            { "eye_l", "LeftEye" },
            { "eye_r", "RightEye" },
            { "jaw", "Jaw" },

            { "finger_thumb_1_l", "Left Thumb Proximal" },
            { "finger_thumb_2_l", "Left Thumb Intermediate" },
            //{ "", "Left Thumb Distal" },

            { "finger_index_1_l", "Left Index Proximal" },
            { "finger_index_2_l", "Left Index Intermediate" },
            //{ "", "Left Index Distal" },

            { "finger_middle_1_l", "Left Middle Proximal" },
            { "finger_middle_2_l", "Left Middle Intermediate" },
            //{ "", "Left Middle Distal" },

            { "finger_ring_1_l", "Left Ring Proximal" },
            { "finger_ring_2_l", "Left Ring Intermediate" },
            //{ "", "Left Ring Distal" },

            { "finger_pinky_1_l", "Left Little Proximal" },
            { "finger_pinky_2_l", "Left Little Intermediate" },
            //{ "", "Left Little Distal" },

            { "finger_thumb_1_r", "Right Thumb Proximal" },
            { "finger_thumb_2_r", "Right Thumb Intermediate" },
            //{ "", "Right Thumb Distal" },

            { "finger_index_1_r", "Right Index Proximal" },
            { "finger_index_2_r", "Right Index Intermediate" },
            //{ "", "Right Index Distal" },

            { "finger_middle_1_r", "Right Middle Proximal" },
            { "finger_middle_2_r", "Right Middle Intermediate" },
            //{ "", "Right Middle Distal" },

            { "finger_ring_1_r", "Right Ring Proximal" },
            { "finger_ring_2_r", "Right Ring Intermediate" },
            //{ "", "Right Ring Distal" },

            { "finger_pinky_1_r", "Right Little Proximal" },
            { "finger_pinky_2_r", "Right Little Intermediate" },
            //{ "", "Right Little Distal" },

            //{ "", "UpperChest" },
        };

        public void CreateCharacterGameObjects(WhRace race, WhGender gender, string[] items)
        {
            var characterWowObject = _exporter.LoadCharacter(race, gender, items);

            characterWowObject.RemoveBones(_normalToHumanoidBoneNamesMapping.Keys.ToArray());

            var containerGo = new GameObject(race.ToString() + " " + gender.ToString());
            containerGo.transform.position = Vector3.zero;

            var characterGo = CreateGameObjectForCharacterWowObject(
                "character items: " + (items?.Count().ToString() ?? "0"),
                containerGo.transform,
                characterWowObject, out var rootBoneGo);

            foreach (var transform in GetSelfAndChildren(rootBoneGo.transform))
            {
                if (_normalToHumanoidBoneNamesMapping.TryGetValue(transform.gameObject.name, out var humanoidBoneName))
                    transform.gameObject.name = humanoidBoneName;
            }

            MakeAvatar(characterGo);

            foreach (var childWowObject in characterWowObject.Children)
                CreateGameObjectForWowObject("detail", characterGo.transform, childWowObject);
        }

        public IEnumerable<Transform> GetSelfAndChildren(Transform transform)
        {
            yield return transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                foreach (var deeperTransform in GetSelfAndChildren(transform.GetChild(i)))
                    yield return deeperTransform;
            }

            yield break;
        }

        private GameObject CreateGameObjectForWowObject(string name, Transform parent, WowObject wowObject)
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

        private GameObject CreateGameObjectForCharacterWowObject(string name, Transform parent, WowObject characterWowObject, out GameObject rootBoneGo)
        {
            var mesh = CreateMeshFromWowMeshWithMaterials(characterWowObject.Mesh);
            var materials = CreateMaterialsFromWowMeshWithMaterials(characterWowObject.Mesh);

            var go = new GameObject(name);
            go.transform.position = new Vector3(characterWowObject.Position.X, characterWowObject.Position.Y, characterWowObject.Position.Z);
            go.transform.parent = parent;

            var skinnedMeshRenderer = go.AddComponent<SkinnedMeshRenderer>();

            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.materials = materials;

            rootBoneGo = CreateSkeletonForWowObject(parent, characterWowObject, out var boneTransforms);

            ApplyMeshBindposesFromBoneHierarchy(mesh, boneTransforms, rootBoneGo.transform);

            skinnedMeshRenderer.bones = boneTransforms;

            return go;
        }

        private void MakeAvatar(GameObject go)
        {
            //var humanDescription = new HumanDescription()
            //{
            //    human = new HumanBone[]
            //    {
            //        new HumanBone() { boneName = "bone body", humanName = "Hips" },
            //    }
            //};

            //var avatar = AvatarBuilder.BuildHumanAvatar(go, humanDescription);
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
            
            boneGo.transform.position = new Vector3(wowBone.LocalPosition.X, wowBone.LocalPosition.Y, wowBone.LocalPosition.Z);
            boneGo.transform.parent = parent;

            return boneGo;
        }

        private Mesh CreateMeshFromWowMeshWithMaterials(WowMeshWithMaterials wowMesh)
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

            mesh.SetUVs(0, uv1);
            mesh.SetUVs(1, uv2);

            mesh.boneWeights = boneWeights;

            mesh.subMeshCount = wowMesh.Submeshes.Count;
            for (int submeshIdx = 0; submeshIdx < wowMesh.Submeshes.Count; submeshIdx++)
                mesh.SetTriangles(wowMesh.Submeshes[submeshIdx].Triangles.Select(x => (int)x).ToList(), submeshIdx);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

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

                    return boneTransform.worldToLocalMatrix * rootTransorm.localToWorldMatrix;
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
