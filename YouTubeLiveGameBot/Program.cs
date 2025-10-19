namespace YouTubeLiveGameBot
{
    public static class Program
    {
        public static string videoID = "p7PfVN6KdPU";
        public static string channelID = "UCL2-DqRS5zojmDuABY9ffvg";
        public static int aktuelleAboAnzahl = 4;

        public static async Task Main()
        {
            // NoxHandler.TrackTouchEvents();
            // NoxHandler.StartNoxEmulator();
            // NoxHandler.StartAdbServer();
            // NoxHandler.StartApp();


            await YouTubeLiveHandler.AuthenticateAsync("E:\\Visual Studio Projekte\\YouTubeLiveGameBot\\YouTubeLiveGameBot\\Secret\\client_secret.json");
            var liveChatId = await YouTubeLiveHandler.GetLiveChatIdFromVideoIdAsync(videoID);
            await YouTubeLiveHandler.ReadChatAsync(liveChatId!);
            
        }
    }
}
