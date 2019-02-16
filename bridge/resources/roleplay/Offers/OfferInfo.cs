using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Offers
{
    public enum OfferType
    {
        None,
        Healing,
        Item,
        Ticket,
        VehicleRepair,
        PersonalDocument,
        VehicleLicenseDocument
    }

    public class OfferInfo
    {
        public Entities.Player sender;
        public Entities.Player receiver;

        public OfferType type;
        public int price;

        public object[] args;

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

            if(type == OfferType.Item)
            {
                var item = Managers.ItemManager.Instance().GetByID((int)args[0]);

                if(!sender.CanUseItem(item))
                {
                    return;
                }

                if(item.isUsed)
                {
                    item.Use(sender);
                }

                item.ChangeOwner(OwnerType.Character, receiver.character.UID);
                sender.handle.SendNotification($"Sprzedałeś przedmiot {item.name} graczu {receiver.formattedName}.");
                receiver.handle.SendNotification($"Kupiłeś przedmiot {item.name} od gracza {sender.formattedName}.");
            }

            if(type == OfferType.Ticket)
            {
                if (!receiver.SendMoneyTo((Entities.Group)args[0], price))
                    return;

                sender.handle.SendNotification($"Gracz {receiver.formattedName} zapłacił mandat w wysokości {price}.");
                receiver.handle.SendNotification($"Zapłaciłeś mandat w wysokości {price} wystawiony przez gracza {sender.formattedName}");
            }

            if(type == OfferType.VehicleRepair)
            {
                Entities.Vehicle vehicle = (Entities.Vehicle)args[0];

                if (sender.handle.Position.DistanceTo(vehicle.handle.Position) > 5)
                {
                    sender.handle.SendNotification($"Jesteś za daleko od pojazdu który próbujesz naprawić.");
                    return;
                }

                if (!receiver.SendMoneyTo((Entities.Group)args[1], price))
                    return;

                vehicle.handle.Repair();

                sender.handle.SendNotification($"Naprawiłeś pojazd gracza {receiver.formattedName}.");
                receiver.handle.SendNotification($"Gracz {sender.formattedName} naprawił twój pojazd.");
            }

            if(type == OfferType.PersonalDocument)
            {
                var item = Managers.ItemManager.Instance().CreateItem();
                item.name = "Dowód osobisty";
                item.type = ItemType.Document;
                item.propertiesString = $"{DocumentType.Personal}|{receiver.character.UID}";
                item.ChangeOwner(OwnerType.Character, receiver.character.UID);
                item.Save();

                sender.handle.SendNotification($"Wystawiłeś dowód osobisty dla {receiver.formattedName}.");
                receiver.handle.SendNotification($"Gracz {sender.formattedName} wyrobił ci dowód osobisty.");
            }

            if (type == OfferType.VehicleLicenseDocument)
            {
                var item = Managers.ItemManager.Instance().CreateItem();
                item.name = "Prawo jazdy";
                item.type = ItemType.Document;
                item.propertiesString = $"{DocumentType.VehicleLicense}|{receiver.character.UID}";
                item.ChangeOwner(OwnerType.Character, receiver.character.UID);
                item.Save();

                sender.handle.SendNotification($"Wystawiłeś prawo jazdy dla {receiver.formattedName}.");
                receiver.handle.SendNotification($"Gracz {sender.formattedName} wyrobił ci prawo jazdy.");
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
