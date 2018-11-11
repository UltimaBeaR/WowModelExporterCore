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

        public void RemoveBones(string[] exceptBoneNames)
        {
            // ToDo: сначала ищем цель в кого миксить(копировать) веса. искать по цепочке родителей, если нашли там кость с именем из списка exceptBoneNames
            // то миксим в нее, если нет, то проходим всех чилдренов начиная с топовой кости (мы до нее доходим когда идем по цепочке родителей) и если находим первую попавшуюся кость с именем из списка - мержим в нее
            // если вообще не нашли кость куда мержить то передаем вместо нее null (в RemoveBone -> copyWeightsToBone)
        }

        public void RemoveBone(WowBone bone, WowBone copyWeightsToBone)
        {
            // ToDo: пройти весь массив вершин в меше и у тех вершин, у которых есть ссылка на bone (через index) - там меняем index на index от copyWeightsToBone кости.
            // если же copyWeightsToBone == Null то меняем индекс на 0 и вес ставим 0, затем текущие оставшиеся веса нужно нормализовать между собой (привести к 1 в суммер)
            // далее, если же copyWeightsToBone не было = Null и мы прописали ее индекс, над посмотреть может в этой вершине уже учитывалась эта новая кость.
            // то есть надо посмотреть есть ли дубликаты по индексам в этой вершине, если есть то осавляем только один такой индекс и вес ему прописываем как сумму всех этих дубликатов
            // (удаленным дубликатам прописываем индекс 0 и вес 0)
        }
    }
}
