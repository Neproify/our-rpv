using System;
using System.Collections.Generic;
using GTANetworkAPI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace roleplay.Entities
{
    public class Item
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("name")]
        public string name;

        [BsonElement("type")]
        public ItemType type;

        [BsonIgnore]
        public int[] properties = new int[8]; // Have no idea why 8, we should think about list maybe?

        [BsonElement("properties")]
        public string propertiesString
        {
            set
            {
                if (value.Length > 0)
                {
                    var temp = value.Split("|");
                    for (int i = 0; i < temp.Length; i++)
                    {
                        properties[i] = Convert.ToInt32(temp[i]);
                    }
                }
            }
            get => string.Join("|", properties);
        }

        [BsonElement("ownertype")]
        public OwnerType ownerType;

        [BsonElement("ownerid")]
        public ObjectId ownerID;

        [BsonElement("groundposition")]
        public Vector3 position;

        [BsonIgnore]
        public bool isUsed;
        [BsonIgnore]
        public GTANetworkAPI.Object objectHandle;

        public Item()
        {
            if(ownerType == OwnerType.World)
            {
                Spawn();
            }
        }

        public virtual bool Use(Player player)
        {
            return player.CanUseItem(this);
        }

        public virtual void Spawn()
        {
            Unspawn();

            objectHandle = NAPI.Object.CreateObject(1688540826, position, new Vector3(), 255, 0);
        }

        public virtual void Unspawn()
        {
            if (objectHandle != null)
                NAPI.Entity.DeleteEntity(objectHandle);
        }

        public virtual void ChangeOwner(OwnerType newOwnerType, ObjectId newOwnerID)
        {
            Entities.Player playerWhosEquipmentShouldBeReloaded = null;

            if(ownerType == OwnerType.Character)
            {
                var player = Managers.PlayerManager.Instance().GetByCharacterID(ownerID);

                if (isUsed)
                {

                    if (player == null)
                    {
                        isUsed = false; // must be bug :(
                    }
                    else
                    {
                        Use(player);
                    }
                }

                playerWhosEquipmentShouldBeReloaded = player;
            }

            Unspawn();

            Managers.ItemManager.Instance().Remove(this);
            ownerType = newOwnerType;
            ownerID = newOwnerID;
            Managers.ItemManager.Instance().Add(this);

            if(ownerType == OwnerType.World)
            {
                Spawn();
            }

			playerWhosEquipmentShouldBeReloaded?.ReloadItems();
        }

        public virtual void Save()
        {
            var collection = Database.Instance().GetGameDatabase().GetCollection<Item>("items");
            var builder = new MongoDB.Driver.FilterDefinitionBuilder<Item>();
            var filter = builder.Where(x => x.UID == this.UID);
            collection.FindOneAndReplace<Item>(filter, this);
        }
    }
}
