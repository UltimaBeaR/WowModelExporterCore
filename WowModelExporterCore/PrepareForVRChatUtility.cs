using System;
using System.Collections.Generic;
using System.Linq;

namespace WowModelExporterCore
{
    // ToDo: потом наверно перенести в отдельную библиотеку с зависимостью на core

    // ToDo: придумать что-то с плечами в врчате. можно сделать какие нибудь фейковые кости или еще что-то.
    // еще переназначение костей с верхних рук на плечи (так сделано у моего роги)

    /// <summary>
    /// Подготавливает модель персонажа из wow для нормальной работы в vrchat (предполагается дальнейший экспорт в fbx)
    /// </summary>
    public static class PrepareForVRChatUtility
    {
        public static void PrepareObject(WowObject wowObject, Dictionary<string, Dictionary<int, BlendShapeUtility.Vertex>> bakedBlendshapes, bool removeToes, bool removeJaw, bool addDummyEyesAndEyelidVisemes, bool fixBlendshapes)
        {
            MakeUnityCompatibleHumanoidSkeleton(wowObject);

            // Удаляем пальцы ног если надо (чтоб не глючило в вр режиме в врчате)
            if (removeToes)
                wowObject.RemoveBonesByNames(new[] { "LeftToes", "RightToes" });

            // Удаляем нижнюю челюсть, если надо (чтобы vrchat не делал автоматическую анимацию речи через эту кость - оно работает глючно)
            if (removeJaw)
                wowObject.RemoveBonesByNames(new[] { "Jaw" });

            // Если не заданы необходимые для движения глаз виземы моргания, создаем пустые (чтобы глаза двигались, т.к. врчат не делает движение глаз при отсутствии этих визем)
            if (addDummyEyesAndEyelidVisemes)
            {
                // Если костей глаз не найдено - добавляем фейковые внутрь кости головы

                if (wowObject.FindBoneByName("LeftEye") == null)
                    wowObject.AddDummyBone("LeftEye", wowObject.FindBoneByName("Head") ?? wowObject.FindBoneByName("Hips"), new WowheadModelLoader.Vec3());
                if (wowObject.FindBoneByName("RightEye") == null)
                    wowObject.AddDummyBone("RightEye", wowObject.FindBoneByName("Head") ?? wowObject.FindBoneByName("Hips"), new WowheadModelLoader.Vec3());

                // добавляем в блендшейп одну вершину. Если в блендшейпе вообще не будет изменений - eye tracking все равно не будет работать, как будто блендшейпа не существует
                var pos = wowObject.MainMesh.Vertices[0].Position;
                var normal = wowObject.MainMesh.Vertices[0].Normal;

                if (!bakedBlendshapes.ContainsKey("vrc.blink_left"))
                    bakedBlendshapes.Add("vrc.blink_left", new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } });
                if (!bakedBlendshapes.ContainsKey("vrc.blink_right"))
                    bakedBlendshapes.Add("vrc.blink_right", new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } });
                if (!bakedBlendshapes.ContainsKey("vrc.lowerlid_left"))
                    bakedBlendshapes.Add("vrc.lowerlid_left", new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } });
                if (!bakedBlendshapes.ContainsKey("vrc.lowerlid_right"))
                    bakedBlendshapes.Add("vrc.lowerlid_right", new Dictionary<int, BlendShapeUtility.Vertex>() { { 0, new BlendShapeUtility.Vertex() { Position = pos, Normal = normal } } });
            }

            // Добавляем небольшое смещение к каждой из позиций вершин, чтобы избавиться от каких-то глюков (repair_shapekeys и repair_shapekeys_mouth функции в cats плагине)
            // как минимум - если пустой blendshape например на vrc.lowerlid_left ничем не отличается от базы - eye tracking работать не будет. При этом очень мелкое значение тоже не влияет, нужно именно определенное
            if (fixBlendshapes)
            {
                var rand = new Random(34523634);

                foreach (var bakedBlendshape in bakedBlendshapes)
                {
                    if (bakedBlendshape.Key.StartsWith("vrc."))
                    {
                        foreach (var vertex in bakedBlendshape.Value)
                        {
                            var tinyChange = 0.00007f * (rand.NextDouble() < 0.5 ? -1f : 1f);

                            vertex.Value.Position = new WowheadModelLoader.Vec3(vertex.Value.Position.X + tinyChange, vertex.Value.Position.Y + tinyChange, vertex.Value.Position.Z + tinyChange);
                        }
                    }
                }
            }

            wowObject.OptimizeBones();
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
    }
}
