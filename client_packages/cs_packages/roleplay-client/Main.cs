using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace roleplay_client
{
    public class Main : Events.Script
    {
        public Main()
        {
            RAGE.Chat.SafeMode = false;
            RAGE.Ui.Cursor.Visible = false;
            RAGE.Chat.Show(false);
        }
    }
}
