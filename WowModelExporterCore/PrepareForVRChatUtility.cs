using System;
using System.Collections.Generic;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    // ToDo: потом наверно перенести в отдельную библиотеку с зависимостью на core

    /// <summary>
    /// Подготавливает модель персонажа из wow для нормальной работы в vrchat (предполагается дальнейший экспорт в fbx)
    /// </summary>
    public static class PrepareForVRChatUtility
    {
        public static string PrepareObject(WowObject wowObject, List<BlendShapeUtility.BakedBlendshape> bakedBlendshapes, float scale, bool removeToes, bool removeJaw, bool addDummyEyesAndEyelidVisemes, bool fixBlendshapes, bool fixShoulders)
        {
            var warnings = "";

            MakeUnityCompatibleHumanoidSkeleton(wowObject);

            // Если не заданы необходимые для движения глаз виземы моргания, создаем пустые (чтобы глаза двигались, т.к. врчат не делает движение глаз при отсутствии этих визем)
            if (addDummyEyesAndEyelidVisemes)
            {
                // Если костей глаз не найдено - добавляем фейковые внутрь кости головы

                if (wowObject.FindBoneByName("LeftEye") == null)
                    wowObject.AddDummyBone("LeftEye", wowObject.FindBoneByName("Head") ?? wowObject.FindBoneByName("Hips"), new WowheadModelLoader.Vec3(-0.05f, 0.1f, 0.07f));
                if (wowObject.FindBoneByName("RightEye") == null)
                    wowObject.AddDummyBone("RightEye", wowObject.FindBoneByName("Head") ?? wowObject.FindBoneByName("Hips"), new WowheadModelLoader.Vec3(0.05f, 0.1f, 0.07f));

                // добавляем в блендшейп одну вершину. Если в блендшейпе вообще не будет изменений - eye tracking все равно не будет работать, как будто блендшейпа не существует
                var pos = wowObject.MainMesh.Vertices[0].Position;
                var normal = wowObject.MainMesh.Vertices[0].Normal;

                if (bakedBlendshapes.FindIndex(x => x.BlendshapeName == "vrc.blink_left") == -1)
                    bakedBlendshapes.Add(new BlendShapeUtility.BakedBlendshape { BlendshapeName = "vrc.blink_left", Changes = new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } } });
                if (bakedBlendshapes.FindIndex(x => x.BlendshapeName == "vrc.blink_right") == -1)
                    bakedBlendshapes.Add(new BlendShapeUtility.BakedBlendshape { BlendshapeName = "vrc.blink_right", Changes = new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } } });
                if (bakedBlendshapes.FindIndex(x => x.BlendshapeName == "vrc.lowerlid_left") == -1)
                    bakedBlendshapes.Add(new BlendShapeUtility.BakedBlendshape { BlendshapeName = "vrc.lowerlid_left", Changes = new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } } });
                if (bakedBlendshapes.FindIndex(x => x.BlendshapeName == "vrc.lowerlid_right") == -1)
                    bakedBlendshapes.Add(new BlendShapeUtility.BakedBlendshape { BlendshapeName = "vrc.lowerlid_right", Changes = new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } } });
            }

            var noBonesWarnings = CheckRequiredBonesExist(wowObject);
            if (noBonesWarnings != null)
                warnings += noBonesWarnings;

            // Удаляем пальцы ног если надо (чтоб не глючило в вр режиме в врчате)
            if (removeToes)
                wowObject.RemoveBonesByNames(new[] { "LeftToes", "RightToes" });

            // Удаляем нижнюю челюсть, если надо (чтобы vrchat не делал автоматическую анимацию речи через эту кость - оно работает глючно)
            if (removeJaw)
                wowObject.RemoveBonesByNames(new[] { "Jaw" });

            if (fixShoulders)
            {
                const float shoulderRotationAngle = (float)(Math.PI / 4);
                RotateShoulderAttachments(wowObject, "attachment_shoulder.L", "LeftUpperArm", -shoulderRotationAngle);
                RotateShoulderAttachments(wowObject, "attachment_shoulder.R", "RightUpperArm", shoulderRotationAngle);
            }

            wowObject.OptimizeBones();

            // Добавляем небольшое смещение к каждой из позиций вершин, чтобы избавиться от каких-то глюков (repair_shapekeys и repair_shapekeys_mouth функции в cats плагине)
            // как минимум - если пустой blendshape например на vrc.lowerlid_left ничем не отличается от базы - eye tracking работать не будет. При этом очень мелкое значение тоже не влияет, нужно именно определенное
            if (fixBlendshapes)
            {
                var rand = new Random(34523634);

                foreach (var bakedBlendshape in bakedBlendshapes)
                {
                    if (bakedBlendshape.BlendshapeName.StartsWith("vrc."))
                    {
                        foreach (var vertex in bakedBlendshape.Changes)
                        {
                            var tinyChange = 0.00007f * (rand.NextDouble() < 0.5 ? -1f : 1f);

                            vertex.Value.Position = new WowheadModelLoader.Vec3(vertex.Value.Position.X + tinyChange, vertex.Value.Position.Y + tinyChange, vertex.Value.Position.Z + tinyChange);
                        }
                    }
                }
            }

            // Сортируем блендшейпы так чтобы _firstBlendshapes (список имен блендшейпов) шел сначала а потом уже все остальные
            bakedBlendshapes.Sort((x, y) =>
            {
                var xIndex = _firstBlendshapes.IndexOf(x.BlendshapeName);
                var yIndex = _firstBlendshapes.IndexOf(y.BlendshapeName);

                if (xIndex != -1 && yIndex == -1)
                    return -1;

                if (xIndex == -1 && yIndex != -1)
                    return 1;

                if (xIndex != -1 && yIndex != -1)
                    return xIndex.CompareTo(yIndex);

                return x.BlendshapeName.CompareTo(y.BlendshapeName);
            });

            if (warnings == "")
                warnings = null;

            return warnings;
        }

        private static void RotateShoulderAttachments(WowObject characterWowObject, string attachmentBoneName, string upperArmBoneName, float rotationAngle)
        {
            var shoulderAttachment = characterWowObject.FindBoneByName(attachmentBoneName);
            var upperArm = characterWowObject.FindBoneByName(upperArmBoneName);

            if (shoulderAttachment != null && upperArm != null)
            {
                var change = new WowVrcFileData.BlendshapeData.BoneData[] { new WowVrcFileData.BlendshapeData.BoneData()
                    {
                        LocalTransform = new WowTransform()
                        {
                            position = new Vec3(),
                            rotation = Quat.RotateY(Quat.Create(), rotationAngle),
                            scale = new Vec3(1f, 1f, 1f)
                        },
                        Name = upperArmBoneName
                    }};

                foreach (var obj in shoulderAttachment.AttachedWowObjects)
                {
                    foreach (var mesh in obj.Meshes)
                    {
                        var basicBakedBlendshape = BlendShapeUtility.BakeBlendShape(obj.GlobalPosition, mesh.Vertices, characterWowObject.Bones, change, 1f);

                        foreach (var basicBakedBlendshapeElement in basicBakedBlendshape)
                        {
                            mesh.Vertices[basicBakedBlendshapeElement.Key].Position = new Vec3(basicBakedBlendshapeElement.Value.Position.X, basicBakedBlendshapeElement.Value.Position.Y, basicBakedBlendshapeElement.Value.Position.Z);
                            mesh.Vertices[basicBakedBlendshapeElement.Key].Normal = new Vec3(basicBakedBlendshapeElement.Value.Normal.X, basicBakedBlendshapeElement.Value.Normal.Y, basicBakedBlendshapeElement.Value.Normal.Z);
                        }
                    }
                }
            }
        }

        private static List<WowMeshWithMaterials> GetMeshesSkinnedToBone(IEnumerable<WowMeshWithMaterials> meshes, byte boneIndex)
        {
            return meshes.Where(mesh =>
            {
                return mesh.Vertices.Any(vertex =>
                    vertex.BoneIndexes[0] == boneIndex ||
                    vertex.BoneIndexes[1] == boneIndex ||
                    vertex.BoneIndexes[2] == boneIndex ||
                    vertex.BoneIndexes[3] == boneIndex);
            }).ToList();
        }

        private static void SetBoneDataToAllVertices(IEnumerable<WowMeshWithMaterials> meshes, byte index0, float weight0, byte index1, float weight1, byte index2, float weight2, byte index3, float weight3)
        {
            foreach (var mesh in meshes)
            {
                foreach (var vertex in mesh.Vertices)
                {
                    var indexes = vertex.BoneIndexes;
                    var weights = vertex.BoneWeights;

                    indexes[0] = index0;
                    weights[0] = weight0;

                    indexes[1] = index1;
                    weights[1] = weight1;

                    indexes[2] = index2;
                    weights[2] = weight2;

                    indexes[3] = index3;
                    weights[3] = weight3;

                    vertex.BoneIndexes = indexes;
                    vertex.BoneWeights = weights;
                }
            }
        }

        private static string CheckRequiredBonesExist(WowObject wowObject)
        {
            string warnings = "";

            var requiredBoneNames = _normalToHumanoidBoneNamesMapping.Values.ToArray();

            foreach (var boneName in requiredBoneNames)
            {
                if (wowObject.FindBoneByName(boneName) == null)
                    warnings += $"Bone \"{boneName}\" was not found\n";
            }

            if (warnings == "")
                warnings = null;

            return warnings;
        }

        private static void MakeUnityCompatibleHumanoidSkeleton(WowObject wowObject)
        {
            // Удаляем все кости кроме тех что заданы в маппинге, либо если приатачен объект к кости
            var normalMappedBoneNames = _normalToHumanoidBoneNamesMapping.Keys.ToArray();
            wowObject.RemoveBones(
                boneToRemove =>
                {
                    // не удаляем кость, если она есть в маппине для костей гуманоида
                    if (Array.IndexOf(normalMappedBoneNames, boneToRemove.GetName()) >= 0)
                        return false;

                    // не удаляем кость, если для нее есть прикрепленные объекты
                    if (boneToRemove.AttachedWowObjects.Count > 0)
                        return false;

                    // иначе удаляем ксоть
                    return true;
                }
            );

            // Оставшиеся кости переименовываем в названия, понятные юнити (для создания humanoid аватара)

            foreach (var bone in wowObject.Bones)
            {
                if (bone != null)
                {
                    var boneName = bone.GetName();

                    if (boneName != null && _normalToHumanoidBoneNamesMapping.TryGetValue(boneName, out var boneHumanoidName))
                        bone.SetName(boneHumanoidName);
                }
            }
        }

        private static readonly Dictionary<string, string> _normalToHumanoidBoneNamesMapping = new Dictionary<string, string>()
        {
            { "body", "Hips" },

            { "leg_upper.L", "LeftUpperLeg" },
            { "leg_upper.R", "RightUpperLeg" },
            { "leg_lower.L", "LeftLowerLeg" },
            { "leg_lower.R", "RightLowerLeg" },
            { "foot.L", "LeftFoot" },
            { "foot.R", "RightFoot" },

            { "spine", "Spine" },
            { "chest", "Chest" },
            { "neck_lower", "Neck" },
            { "head", "Head" },

            { "shoulder.L", "LeftShoulder" },
            { "shoulder.R", "RightShoulder" },
            { "arm_upper.L", "LeftUpperArm" },
            { "arm_upper.R", "RightUpperArm" },
            { "arm_lower.L", "LeftLowerArm" },
            { "arm_lower.R", "RightLowerArm" },
            { "hand.L", "LeftHand" },
            { "hand.R", "RightHand" },

            { "toes.L", "LeftToes" },
            { "toes.R", "RightToes" },

            { "face_eye.L", "LeftEye" },
            { "face_eye.R", "RightEye" },
            { "face_jaw", "Jaw" },

            { "finger_thumb_1.L", "Left Thumb Proximal" },
            { "finger_thumb_2.L", "Left Thumb Intermediate" },
            //{ "", "Left Thumb Distal" },

            { "finger_index_1.L", "Left Index Proximal" },
            { "finger_index_2.L", "Left Index Intermediate" },
            //{ "", "Left Index Distal" },

            { "finger_middle_1.L", "Left Middle Proximal" },
            { "finger_middle_2.L", "Left Middle Intermediate" },
            //{ "", "Left Middle Distal" },

            { "finger_ring_1.L", "Left Ring Proximal" },
            { "finger_ring_2.L", "Left Ring Intermediate" },
            //{ "", "Left Ring Distal" },

            { "finger_pinky_1.L", "Left Little Proximal" },
            { "finger_pinky_2.L", "Left Little Intermediate" },
            //{ "", "Left Little Distal" },

            { "finger_thumb_1.R", "Right Thumb Proximal" },
            { "finger_thumb_2.R", "Right Thumb Intermediate" },
            //{ "", "Right Thumb Distal" },

            { "finger_index_1.R", "Right Index Proximal" },
            { "finger_index_2.R", "Right Index Intermediate" },
            //{ "", "Right Index Distal" },

            { "finger_middle_1.R", "Right Middle Proximal" },
            { "finger_middle_2.R", "Right Middle Intermediate" },
            //{ "", "Right Middle Distal" },

            { "finger_ring_1.R", "Right Ring Proximal" },
            { "finger_ring_2.R", "Right Ring Intermediate" },
            //{ "", "Right Ring Distal" },

            { "finger_pinky_1.R", "Right Little Proximal" },
            { "finger_pinky_2.R", "Right Little Intermediate" },
            //{ "", "Right Little Distal" },

            //{ "", "UpperChest" },
        };

        private static readonly List<string> _firstBlendshapes = new List<string>
        {
            "vrc.blink_left",
            "vrc.blink_right",

            "vrc.lowerlid_left",
            "vrc.lowerlid_right",

            "vrc.v_aa",
            "vrc.v_ch",
            "vrc.v_dd",
            "vrc.v_e",
            "vrc.v_ff",
            "vrc.v_ih",
            "vrc.v_kk",
            "vrc.v_nn",
            "vrc.v_oh",
            "vrc.v_ou",
            "vrc.v_pp",
            "vrc.v_rr",
            "vrc.v_sil",
            "vrc.v_ss",
            "vrc.v_th"
        };
    }
}
