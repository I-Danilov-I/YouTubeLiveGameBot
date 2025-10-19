namespace YouTubeLiveGameBot
{
    internal class CommandsLiveChat
    {
        public static async Task HitStandart(string text, string username, string liveChatId)
        {
            if (text.Equals("Hit", StringComparison.OrdinalIgnoreCase))
            {
                await YouTubeLiveHandler.SendMessageAsync(liveChatId, $"USER <<<[ {username} ]>>> Hitting 3x...");

                for (int i = 0; i < 3; i++)
                {
                    NoxHandler.ClickAtTouchPositionWithHexa("000001df", "000002ea");
                    await Task.Delay(50);
                }
            }
        }


        public static async Task FastHits(string text, string username, string liveChatId)
        {
            if (text.Equals("Chat Control Game", StringComparison.OrdinalIgnoreCase))
            {
                await YouTubeLiveHandler.SendMessageAsync(liveChatId, $"USER <<<[ {username} ]>>> Fast Hitting 30x...");

                for (int i = 0; i < 30; i++)
                {
                    NoxHandler.ClickAtTouchPositionWithHexa("000001df", "000002ea");
                    await Task.Delay(50);
                }
            }
        }


    }
}
