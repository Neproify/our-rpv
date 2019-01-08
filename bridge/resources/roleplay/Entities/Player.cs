using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace roleplay.Entities
{
    public class Player
    {
        public bool isLogged;

        public Client handle;
        public GlobalInfo globalInfo;
        public Character character;
    }

    public class GlobalInfo
    {
        public int UID;
        public string name;
    }

    public class Character
    {
        public int UID;
        public int GID;
        public string name;
    }
}
