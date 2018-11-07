using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace WowheadModelLoader
{
    public static class DataLoaderBase
    {
        /// <summary>
        /// Если установлено в true, значит при попытке загрузки каких либо ресурсов сначала будет попытка взять их из кэша во временной папке
        /// кэширование идет по полному url запрашиваемого ресурса
        /// </summary>
        public static bool UseCache { get; set; } = true;

        public static string LoadString(string url, string cacheExtension = ".txt")
        {
            if (UseCache)
            {
                var cachedResult = ReadStringFromCache(url, cacheExtension);
                if (cachedResult != null)
                    return cachedResult;
            }

            var client = new HttpClient();
            var result = client.GetStringAsync(url).Result;

            if (UseCache)
            {
                WriteStringToCache(url, cacheExtension, result);
            }

            return result;
        }

        public static byte[] LoadBinary(string url, string cacheExtension = ".bin")
        {
            if (UseCache)
            {
                var cachedResult = ReadBinaryFromCache(url, cacheExtension);
                if (cachedResult != null)
                    return cachedResult;
            }

            var client = new HttpClient();
            var result = client.GetByteArrayAsync(url).Result;

            if (UseCache)
            {
                WriteBinaryToCache(url, cacheExtension, result);
            }

            return result;
        }

        private static string ReadStringFromCache(string url, string cacheExtension)
        {
            var fileName = GetCacheFileName(url, cacheExtension);

            if (!File.Exists(fileName))
                return null;

            return File.ReadAllText(fileName);
        }

        private static byte[] ReadBinaryFromCache(string url, string cacheExtension)
        {
            var fileName = GetCacheFileName(url, cacheExtension);

            if (!File.Exists(fileName))
                return null;

            return File.ReadAllBytes(fileName);
        }

        private static void WriteStringToCache(string url, string cacheExtension, string contents)
        {
            Directory.CreateDirectory(CacheDirectory);
            File.WriteAllText(GetCacheFileName(url, cacheExtension), contents);
        }

        private static void WriteBinaryToCache(string url, string cacheExtension, byte[] contents)
        {
            Directory.CreateDirectory(CacheDirectory);
            File.WriteAllBytes(GetCacheFileName(url, cacheExtension), contents);
        }

        static string GetHashFromUrl(string url)
        {
            byte[] data = _sha.ComputeHash(Encoding.UTF8.GetBytes(url));

            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }

        private static string GetCacheFileName(string url, string cacheExtension)
        {
            return Path.Combine(CacheDirectory, GetHashFromUrl(url) + cacheExtension);
        }

        private static readonly string CacheDirectory = Path.Combine(Path.GetTempPath(), "WowheadModelLoader__DataLoaderCache");

        private static readonly SHA256 _sha = SHA256.Create();
    }
}
