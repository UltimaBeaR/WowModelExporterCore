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
        /// Индекс кости (тот, который указывается в <see cref="WowVertex.BoneIndexes"/>)
        /// </summary>
        public byte Index { get; set; }

        /// <summary>
        /// Локальная позиция кости, относительно родителя
        /// </summary>
        public Vec3 LocalPosition { get; set; }
    }
}
