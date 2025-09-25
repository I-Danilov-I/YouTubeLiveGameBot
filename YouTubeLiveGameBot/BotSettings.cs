namespace YouTubeLiveGameBot
{
    public static class BotSettings
    {
        public static string YouName { get; set; } = "Your Name";

        public static string BaseDirectory { get; private set; } = AppContext.BaseDirectory
            .Replace("\\bin\\Debug\\net8.0-windows\\", "")
            .Replace("\\bin\\Debug\\net8.0\\", "");

        public static string DeviceIP { get; private set; } = ""; // Default
        public static string InputDevice { get; private set; } = "/dev/input/event4"; // Default
        public static string PackageName { get; private set; } = "si.a4c.dwarf"; // Default

        public static string AdbPath { get; set; } = "C:\\Program Files (x86)\\Nox\\bin\\adb.exe";
        public static string NoxExePath { get; set; } = "C:\\Program Files (x86)\\Nox\\bin\\Nox.exe"; 

        public static string ScreenshotDirectory { get; private set; } = Path.Combine(BaseDirectory, "Detection\\Temp");
        public static string LocalScreenshotPath { get; private set; } = Path.Combine(ScreenshotDirectory, "screenshot.png");

        public static string SearchObjektPath { get; private set; } = Path.Combine(BaseDirectory, "Detection\\Search_Database");
       
        public static string LogFileFolderPathWithName { get; private set; } = Path.Combine(BaseDirectory, "_Logfiles", "Logs.txt");

        public static int ReconectingTimeIfAccautnUsed = 15; // Min
        public static int TimeoutAfterComand { get; set; } = 3000;
        public static int IterationsCounter { get; set; } = 0;
    }
}
