using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Offers
{
    public enum OfferType
    {
        None,
        Healing
    }

    public class OfferInfo
    {
        public Entities.Player sender;
        public Entities.Player receiver;

        public OfferType type;
        public int price;

        public void ShowInfo(Entities.Player player)
        {
            player.handle.SendChatMessage("====Dane oferty====");
            player.handle.SendChatMessage($"Typ: x, nadawca: {sender.formattedName}, odbiorca: {receiver.formattedName}.");
            player.handle.SendChatMessage($"Cena: ${price}");
        }

        public void Accept(Entities.Player player)
        {
            if (receiver != player)
                return;

            if(sender.handle.Position.DistanceTo(receiver.handle.Position) > 10f)
            {
                sender.handle.SendNotification("~r~Oferta została zaakceptowana, ale znajdujecie się za daleko od siebie.");
                receiver.handle.SendNotification("~r~Oferta została zaakceptowana, ale znajdujecie się za daleko od siebie.");
                return;
            }

            if(type == OfferType.Healing)
            {
                if (!sender.SendMoneyTo(receiver, price))
                    return;

                receiver.handle.Health = 100;
                sender.handle.SendNotification($"Uleczyłeś {receiver.formattedName}.");
                receiver.handle.SendNotification($"Zostałeś uleczony przez {sender.formattedName}.");
            }

            sender.offerInfo = null;
            receiver.offerInfo = null;
        }

        public void Reject(Entities.Player player)
        {
            if (receiver != player && sender != player)
                return;

            if(player == sender)
            {
                sender.handle.SendNotification("Anulowałeś swoją ofertę.");
                receiver.handle.SendNotification($"{sender.formattedName} anulował swoją ofertę.");
            }

            if(player == receiver)
            {
                sender.handle.SendNotification($"{receiver.formattedName} odrzucił twoją ofertę.");
                receiver.handle.SendNotification($"Odrzuciłeś ofertę złożoną przez {sender.formattedName}.");
            }

            sender.offerInfo = null;
            receiver.offerInfo = null;
        }
    }
}
