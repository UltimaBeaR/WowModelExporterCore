using System.Collections.Generic;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public static class BlendShapeBaker
    {
        /// <summary>
        /// Формирует измененные позиции вершин (ключ словаря - индекс  в переданном массиве vertices, значение - новая позиция этой вершины) на основании переданного массива вершин,
        /// скелета (список костей) к которому эти вершины привязаны а также набора трансформаций с этими костями.
        /// То есть по сути формирует состояние вершин модели при изменении костей (как при скелетной анимации)
        /// </summary>
        public static Dictionary<int, Vec3> BakeBlendShape(WowVertex[] vertices, WowBone[] allBones, WowVrcFileData.BlendshapeData.BoneData[] blendShapeBoneChanges)
        {
            var allBonesList = allBones.ToList();

            var transforms = new Dictionary<int, WowTransform>(blendShapeBoneChanges.Length);

            foreach (var blendshapeBoneChange in blendShapeBoneChanges)
            {
                var boneIndex = allBonesList.FindIndex(x => x.GetName() == blendshapeBoneChange.Name);
                var bone = allBones[boneIndex];

                var transform = blendshapeBoneChange.LocalTransform;

                // ToDo:
                // bone.LocalPosition - построить из локальной позиции матрицу и вычесть ее / из нее матрицу от локальной позиции/поворота/скейла блендшейпа
                // по хорошему надо строить так матрицу глобальную учитывая парентов, но так как там только позиции, то поидее можно просто вычесть эти позиции (а они одинаковые 
                // у кости и блендшейп кости) и посчитать разницу для локальных матриц
                // https://www.gamedev.net/forums/topic/557605-calculating-the-difference-between-two-transform-matrices/
                // отсюда следует что разница между матрицами это одну инвертнуть и умножить на другую.
                // надо сначала эти вычисления вершин попробовать в редакторе юнити сделат чтобы проверить что все правильно вычисляется.
                // можно взять уже трансформнутую кость и посчитать также на основе разницы локальной между базовой костью и трансормнутой эту матрицу и применить к 
                // вершине 0 0 0 (сама эта кость) - через Vec4.TransformMat4 ну или еще что-то. можно попробовать прям там построить для всех вершин трансформированные вершины и создавать кубики например
                // в координатах вершин. после того как там все ок будет можно тут уже делать

                transforms[boneIndex] = transform;
            }







            throw new System.NotImplementedException();
        }
    }
}
