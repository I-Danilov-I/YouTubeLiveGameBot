using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

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
                [
                    YouTubeService.Scope.Youtube,
                    YouTubeService.Scope.YoutubeForceSsl
                ],
                "user",
                CancellationToken.None,
                new FileDataStore("YouTube.Auth.Store.FullAccess")
            );

            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "LiveChatReader"
            });

            Console.WriteLine("Authentifizierung abgeschlossen.");
        }

        // 2️⃣ LiveChat-ID vom Video holen
        public static async Task<string?> GetLiveChatIdFromVideoIdAsync(string videoId)
        {
            if (_youTubeService == null) throw new InvalidOperationException("Nicht authentifiziert.");

            var videoRequest = _youTubeService.Videos.List("liveStreamingDetails");
            videoRequest.Id = videoId;

            var videoResponse = await videoRequest.ExecuteAsync();

            if (videoResponse.Items.Count == 0)
            {
                Console.WriteLine("Video nicht gefunden.");
                return null;
            }

            var liveChatId = videoResponse.Items[0].LiveStreamingDetails?.ActiveLiveChatId;
            if (string.IsNullOrEmpty(liveChatId))
            {
                Console.WriteLine("Live-Chat nicht verfügbar. Prüfe, ob das Video live ist.");
                return null;
            }

            Console.WriteLine($"Live-Chat-ID gefunden: {liveChatId}");
            return liveChatId;
        }

        // 3️⃣ Live-Chat lesen
        public static async Task ReadChatAsync(string liveChatId)
        {
            if (_youTubeService == null) throw new InvalidOperationException("Nicht authentifiziert.");

            string? nextPageToken = null;

            while (true)
            {
                try
                {
                    var chatRequest = _youTubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
                    chatRequest.MaxResults = 200;
                    if (!string.IsNullOrEmpty(nextPageToken))
                        chatRequest.PageToken = nextPageToken;

                    var chatResponse = await chatRequest.ExecuteAsync();

                    foreach (var message in chatResponse.Items)
                    {
                        string username = message.AuthorDetails.DisplayName.ToUpper();
                        string text = message.Snippet.DisplayMessage;


                        await CommandsLiveChat.HitStandart(text, username, liveChatId); // Standard Hit
                        await CommandsLiveChat.FastHits(text, username, liveChatId);
                    }

                    nextPageToken = chatResponse.NextPageToken;
                    await Task.Delay(30000); // Warten bevor Chat wieder gelesen wird
                }
                catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Live-Chat existiert nicht mehr oder Video beendet.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Lesen des Chats: {ex.Message}");
                    await Task.Delay(10000); // Fehlerbehandlung: 10 Sekunden warten
                }
            }

            Console.WriteLine("Live-Chat beendet.");
        }

        // 4️⃣ Nachricht senden
        public static async Task SendMessageAsync(string liveChatId, string message)
        {
            if (_youTubeService == null) throw new InvalidOperationException("Nicht authentifiziert.");

            var liveChatMessage = new LiveChatMessage
            {
                Snippet = new LiveChatMessageSnippet
                {
                    LiveChatId = liveChatId,
                    Type = "textMessageEvent",
                    TextMessageDetails = new LiveChatTextMessageDetails
                    {
                        MessageText = message
                    }
                }
            };

            try
            {
                var insertRequest = _youTubeService.LiveChatMessages.Insert(liveChatMessage, "snippet");
                await insertRequest.ExecuteAsync();
                Console.WriteLine($"Nachricht gesendet: {message}");
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("Live-Chat existiert nicht mehr. Nachricht konnte nicht gesendet werden.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Senden der Nachricht: {ex.Message}");
            }
        }
    }
}
