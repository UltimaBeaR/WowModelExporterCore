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

            GlobalPosition = new Vec3();

            Mesh = null;

            Bones = null;
        }

        public WowObject Parent { get; set; }
        public List<WowObject> Children { get; set; }

        public Vec3 GlobalPosition { get; set; }

        public WowMeshWithMaterials Mesh { get; set; }

        /// <summary>
        /// Все кости в виде массива. Позиция в массиве это <see cref="WowBone.Index"/>
        /// </summary>
        public WowBone[] Bones { get; set; }

        public WowBone GetRootBone()
        {
            return Bones.First(x => x != null && x.ParentBone == null);
        }

        public void RemoveBonesByNames(string[] boneNames)
        {
            RemoveBones(x => Array.IndexOf(boneNames, x.GetName()) > -1);
        }

        /// <summary>
        /// "Схлопывает" все кости, не включенные в список к родителям (мержит свои веса и иерархию в родителей)
        /// </summary>
        public void RemoveBones(Predicate<WowBone> predicate)
        {
            foreach (var boneToRemove in Bones.Where(x => x != null && predicate(x)))
                RemoveBone(boneToRemove);

            var rootBone = GetRootBone();
            if (predicate(rootBone))
                RemoveBone(rootBone);
        }

        /// <summary>
        /// Удаляет заданую кость, перенося при этом внутреннюю иерархию и веса на родителя.
        /// Если родителя нет, то удалит кость (веса никуда не перенесутся) и сделает дочернюю кость рутом (если таких костей несколько, ничего не произойдет, метод вернет false)
        /// </summary>
        public bool RemoveBone(WowBone boneToRemove)
        {
            if (boneToRemove == null || (boneToRemove.ParentBone == null && boneToRemove.ChildBones.Count > 1))
                return false;

            int boneIndex = boneToRemove.Index;
            ByteVec4 boneIndexes;
            Vec4 boneWeights;
            bool changed;

            // В случае если удаляем корневую кость (boneToRemove.ParentBone == null) то либо у нее не будет child костей вообще либо будет одна (проверяется выше)
            var boneToRemoveSingleChild = boneToRemove.ChildBones.Count == 1 ? boneToRemove.ChildBones[0] : null;

            // ToDo: возможно надо будет менять не только вершины этого объекта но и всякие приатаченные объекты,
            // которые также могут быть завязаны на эти кости (надо видимо будет хранить в объекте список мешей которые привязаны
            // к костям этим либо вообще отдельно скелет хранить и в нем уже список мешей/wowobject-ов)

            // Меняем веса у вершин в соответствии с новой иерархией костей
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
                        else if (boneToRemoveSingleChild != null)
                            boneIndexes[i] = boneToRemoveSingleChild.Index;
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
                }
            }

            // ToDo: сейчас я не перестраиваю массив костей после удаления. Поидее надо удалить null элементы из него и перепрописать индексы для всех вершин на измененные.
            // возможно стоит сделать для этого действия отдельный метод чтобы вызывать его вручную после пачки удалений, т.к. эта операция может быть долгой

            // Удаляем кость из списка костей и перебрасываем ее children на родителя удаляемой кости
            for (int i = 0; i < Bones.Length; i++)
            {
                if (Bones[i] == boneToRemove)
                {
                    Bones[i] = null;

                    foreach (var child in boneToRemove.ChildBones)
                    {
                        child.SetParentAndKeepGlobalPosition(boneToRemove.ParentBone);
                        child.ParentBone?.ChildBones.Add(child);
                    }

                    boneToRemove.ParentBone?.ChildBones.Remove(boneToRemove);

                    return true;
                }
            }

            throw new InvalidOperationException("Похоже что идет удаление кости, которой не существет в массиве костей");
        }

        public void OptimizeBones()
        {
            // Для всех вершин мержу веса костей в одно значение если есть 2 (и более) одинаковые кости в вершине
            // далее нормализирую веса в каждой вершине. Индексы костей при этом сортируются в порядке возрастания (в конце идут неиспользуемые кости с индексом и весом 0)

            var indexedWeights = new Dictionary<byte, float>();
            foreach (var vertex in Mesh.Vertices)
            {
                int i;

                indexedWeights.Clear();
                for (i = 0; i < 4; i++)
                    indexedWeights[vertex.BoneIndexes[i]] = 0f;

                for (i = 0; i < 4; i++)
                    indexedWeights[vertex.BoneIndexes[i]] += vertex.BoneWeights[i];

                var newIndexes = new ByteVec4();
                var newWeights = new Vec4();

                i = 0;
                foreach (var indexedWeight in indexedWeights.OrderBy(x => x.Key))
                {
                    newIndexes[i] = indexedWeight.Key;
                    newWeights[i] = indexedWeight.Value;
                    i++;
                }

                newWeights.NormalizeSum();

                vertex.BoneIndexes = newIndexes;
                vertex.BoneWeights = newWeights;
            }
        }
    }
}
