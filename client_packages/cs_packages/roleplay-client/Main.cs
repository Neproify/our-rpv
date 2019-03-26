using System.Collections.Generic;
using RAGE;

namespace roleplay_client
{
    public class Main : Events.Script
    {

        public Main()
        {
            RAGE.Ui.Cursor.Visible = false;
            Events.Tick += Tick;
            Discord.Update("Gra na Our Role Play Developer", "Logowanie");
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            //Pause menu
            RAGE.Game.Pad.DisableControlAction(1, 199, true);
            RAGE.Game.Pad.DisableControlAction(1, 200, true);
        }
    }
}
