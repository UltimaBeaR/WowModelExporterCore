using System.Collections.Generic;

namespace WowModelExporterCore
{
    public class WowMeshWithMaterials
    {
        public WowVertex[] Vertices { get; set; }
        public List<WowSubmeshWithMaterials> Submeshes { get; set; }
    }
}
