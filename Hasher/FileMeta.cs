using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace Hasher
{
    internal class FileMeta
    {
        public string FileName { get; set; }
        public string UserId { get; set; }

        public FileMeta(string fileName, string userId)
        {
            FileName = fileName;
            UserId = userId;
        }

        public static FileMeta? GetMeta(string hash)
        {
            if (!File.Exists(Path.Combine("meta", hash)))
                return null;

            string metaJson = File.ReadAllText(Path.Combine("meta", hash));
            return JsonSerializer.Deserialize<FileMeta>(metaJson);
        }

        public static string GetUserId(IPAddress ip) => Convert.ToBase64String(SHA1.HashData(ip.GetAddressBytes()));
    }
}
