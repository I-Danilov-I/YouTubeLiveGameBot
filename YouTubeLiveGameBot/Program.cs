namespace YouTubeLiveGameBot
{
    public static class Program
    {
        public static string videoID = "8LYRm3LFDps";


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
