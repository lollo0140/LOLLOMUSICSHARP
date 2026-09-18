using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace LOLLOMUSICX
{
    public class ApiMaps
    {
        private static HttpClient c;

        public static void MapApiPorts(WebApplication app)
        {
            app.MapGet("/api/audio/{id}", async (string id) =>
            {
                try
                {
                    if (Utility.IsVideoLocal(id))
                    {
                        string localPath = Utility.GetLocalStreamingPath(id);
                        return Results.File(localPath, contentType: "audio/webm", enableRangeProcessing: true);
                    }

                    string P = Path.Combine(Program.cachedVideosPath, $"{id}.webm");

                    if (!File.Exists(P))
                    {
                        await Program.yTMusicClient.DownloadVideoById(id, P);
                    }

                    return Results.File(P, contentType: "audio/webm", enableRangeProcessing: true);
                }
                catch
                {
                    return Results.Problem($"Error while loading video: {id}");
                }
            });

            app.MapGet("/api/img/{x}/{**url}", async (int x, string url, bool save) =>
            {
                string decodedUrl = Uri.UnescapeDataString(url);
                string safeFileName = $"{x}_" + string.Join("_", decodedUrl.Split(Path.GetInvalidFileNameChars())) + ".png";
                string P = Path.Combine(Program.cachedImmagesPath, safeFileName);

                if (save)
                {

                    Directory.CreateDirectory(Program.cachedImmagesPath);

                    if (File.Exists(P))
                    {
                        return Results.File(P, contentType: "image/png", enableRangeProcessing: true);
                    }
                }


                string targetUrl = decodedUrl.StartsWith("http")
                    ? decodedUrl
                    : "https://yt3.googleusercontent.com/" + decodedUrl;



                c ??= new();
                byte[] imageBytes = await c.GetByteArrayAsync(targetUrl);

                if (save)
                {
                    await File.WriteAllBytesAsync(P, imageBytes);
                    return Results.File(P, contentType: "image/png", enableRangeProcessing: true);
                }


                return Results.Bytes(imageBytes, contentType: "image/png");
            });

            app.MapGet("/api/videoimg/{x}/{**url}", async (int x, string url, bool save) =>
            {
                string decodedUrl = Uri.UnescapeDataString(url);

                string safeFileName = $"{x}_" + string.Join("_", decodedUrl.Split(Path.GetInvalidFileNameChars())) + ".png";
                string P = Path.Combine(Program.cachedImmagesPath, safeFileName);

                if (save)
                {
                    Directory.CreateDirectory(Program.cachedImmagesPath);

                    if (File.Exists(P))
                    {
                        return Results.File(P, contentType: "image/png", enableRangeProcessing: true);
                    }
                }



                string targetUrl = decodedUrl.StartsWith("http")
                    ? decodedUrl
                    : "https://i.ytimg.com/vi/" + decodedUrl;



                c ??= new();
                byte[] imageBytes = await c.GetByteArrayAsync(targetUrl);

                if (save)
                {
                    await File.WriteAllBytesAsync(P, imageBytes);
                    return Results.File(P, contentType: "image/png", enableRangeProcessing: true);
                }

                return Results.Bytes(imageBytes, contentType: "image/png");

            });

        }
    }
}
