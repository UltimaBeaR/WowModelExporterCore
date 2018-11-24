using System;
using System.Collections.Generic;
using System.Linq;

namespace WowModelExporterCore
{
    // ToDo: потом наверно перенести в отдельную библиотеку с зависимостью на core

    /// <summary>
    /// Подготавливает модель персонажа из wow для нормальной работы в vrchat (предполагается дальнейший экспорт в fbx)
    /// </summary>
    public static class PrepareForVRChatUtility
    {
        public static void PrepareObject(WowObject wowObject, bool removeToes, bool removeJaw)
        {
            MakeHumanoidSkeleton(wowObject, removeToes, removeJaw);
        }

        private static void MakeHumanoidSkeleton(WowObject wowObject, bool removeToes, bool removeJaw)
        {
            // Удаляем все кости кроме тех что заданы в маппинге, либо если приатачен объект к кости
            var normalMappedBoneNames = _normalToHumanoidBoneNamesMapping.Keys.ToArray();
            wowObject.RemoveBones(
                boneToRemove =>
                {
                    // не удаляем кость, если она есть в маппине для костей гуманоида
                    if (Array.IndexOf(normalMappedBoneNames, boneToRemove.GetName()) >= 0)
                        return false;

                    // неу удаляем кость, если для нее есть прикрепленные объекты
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

            // Удаляем пальцы ног если надо (чтоб не глючило в вр режиме в врчате)
            if (removeToes)
                wowObject.RemoveBonesByNames(new[] { "LeftToes", "RightToes" });

            // Удаляем нижнюю челюсть, если надо (чтобы vrchat не делал автоматическую анимацию речи через эту кость - оно работает глючно)
            if (removeJaw)
                wowObject.RemoveBonesByNames(new[] { "Jaw" });
        }

        private static readonly Dictionary<string, string> _normalToHumanoidBoneNamesMapping = new Dictionary<string, string>()
        {
            { "ROOT", "ROOT" },

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

            { "eye.L", "LeftEye" },
            { "eye.R", "RightEye" },
            { "jaw", "Jaw" },

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
