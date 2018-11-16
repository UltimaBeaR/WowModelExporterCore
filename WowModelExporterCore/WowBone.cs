using System.Collections.Generic;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowBone
    {
        public WowBone()
        {
            ParentBone = null;
            ChildBones = new List<WowBone>();

            Index = 0;

            LocalPosition = new Vec3();
        }

        public WowBone ParentBone { get; set; }
        public List<WowBone> ChildBones { get; set; }

        /// <summary>
        /// Индекс кости в массиве костей (тот, который указывается в <see cref="WowVertex.BoneIndexes"/>)
        /// </summary>
        public byte Index { get; set; }

        /// <summary>
        /// id костей по которому можно идентифицировать кость. Например у всех персов что я смотрел совпадают id для костей табарда, верхней части ноги (это то что я смотрел. но скорее всего все так),
        /// то есть скорее всего эти id  одни и те же для похожих костей у разных рас и полов. Эти id уникальные на каждую отдельную кость. В оригинальной вовхедовской кости есть еще некий keyId, вот он может
        /// быть одним и тем же на разные кости, так что его использовать для определения костей нельзя, это что-то другое
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Локальная позиция кости, относительно родителя
        /// </summary>
        public Vec3 LocalPosition { get; set; }

        /// <summary>
        /// Задает имя. После этого имя перестанет генерироваться из таблицы подстановки имен по id кости. Чтобы убрать явно заданное имя можно установить его в null
        /// </summary>
        /// <returns></returns>
        public void SetName(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Возвращает название кости (идет маппинг на Id), по которому кость потом можно найти
        /// </summary>
        public string GetName()
        {
            return _name ?? (_boneNames.TryGetValue(Id, out var val) ? val : null);
        }

        private string _name;

        private static readonly Dictionary<uint, string> _boneNames = new Dictionary<uint, string>()
        {
            { 521822810,    "ROOT" },

            { 3066451557,   "body" },
            { 40275131,     "body_upper" },
            { 727987715,    "body_lower" },

            { 4144451481,   "leg_upper_r" },
            { 420032181,    "leg_upper_middle_r" },
            { 3337573636,   "leg_lower_r" },
            { 685847592,    "leg_lower_middle_r" },
            { 90603765,     "foot_r" },
            { 2608964796,   "toe_r" },

            { 3118917107,   "leg_upper_l" },
            { 1474856159,   "leg_upper_middle_l" },
            { 2282684270,   "leg_lower_l" },
            { 1711316546,   "leg_lower_middle_l" },
            { 4285119894,   "foot_l" },
            { 1656702206,   "toe_l" },

            { 2031597313,   "spine" },
            { 981834931,    "chest" },

            { 1278252913,   "shoulder_r" },
            { 4204807211,   "arm_upper_r" },
            { 3264325347,   "arm_upper_middle_1_r" },
            { 3046545013,   "arm_upper_middle_2_r" },
            { 2133193007,   "arm_lower_r" },
            { 4084841598,   "arm_lower_middle_1_r" },
            { 2222886120,   "arm_lower_middle_2_r" },
            { 294203841,    "hand_r" },
            { 3151027085,   "finger_thumb_1_r" },
            { 1884120296,   "finger_thumb_2_r" },
            { 3244039095,   "finger_index_1_r" },
            { 1984240785,   "finger_index_2_r" },
            { 4276058962,   "finger_middle_1_r" },
            { 3357587416,   "finger_middle_2_r" },
            { 2357464676,   "finger_ring_1_r" },
            { 1178020679,   "finger_ring_2_r" },
            { 1788646044,   "finger_pinky_1_r" },
            { 863523977,    "finger_pinky_2_r" },

            { 3057625618,   "shoulder_l" },
            { 11499848,     "arm_upper_l" },
            { 2356153481,   "arm_upper_middle_1_l" },
            { 4218895391,   "arm_upper_middle_2_l" },
            { 2234174540,   "arm_lower_l" },
            { 3180860948,   "arm_lower_middle_1_l" },
            { 3399427714,   "arm_lower_middle_2_l" },
            { 3951430818,   "hand_l" },
            { 1105192686,   "finger_thumb_1_l" },
            { 2319604107,   "finger_thumb_2_l" },
            { 995305172,    "finger_index_1_l" },
            { 2353668594,   "finger_index_2_l" },
            { 80758321,     "finger_middle_1_l" },
            { 841976507,    "finger_middle_2_l" },
            { 1988834055,   "finger_ring_1_l" },
            { 3157792292,   "finger_ring_2_l" },
            { 2425597951,   "finger_pinky_1_l" },
            { 3380046314,   "finger_pinky_2_l" },

            { 657769981,    "neck_lower" },
            { 3191706695,   "neck_upper" },
            { 130111906,    "head" },

            { 2356232324,   "teeth_upper" },

            { 3391925095,   "nose_tip" },

            { 3778922811,   "nose_cheek_r" },
            { 319601099,    "nose_r" },

            { 2235519192,   "nose_cheek_l" },
            { 2050857682,   "nose_l" },

            { 3228034019,   "eyebrow_middle" },

            { 4148793443,   "eyebrow_to_middle_1_r" },
            { 2152636661,   "eyebrow_to_middle_2_r" },
            { 424145231,    "eyebrow_to_middle_3_r" },

            { 2658710394,   "eyebrow_to_middle_1_l" },
            { 3917448172,   "eyebrow_to_middle_2_l" },
            { 1886835286,   "eyebrow_to_middle_3_l" },

            { 1764558272,   "forehead_r" },

            { 1844953,      "forehead_l" },
            
            { 770235359,    "cheek_upper_r" },
            { 3034679909,   "cheek_lower_r" },
            { 1525672777,   "cheek_lower_side_r" },
            
            { 1240089148,   "cheek_upper_l" },
            { 3504542598,   "cheek_lower_l" },
            { 1055748778,   "cheek_lower_side_l" },

            { 398530341,    "lip_upper_to_middle_1_r" },
            { 1623599027,   "lip_upper_to_middle_2_r" },
            { 4191122953,   "lip_upper_to_middle_3_r" },

            { 3946969320,   "lip_upper_to_middle_1_l" },
            { 2621884542,   "lip_upper_to_middle_2_l" },
            { 89102788,     "lip_upper_to_middle_3_l" },

            { 3498022390,   "eye_r" },

            { 3885920869,   "eye_l" },

            { 211345957,    "eyelid_upper_r" },
            { 2509353887,   "eyelid_lower_r" },

            { 905925502,    "eyelid_upper_l" },
            { 2901803716,   "eyelid_lower_l" },

            { 818638717,    "jaw" },

            { 193042082,    "teeth_lower" },

            { 1563338754,   "chin" },

            { 3731153341,   "lip_lower_to_middle_1_r" },
            { 2841891115,   "lip_lower_to_middle_2_r" },
            { 812286097,    "lip_lower_to_middle_3_r" },

            { 585371248,    "lip_lower_to_middle_1_l" },
            { 1440956134,   "lip_lower_to_middle_2_l" },
            { 3437915996,   "lip_lower_to_middle_3_l" },
        };
    }
}
