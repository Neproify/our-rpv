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

            string offerType = args[2];

            if (!int.TryParse(args[0], out var playerID) || !int.TryParse(args[1], out var price))
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
                if(!player.IsOnDutyOfGroupType(GroupType.Medical))
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień aby leczyć inne osoby!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo
                {
                    sender = player, receiver = secondPlayer, type = OfferType.Healing, price = price
                };


                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;
            }

            if(args[2] == "przedmiot")
            {
                if(args[3] == null)
                {
                    player.handle.SendNotification($"Użycie komendy: /o {playerID} {price} przedmiot [identyfikator przedmiotu.");
                    return;
                }

                if(!int.TryParse(args[3], out var itemID))
                {
                    player.handle.SendNotification($"Użycie komendy: /o {playerID} {price} przedmiot [identyfikator przedmiotu.");
                    return;
                }

                var item = Managers.ItemManager.Instance().GetByID(itemID);

                if (!player.CanUseItem(item))
                {
                    player.handle.SendNotification("~r~Nie możesz przekazać przedmiotu którego nie posiadasz!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo
                {
                    sender = player,
                    receiver = secondPlayer,
                    type = OfferType.Item,
                    price = price,
                    args = {[0] = itemID}
                };


                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;
            }

            if(args[2] == "mandat")
            {
                if(!player.IsOnDutyOfGroupType(GroupType.Police))
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień do wystawiania mandatów!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo
                {
                    sender = player,
                    receiver = secondPlayer,
                    type = OfferType.Ticket,
                    price = price,
                    args = {[0] = player.groupDuty.member.group}
                };


                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;
            }

            if(args[2] == "napraw")
            {
                if(!player.IsOnDutyOfGroupType(GroupType.Workshop))
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień do naprawiania pojazdów!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo
                {
                    sender = player,
                    receiver = secondPlayer,
                    type = OfferType.VehicleRepair,
                    price = price,
                    args = {[0] = secondPlayer.vehicle, [1] = player.groupDuty.member.group}
                };


                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;
            }

            if (args[2] == "dowod" || args[2] == "dowód")
            {
                if(!player.IsOnDutyOfGroupType(GroupType.Government))
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień do wystawiania dowodów osobistych!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo
                {
                    sender = player, receiver = secondPlayer, type = OfferType.PersonalDocument, price = price
                };


                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;
            }

            if (args[2] == "prawko")
            {
                if (!player.IsOnDutyOfGroupType(GroupType.Government))
                {
                    player.handle.SendNotification("~r~Nie masz uprawnień do wystawiania dowodów osobistych!");
                    return;
                }

                OfferInfo offerInfo = new OfferInfo
                {
                    sender = player, receiver = secondPlayer, type = OfferType.VehicleLicenseDocument, price = price
                };


                player.offerInfo = offerInfo;
                secondPlayer.offerInfo = offerInfo;
            }


            if (player.offerInfo != null)
            {
                player.handle.SendNotification("Wysłałeś ofertę.");
                secondPlayer.handle.SendNotification($"Otrzymałeś ofertę od {player.formattedName}. Więcej informacji: /o info.");
                return;
            }

            goto Usage;
        Usage:
            player.handle.SendNotification("Użycie komendy: /o [id gracza] [cena] [ulecz, przedmiot, mandat, napraw, dowód, prawko]");
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
