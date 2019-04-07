using System;
using GTANetworkAPI;

namespace roleplay.Admin
{
    public class ObjectCommands : Script
    {
        [Command("aobiekt", GreedyArg = true)]
        public void AdminObjectsCommand(Client client, string parameters)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            if (!player.IsAdminOfLevel(AdminLevel.Supporter))
            {
                player.handle.SendNotification("~r~Nie masz uprawnień do użycia tej komendy!");
                return;
            }

            string[] args = parameters.Split(" ");

            if (args[0] == "stworz" || args[0] == "stwórz")
            {
                Entities.Object createdObject = Managers.ObjectManager.Instance().CreateObject();
                player.handle.SendNotification($"ID stworzonego pojazdu: {createdObject.UID}.");
                return;
            }

            if (args.Length < 2)
                goto Usage;

            if (!Int32.TryParse(args[0], out var objectID))
                goto Usage;

            Entities.Object @object = Managers.ObjectManager.Instance().GetByID(objectID);
            if (@object == null)
            {
                player.handle.SendNotification("~r~Nie znaleziono obiektu o podanym identyfikatorze.");
                return;
            }

            if (args[1] == "wlasciciel" || args[1] == "właściciel")
            {
                if (args.Length != 4)
                {
                    goto OwnerUsage;
                }

                OwnerType type = Utils.GetOwnerTypeByName(args[2]);

                if (type == OwnerType.Invalid)
                {
                    player.handle.SendNotification("~r~Podałeś nieprawidłowy typ właściciela.");
                    return;
                }

                if (!Int32.TryParse(args[3], out var ownerID))
                {
                    goto OwnerUsage;
                }

                @object.ownerType = type;
                @object.ownerID = ownerID;

                @object.Save();
                return;
            }

            if (args[1] == "tpto")
            {
                if (!@object.IsSpawned())
                    return;

                player.handle.Position = @object.handle.Position;

                return;
            }

            if (args[1] == "tphere")
            {
                if (!@object.IsSpawned())
                    return;

                @object.handle.Position = player.handle.Position;

                return;
            }

            if (args[1] == "model")
            {
                if (args.Length != 3)
                    goto ModelUsage;

                if (!UInt32.TryParse(args[2], out var modelHash))
                    goto ModelUsage;

                @object.model = modelHash;
                NAPI.Entity.SetEntityModel(@object.handle, modelHash);

                @object.Save();
                return;
            }

        Usage:
            player.handle.SendNotification("Użycie komendy: /aobiekt [id obiektu/stwórz] [właściciel, tpto, tphere, model]");
            return;
        OwnerUsage:
            player.handle.SendNotification($"Użycie komendy: /aobiekt {objectID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator właściciela]");
            return;
        ModelUsage:
            player.handle.SendNotification($"Użycie komendy: /aobiekt {objectID} model [hash modelu]");
            return;
        }
    }
}
