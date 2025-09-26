using YouTubeLiveGameBot.NoxOperations;

namespace YouTubeLiveGameBot
{
    public static class Program
    {
        public static List<IBotSystemManger> I_BotControls { get; private set; } = [];


        public static async Task Main()
        {
            //_Devlop.DevlopHelper.TrackTouchEvents();
            
            I_BotControls = [new Adb(), new Nox(), new App()];
            BotStartAndStabilityManager.Check(I_BotControls);

            Console.WriteLine(Path.GetFullPath("YouTube.Auth.Store"));
            var youTubeService = await YouTubeOperations.AuthentifizierungsProzes.Authentifizierung();
            await YouTubeOperations.ReadChat.ReadChatFromVideoID(youTubeService);         
            while (true)
            {
                BOT.ClickAtTouchPositionWithHexa("000001df", "000002ea");
                Console.WriteLine("HIT!");
                Thread.Sleep(100);
            }

        }


    }
}
