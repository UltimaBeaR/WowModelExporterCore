using System.IO;

namespace WowheadModelLoader
{
    public class WhRenderFlag
    {
        public WhRenderFlag()
        {
        }

        public WhRenderFlag(BinaryReader r)
        {
            Flags = r.ReadUInt16();
            Blend = r.ReadUInt16();
        }

        public ushort Flags { get; set; }
        public ushort Blend { get; set; }

        public static void ComputeFlags(WhTexUnit self) {
            self.Flip = false;
            self.NoZWrite = false;
            self.Cull = true;
            self.Unlit = false;

            if (self.Model.RenderFlags != null && self.RenderFlagIndex < self.Model.RenderFlags.Length)
                self.RenderFlag = self.Model.RenderFlags[self.RenderFlagIndex];
            else
            {
                self.RenderFlag = new WhRenderFlag()
                {
                    Flags = 0,
                    Blend = 0
                };
            }

            self.Unlit = (self.RenderFlag.Flags & 1) != 0;
            self.Cull = (self.RenderFlag.Flags & 4) == 0;
            self.NoZWrite = (self.RenderFlag.Flags & 16) != 0;

            //WH.debug("RenderFlag.ComputeFlags", "self.flags", self.flags, "self.priorityPlane", self.priorityPlane, "self.renderFlag.flags", self.renderFlag.flags, "self.renderFlag.blend", self.renderFlag.blend, "self.materialLayer", self.materialLayer, "self.materialIndex", self.materialIndex, "self.texUnitIndex", self.texUnitIndex)
        }
    }
}
