using System;
using System.Collections.Generic;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    // ToDo: надо избавиться от этого объекта, либо оставить но он уже будет описывать персонажа, а не просто универсальное что-то.
    // то есть в нем не будет children и parent, а будут конкретные вещи типа WowMeshWithMaterials для body (в WowMeshWithMaterials можно засунуть и GlobalPosition)
    // далее отдельно для attachments и для всего остального. далее кости будут тоже в WowObject но это будет уже character bones а не просто что-то абстрактное.
    // если дальше где то понадобится еще иерархия костей то это на этом же уровне скорее всего будет но с другим названием.
    // Кости при этом будут не массивом а отдельным объектом (скелет), и надо как то сделать чтобы при перестроении (оптимизации) индексов костей в скелете они обновлялись во всех ссылках на него.
    // Думаю можно для этого во всех местах где идет ссылка на кости (включая WowVertex) сделать ссылку не по индексу а по объекту кости + ссылке на сам скелет(на случай если их может быть несколько).
    // тогда при экспорте в fbx индекс в уже оптимизированном массиве можно будет узнать просто найдя эту кость в массиве.
    // далее в wowvertex стоит хранить индексы и веса не как щас а хранить Dictionary<Ссылка на кость, вес>, что то подобное сейчас исоплзуется при мерже весов в оптимизации
    public class WowObject
    {
        public WowObject()
        {
            Parent = null;

            GlobalPosition = new Vec3();

            Meshes = new List<WowMeshWithMaterials>(1);

            Bones = null;
        }

        public WowObject Parent { get; set; }

        public Vec3 GlobalPosition { get; set; }

        // По сути "body" меш - самый первый в списке
        public WowMeshWithMaterials MainMesh
            => Meshes.FirstOrDefault();

        public List<WowMeshWithMaterials> Meshes { get; set; }

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
            foreach (var mesh in Meshes)
            {
                foreach (var vertex in mesh.Vertices)
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
            }

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

            foreach (var mesh in Meshes)
            {
                foreach (var vertex in mesh.Vertices)
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

            // Перестраиваем массив костей, удаляя пустые костаи и заменяя старые индексы в вершинах на новые
            // ToDo: сделать это, но только сейчас есть проблема с поиском индексов, которые ссылаются на эти кости. 
            // проблема в том что отсюда надо знать какие меши ссылаются на эти кости. Сейчас уже есть основной меш + аттачменты.
            // сначало нужно сделать систему при которой скелет будет знать какие мешы к нему привязаны а потом уже делать эту оптимизацию
            // дальше еще есть проблема что индексы кости просто в разных объектах могут висеть (в Wh* классах точно они есть), надо убедиться что их дальше использования не будет.
            // Все исползования должны быть только по ссылке на WowBone.
        }
    }
}
