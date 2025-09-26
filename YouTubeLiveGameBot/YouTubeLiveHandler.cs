using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace YouTubeLiveGameBot
{
    internal static class YouTubeLiveHandler
    {
        private static YouTubeService? _youTubeService;

        // 1️⃣ Authentifizierung
        public static async Task AuthenticateAsync(string clientSecretPath)
        {
            using var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read);
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                [YouTubeService.Scope.YoutubeReadonly],
                "user",
                CancellationToken.None,
                new FileDataStore("YouTube.Auth.Store")
            );

            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "LiveChatReader"
            });
        }

        // 2️⃣ LiveChat-ID holen
        public static async Task<string?> GetLiveChatIdFromVideoIdAsync()
        {
            if (_youTubeService == null) throw new InvalidOperationException("Nicht authentifiziert.");

            Console.Write("Bitte die Video-ID eingeben: ");
            string videoId = Console.ReadLine()!;
            var videoRequest = _youTubeService.Videos.List("liveStreamingDetails");
            videoRequest.Id = videoId;
            var videoResponse = await videoRequest.ExecuteAsync();

            if (videoResponse.Items.Count == 0)
            {
                Console.WriteLine("Video nicht gefunden oder kein Live-Stream aktiv.");
                return null;
            }

            return videoResponse.Items[0].LiveStreamingDetails.ActiveLiveChatId;
        }

        // 3️⃣ Chat lesen
        public static async Task ReadChatAsync(string liveChatId)
        {
            if (_youTubeService == null) throw new InvalidOperationException("Nicht authentifiziert.");

            string? nextPageToken = null;

            while (true)
            {
                var chatRequest = _youTubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
                chatRequest.MaxResults = 200;
                if (!string.IsNullOrEmpty(nextPageToken))
                    chatRequest.PageToken = nextPageToken;

                var chatResponse = await chatRequest.ExecuteAsync();

                foreach (var message in chatResponse.Items)
                {
                    string username = message.AuthorDetails.DisplayName;
                    string text = message.Snippet.DisplayMessage;
                    Console.WriteLine($"{username}: {text}");

                    if (text == "Hit")
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            NoxHandler.ClickAtTouchPositionWithHexa("000001df", "000002ea");
                            Console.WriteLine($"{username}: HIT!");
                        }
                    }
                }

                nextPageToken = chatResponse.NextPageToken;
                await Task.Delay(30000);
            }
        }
    }
}
