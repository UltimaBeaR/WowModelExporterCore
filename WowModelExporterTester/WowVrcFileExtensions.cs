﻿using System.Collections.Generic;
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
            var bakedBlendshapes = new Dictionary<string, Dictionary<int, Vec3>>();
            foreach (var blendshape in file.Blendshapes)
            {
                if (blendshape.Bones.Length > 0)
                    bakedBlendshapes.Add(blendshape.Name, BlendShapeBaker.BakeBlendShape(characterWowObject.Mesh.Vertices, characterWowObject.Bones, blendshape.Bones));
            }

            PrepareForVRChatUtility.PrepareObject(characterWowObject, true, true);

            var fbxExporter = new Exporter();

            return fbxExporter.ExportWowObject(characterWowObject, bakedBlendshapes, exportDirectory);
        }
    }
}