using System;
using System.Diagnostics;
using YouTubeLiveGameBot.Logging; // Füge dies für die Process-Klasse hinzu

namespace YouTubeLiveGameBot._Devlop
{
    public static class DevlopHelper
    {
        public static void TrackTouchEvents()
        {
            string command = $"shell getevent -lt {BotSettings.InputDevice}"; // Verwende getevent ohne -lp für Live-Daten
            Log.L("Starte die Erfassung von Touch-Ereignissen...");

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = BotSettings.AdbPath;
                process.StartInfo.Arguments = command;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Log.L(args.Data); // Direkt auf der Konsole ausgeben
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.L($"Fehler bei der Erfassung der Touch-Ereignisse: {ex.Message}");
            }
        }
    }
}
