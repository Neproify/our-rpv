using System;
using GTANetworkAPI;

namespace roleplay.Scripts
{
    public class Phones : Script
    {
        [Command("tel", GreedyArg = true)]
        public void PhoneCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (player.activePhone == null)
            {
                player.SendNotification("~r~Nie masz żadnego aktywnego telefonu!");
                return;
            }

            string[] args = parameters.Split(" ");

            if (args.Length < 1)
                goto Usage;

            if (args[0] == "odbierz")
            {
                if (player.phoneCall == null)
                {
                    player.SendNotification("~r~Nikt do ciebie nie dzwoni. Nie możesz odebrać.");
                    return;
                }

                if (player.phoneCall.active)
                {
                    player.SendNotification("~r~Rozmowa już trwa. Nie możesz odebrać ponownie.");
                    return;
                }

                if (player.phoneCall.receiver != player)
                {
                    player.SendNotification("~r~Nie możesz odebrać. Jesteś dzwoniącym.");
                    return;
                }

                player.phoneCall.active = true;
                player.SendNotification("~r~Odebrałeś połączenie telefoniczne.");
                player.phoneCall.sender.SendNotification("~r~Rozmówca odebrał połączenie. Możecie rozmawiać.");
                return;
            }

            if (args[0] == "zakończ")
            {
                if (player.phoneCall == null)
                {
                    player.SendNotification("~r~Nie masz aktywnego połączenia. Nie możesz zakończyć.");
                    return;
                }

                player.phoneCall.active = false;

                player.phoneCall.sender.phoneCall = null;
                player.phoneCall.receiver.phoneCall = null;
                return;
            }

            if (!Int32.TryParse(args[0], out var phoneNumber))
                goto Usage;

            player.OutputMe("wyciąga telefon i dzwoni.");

            if (player.phoneCall != null)
            {
                player.SendNotification("~r~Posiadasz aktywną rozmowę telefoniczną.");
                return;
            }

            if(phoneNumber == 911) // Numer alarmowy
            {
                Items.ItemType.PhoneCall alarmCall = new Items.ItemType.PhoneCall
                {
                    senderPhone = player.activePhone.phoneNumber,
                    sender = player,
                    receiverPhone = 911,
                    active = true
                };
                player.phoneCall = alarmCall;
                player.SendChatMessage("!{{#FFFFFF}}Telefon(Operator): 911, podaj swoje zgłoszenie i lokalizację.");
                return;
            }

            var phone = Managers.ItemManager.Instance().GetPhones().Find(x => x.phoneNumber == phoneNumber);

            if (phone?.ownerType != OwnerType.Character)
            {
                player.SendPhoneIsNotRespondingNotification();
                return;
            }

            var secondPlayer = Managers.PlayerManager.Instance().GetByCharacterID(phone.ownerID);

            if (secondPlayer?.activePhone != phone)
            {
                player.SendPhoneIsNotRespondingNotification();
                return;
            }

            if (secondPlayer.phoneCall != null)
            {
                player.SendNotification("~r~Linia jest zajęta.");
                return;
            }

            Items.ItemType.PhoneCall phoneCall = new Items.ItemType.PhoneCall
            {
                sender = player,
                senderPhone = player.activePhone.phoneNumber,
                receiver = secondPlayer,
                receiverPhone = secondPlayer.activePhone.phoneNumber,
                active = false
            };

            player.phoneCall = phoneCall;
            secondPlayer.phoneCall = phoneCall;

            secondPlayer.OutputDo("Słychać dzwonek telefonu.");

            secondPlayer.SendNotification("Ktoś do ciebie dzwoni!");
            secondPlayer.SendNotification("Użyj /tel odbierz aby odebrać.");

        Usage:
            player.SendNotification("Użyj: /tel [numer telefonu/odbierz/zakończ]");
            return;
        }
    }
}
