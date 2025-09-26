using System.Text.Json;

namespace YouTubeLiveGameBot
{
    internal static class JsonOperations
    {
        private static readonly string file =
            "E:\\Visual Studio Projekte\\YouTubeLiveGameBot\\YouTubeLiveGameBot\\data.json";

        private static Dictionary<string, object> data = new();

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        private static void EnsureLoaded()
        {
            if (data.Count > 0) return;

            if (File.Exists(file))
            {
                var content = File.ReadAllText(file);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    data = JsonSerializer.Deserialize<Dictionary<string, object>>(content)!;
                }
            }
        }

        public static object? Get(string key)
        {
            EnsureLoaded();
            return data.TryGetValue(key, out var value) ? value : null;
        }

        public static void Set(string key, object value)
        {
            EnsureLoaded();
            data[key] = value;
            Save();
        }

        public static void Save()
        {
            File.WriteAllText(file, JsonSerializer.Serialize(data, options));
        }

        public static void Load()
        {
            data.Clear();
            EnsureLoaded();
        }
    }
}
