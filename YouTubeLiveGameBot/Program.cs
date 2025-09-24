namespace YouTubeLiveGameBot
{
    public static class Program
    {
        public static async Task Main()
        {
                var youTubeService = await YouTubeOperations.AuthentifizierungsProzes.Authentifizierung();
                await YouTubeOperations.ReadChat.ReadChatFromVideoID(youTubeService);
        }
    }
}
