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
            // Удаляем все кости кроме тех что заданы в маппинге
            wowObject.RemoveAllBonesExceptSpecifiedByNames(_normalToHumanoidBoneNamesMapping.Keys.ToArray());

            // Оставшиеся кости переименовываем в названия, понятные юнити (для создания humanoid аватара)

            foreach (var bone in wowObject.Bones)
            {
                if (bone != null)
                {
                    if (_normalToHumanoidBoneNamesMapping.TryGetValue(bone.GetName(), out var boneHumanoidName))
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

            { "toes_l", "LeftToes" },
            { "toes_r", "RightToes" },

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
    }
}
