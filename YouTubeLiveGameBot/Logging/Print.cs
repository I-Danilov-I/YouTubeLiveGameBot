using System.Diagnostics;

namespace YouTubeLiveGameBot.Logging
{
    internal static class Print
    {

        public static void DisplayCountdownInline(int totalSeconds)
        {
            int interval = 1; // Update-Intervall in Sekunden
            for (int remaining = totalSeconds; remaining > 0; remaining -= interval)
            {
                Console.Write($"\rWartezeit: {remaining / 60:D2} Min {remaining % 60:D2} Sek   ");
                Thread.Sleep(interval * 1000);
            }
        }

        public static void PrintByStartAutomation(string automationName)
        {
            Log.L(" ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Log.L($"[ {automationName} ]");
            Log.L($"----------------------------------------------------------------------------------------");
            Console.ResetColor();  // Setzt die Farbe nach dem Loggen zurück
        }

        public static void PrintByFinischAutomation(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log.L($"{BotSettings.YouName}, {text} ");
            Console.ResetColor();  // Setzt die Farbe nach dem Loggen zurück
        }

        public static void PrintSequens(string name)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Log.L($"{BotSettings.YouName}, {name}");
            Console.ResetColor();  // Setzt die Farbe nach dem Loggen zurück
        }

        public static void PrintByNotFinisch(string name)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log.L($"{BotSettings.YouName}, {name}");
            Console.ResetColor();  // Setzt die Farbe nach dem Loggen zurück
        }

        public static void PrintStopTime(Stopwatch time)
        {
            time.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Log.L($"Complete. Zeitdauer: {time.ElapsedMilliseconds / 1000.0:F2} Sekunden");
            Console.ResetColor();  // Setzt die Farbe nach dem Loggen zurück
        }

        public static void PrintByFirstStartProgram()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Log.L($"\n\nWhiteout Survivlal Bot | byDanyFaust");
            Log.L($"----------------------------------------------------------------------------------------"); ;
            Console.ResetColor();
        }

        public static void PrintByEndTheRound(int iterationAnzahl, Stopwatch interationDauer, Stopwatch gesamtDauer)
        {
            // Berechnung der Gesamtdauer mit TimeSpan
            var gesamtZeit = gesamtDauer.Elapsed;

            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            Log.L($"----------------------------------------------------------------------------------------");
            Log.L($"Iteration: {iterationAnzahl} " +
                  $"| Lastime: {interationDauer.Elapsed.Minutes} Min {interationDauer.Elapsed.Seconds} Sek " +
                  $"| Fulltime: {gesamtZeit.Days} Tage {gesamtZeit.Hours} Stunden {gesamtZeit.Minutes} Min {gesamtZeit.Seconds} Sek");
            Log.L($"----------------------------------------------------------------------------------------");

            Console.ResetColor();

            // Interaktionsdauer zurücksetzen
            interationDauer.Restart();
        }

    }
}
