using System;
using System.Collections.Generic;
using System.Text;

namespace roleplay.Penalties
{
    public enum PenaltyType
    {
        Warning,
        Reward,
        CharacterKill,
        Kick,
        Ban
    }

    public class Penalty
    {
        private bool isLoaded = false;

        public int UID;
        public int globalID;
        public int characterID;
        public PenaltyType type;
        public string reason;
        public int penaltiedBy;
        public DateTime expireDate;

        public void Load(int UID)
        {
            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "SELECT * FROM `rp_penalties` WHERE `UID`=@UID LIMIT 1;";
            command.Prepare();

            command.Parameters.AddWithValue("@UID", UID);

            var reader = command.ExecuteReader();

            reader.Read();

            Load(reader);

            reader.Close();

            isLoaded = true;
        }

        public void Load(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            UID = reader.GetInt32("UID");
            globalID = reader.GetInt32("globalID");
            characterID = reader.GetInt32("characterID");
            type = (PenaltyType)reader.GetInt32("type");
            reason = reader.GetString("reason");
            penaltiedBy = reader.GetInt32("penaltiedBy");
            expireDate = reader.GetDateTime("expireDate");
        }

        public void Save()
        {
            if (!isLoaded)
                return;

            var command = Database.Instance().Connection.CreateCommand();
            command.CommandText = "UPDATE `rp_penalties` SET `globalID`=@globalID, `characterID`=@characterID, `type`=@type, `reason`=@reason, `penaltiedBy`=@penaltiedBy, `expireDate`=@expireDate WHERE `UID`=@UID";
            command.Prepare();

            command.Parameters.AddWithValue("@globalID", globalID);
            command.Parameters.AddWithValue("@characterID", characterID);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@reason", reason);
            command.Parameters.AddWithValue("@penaltiedBy", penaltiedBy);
            command.Parameters.AddWithValue("@expireDate", expireDate);
            command.Parameters.AddWithValue("@UID", UID);

            command.ExecuteNonQuery();
        }
    }
}
