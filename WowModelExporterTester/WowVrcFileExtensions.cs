using System.Collections.Generic;
using WowheadModelLoader;
using WowModelExporterCore;
using WowModelExporterFbx;

namespace WowModelExporterTester
{
    public static class WowVrcFileExtensions
    {
        public static bool ExportToFbx(this WowVrcFile file, string exportDirectory, bool prepareForVRChat)
        {
            var exporter = new WowModelExporter();

            WowObject characterWowObject;

            var opts = file.GetOpts();
            if (opts != null)
            {
                characterWowObject = exporter.LoadCharacter(WhViewerOptions.FromJson(opts));
            }
            else
            {
                var manualHeader = file.GetManualHeader();
                if (manualHeader == null)
                    throw new System.InvalidOperationException();

                characterWowObject = exporter.LoadCharacter(manualHeader.Race, manualHeader.Gender, manualHeader.ItemIds);
            }

            // ToDo: после запекания идет привязка на текущие индексы вершин. Если вершины будут перестроены, надо тут тоже обновить индексы 
            var bakedBlendshapes = new List<BlendShapeUtility.BakedBlendshape>();
            if (file.Blendshapes != null)
            {
                foreach (var blendshape in file.Blendshapes)
                {
                    if (blendshape.Bones.Length > 0)
                    {
                        if (blendshape.Name == WowVrcFileData.BlendshapeData.basicBlendshapeName)
                        {
                            var basicBakedBlendshape = BlendShapeUtility.BakeBlendShape(characterWowObject.MainMesh.Vertices, characterWowObject.Bones, blendshape.Bones);

                            foreach (var basicBakedBlendshapeElement in basicBakedBlendshape)
                            {
                                characterWowObject.MainMesh.Vertices[basicBakedBlendshapeElement.Key].Position = new Vec3(basicBakedBlendshapeElement.Value.Position.X, basicBakedBlendshapeElement.Value.Position.Y, basicBakedBlendshapeElement.Value.Position.Z);
                                characterWowObject.MainMesh.Vertices[basicBakedBlendshapeElement.Key].Normal = new Vec3(basicBakedBlendshapeElement.Value.Normal.X, basicBakedBlendshapeElement.Value.Normal.Y, basicBakedBlendshapeElement.Value.Normal.Z);
                            }
                        }
                        else
                        {
                            bakedBlendshapes.Add(new BlendShapeUtility.BakedBlendshape
                            {
                                BlendshapeName = blendshape.Name,
                                Changes = BlendShapeUtility.BakeBlendShape(characterWowObject.MainMesh.Vertices, characterWowObject.Bones, blendshape.Bones)
                            });
                        }
                    }
                }
            }

            if (prepareForVRChat)
                PrepareForVRChatUtility.PrepareObject(characterWowObject, bakedBlendshapes, true, true, true, true);

            var fbxExporter = new Exporter();

            return fbxExporter.ExportWowObject(characterWowObject, bakedBlendshapes, exportDirectory, false);
        }
    }
}
