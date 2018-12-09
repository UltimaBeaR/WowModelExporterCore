using System.Collections.Generic;
using System.Linq;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public static class BlendShapeBaker
    {
        /// <summary>
        /// Формирует измененные позиции вершин (ключ словаря - индекс  в переданном массиве vertices, значение - новая позиция и нормаль этой вершины) на основании переданного массива вершин,
        /// скелета (список костей) к которому эти вершины привязаны а также набора трансформаций с этими костями.
        /// То есть по сути формирует состояние вершин модели при изменении костей (как при скелетной анимации)
        /// </summary>
        public static Dictionary<int, Vertex> BakeBlendShape(WowVertex[] vertices, WowBone[] bones, WowVrcFileData.BlendshapeData.BoneData[] blendShapeBoneChanges)
        {
            // Создаем оригинальные/трансформированные кости и просчитываем локальные матрицы для них

            var originalBones = new BoneTransform[bones.Length];
            var blenshapeBones = new BoneTransform[bones.Length];

            for (int boneIdx = 0; boneIdx < bones.Length; boneIdx++)
            {
                originalBones[boneIdx] = new BoneTransform();
                blenshapeBones[boneIdx] = new BoneTransform();

                originalBones[boneIdx].SetLocalDataFromWowBone(bones[boneIdx]);

                var blendshapeChange = blendShapeBoneChanges.FirstOrDefault(x => x.Name == bones[boneIdx].GetName());
                if (blendshapeChange != null)
                    blenshapeBones[boneIdx].SetLocalDataFromBlendshapeBone(blendshapeChange);
                else
                    blenshapeBones[boneIdx].SetLocalDataFromWowBone(bones[boneIdx]);
            }

            // Прописываем иерархию для оригинальных/трансформированных костей

            for (int boneIdx = 0; boneIdx < bones.Length; boneIdx++)
            {
                if (bones[boneIdx].ParentBone != null)
                {
                    originalBones[boneIdx].Parent = originalBones[bones[boneIdx].ParentBone.Index];
                    blenshapeBones[boneIdx].Parent = blenshapeBones[bones[boneIdx].ParentBone.Index];
                }
            }

            // Считаем глобальные матрицы разницы между оригинальной и blendshape костью для каждой из костей в исходном массиве костей
            // В случае если изменений в кости (с учетом родительской иерархии) не было, матрица изменений кости будет равна (или приблизительно равна) identity

            var blendshapeDifferenceMatricesPerBone = new Mat4[bones.Length];

            for (int boneIdx = 0; boneIdx < bones.Length; boneIdx++)
                blendshapeDifferenceMatricesPerBone[boneIdx] = Mat4.Multiply(blenshapeBones[boneIdx].GetGlobalMatrix(), Mat4.Invert(originalBones[boneIdx].GetGlobalMatrix()));

            // Меняем заданные вершины в соответствии с просчитанными матрицами изменений.
            // Если значение вершины изменилось в результате примененных трансформаций (то есть если на вершину влияла хотя бы одна кость, матрица изменений которой не identity)
            // то добавляем эту вершину в словарь измененных вершин, который далее возвращаем

            var changedVertices = new Dictionary<int, Vertex>();

            for (int vertexIdx = 0; vertexIdx < vertices.Length; vertexIdx++)
            {
                var vertex = vertices[vertexIdx];

                var changedVertexPos = new Vec3();
                var changedVertexNormal = new Vec3();

                for (int boneInVertexIdx = 0; boneInVertexIdx < 4; boneInVertexIdx++)
                {
                    var boneIdx = vertex.BoneIndexes[boneInVertexIdx];
                    if (boneIdx < 0)
                        continue;

                    var boneWeight = vertex.BoneWeights[boneInVertexIdx];

                    var positionFromBone = Vec3.TransformMat4(vertex.Position, blendshapeDifferenceMatricesPerBone[boneIdx]);
                    changedVertexPos.X += positionFromBone.X * boneWeight;
                    changedVertexPos.Y += positionFromBone.Y * boneWeight;
                    changedVertexPos.Z += positionFromBone.Z * boneWeight;

                    // Незнаю в чем дело, но если используется scale в нормалях то результат не соответствует скинингу из юнити. При чем непонятно где правильно сделано, как тут или как там
                    var normalFromBone = Vec3.TransformMat4(vertex.Normal, Mat4.Transpose(Mat4.Invert(blendshapeDifferenceMatricesPerBone[boneIdx])));
                    changedVertexNormal.X += normalFromBone.X * boneWeight;
                    changedVertexNormal.Y += normalFromBone.Y * boneWeight;
                    changedVertexNormal.Z += normalFromBone.Z * boneWeight;
                }

                changedVertexNormal = Vec3.Normalize(changedVertexNormal);

                if (!Vec3.AreNearlyEqual(changedVertexPos, vertex.Position))
                    changedVertices.Add(vertexIdx, new Vertex() { Position = changedVertexPos, Normal = changedVertexNormal });
            }

            return changedVertices;
        }

        public class Vertex
        {
            public Vec3 Position { get; set; }
            public Vec3 Normal { get; set; }
        }

        /// <summary>
        /// Кость, описанная в виде матрицы и указателя на родителя (для формирования иерархии).
        /// Матрица формируется из локальной позиции кости либо локальной позиции/поворота/скейла блендшейп-кости
        /// </summary>
        private class BoneTransform
        {
            public BoneTransform Parent { get; set; }

            public Mat4 LocalMatrix { get; set; }

            /// <summary>
            /// Вычисление глобальной матрицы кости
            /// </summary>
            /// <returns></returns>
            public Mat4 GetGlobalMatrix()
            {
                if (Parent == null)
                    return LocalMatrix;

                return Mat4.Multiply(Parent.GetGlobalMatrix(), LocalMatrix);
            }

            public void SetLocalDataFromWowBone(WowBone bone)
            {
                var matrix = Mat4.Identity();
                matrix = Mat4.Translate(matrix, bone.LocalPosition);

                // Поворота и скейла у базовых костей нет

                LocalMatrix = matrix;
            }

            public void SetLocalDataFromBlendshapeBone(WowVrcFileData.BlendshapeData.BoneData bone)
            {
                var matrix = Mat4.Identity();
                matrix = Mat4.Translate(matrix, bone.LocalTransform.position);
                matrix = Mat4.Multiply(matrix, Mat4.FromQuat(bone.LocalTransform.rotation));
                matrix = Mat4.Scale(matrix, bone.LocalTransform.scale);

                LocalMatrix = matrix;
            }
        }
    }
}
