﻿using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Item
    {
        public int UID;
        public string name;
        public int type;
        public int[] properties = new int[8]; // Have no idea why 8, we should think about list maybe?
        private string _propertiesString;
        public string propertiesString
        {
            set
            {
                var temp = value.Split("|");
                for (int i = 0; i < temp.Length; i++)
                {
                    properties[i] = Convert.ToInt32(temp[i]);
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

        public bool isUsed;

        public virtual void Use(Entities.Player player)
        {
#warning Make some protection for unauthorized use
        }
    }
}