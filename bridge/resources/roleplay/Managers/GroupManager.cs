﻿using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Managers
{
    public class GroupManager
    {
        public List<Entities.Group> groups = new List<Entities.Group>();

        private static GroupManager _instance;
        public static GroupManager Instance()
        {
            if (_instance == null)
                _instance = new GroupManager();
            return _instance;
        }

        public void Add(Entities.Group group)
        {
            groups.Add(group);
        }

        public void SaveAll()
        {
            groups.ForEach(x => x.Save());
        }

        public List<Entities.Group> GetPlayerGroups(Entities.Player player)
        {
            return groups.FindAll(x => x.members.Find(y => y.charID == player.character.UID) != null);
        }

        public void LoadFromDatabase()
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_groups`";
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var group = new Entities.Group
                {
                    UID = reader.GetInt32("UID"),
                    name = reader.GetString("name"),
                    bank = reader.GetInt32("bank"),
                    leaderRank = reader.GetInt32("leaderRank"),
                    leaderID = reader.GetInt32("leaderID"),
                    type = reader.GetInt32("type"),
                    specialPermissions = reader.GetInt32("specialPermissions")
                };

                Add(group);
            }

            reader.Close();

            groups.ForEach(x => x.LoadRanks());
            groups.ForEach(x => x.LoadMembers());
        }
    }
}