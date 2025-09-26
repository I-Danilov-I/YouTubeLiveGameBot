using System.Text.Json;
using System.Text.Json.Nodes;

namespace YouTubeLiveGameBot
{
    internal static class JsonOperations
    {
        private static readonly string file = "data.json";
        private static Dictionary<string, Object>? data;

        // Einmalige Optionen für alle Serialisierungen
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        public static object? Get(string key)
        {
            return data!.TryGetValue(key, out var value) ? value : null;
        }

        public static void Set(string key, object value) 
        {
            data![key] = value;
        }

        public static void Save() 
        {
            File.WriteAllText(file, JsonSerializer.Serialize(data, options));
        }

        public static void Load() 
        {
            data = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(file))!;
        }
    }
}
