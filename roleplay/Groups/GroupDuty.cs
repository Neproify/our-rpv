using System;

namespace roleplay
{
    public class GroupDuty
    {
        public GroupMember member;

        public DateTime startDateTime;
        public DateTime endDateTime;

        public void StartDuty()
        {
            startDateTime = DateTime.Now;;
            endDateTime = DateTime.Now;
        }

        public void EndDuty()
        {
            endDateTime = DateTime.Now;
            int elapsedTime = (int)(endDateTime - startDateTime).TotalSeconds;
            member.dutyTime += elapsedTime;
            member.PayForDuty();
        }
    }
}
