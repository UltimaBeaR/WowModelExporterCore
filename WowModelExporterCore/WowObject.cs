using System.Collections.Generic;

namespace WowModelExporterCore
{
    public class WowObject
    {
        public WowObject Parent { get; set; }
        public List<WowObject> Children { get; set; }

        public WowMeshWithMaterials Mesh { get; set; }
    }
}
