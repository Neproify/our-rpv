using System;
using System.Collections.Generic;
using System.Text;
using RAGE;
using RAGE.Elements;

namespace roleplay_client
{
    public class Main : Events.Script
    {

        public Main()
        {
            RAGE.Chat.SafeMode = false;
            RAGE.Ui.Cursor.Visible = false;
            RAGE.Chat.Show(false);
            Events.Tick += Tick;
            RAGE.Discord.Update("Gra na Our Role Play Developer", "Logowanie");
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            //Pause menu
            RAGE.Game.Pad.DisableControlAction(1, 199, true);
            RAGE.Game.Pad.DisableControlAction(1, 200, true);
        }
    }
}
