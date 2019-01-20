﻿using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class HelpCommands : Script
    {
        [Command("apomoc")]
        public void HelpCommand(Client client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if(!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            player.handle.SendChatMessage("====LISTA KOMEND ADMINISTRACYJNYCH====");
            player.handle.SendChatMessage("/agracz, /apojazd, /aprzedmiot, /agrupa");
            player.handle.SendChatMessage("====KONIEC LISTY====");
        }
    }
}
