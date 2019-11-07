using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace roleplay
{
    public class GroupMember
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId UID;

        [BsonElement("characterid")]
        public ObjectId charID;

#warning TODO: Duty time counting.
        [BsonElement("dutytime")]
        public int dutyTime;

        [BsonElement("lastpayment")]
        public DateTime lastPayment;

        [BsonIgnore]
        public Entities.Group group;
        [BsonIgnore]
        public GroupRank rank;

        public void Save()
        {
        }

        public void PayForDuty()
        {
            var player = Managers.PlayerManager.Instance().GetByCharacterID(charID);

            if (player == null)
                return;

            if (lastPayment.AddDays(1) < DateTime.Now)
                return;

            if (dutyTime < 60 * 30)
                return;

            lastPayment = DateTime.Now;
            player.money += rank.salary;

            player.SendNotification($"~g~Otrzymałeś wypłatę w wysokości ${rank.salary}.");
        }
    }
}
