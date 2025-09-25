using Google.Apis.YouTube.v3;


namespace YouTubeLiveGameBot.YouTubeOperations
{
    internal static class ReadChat
    {
        public static async Task ReadChatFromVideoID(YouTubeService youtubeService)
        {
            // === 3️⃣ Video-ID eintragen ===
            Console.Write("Bitte die Video-ID deines Streams eingeben: ");
            string? videoId = Console.ReadLine();

            // === 4️⃣ Live-Chat-ID abrufen ===
            var videoRequest = youtubeService.Videos.List("liveStreamingDetails");
            videoRequest.Id = videoId;
            var videoResponse = await videoRequest.ExecuteAsync();

            if (videoResponse.Items.Count == 0)
            {
                Console.WriteLine("Video nicht gefunden oder kein Live-Stream aktiv.");
                return;
            }

            string liveChatId = videoResponse.Items[0].LiveStreamingDetails.ActiveLiveChatId;
            Console.WriteLine("Live-Chat-ID: " + liveChatId);

            // === 5️⃣ Nachrichten kontinuierlich abrufen ===
            string nextPageToken = null!;

            while (true)
            {
                var chatRequest = youtubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
                chatRequest.MaxResults = 200;
                if (!string.IsNullOrEmpty(nextPageToken))
                    chatRequest.PageToken = nextPageToken;

                var chatResponse = await chatRequest.ExecuteAsync();

                foreach (var message in chatResponse.Items)
                {
                    string username = message.AuthorDetails.DisplayName;
                    string text = message.Snippet.DisplayMessage; // Hier müssen ie BEfhle Abgefangen und zu ADB Weitergeben werden!!!
                    Console.WriteLine($"{username}: {text}");
                }

                nextPageToken = chatResponse.NextPageToken;

                // Alle 30 Sekunden neue Nachrichten abrufen
                await Task.Delay(30000);
            }
        }
    }
}
