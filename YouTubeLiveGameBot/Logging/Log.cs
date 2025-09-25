using System;
using System.Diagnostics;
using System.IO;

namespace YouTubeLiveGameBot.Logging
{
    internal static class Log
    {
        // Methode zum Schreiben von Logs
        public static void L(string inputString)
        {
            string logFilePath = BotSettings.LogFileFolderPathWithName;

            // Verzeichnis prüfen und erstellen, falls es nicht existiert
            string? directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(directory)) // Sicherstellen, dass directory nicht null oder leer ist
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                throw new InvalidOperationException("Das Verzeichnis für die Logdatei konnte nicht ermittelt werden.");
            }

            // An die Logdatei anhängen
            using StreamWriter writer = new(logFilePath, true); // 'true' bedeutet, dass an die Datei angehängt wird.

            if (!string.IsNullOrEmpty(inputString))
            {
                // Ausgabe auf der Konsole und im Debugger
                Console.WriteLine($"{inputString}");
                Debug.WriteLine(inputString);

                // Schreibe in die Logdatei
                writer.WriteLine($"{inputString}");
            }
        }
    }
}
