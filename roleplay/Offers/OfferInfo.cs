using MongoDB.Bson;

namespace roleplay.Offers
{
    public class OfferInfo
    {
        public Entities.Player sender;
        public Entities.Player receiver;

        public OfferType type;
        public int price;

        public object[] args;

        public void ShowInfo(Entities.Player player)
        {
            player.SendChatMessage("====Dane oferty====");
            player.SendChatMessage($"Typ: x, nadawca: {sender.formattedName}, odbiorca: {receiver.formattedName}.");
            player.SendChatMessage($"Cena: ${price}");
        }

        public void Accept(Entities.Player player)
        {
            if (receiver != player)
                return;

            if(sender.position.DistanceTo(receiver.position) > 10f)
            {
                sender.SendNotification("~r~Oferta została zaakceptowana, ale znajdujecie się za daleko od siebie.");
                receiver.SendNotification("~r~Oferta została zaakceptowana, ale znajdujecie się za daleko od siebie.");
                return;
            }

            if(type == OfferType.Healing)
            {
                if (!sender.SendMoneyTo(receiver, price))
                    return;

                receiver.health = 100;
                sender.SendNotification($"Uleczyłeś {receiver.formattedName}.");
                receiver.SendNotification($"Zostałeś uleczony przez {sender.formattedName}.");
            }

            if(type == OfferType.Item)
            {
                var item = Managers.ItemManager.Instance().GetByID(ObjectId.Parse(args[1].ToString()));

                if(!sender.CanUseItem(item))
                {
                    return;
                }

                if(item.isUsed)
                {
                    item.Use(sender);
                }

                item.ChangeOwner(OwnerType.Character, receiver.character.UID);
                sender.SendNotification($"Sprzedałeś przedmiot {item.name} graczu {receiver.formattedName}.");
                receiver.SendNotification($"Kupiłeś przedmiot {item.name} od gracza {sender.formattedName}.");
            }

            if(type == OfferType.Ticket)
            {
                if (!receiver.SendMoneyTo((Entities.Group)args[0], price))
                    return;

                sender.SendNotification($"Gracz {receiver.formattedName} zapłacił mandat w wysokości {price}.");
                receiver.SendNotification($"Zapłaciłeś mandat w wysokości {price} wystawiony przez gracza {sender.formattedName}");
            }

            if(type == OfferType.VehicleRepair)
            {
                Entities.Vehicle vehicle = (Entities.Vehicle)args[0];

                if (sender.position.DistanceTo(vehicle.position) > 5)
                {
                    sender.SendNotification("Jesteś za daleko od pojazdu który próbujesz naprawić.");
                    return;
                }

                if (!receiver.SendMoneyTo((Entities.Group)args[1], price))
                    return;

                vehicle.Repair();

                sender.SendNotification($"Naprawiłeś pojazd gracza {receiver.formattedName}.");
                receiver.SendNotification($"Gracz {sender.formattedName} naprawił twój pojazd.");
            }

            if(type == OfferType.PersonalDocument)
            {
                var item = Managers.ItemManager.Instance().CreateItem("Dowód osobisty", ItemType.Document) as Items.ItemType.Document;
                item.ChangeOwner(OwnerType.Character, receiver.character.UID);
                item.documentType = DocumentType.Personal;
                item.personID = receiver.character.UID;
                item.Save();

                sender.SendNotification($"Wystawiłeś dowód osobisty dla {receiver.formattedName}.");
                receiver.SendNotification($"Gracz {sender.formattedName} wyrobił ci dowód osobisty.");
            }

            if (type == OfferType.VehicleLicenseDocument)
            {
                var item = Managers.ItemManager.Instance().CreateItem("Prawo jazdy", ItemType.Document) as Items.ItemType.Document;
                item.ChangeOwner(OwnerType.Character, receiver.character.UID);
                item.documentType = DocumentType.VehicleLicense;
                item.personID = receiver.character.UID;
                item.Save();

                sender.SendNotification($"Wystawiłeś prawo jazdy dla {receiver.formattedName}.");
                receiver.SendNotification($"Gracz {sender.formattedName} wyrobił ci prawo jazdy.");
            }

            if (type == OfferType.VehicleSell)
            {
                var vehicle = (Entities.Vehicle)args[0];

                if (!receiver.SendMoneyTo(sender, price))
                    return;

                vehicle.vehicleData.ownerType = OwnerType.Character;
                vehicle.vehicleData.ownerID = receiver.character.UID;

                sender.SendNotification($"Sprzedałeś pojazd za ~g~${price} graczu {receiver.formattedName}.");
                receiver.SendNotification($"Kupiłeś pojazd za ~g~${price} od gracza {sender.formattedName}.");
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
                sender.SendNotification("Anulowałeś swoją ofertę.");
                receiver.SendNotification($"{sender.formattedName} anulował swoją ofertę.");
            }

            if(player == receiver)
            {
                sender.SendNotification($"{receiver.formattedName} odrzucił twoją ofertę.");
                receiver.SendNotification($"Odrzuciłeś ofertę złożoną przez {sender.formattedName}.");
            }

            sender.offerInfo = null;
            receiver.offerInfo = null;
        }
    }
}
