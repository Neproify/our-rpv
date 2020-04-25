using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class Radio : Script
    {
        [Command("news", GreedyArg = true)]
        public void NewsCommand(Player client, string text)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if(!player.IsReady())
            {
                return;
            }

            if(!player.IsOnDutyOfGroupType(GroupType.Radio))
            {
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            NAPI.Chat.SendChatMessageToAll($"[RADIO]{player.formattedName}: {text}");
        }
    }
}
