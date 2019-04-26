﻿using roleplay.Entities;
using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Items.ItemType
{
    public class Weapon : Item
    {
        [BsonIgnore]
        public uint weaponHash
        {
            get
            {
                return (uint)properties["weaponhash"];
            }
            set
            {
                properties["weaponhash"] = value;
            }
        }

        [BsonIgnore]
        public int ammo
        {
            get
            {
                return (int)properties["ammo"];
            }
            set
            {
                properties["ammo"] = value;
            }
        }

        [BsonIgnore]
        public ObjectId flaggedGroupId
        {
            get
            {
                return ObjectId.Parse((string)properties["flaggedgroupid"]);
            }
            set
            {
                properties["flaggedgroupid"] = value;
            }
        }

        public override bool Use(Player player)
        {
            if (!base.Use(player))
                return false;

            if (!isUsed)
            {
                if (player.GetWeapons().Find(x => x.weaponHash == weaponHash && x.isUsed) != null)
                {
                    player.SendNotification("~r~Posiadasz już wyjętą broń tego typu.");
                    return false;
                }

                if (ammo <= 0)
                {
                    player.SendNotification("~r~Wybrana broń nie posiada amunicji.");
                    return false;
                }

                if (flaggedGroupId != null && flaggedGroupId != ObjectId.Empty)
                {
                    if (!player.IsOnDutyOfGroupID(flaggedGroupId))
                    {
                        player.SendNotification("~r~Nie masz uprawnień do użycia tej broni. Jest ona podpisana pod grupę.");
                        return false;
                    }
                }

                player.GiveWeapon((WeaponHash)weaponHash, ammo);
                isUsed = true;
                player.OutputMe($"wyciąga {name}.");
            }
            else
            {
                player.RemoveWeapon((WeaponHash)weaponHash);
                isUsed = false;
                player.OutputMe($"chowa {name}.");
                Save();
            }

            player.TriggerEvent("HidePlayerItems");

            return true;
        }
    }
}
