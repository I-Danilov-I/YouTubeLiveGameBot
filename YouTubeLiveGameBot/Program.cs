namespace YouTubeLiveGameBot
{
    public static class Program
    {
        public static async Task Main()
        {
            //NoxHandler.TrackTouchEvents();
            //NoxHandler.StartNoxEmulator();
            //NoxHandler.StartAdbServer();
            //NoxHandler.StartApp();


            await YouTubeLiveHandler.AuthenticateAsync("E:\\Visual Studio Projekte\\YouTubeLiveGameBot\\YouTubeLiveGameBot\\Secret\\client_secret.json");
            var liveChatId = await YouTubeLiveHandler.GetLiveChatIdFromVideoIdAsync();
            await YouTubeLiveHandler.ReadChatAsync(liveChatId!);         
        }
    }
}
