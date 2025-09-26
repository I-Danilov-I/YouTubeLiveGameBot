using System.Text.Json;

namespace YouTubeLiveGameBot.Helper
{
    /// <summary>
    /// Statische Hilfsklasse zur Verwaltung des Pfads zur client_secret.json-Datei.
    /// Speichert den Pfad in einer JSON-Konfigurationsdatei, liest ihn wieder aus und fragt den Nutzer bei Bedarf.
    /// </summary>
    internal static class PathHelperManagement
    {

        // Name der Konfigurationsdatei im Programmordner
        private const string configFile = "config.json";

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        // JsonSerializerOptions einmalig erzeugen und wiederverwenden
        // WriteIndented = true sorgt dafür, dass die JSON-Datei schön formatiert wird
        private static readonly JsonSerializerOptions jsonOptions = jsonSerializerOptions;

        /// <summary>
        /// Fragt den Pfad zur client_secret.json ab oder liest ihn aus der Konfigurationsdatei.
        /// </summary>
        /// <returns>Gültiger Pfad als String</returns>
        public static string PathManagement()
        {
            // =======================
            // 1. Prüfen ob Datei schon existiert
            // =======================
            if (File.Exists(configFile))
            {
                // Konfigurationsdatei als Text einlesen
                var json = File.ReadAllText(configFile);

                // JSON in Dictionary<string, string> deserialisieren
                // Key = Name der Einstellung, Value = Pfad
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                // Prüfen, ob der Schlüssel "client_secret" vorhanden ist
                if (config != null && config.TryGetValue("path_client_secret", out string? savedPath))
                {
                    // Prüfen, ob der gespeicherte Pfad nicht leer ist und die Datei existiert
                    if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
                    {
                        Console.WriteLine($"Gespeicherter Pfad gefunden: {savedPath}");
                        // Pfad zurückgeben, keine weitere Eingabe nötig
                        return savedPath;
                    }
                }
            }

            // =======================
            // 2. Nutzer nach Pfad fragen, falls nicht gespeichert oder ungültig
            // =======================
            while (true)
            {
                Console.WriteLine("Bitte geben Sie den Pfad zu der `client_secret.json` ein:");
                string? input = Console.ReadLine();
                if (input != null)
                {
                    input = input.Replace("\"", "").Replace("`",""); // entfernt alle " aus der Eingabe
                }


                // Prüfen: Eingabe darf nicht null, leer oder nur Leerzeichen sein
                // und die Datei muss existieren
                if (string.IsNullOrWhiteSpace(input) || !File.Exists(input))
                {
                    Console.WriteLine("Der Pfad konnte nicht gefunden werden!");
                    continue; // Schleife wiederholen, solange ungültig
                }

                // Eingabe ist gültig → in Dictionary speichern
                var config = new Dictionary<string, string>
                {
                    { "client_secret", input } // Key = client_secret, Value = Pfad
                };

                // Dictionary als JSON in die Konfigurationsdatei schreiben
                // jsonOptions sorgt für schön formatiertes JSON
                File.WriteAllText(configFile, JsonSerializer.Serialize(config, jsonOptions));

                // Gültigen Pfad zurückgeben
                Console.WriteLine("Pfad gefunden.");
                return input;
            }
        }
    }
}
