using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using YouTubeLiveGameBot.Helper;

namespace YouTubeLiveGameBot.YouTubeOperations
{
    internal static class AuthentifizierungsProzes
    {
        public static async Task<YouTubeService> Authentifizierung()
        {
            string path = PathHelperManagement.PathManagement();

            // === 1️⃣ OAuth-Authentifizierung ===
            UserCredential credential;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
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
            YouTubeService youTubeService = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "LiveChatReader"
            });
            var youtubeService = youTubeService;
            return youTubeService;
        }
    }
}
