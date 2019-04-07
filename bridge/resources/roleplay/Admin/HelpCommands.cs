using GTANetworkAPI;

namespace roleplay.Admin
{
    public class HelpCommands : Script
    {
        [Command("apomoc")]
        public void HelpCommand(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if(!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            player.SendChatMessage("====LISTA KOMEND ADMINISTRACYJNYCH====");
            player.SendChatMessage("/agracz, /apojazd, /aprzedmiot, /agrupa, /aobiekt, /abudynek");
            player.SendChatMessage("/ado, /gooc, /kick, /ban, /aj");
            player.SendChatMessage("====KONIEC LISTY====");
        }
    }
}
