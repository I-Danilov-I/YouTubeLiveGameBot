using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace YouTubeLiveGameBot.YouTubeOperations
{
    internal static class AuthentifizierungsProzes
    {
        public static async Task<YouTubeService?> Authentifizierung()
        {
            try
            {
                // === 1️⃣ OAuth-Authentifizierung ===
                UserCredential credential;
                using (var stream = new FileStream(
                    "E:\\Visual Studio Projekte\\YouTubeChatBot\\client_secret.json",
                    FileMode.Open,
                    FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        [YouTubeService.Scope.YoutubeReadonly],
                        "user",
                        CancellationToken.None,
                        new FileDataStore("YouTube.Auth.Store")
                    );
                }

                // === 2️⃣ YouTube Service erstellen ===
                var youTubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "LiveChatReader"
                });

                return youTubeService;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler: " + ex.Message);
                return null;
            }
        }
    }
}
