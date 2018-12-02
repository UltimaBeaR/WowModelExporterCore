using WowheadModelLoader;
using WowModelExporterCore;
using WowModelExporterFbx;

namespace WowModelExporterTester
{
    public static class WowVrcFileExtensions
    {
        public static bool ExportToFbx(this WowVrcFile file, string exportDirectory)
        {
            var exporter = new WowModelExporter();

            WowObject wowObject;

            var opts = file.GetOpts();
            if (opts != null)
            {
                wowObject = exporter.LoadCharacter(WhViewerOptions.FromJson(opts));
            }
            else
            {
                var manualHeader = file.GetManualHeader();
                if (manualHeader == null)
                    throw new System.InvalidOperationException();

                wowObject = exporter.LoadCharacter(manualHeader.Race, manualHeader.Gender, manualHeader.ItemIds);
            }

            PrepareForVRChatUtility.PrepareObject(wowObject, true, true);

            var fbxExporter = new Exporter();

            return fbxExporter.ExportWowObject(wowObject, exportDirectory);
        }
    }
}
