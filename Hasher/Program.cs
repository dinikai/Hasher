using Hasher;
using HttpEngine.Core;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var app = new HttpApplicationBuilder(new HttpApplicationBuilderOptions
{
    Hosts = new string[]
    {
        "http://*:80/"
    },
    CacheControl = CacheControl.NoCache
}).Build();

app.Use404<Error404Model>();

app.UseModel<IndexModel>();
app.UseModel<EncryptModel>();
app.UseModel<AboutModel>();

app.MapPost("/add", request =>
{
    if (request.Arguments is MultipartRequestArguments arguments)
    {
        string key = arguments.Arguments["key"];
        foreach (var file in arguments.Files)
        {
            bool hideName = request.Arguments.Arguments.ContainsKey("hideName");
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] checksum = SHA256.HashData(file.Data);

            byte[] encryptedData = Cryptor.Encrypt(keyBytes, file.Data.Concat(checksum).ToArray());
            string hash = BitConverter.ToString(SHA256.HashData(encryptedData)).Replace("-", "").ToLower();
            string userId = FileMeta.GetUserId(request.ClientAddress);
            string metaJson = JsonSerializer.Serialize(new FileMeta(hideName ? "&lt;hidden&gt;" : file.FileName, userId));
            File.WriteAllText(Path.Combine("meta", hash), metaJson);
            File.WriteAllBytes(Path.Combine("files", hash), encryptedData);
        }
    }

    return Model.RedirectBack(request);
});

app.MapGet("/check", request =>
{
    if (File.Exists(Path.Combine("files", request.Arguments.Arguments["hash"])))
    {
        byte[] data = Cryptor.Decrypt(Encoding.UTF8.GetBytes(request.Arguments.Arguments["key"]),
            File.ReadAllBytes(Path.Combine("files", request.Arguments.Arguments["hash"])));

        if (Cryptor.CheckChecksum(Encoding.UTF8.GetBytes(request.Arguments.Arguments["key"]), data, false))
            return new ModelResult("true");
        else
            return new ModelResult("false");
    }

    return new ModelResult("File not found");
});

app.MapGet("/delete", request =>
{
    if (File.Exists(Path.Combine("files", request.Arguments.Arguments["hash"])))
    {
        byte[] data = Cryptor.Decrypt(Encoding.UTF8.GetBytes(request.Arguments.Arguments["key"]),
            File.ReadAllBytes(Path.Combine("files", request.Arguments.Arguments["hash"])));

        if (Cryptor.CheckChecksum(Encoding.UTF8.GetBytes(request.Arguments.Arguments["key"]), data, false))
        {
            File.Delete(Path.Combine("files", request.Arguments.Arguments["hash"]));
            File.Delete(Path.Combine("meta", request.Arguments.Arguments["hash"]));
        }
    }

    return Model.RedirectBack(request);
});

app.MapPost("/encrypt", request =>
{
    if (request.Arguments is MultipartRequestArguments arguments)
    {
        string key = arguments.Arguments["key"];
        foreach (var file in arguments.Files)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encryptedData = Cryptor.Encrypt(keyBytes, file.Data);

            return Model.Download(encryptedData, file.FileName);
        }
    }

    return Model.RedirectBack(request);
});

app.MapPost("/decrypt", request =>
{
    if (request.Arguments is MultipartRequestArguments arguments)
    {
        string key = arguments.Arguments["key"];
        foreach (var file in arguments.Files)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decryptedData = Cryptor.Decrypt(keyBytes, file.Data);

            return Model.Download(decryptedData, file.FileName);
        }
    }

    return Model.RedirectBack(request);
});

app.MapGet("/meta", request =>
{
    FileMeta? meta = FileMeta.GetMeta(request.Arguments.Arguments["hash"]);
    if (meta == null)
        return new ModelResult("File not exists");

    return new ModelResult(JsonSerializer.Serialize(meta));
});

app.MapGet("/{hash}", request =>
{
    if (File.Exists(Path.Combine("files", request.Arguments.Arguments["hash"])))
    {
        byte[] data = Cryptor.Decrypt(Encoding.UTF8.GetBytes(request.Arguments.Arguments["key"]),
            File.ReadAllBytes(Path.Combine("files", request.Arguments.Arguments["hash"])));

        if (!Cryptor.CheckChecksum(Encoding.UTF8.GetBytes(request.Arguments.Arguments["key"]), data, false))
            return new ModelResult("Invalid key");

        FileMeta? meta = FileMeta.GetMeta(request.Arguments.Arguments["hash"]);
        if (meta == null)
            return Model.RedirectBack(request);

        data = data.ToList().GetRange(0, data.Length - 32).ToArray();

        if (request.Arguments.Arguments["download"] == "download")
            return Model.Download(data, request.Arguments.Arguments["hash"] + Path.GetExtension(meta.FileName));
        else
            return new ModelResult(data);
    }

    return Model.Skip();
});

app.Run();