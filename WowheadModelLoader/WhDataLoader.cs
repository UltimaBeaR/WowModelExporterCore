using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WowheadModelLoader.Json;

namespace WowheadModelLoader
{
    public static class WhDataLoader
    {
        /// <summary>
        /// Работает аналогично WH.Gatherer.fetchItems, чтобы получить данные по переданным итемам
        /// </summary>
        public static Dictionary<string, WhJsonGathererItem> LoadItemsFromGatherer(string[] itemIds)
        {
            if (itemIds == null || itemIds.Length == 0)
                return new Dictionary<string, WhJsonGathererItem>(0);

            itemIds = itemIds.OrderBy(x => x).ToArray();

            var relativeUrl = $"/gatherer?items={string.Join(",", itemIds)}";
            var url = GetWowheadUrl(relativeUrl);
            var javascriptCode = DataLoaderBase.LoadString(url, ".js");

            var itemsJson = JavaScriptDigger.GetItemsFromGathererResult(javascriptCode);

            return JsonConvert.DeserializeObject<Dictionary<string, WhJsonGathererItem>>(itemsJson);
        }

        public static Bitmap LoadTexture(uint file)
        {
            var relativeUrl = $"textures/{file}.png";
            var url = GetModelViewerUrl(relativeUrl);
            var binary = DataLoaderBase.LoadBinary(url, ".png");

            using (var ms = new MemoryStream(binary))
                return (Bitmap)Image.FromStream(ms);
        }

        public static WhJsonCustomizationData LoadMetaCharacterCustomization(WhRace race, WhGender gender)
        {
            var relativeUrl = $"meta/charactercustomization/{(int)race}_{(int)gender}.json";
            var url = GetModelViewerUrl(relativeUrl);
            var json = DataLoaderBase.LoadString(url, ".json");

            return JsonConvert.DeserializeObject<WhJsonCustomizationData>(json);
        }

        public static WhJsonMeta LoadMeta(string metaPath, string id)
        {
            var relativeUrl = $"meta/{metaPath}/{id}.json";
            var url = GetModelViewerUrl(relativeUrl);
            var json = DataLoaderBase.LoadString(url, ".json");

            return JsonConvert.DeserializeObject<WhJsonMeta>(json);
        }

        public static byte[] LoadMo3(string id)
        {
            var relativeUrl = $"mo3/{id}.mo3";
            var url = GetModelViewerUrl(relativeUrl);
            var binary = DataLoaderBase.LoadBinary(url, ".mo3");

            return binary;
        }

        private static string GetWowheadUrl(string relativeUrl)
        {
            return new Uri(_wowheadUrl, relativeUrl).ToString();
        }

        private static string GetModelViewerUrl(string relativeUrl)
        {
            return new Uri(_modelViewerUrl, relativeUrl).ToString();
        }

        private static readonly Uri _wowheadUrl = new Uri("https://www.wowhead.com");
        private static readonly Uri _modelViewerUrl = new Uri("https://wow.zamimg.com/modelviewer/");
    }
}
