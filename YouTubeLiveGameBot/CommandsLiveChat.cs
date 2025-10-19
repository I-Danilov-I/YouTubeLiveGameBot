namespace YouTubeLiveGameBot
{
    internal class CommandsLiveChat
    {
        public static async Task HitStandart(string text, string username, string liveChatId)
        {
            if (text.Equals("Hit", StringComparison.OrdinalIgnoreCase))
            {
                await YouTubeLiveHandler.SendMessageAsync(liveChatId, $" [🧑{username}]>> Hitting 3x 🪓💥...");

                for (int i = 0; i < 3; i++)
                {
                    NoxHandler.ClickAtTouchPositionWithHexa("000001c7", "000003b4");
                    await Task.Delay(100);
                }
            }
        }


        public static async Task FastHits(string text, string username, string liveChatId)
        {
            if (text.Equals("Go Berserk", StringComparison.OrdinalIgnoreCase))
            {
                await YouTubeLiveHandler.SendMessageAsync(liveChatId, $" [🧑{username}]>> Go berserk 🪓💥...");

                for (int i = 0; i < 50; i++)
                {
                    NoxHandler.ClickAtTouchPositionWithHexa("000001df", "000002ea");
                    await Task.Delay(100);
                }
            }
        }


        public static async Task UpgradeAxe()
        {
            var aboAnzahlNew = await YouTubeLiveHandler.GetSubscriberCountAsync(Program.channelID);
            Console.WriteLine("Aboanzahl: " + aboAnzahlNew);
            if (aboAnzahlNew > Program.aktuelleAboAnzahl)
            {
                // Neue Axt Erstellen 
                Console.WriteLine("Aboanzahl hat sich geändert!!!");
                Program.aktuelleAboAnzahl = aboAnzahlNew.Value;
                NoxHandler.ClickAtTouchPositionWithHexa("000000d5", "000005c0");
                await Task.Delay(4000);
                NoxHandler.ClickAtTouchPositionWithHexa("000001b5", "00000556");
                await Task.Delay(10000);
                NoxHandler.ClickAtTouchPositionWithHexa("000001b8", "00000556");
                await Task.Delay(4000);
            }
        }


    }
}
