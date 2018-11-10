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
        }

        public WowObject Parent { get; set; }
        public List<WowObject> Children { get; set; }

        public Vec3 Position { get; set; }

        public WowMeshWithMaterials Mesh { get; set; }





        public Vec3[] Bones { get; set; }
    }
}
