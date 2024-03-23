using HttpEngine.Core;
using System.Security.Cryptography;
using System.Text;

namespace Hasher
{
    internal class IndexModel : Model
    {
        public IndexModel()
        {
            Routes = new()
            {
                "/"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            var data = File("pages/index.html", request);

            var files = new List<(string File, string FileName)>();
            foreach (var file in Directory.GetFiles("files"))
            {
                string hash = Path.GetFileName(file);

                FileMeta? meta = FileMeta.GetMeta(hash);
                if (meta == null)
                    continue;
                if (FileMeta.GetUserId(request.ClientAddress) != meta.UserId)
                    continue;

                files.Add((hash, meta.FileName));
            }

            data.ParseView(new()
            {
                ["files"] = string.Join("", files.Select(x =>
                    data.GetSection("file", new()
                    {
                        { "hash", JavascriptEscape.Escape(x.File) },
                        { "fileName", JavascriptEscape.Escape(x.FileName) },
                    }))),
                ["fileCount"] = files.Count
            });

            return new(data);
        }
    }
}
