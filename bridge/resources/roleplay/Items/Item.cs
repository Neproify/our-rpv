using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Item
    {
        public int UID;
        public string name;
        public ItemType type;
        public int[] properties = new int[8]; // Have no idea why 8, we should think about list maybe?
        private string _propertiesString;
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
                this._propertiesString = value;
            }
            get
            {
                return string.Join("|", properties);
            }
        }
        public OwnerType ownerType;
        public int ownerID;

        public Vector3 position;

        public bool isUsed;
        public GTANetworkAPI.Object objectHandle;

        public Item()
        {
            if(ownerType == OwnerType.World)
            {
                Spawn();
            }
        }

        public virtual void Use(Entities.Player player)
        {
            if (!player.CanUseItem(this))
                return;
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

        public virtual void ChangeOwner(OwnerType ownerType, int ownerID)
        {
            if(this.ownerType == OwnerType.Character)
            {
                if(this.isUsed)
                {
                    var player = Managers.PlayerManager.Instance().GetByCharacterID(this.ownerID);

                    if (player == null)
                    {
                        this.isUsed = false; // must be bug :(
                    }
                    else
                    {
                        Use(player);
                    }
                }
            }

            Unspawn();

            Managers.ItemManager.Instance().Remove(this);
            this.ownerType = ownerType;
            this.ownerID = ownerID;
            Managers.ItemManager.Instance().Add(this);

            if(this.ownerType == OwnerType.World)
            {
                Spawn();
            }
        }

        public virtual void Save()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_items` SET `name`=@name, `type`=@type, `properties`=@properties, `ownerType`=@ownerType, `ownerID`=@ownerID, `positionX`=@positionX, `positionY`=@positionY, `positionZ`=@positionZ WHERE `UID`=@UID";
            command.Prepare();

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@properties", propertiesString);
            command.Parameters.AddWithValue("@ownerType", ownerType);
            command.Parameters.AddWithValue("@ownerID", ownerID);
            command.Parameters.AddWithValue("@positionX", position.X);
            command.Parameters.AddWithValue("@positionY", position.Y);
            command.Parameters.AddWithValue("@positionZ", position.Z);
            command.Parameters.AddWithValue("@UID", UID);
            command.ExecuteNonQuery();
        }
    }
}
