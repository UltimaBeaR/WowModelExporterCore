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
    }
}
