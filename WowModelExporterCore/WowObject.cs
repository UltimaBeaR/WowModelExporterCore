using System;
using System.Collections.Generic;
using System.Linq;
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
            return Bones.First(x => x != null && x.ParentBone == null);
        }



        /// <summary>
        /// "Схлопывает" все кости, не включенные в список к родителям (мержит свои веса и иерархию в родителей)
        /// Возможна ситуация что останется рутовая кость, которая отсутствует в списке, а внутри нее будет несколько "детей" из списка
        /// </summary>
        public void RemoveBones(string[] exceptBoneNames)
        {
            foreach (var boneToRemove in Bones.Where(x => x != null && Array.IndexOf(exceptBoneNames, x.GetName()) <= -1))
                RemoveBone(boneToRemove);
            
            var rootBone = GetRootBone();
            if (Array.IndexOf(exceptBoneNames, rootBone.GetName()) <= -1)
                RemoveBone(rootBone);
        }

        /// <summary>
        /// Удаляет заданую кость, перенося при этом внутреннюю иерархию и веса на родителя.
        /// Если родителя нет, то удалит кость (веса никуда не перенесутся) и сделает дочернюю кость рутом (если таких костей несколько, ничего не произойдет, метод вернет false)
        /// </summary>
        public bool RemoveBone(WowBone boneToRemove)
        {
            // ToDo: какая то хрень происходит после удаления костей, перс скейлится. если оставить root кость то все хорошо.
            // я думаю это из за удаления с null родителем, веса перехерачиваются, надо посмотреть...

            if (boneToRemove == null || boneToRemove.ParentBone == null && boneToRemove.ChildBones.Count > 1)
                return false;

            int boneIndex = boneToRemove.Index;
            ByteVec4 boneIndexes;
            Vec4 boneWeights;
            bool changed;

            foreach (var vertex in Mesh.Vertices)
            {
                boneIndexes = vertex.BoneIndexes;
                boneWeights = vertex.BoneWeights;
                changed = false;

                for (int i = 0; i < 4; i++)
                {
                    if (boneIndexes[i] == boneIndex)
                    {
                        if (boneToRemove.ParentBone != null)
                            boneIndexes[i] = boneToRemove.ParentBone.Index;
                        else
                        {
                            boneIndexes[i] = 0;
                            boneWeights[i] = 0;
                        }
                            
                        changed = true;
                    }
                }

                if (changed)
                {
                    boneWeights.NormalizeSum();

                    vertex.BoneIndexes = boneIndexes;
                    vertex.BoneWeights = boneWeights;

                    // ToDo: убрать дубликаты индексов в вершине и объеденить веса (необязательно, но желательно)
                }
            }

            // ToDo: сейчас я не перестраиваю массив костей после удаления. Поидее надо удалить null элементы из него и перепрописать индексы для всех вершин на измененные.
            // возможно стоит сделать для этого действия отдельный метод чтобы вызывать его вручную после пачки удалений, т.к. эта операция может быть долгой

            for (int i = 0; i < Bones.Length; i++)
            {
                if (Bones[i] == boneToRemove)
                {
                    Bones[i] = null;

                    foreach (var child in boneToRemove.ChildBones)
                    {
                        child.ParentBone = boneToRemove.ParentBone;
                        child.ParentBone?.ChildBones.Add(child);
                    }

                    boneToRemove.ParentBone?.ChildBones.Remove(boneToRemove);

                    return true;
                }
            }

            throw new System.InvalidOperationException("Похоже что идет удаление кости, которой не существет в массиве костей");
        }
    }
}
