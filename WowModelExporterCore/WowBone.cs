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

            AttachedWowObjects = new List<WowObject>();
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

        public List<WowObject> AttachedWowObjects { get; set; }

        /// <summary>
        /// Устанавливает родительскую кость без изменения глобальной позиции текущей кости
        /// </summary>
        public void SetParentAndKeepGlobalPosition(WowBone parentBone)
        {
            var globalPosition = GetGlobalPosition();

            ParentBone = parentBone;

            var parentGlobalPosition = ParentBone?.GetGlobalPosition() ?? new Vec3(0, 0, 0);

            LocalPosition = new Vec3(
                globalPosition.X - parentGlobalPosition.X,
                globalPosition.Y - parentGlobalPosition.Y,
                globalPosition.Z - parentGlobalPosition.Z);
        }

        /// <summary>
        /// Возвращает глобальную позицию для для указанной локальной позиции, учитывая текущего родителя
        /// </summary>
        public Vec3 GetGlobalPositionRelativeToThisParent(Vec3 localPosition)
        {
            if (ParentBone == null)
                return localPosition;

            var parentGlobalPosition = ParentBone.GetGlobalPosition();

            return new Vec3(
                localPosition.X + parentGlobalPosition.X,
                localPosition.Y + parentGlobalPosition.Y,
                localPosition.Z + parentGlobalPosition.Z);
        }

        public Vec3 GetGlobalPosition()
        {
            return GetGlobalPositionRelativeToThisParent(LocalPosition);
        }

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

        public override string ToString()
        {
            return GetName() ?? $"[bone id: {Id}]";
        }

        private string _name;

        private static readonly Dictionary<uint, string> _boneNames = new Dictionary<uint, string>()
        {
            // Тут я в регионах прописываю какая кость у кого была найдена, чтобы потом можно было разрулить конфликты с именами, если такие будут
            // но многие кости при этом могут быть общими у разных расс/полов

            #region human male

            { 521822810,    "ROOT" },

            { 3066451557,   "body" },
            { 40275131,     "body_upper" },
            { 727987715,    "body_lower" },

            { 4144451481,   "leg_upper.L" },
            { 420032181,    "leg_upper_middle.L" },
            { 3337573636,   "leg_lower.L" },
            { 685847592,    "leg_lower_middle.L" },
            { 90603765,     "foot.L" },
            { 2608964796,   "toes.L" },

            { 3118917107,   "leg_upper.R" },
            { 1474856159,   "leg_upper_middle.R" },
            { 2282684270,   "leg_lower.R" },
            { 1711316546,   "leg_lower_middle.R" },
            { 4285119894,   "foot.R" },
            { 1656702206,   "toes.R" },

            { 2031597313,   "spine" },
            { 981834931,    "chest" },

            { 1278252913,   "shoulder.L" },
            { 4204807211,   "arm_upper.L" },
            { 3264325347,   "arm_upper_middle_1.L" },
            { 3046545013,   "arm_upper_middle_2.L" },
            { 2133193007,   "arm_lower.L" },
            { 4084841598,   "arm_lower_middle_1.L" },
            { 2222886120,   "arm_lower_middle_2.L" },
            { 294203841,    "hand.L" },
            { 3151027085,   "finger_thumb_1.L" },
            { 1884120296,   "finger_thumb_2.L" },
            { 3244039095,   "finger_index_1.L" },
            { 1984240785,   "finger_index_2.L" },
            { 4276058962,   "finger_middle_1.L" },
            { 3357587416,   "finger_middle_2.L" },
            { 2357464676,   "finger_ring_1.L" },
            { 1178020679,   "finger_ring_2.L" },
            { 1788646044,   "finger_pinky_1.L" },
            { 863523977,    "finger_pinky_2.L" },

            { 3057625618,   "shoulder.R" },
            { 11499848,     "arm_upper.R" },
            { 2356153481,   "arm_upper_middle_1.R" },
            { 4218895391,   "arm_upper_middle_2.R" },
            { 2234174540,   "arm_lower.R" },
            { 3180860948,   "arm_lower_middle_1.R" },
            { 3399427714,   "arm_lower_middle_2.R" },
            { 3951430818,   "hand.R" },
            { 1105192686,   "finger_thumb_1.R" },
            { 2319604107,   "finger_thumb_2.R" },
            { 995305172,    "finger_index_1.R" },
            { 2353668594,   "finger_index_2.R" },
            { 80758321,     "finger_middle_1.R" },
            { 841976507,    "finger_middle_2.R" },
            { 1988834055,   "finger_ring_1.R" },
            { 3157792292,   "finger_ring_2.R" },
            { 2425597951,   "finger_pinky_1.R" },
            { 3380046314,   "finger_pinky_2.R" },

            { 657769981,    "neck_lower" },
            { 3191706695,   "neck_upper" },
            { 130111906,    "head" },

            { 2356232324,   "face_teeth_upper" },

            { 3391925095,   "face_nose_tip" },

            { 3778922811,   "face_nose_cheek.L" },
            { 319601099,    "face_nose.L" },

            { 2235519192,   "face_nose_cheek.R" },
            { 2050857682,   "face_nose.R" },

            { 3228034019,   "face_eyebrow_middle" },

            { 4148793443,   "face_eyebrow_to_middle_1.L" },
            { 2152636661,   "face_eyebrow_to_middle_2.L" },
            { 424145231,    "face_eyebrow_to_middle_3.L" },

            { 2658710394,   "face_eyebrow_to_middle_1.R" },
            { 3917448172,   "face_eyebrow_to_middle_2.R" },
            { 1886835286,   "face_eyebrow_to_middle_3.R" },

            { 1764558272,   "face_forehead.L" },

            { 1844953,      "face_forehead.R" },
            
            { 770235359,    "face_cheek_upper.L" },
            { 3034679909,   "face_cheek_lower.L" },
            { 1525672777,   "face_cheek_lower_side.L" },
            
            { 1240089148,   "face_cheek_upper.R" },
            { 3504542598,   "face_cheek_lower.R" },
            { 1055748778,   "face_cheek_lower_side.R" },

            { 398530341,    "face_lip_upper_to_middle_1.L" },
            { 1623599027,   "face_lip_upper_to_middle_2.L" },
            { 4191122953,   "face_lip_upper_to_middle_3.L" },

            { 3946969320,   "face_lip_upper_to_middle_1.R" },
            { 2621884542,   "face_lip_upper_to_middle_2.R" },
            { 89102788,     "face_lip_upper_to_middle_3.R" },

            { 3498022390,   "face_eye.L" },

            { 3885920869,   "face_eye.R" },

            { 211345957,    "face_eyelid_upper.L" },
            { 2509353887,   "face_eyelid_lower.L" },

            { 905925502,    "face_eyelid_upper.R" },
            { 2901803716,   "face_eyelid_lower.R" },

            { 818638717,    "face_jaw" },

            { 193042082,    "face_teeth_lower" },

            { 1563338754,   "face_chin" },

            { 3731153341,   "face_lip_lower_to_middle_1.L" },
            { 2841891115,   "face_lip_lower_to_middle_2.L" },
            { 812286097,    "face_lip_lower_to_middle_3.L" },

            { 585371248,    "face_lip_lower_to_middle_1.R" },
            { 1440956134,   "face_lip_lower_to_middle_2.R" },
            { 3437915996,   "face_lip_lower_to_middle_3.R" },

            { 2526474064,   "attachment_head" },
            { 28063340,     "attachment_shoulder.L" },
            { 4221766415,   "attachment_shoulder.R" },
            { 2776517278,   "attachment_belt" },

            #endregion

            #region nightelf female

            { 2896842011,   "face_eyelid_upper.L" },
            { 899882145,    "face_eyelid_lower.L" },

            { 2513304640,   "face_eyelid_upper.R" },
            { 214216186,    "face_eyelid_lower.R" },


            { 3351897000,   "face_ear_1.L" },
            { 1589686802,   "face_ear_2.L" },

            { 4029154363,   "face_ear_1.R" },
            { 1763792257,   "face_ear_2.R" },

            { 2925917466,   "face_teeth_upper" },
            { 1740637058,   "face_teeth_lower" },

            { 1073771550,   "face_chin_side.L" },
            { 3162545107,   "face_chin_side.R" },

            { 2978057156,   "face_ponytail_1" },
            { 680057470,    "face_ponytail_2" },
            { 1603267304,   "face_ponytail_3" },
            { 3253436235,   "face_ponytail_4" },


            { 2309325446,   "face_moustache.L" },
            { 1965387083,   "face_moustache.R" },

            { 2605442443,   "face_long_brow_1.L" },
            { 37925937,     "face_long_brow_2.L" },

            { 1741359686,   "face_long_brow_1.R" },
            { 4274150396,   "face_long_brow_2.R" },

            #endregion

            #region draenei female
                
            { 1639899198,   "attachment_weapon.R" },

            { 490258427,    "face_tentacles_1.L" },
            { 2217864769,   "face_tentacles_2.L" },
            { 4080459479,   "face_tentacles_3.L" },
            { 1834109812,   "face_tentacles_4.L" },

            { 4161896729,   "face_tentacles_1.R" },
            { 1629016227,   "face_tentacles_2.R" },
            { 371187765,    "face_tentacles_3.R" },
            { 2289792406,   "face_tentacles_4.R" },

            { 2154402137,   "face_long_back_hair_1" },
            { 425771235,    "face_long_back_hair_2" },
            { 1852305525,   "face_long_back_hair_3" },
            { 4026754518,   "face_long_back_hair_4" },

            #endregion

            #region tauren male
             
            { 3008775997,   "face_eye.L" },
            { 1339468016,   "face_eye.R" },

            { 616214639,    "face_lower_lid.L" },
            { 2308168067,   "face_lower lid.R" },

            { 2121990039,   "face_horn.L" },
            { 390847630,    "face_horn.R" },

            { 1033699804,   "face_upper_teeth.L" },
            { 3239964177,   "face_upper_teeth.R" },

            { 1394997981,   "face_nose_tauren" },

            { 1935429588,   "face_cheek_tauren_lower.L" },
            { 1245445775,   "face_cheek_tauren_lower.R" },

            { 3931446894,   "face_cheek_tauren_upper.L" },
            { 3543314229,   "face_cheek_tauren_upper.R" },

            { 4272075280,   "face_tauren_1.L" },
            { 35806685,     "face_tauren_1.R" },

            #endregion
        };
    }
}
