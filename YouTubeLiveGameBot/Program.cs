namespace YouTubeLiveGameBot
{
    public static class Program
    {
        public static List<IBotSystemManger> I_BotControls { get; private set; } = [];


        public static async Task Main()
        {
            //I_BotControls = [new Adb(), new Nox(), new App()];
            //BotStartAndStabilityManager.Check(I_BotControls);
            Console.WriteLine(Path.GetFullPath("YouTube.Auth.Store"));

            var youTubeService = await YouTubeOperations.AuthentifizierungsProzes.Authentifizierung();
            await YouTubeOperations.ReadChat.ReadChatFromVideoID(youTubeService);         
            //_Devlop.DevlopHelper.TrackTouchEvents();
            while (true)
            {
                ClickAtTouchPositionWithHexa("000001df", "000002ea");
                Console.WriteLine("I´m HIT!!!");
                Thread.Sleep(100);
            }

        }




        public static void ClickAtTouchPositionWithHexa(string hexX, string hexY)
        {
            int x = int.Parse(hexX, System.Globalization.NumberStyles.HexNumber);
            int y = int.Parse(hexY, System.Globalization.NumberStyles.HexNumber);

            string adbCommand = $"shell input tap {x} {y}";
            BOT.ExecuteAdbCommand(adbCommand);
            //Thread.Sleep(BotSettings.TimeoutAfterComand);
        }
    }
}
