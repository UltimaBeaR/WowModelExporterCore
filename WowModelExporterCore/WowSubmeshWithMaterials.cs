namespace WowModelExporterCore
{
    public class WowSubmeshWithMaterials
    {
        public WowSubmeshWithMaterials(WowMeshWithMaterials mesh, ushort[] triangles, WowMaterial material)
        {
            Mesh = mesh;
            Triangles = triangles;
            Material = material;
        }

        public WowMeshWithMaterials Mesh { get; private set; }
        public ushort[] Triangles { get; private set; }
        public WowMaterial Material { get; private set; }
    }
}
