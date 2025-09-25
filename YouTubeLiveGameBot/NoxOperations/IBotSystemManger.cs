using System.Diagnostics;
using YouTubeLiveGameBot.Logging;

namespace YouTubeLiveGameBot
{
    public interface IBotSystemManger
    {
        void CheckPath();
        void Start(string appPackageName);
        void Stop(string appPackageName);
        bool WaitForReady();
        bool IsRunning();
    }

    internal static class BOT
    {
        public static void CloseAll(List<IBotSystemManger> botControls)
        {
            foreach (var control in botControls.AsEnumerable().Reverse())
            {
                control.Stop(BotSettings.PackageName);
                Thread.Sleep(BotSettings.TimeoutAfterComand);
            }

            Thread.Sleep(BotSettings.TimeoutAfterComand); // Optional, falls Pause nach der Schleife benötigt wird
            Print.DisplayCountdownInline(BotSettings.ReconectingTimeIfAccautnUsed * 60); // Zeit in Sekunden
        }

        internal static readonly char[] separator = ['\n', '\r'];

        public static string GetInstalledPackages()
        {
            // Führe den ADB-Befehl aus, um die Liste der Pakete abzurufen
            string output = BOT.ExecuteAdbCommand("shell pm list packages");

            if (string.IsNullOrEmpty(output))
            {
                Log.L("Keine Pakete gefunden oder Fehler beim Abrufen der Pakete.");
                return "";
            }

            // Entferne das Präfix "package:" aus der Ausgabe
            var packageList = output.Replace("package:", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);

            // Protokolliere die Pakete zur Überprüfung
            foreach (var package in packageList)
            {
                Log.L($"Gefundenes Paket: {package.Trim()}");
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
                        FileName = BotSettings.AdbPath,
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

    }




    internal static class BotStartAndStabilityManager
    {
        internal static void Check(List<IBotSystemManger> controls)
        {
            foreach (var control in controls)
            {
                control.CheckPath();

                if (control.IsRunning())
                {
                    continue;
                }

                if (control.GetType().Name == "Adb")
                {
                    Log.L($"Starte {control.GetType().Name} ...");
                    control.Start("");
                }
                else if (control.GetType().Name == "Nox")
                {
                    Log.L($"Starte {control.GetType().Name} ...");
                    control.Start("0");
                    Thread.Sleep(7000);
                }
                else if (control.GetType().Name == "App")
                {
                    Log.L($"Starte {control.GetType().Name} ...");
                    control.Start(BotSettings.PackageName);
                    Thread.Sleep(1000 * 60);
                }

                if (control.WaitForReady())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Log.L($"{control.GetType().Name} ist bereit.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.L($"{control.GetType().Name} konnte nicht gestartet werden. Stoppe und versuche erneut.");
                    Console.ResetColor();
                    control.Stop("0");
                    Thread.Sleep(7000);
                    continue;
                }
            }
        }
    }




    internal class Adb : IBotSystemManger
    {

        public void CheckPath()
        {
            while (true)
            {
                if (!File.Exists(BotSettings.NoxExePath))
                {
                    Log.L($"Pfad nicht gefunden: [{BotSettings.NoxExePath}]");
                    Log.L($"Bitte geben sie den Pfad mauell ein: ");
                    BotSettings.AdbPath = Console.ReadLine()?.Trim('"')!;
                    Log.L(" ");
                    return;
                }
                else { break; }
            }
        }

        public void Start(string i)
        {
            BOT.ExecuteAdbCommand("start-server");
            BOT.ExecuteAdbCommand($"connect {BotSettings.DeviceIP}");
        }

        public void Stop(string i)
        {
            string adbCommand = $"disconnect {BotSettings.DeviceIP}";
            string output = BOT.ExecuteAdbCommand(adbCommand);
            if (output.Contains("disconnected"))
            {
            }
            else
            {
                BOT.ExecuteAdbCommand("kill-server");
            }
        }

        public bool WaitForReady()
        {
            for (int i = 0; i < 5; i++) 
            {
                if (IsRunning())
                {
                    return true;
                }

                Thread.Sleep(7000);
            }

            return false;
        }

        public bool IsRunning()
        {
            string output = BOT.ExecuteAdbCommand("devices");
            if (!string.IsNullOrEmpty(output) && !output.Contains("error"))
            {
                return true;
            }
            return false;
        }
    }




    public class Nox : IBotSystemManger
    {
        public void CheckPath()
        {
            while (true)
            {
                if (!File.Exists(BotSettings.NoxExePath))
                {
                    Log.L($"Pfad nicht gefunden: [{BotSettings.NoxExePath}]");
                    Log.L($"Bitte geben sie den Pfad mauell ein: ");
                    BotSettings.NoxExePath = Console.ReadLine()?.Trim('"')!;
                    Log.L(" ");
                    return;
                }
                else { break; }
            }
        }

        public void Start(string indexOfInstanze)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = BotSettings.NoxExePath,
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
                Log.L($"Fehler beim Starten der Instanzen: {ex.Message}");
            }
        }

        public void Stop(string i)
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
                    Log.L($"Fehler beim Beenden des Prozesses {process.ProcessName}: {ex.Message}");
                }
                Thread.Sleep(1000); // Kleine Verzögerung zwischen den Beendigungen
            }

            Thread.Sleep(10000); // Wartezeit nach dem Beenden aller Prozesse
        }

        public bool WaitForReady()
        {
            for (int i = 0; i < 10; i++) // 10 Versuche
            {
                if (IsRunning())
                {
                    Thread.Sleep(7000);
                    return true;
                }

                Thread.Sleep(7000);
            }

            return false;
        }

        public bool IsRunning()
        {
            Process[] noxProcesses = Process.GetProcessesByName("Nox");
            Process[] noxVmProcesses = Process.GetProcessesByName("NoxVMHandle");
            string output = BOT.ExecuteAdbCommand("shell getprop init.svc.bootanim");

            bool processesRunning = noxProcesses.Length > 0 || noxVmProcesses.Length > 0;
            bool bootAnimStopped = output.Contains("stopped");

            return processesRunning && bootAnimStopped;
        }

    }




    internal class App : IBotSystemManger
    {
        public void CheckPath(){}

        public void Start(string appPackageName)
        {
            BOT.ExecuteAdbCommand($"shell monkey -p {appPackageName} -c android.intent.category.LAUNCHER 1");
        }

        public void Stop(string appPackageName)
        {
            // Versuche, die App mit "am force-stop" zu stoppen
            BOT.ExecuteAdbCommand($"shell am force-stop {appPackageName}");

            // Hole die PID der App
            string pid = BOT.ExecuteAdbCommand($"shell pidof {appPackageName}");

            if (!string.IsNullOrEmpty(pid))
            {
                // Erzwinge das Beenden der App, falls die PID gefunden wird
                BOT.ExecuteAdbCommand($"shell kill -9 {pid.Trim()}");
                //Log.L($"App {appPackageName} wurde mit PID {pid} beendet.");
            }
            else
            {
                //og.L($"PID für {appPackageName} konnte nicht gefunden werden. App läuft möglicherweise nicht.");
            }
        }


        public bool WaitForReady()
        {
            for (int i = 0; i < 12; i++) // 5 Versuche
            {
                if (IsRunning())
                {
                    return true;
                }
                Thread.Sleep(7000); // 1 Sekunde warten
            }

            return false;
        }

        public bool IsRunning()
        {
            string output = BOT.ExecuteAdbCommand($"shell pidof {BotSettings.PackageName}");

            if (!string.IsNullOrEmpty(output))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
