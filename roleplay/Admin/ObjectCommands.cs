using System;
using GTANetworkAPI;
using MongoDB.Bson;

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
                player.SendNoPermissionsToCommandNotification();
                return;
            }

            string[] args = parameters.Split(" ");

            if (args[0] == "stworz" || args[0] == "stwórz")
            {
                Entities.Object createdObject = Managers.ObjectManager.Instance().CreateObject();
                player.SendChatMessage($"ID stworzonego obiektu: {createdObject.UID}.");
                player.selectedEntities.selectedObject = createdObject;
                return;
            }

            if (args.Length < 2)
                goto Usage;

            var objectID = AdminHelpers.GetObjectId(args[0], EntityType.Object, player);

            if (objectID == ObjectId.Empty)
                goto Usage;

            Entities.Object @object = Managers.ObjectManager.Instance().GetByID(objectID);

            if (@object == null)
            {
                player.SendNotification("~r~Nie znaleziono obiektu o podanym identyfikatorze.");
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
                    player.SendInvalidOwnerTypeNotification();
                    return;
                }

                var ownerID = AdminHelpers.GetObjectId(args[3], Utils.GetEntityTypeFromOwnerType(type), player);

                @object.ownerType = type;

                @object.ownerID = ownerID;

                @object.Save();
                return;
            }

            if (args[1] == "tpto")
            {
                if (!@object.IsSpawned())
                    return;

                player.position = @object.handle.Position;

                return;
            }

            if (args[1] == "tphere")
            {
                if (!@object.IsSpawned())
                    return;

                @object.handle.Position = player.position;

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
            player.SendUsageNotification("Użycie komendy: /aobiekt [id obiektu/stwórz] [właściciel, tpto, tphere, model]");
            return;
        OwnerUsage:
            player.SendUsageNotification($"Użycie komendy: /aobiekt {objectID} właściciel [{Utils.GetOwnerTypes()}] [identyfikator właściciela]");
            return;
        ModelUsage:
            player.SendUsageNotification($"Użycie komendy: /aobiekt {objectID} model [hash modelu]");
            return;
        }
    }
}
