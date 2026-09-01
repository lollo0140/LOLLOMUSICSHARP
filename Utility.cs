using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LOLLOMUSICX
{
    class Utility
    {
        // Usa .ToJsonString() anziché JsonSerializer.Serialize per evitare totalmente la Reflection
        public static void WriteJsonFile(string path, JsonNode content)
        {
            if (content != null)
                File.WriteAllText(path, content.ToJsonString());
        }

        public static void WriteJsonFile(string path, JsonObject content)
        {
            if (content != null)
                File.WriteAllText(path, content.ToJsonString());
        }

        public static void WriteJsonFile(string path, JsonArray content)
        {
            if (content != null)
                File.WriteAllText(path, content.ToJsonString());
        }

        public static JsonArray ReadJsonArray(string path)
        {
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var node = JsonNode.Parse(content);
                    if (node is JsonArray array)
                        return array;
                }
            }

            return new JsonArray();
        }

        public static JsonObject ReadJsonObject(string path)
        {
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var node = JsonNode.Parse(content);
                    if (node is JsonObject obj)
                        return obj;
                }
            }

            // Ritorna un JsonObject vuoto anziché [] (che rappresenta un array)
            return new JsonObject();
        }

        // Database canzoni locali
        public static void ValidateDownloaded()
        {
            var P = Program.JSONDownloadsPath;

            JsonObject database = ReadJsonObject(P);

            // Trova le chiavi dei file non esistenti prima di rimuoverle
            // per evitare di modificare la collezione durante l'enumerazione (foreach)
            var keysToRemove = new List<string>();

            foreach (var item in database)
            {
                var pathNode = item.Value?["path"];
                if (pathNode == null || !File.Exists(pathNode.GetValue<string>()))
                {
                    keysToRemove.Add(item.Key);
                }
            }

            // Rimuove i record orfani
            foreach (var key in keysToRemove)
            {
                database.Remove(key);
            }

            WriteJsonFile(P, database);
        }

        public static bool IsVideoLocal(string id)
        {
            var P = Program.JSONDownloadsPath;
            return ReadJsonObject(P).ContainsKey(id);
        }

        public static string GetLocalStreamingPath(string id)
        {
            var P = Program.JSONDownloadsPath;
            JsonObject DB = ReadJsonObject(P);
            return DB[id]?["path"]?.GetValue<string>() ?? string.Empty;
        }

        public static void CleanDir(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
            Directory.CreateDirectory(path);
        }
    }
}
