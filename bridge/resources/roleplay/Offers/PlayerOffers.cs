using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Offers
{
    public class PlayerOffers : Script
    {
        [Command("o", GreedyArg = true)]
        public void OfferCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (parameters == null)
            {
                if(player.offerInfo != null)
                {
                    goto UsageActiveOffer;
                }
                goto Usage;
            }

            string[] args = parameters.Split(" ");

            if (player.offerInfo != null)
            {
                goto InfoAndManagement;
            }

            if (args.Length < 3)
            {
                goto Usage;
            }

            int playerID, price;
            string offerType = args[2];

            if (!int.TryParse(args[0], out playerID) || !int.TryParse(args[1], out price))
            {
                goto Usage;
            }

            var secondPlayer = Managers.PlayerManager.Instance().GetByID(playerID);

            if (secondPlayer == null || !secondPlayer.isLogged || secondPlayer.character == null)
            {
                player.handle.SendNotification("~r~Podałeś nieprawidłowy identyfikator gracza!");
                return;
            }

            if (player.handle.Position.DistanceTo(secondPlayer.handle.Position) > 10f)
            {
                player.handle.SendNotification("~r~Gracz znajduje się za daleko!");
                return;
            }

            if (player.offerInfo != null)
            {
                player.handle.SendNotification("~r~Masz już aktywną ofertę. Nie możesz złożyć następnej.");
                return;
            }

            if (secondPlayer.offerInfo != null)
            {
                player.handle.SendNotification("~r~Gracz ma już aktywną ofertę. Nie możesz złożyć następnej.");
                return;
            }

            if (args[2] == "ulecz")
            {
                if(!player.IsOnDutyOfGroupType(Groups.GroupType.Medical))
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień aby leczyć inne osoby!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo();

                offerInfo.sender = player;
                offerInfo.receiver = secondPlayer;
                offerInfo.type = OfferType.Healing;
                offerInfo.price = price;

                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;

                player.handle.SendNotification("Wysłałeś ofertę.");
                secondPlayer.handle.SendNotification($"Otrzymałeś ofertę od {player.formattedName}. Więcej informacji: /o info.");
            }

            goto Usage;
        Usage:
            player.handle.SendNotification("Użycie komendy: /o [id gracza] [cena] [ulecz]");
            return;

        InfoAndManagement:
            if(args[0] == "info")
            {
                player.offerInfo.ShowInfo(player);
                return;
            }

            if(args[0] == "akceptuj")
            {
                player.offerInfo.Accept(player);
                return;
            }

            if(args[0] == "odrzuc")
            {
                player.offerInfo.Reject(player);
                return;
            }

            goto UsageActiveOffer;

        UsageActiveOffer:
            player.handle.SendNotification("Użycie komendy: /o [info, akceptuj, odrzuc]");
            return;
        }
    }
}
