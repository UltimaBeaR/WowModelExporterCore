using System.Collections.Generic;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowObject
    {
        public WowObject()
        {
            Parent = null;
            Children = new List<WowObject>();

            Position = new Vec3();

            Mesh = null;

            Bones = null;
        }

        public WowObject Parent { get; set; }
        public List<WowObject> Children { get; set; }

        public Vec3 Position { get; set; }

        public WowMeshWithMaterials Mesh { get; set; }

        /// <summary>
        /// Все кости в виде массива. Позиция в массиве это <see cref="WowBone.Index"/>
        /// </summary>
        public WowBone[] Bones { get; set; }

        public WowBone GetRootBone()
        {
            if (Bones == null || Bones.Length == 0)
                return null;

            // ToDo: Будем пока считать что самая первая кость в списке это и есть главная кость (надоо проверить так ли это).
            // теоретически костей без родителя может быть много и тогда не ясно какую делать основной, по этому я не стал тут искать кость без родителя чтобы сделать ее основной
            var rootBone = Bones[0];
            System.Diagnostics.Debug.Assert(rootBone.ParentBone == null);

            return rootBone;
        }
    }
}
