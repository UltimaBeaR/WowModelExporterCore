using System;
using System.Windows.Forms;
using WowModelExporterCore;
using WowheadModelLoader;
using WowModelExporterFbx;
using System.Collections.Generic;
using System.Linq;

namespace WowModelExporterTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var exporter = new WowModelExporter();

            var wowObject = exporter.LoadCharacter(WhRace.HUMAN, WhGender.MALE, new string[]
            {
                //// шлем
                //"161600",
                //// плечи
                //"161621",
                //// плащ
                //"163355",
                //// чест
                //"161602",
                //// брасы
                //"161629",
                //// руки
                //"161610",
                //// пояс
                //"161624",
                //// ноги
                //"161616",
                //// ступни
                //"161605"
            });

            MakeHumanoidSkeleton(wowObject);

            var test = new Exporter();

            textBox1.Text = test.ExportWowObject(wowObject, "newtest").ToString();
        }

        private static void MakeHumanoidSkeleton(WowObject wowObject)
        {
            wowObject.RemoveBones(_normalToHumanoidBoneNamesMapping.Keys.ToArray());

            foreach (var bone in wowObject.Bones)
            {
                if (bone != null)
                {
                    if (_normalToHumanoidBoneNamesMapping.TryGetValue(bone.GetName(), out var boneHumanoidName))
                        bone.SetName(boneHumanoidName);
                }
            }
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
    }
}
