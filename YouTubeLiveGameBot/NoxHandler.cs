using System.Diagnostics;

namespace YouTubeLiveGameBot
{
    internal static class NoxHandler
    {
        public static string InputDevice { get; private set; } = "/dev/input/event4";
        public static string AppPackageName { get; private set; } = "si.a4c.dwarf";
        public static string AdbPath { get; set; } = "C:\\Program Files (x86)\\Nox\\bin\\adb.exe";
        public static string NoxExePath { get; set; } = "C:\\Program Files (x86)\\Nox\\bin\\Nox.exe";
        public static string indexOfInstanze = "1";
        public static int TimeoutAfterComand { get; set; } = 3000;
        public static string DeviceIP { get; private set; } = "";


        public static string GetInstalledPackages()
        {
            // Führe den ADB-Befehl aus, um die Liste der Pakete abzurufen
            string output = ExecuteAdbCommand("shell pm list packages");

            if (string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Keine Pakete gefunden oder Fehler beim Abrufen der Pakete.");
                return "";
            }

            // Entferne das Präfix "package:" aus der Ausgabe
            char[] separator = ['\n', '\r'];
            var packageList = output.Replace("package:", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);

            // Protokolliere die Pakete zur Überprüfung
            foreach (var package in packageList)
            {
                Console.WriteLine($"Gefundenes Paket: {package.Trim()}");
            }

            return string.Join("\n", packageList); // Falls die Liste weiterverarbeitet werden soll
        }


        public static string ExecuteAdbCommand(string command)
        {
            try
            {
                Process process = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = AdbPath,
                        Arguments = command,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
            catch
            {
                return "";
            }
        }


        public static void ClickAtTouchPositionWithHexa(string hexX, string hexY)
        {
            int x = int.Parse(hexX, System.Globalization.NumberStyles.HexNumber);
            int y = int.Parse(hexY, System.Globalization.NumberStyles.HexNumber);

            string adbCommand = $"shell input tap {x} {y}";
            ExecuteAdbCommand(adbCommand);
        }


        public static void TrackTouchEvents()
        {
            string command = $"shell getevent -lt {InputDevice}"; // Verwende getevent ohne -lp für Live-Daten
            Console.WriteLine("Starte die Erfassung von Touch-Ereignissen...");

            try
            {
                Process process = new();
                process.StartInfo.FileName = AdbPath;
                process.StartInfo.Arguments = command;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine(args.Data); // Direkt auf der Konsole ausgeben
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Erfassung der Touch-Ereignisse: {ex.Message}");
            }
        }


        public static void StartAdbServer()
        {
            NoxHandler.ExecuteAdbCommand("start-server");
            NoxHandler.ExecuteAdbCommand($"connect {NoxHandler.DeviceIP}");
        }


        public static void StopAdbServer()
        {
            string adbCommand = $"disconnect {NoxHandler.DeviceIP}";
            string output = NoxHandler.ExecuteAdbCommand(adbCommand);
            if (output.Contains("disconnected"))
            {
            }
            else
            {
                NoxHandler.ExecuteAdbCommand("kill-server");
            }
        }


        public static void StartNoxEmulator()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = NoxHandler.NoxExePath,
                Arguments = $"launch -index:{indexOfInstanze}",
                UseShellExecute = true, // Shell verwenden
                CreateNoWindow = false // Fenster anzeigen
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Starten der Instanzen: {ex.Message}");
            }
        }


        public static void StopNoxEmulator()
        {
            var noxProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.Contains("Nox", StringComparison.OrdinalIgnoreCase));

            foreach (var process in noxProcesses)
            {
                try
                {
                    process.Kill(); // Beendet den Prozess
                    process.WaitForExit(); // Wartet, bis der Prozess vollständig beendet ist
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Beenden des Prozesses {process.ProcessName}: {ex.Message}");
                }
                Thread.Sleep(1000); // Kleine Verzögerung zwischen den Beendigungen
            }

            Thread.Sleep(10000); // Wartezeit nach dem Beenden aller Prozesse
        }


        public static void StartApp()
        {
            NoxHandler.ExecuteAdbCommand($"shell monkey -p {AppPackageName} -c android.intent.category.LAUNCHER 1");
        }


        public static void StopApp()
        {
            // Versuche, die App mit "am force-stop" zu stoppen
            NoxHandler.ExecuteAdbCommand($"shell am force-stop {AppPackageName}");

            // Hole die PID der App
            string pid = NoxHandler.ExecuteAdbCommand($"shell pidof {AppPackageName}");

            if (!string.IsNullOrEmpty(pid))
            {
                // Erzwinge das Beenden der App, falls die PID gefunden wird
                NoxHandler.ExecuteAdbCommand($"shell kill -9 {pid.Trim()}");
                //Log.L($"App {appPackageName} wurde mit PID {pid} beendet.");
            }
            else
            {
                //og.L($"PID für {appPackageName} konnte nicht gefunden werden. App läuft möglicherweise nicht.");
            }
        }

    }
}
