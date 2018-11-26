using System.Collections.Generic;
using WowheadModelLoader;

namespace WowModelExporterCore
{
    public class WowMeshWithMaterials
    {
        public WowVertex[] Vertices { get; set; }
        public List<WowSubmeshWithMaterials> Submeshes { get; set; }

        /// <summary>
        /// Применяет трансформацию ко всем вершинам меша
        /// </summary>
        public void ApplyTransform(Vec3? whTranslation, Vec4? whRotationQuat, Vec3? whScale)
        {
            foreach (var vertex in Vertices)
            {
                // ToDo: тестил только поворот и скейл, возможно если придет транслейт, то будут глюки, так как тут может что-то зависеть от порядка применения
                // трансформаций (вроде сделал в том же порядке что применяются матрицы в костях в оригинале)
                // также возможны глюки если у костей будут родительские кости с ненулевыми матрицами
                
                if (whTranslation.HasValue)
                {
                    // Смещаем
                    vertex.WhPosition = new Vec3(
                        vertex.WhPosition.X + whTranslation.Value.X,
                        vertex.WhPosition.Y + whTranslation.Value.Y,
                        vertex.WhPosition.Z + whTranslation.Value.Z);
                }

                if (whRotationQuat.HasValue)
                {
                    // Поворачиваем
                    vertex.WhPosition = Vec3.TransformQuat(vertex.WhPosition, whRotationQuat.Value);

                    // Также поворачиваем нормаль (для транслейта и скейла этого не надо)
                    vertex.WhNormal = Vec3.TransformQuat(vertex.WhNormal, whRotationQuat.Value);
                }

                if (whScale.HasValue)
                {
                    // Скейлим
                    vertex.WhPosition = new Vec3(
                        vertex.WhPosition.X * whScale.Value.X,
                        vertex.WhPosition.Y * whScale.Value.Y,
                        vertex.WhPosition.Z * whScale.Value.Z);
                }
            }
        }

        /// <summary>
        /// Удаляет вершины, которые не используются ни в одном face(треугольнике)
        /// </summary>
        public void RemoveUnusedVertices()
        {
            // Ключ - старый (до перестроения массива вершин) индекс вершины, значение - новый (после перестроения массива вершин) индекс вершины
            var oldToNewIndices = new Dictionary<int, int>(Vertices.Length);

            // Заполняем старые значения (в ключах получатся все исползуемые индексы)
            foreach (var submesh in Submeshes)
            {
                foreach (var vertexIndex in submesh.Triangles)
                    oldToNewIndices[vertexIndex] = -1;
            }

            var newVertices = new List<WowVertex>(oldToNewIndices.Count);
            for (int vertexIndex = 0; vertexIndex < Vertices.Length; vertexIndex++)
            {
                if (oldToNewIndices.ContainsKey(vertexIndex))
                {
                    oldToNewIndices[vertexIndex] = newVertices.Count;
                    newVertices.Add(Vertices[vertexIndex]);
                }
            }

            foreach (var submesh in Submeshes)
            {
                for (int i = 0; i < submesh.Triangles.Length; i++)
                    submesh.Triangles[i] = (ushort)oldToNewIndices[submesh.Triangles[i]];
            }

            Vertices = newVertices.ToArray();
        }
    }
}
