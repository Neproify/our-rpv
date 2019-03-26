using GTANetworkAPI;
using System.Timers;

namespace roleplay.Scripts
{
    public class BrutallyWounded : Script
    {
        public BrutallyWounded()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += BrutallyWoundedTimer;
            timer.Start();
        }

        private void BrutallyWoundedTimer(object sender, ElapsedEventArgs e)
        {
            var players = Managers.PlayerManager.Instance().GetAll().FindAll(x => x.isBrutallyWounded);
            foreach(var player in players)
            {
                player.secondsToEndOfBrutallyWounded -= 1;
                if(player.secondsToEndOfBrutallyWounded <= 1)
                {
                    player.SetIsBrutallyWounded(false);
                }
            }
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client client, Client killer, uint reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            player.SetIsBrutallyWounded(true);
        }

        [Command("akceptujsmierc", GreedyArg = true)]
        public void CharacterKillCommand(Client client, string reason)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.isLogged || player.character == null)
                return;

            if (player.isBrutallyWounded)
                player.KillCharacter(reason);

            foreach(var loopPlayer in Managers.PlayerManager.Instance().GetAll())
            {
                loopPlayer.handle.SendNotification($"~r~ {player.formattedName} zmarł, powód: {reason}");
            }
        }
    }
}
