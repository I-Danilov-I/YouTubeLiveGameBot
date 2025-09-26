using Google.Apis.YouTube.v3;
using YouTubeLiveGameBot.NoxOperations;

namespace YouTubeLiveGameBot.YouTubeOperations
{
    internal static class ReadChat
    {
        public static async Task<string?> ReadChatFromVideoID(YouTubeService youtubeService)
        {
            Console.Write("Bitte die Video-ID deines Streams eingeben: ");
            string? videoId = Console.ReadLine();

            var videoRequest = youtubeService.Videos.List("liveStreamingDetails");
            videoRequest.Id = videoId;
            var videoResponse = await videoRequest.ExecuteAsync();

            if (videoResponse.Items.Count == 0)
            {
                Console.WriteLine("Video nicht gefunden oder kein Live-Stream aktiv.");
                return null;
            }

            string liveChatId = videoResponse.Items[0].LiveStreamingDetails.ActiveLiveChatId;
            Console.WriteLine("Live-Chat-ID: " + liveChatId);

            string? nextPageToken = null;

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
                    string text = message.Snippet.DisplayMessage;
                    Console.WriteLine($"{username}: {text}");

                    // Hier prüfen ob "Hit" geschrieben wurde
                    if (text == "Hit")
                    {
                        int i = 0;
                        while (i < 10)
                        {
                            i++;
                            BOT.ClickAtTouchPositionWithHexa("000001df", "000002ea");
                            Console.WriteLine("Im HIT!");
                        }
                    }
                }

                nextPageToken = chatResponse.NextPageToken;

                await Task.Delay(30000);
            }
        }
    }
}
