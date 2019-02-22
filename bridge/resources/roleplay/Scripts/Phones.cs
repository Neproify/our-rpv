﻿using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class Phones : Script
    {
        [Command("tel", GreedyArg = true)]
        public void PhoneCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.activePhone == null)
            {
                player.handle.SendNotification("~r~Nie masz żadnego aktywnego telefonu!");
                return;
            }

            string[] args = parameters.Split(" ");

            if (args.Length < 1)
                goto Usage;

            if (args[0] == "odbierz")
            {
                if (player.phoneCall == null)
                {
                    player.handle.SendNotification("~r~Nikt do ciebie nie dzwoni. Nie możesz odebrać.");
                    return;
                }

                if (player.phoneCall.active == true)
                {
                    player.handle.SendNotification("~r~Rozmowa już trwa. Nie możesz odebrać ponownie.");
                    return;
                }

                if (player.phoneCall.receiver != player)
                {
                    player.handle.SendNotification("~r~Nie możesz odebrać. Jesteś dzwoniącym.");
                    return;
                }

                player.phoneCall.active = true;
                player.handle.SendNotification("~r~Odebrałeś połączenie telefoniczne.");
                player.phoneCall.sender.handle.SendNotification("~r~Rozmówca odebrał połączenie. Możecie rozmawiać.");
                return;
            }

            if (args[0] == "zakończ")
            {
                if (player.phoneCall == null)
                {
                    player.handle.SendNotification("~r~Nie masz aktywnego połączenia. Nie możesz zakończyć.");
                    return;
                }

                player.phoneCall.active = false;

                player.phoneCall.sender.phoneCall = null;
                player.phoneCall.receiver.phoneCall = null;
                return;
            }

            int phoneNumber;

            if (!Int32.TryParse(args[0], out phoneNumber))
                goto Usage;

            player.OutputMe("wyciąga telefon i dzwoni.");

            if (player.phoneCall != null)
            {
                player.handle.SendNotification("~r~Posiadasz aktywną rozmowę telefoniczną.");
                return;
            }

            if(phoneNumber == 911) // Numer alarmowy
            {
                Items.ItemType.PhoneCall alarmCall = new Items.ItemType.PhoneCall();
                alarmCall.senderPhone = player.activePhone.properties[0];
                alarmCall.sender = player;
                alarmCall.receiverPhone = 911;
                alarmCall.active = true;
                player.phoneCall = alarmCall;
                player.handle.SendChatMessage($"!{{#FFFFFF}}Telefon(Operator): 911, podaj swoje zgłoszenie i lokalizację.");
                return;
            }

            var phone = Managers.ItemManager.Instance().GetByTypeAndProperty(ItemType.Phone, 0, phoneNumber);

            if (phone == null || phone?.ownerType != OwnerType.Character)
            {
                player.handle.SendNotification("~r~Telefon nie odpowiada.");
                return;
            }

            var secondPlayer = Managers.PlayerManager.Instance().GetByCharacterID(phone.ownerID);

            if (secondPlayer == null)
            {
                player.handle.SendNotification("~r~Telefon nie odpowiada.");
                return;
            }

            if (secondPlayer.activePhone != phone)
            {
                player.handle.SendNotification("~r~Telefon nie odpowiada.");
                return;
            }

            if (secondPlayer.phoneCall != null)
            {
                player.handle.SendNotification("~r~Linia jest zajęta.");
                return;
            }

            Items.ItemType.PhoneCall phoneCall = new Items.ItemType.PhoneCall();
            phoneCall.sender = player;
            phoneCall.senderPhone = player.activePhone.properties[0];
            phoneCall.receiver = secondPlayer;
            phoneCall.receiverPhone = secondPlayer.activePhone.properties[0];
            phoneCall.active = false;

            player.phoneCall = phoneCall;
            secondPlayer.phoneCall = phoneCall;

            secondPlayer.OutputDo("Słychać dzwonek telefonu.");

            secondPlayer.handle.SendNotification("Ktoś do ciebie dzwoni!");
            secondPlayer.handle.SendNotification("Użyj /tel odbierz aby odebrać.");

        Usage:
            player.handle.SendNotification("Użyj: /tel [numer telefonu/odbierz/zakończ]");
            return;
        }
    }
}