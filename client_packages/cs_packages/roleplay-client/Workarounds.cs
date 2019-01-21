using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace roleplay_client
{
    public class Workarounds : Events.Script
    {
        public Workarounds()
        {
            Events.Add("SetMoney", SetMoney);
        }

        private void SetMoney(object[] args)
        {
            int money;
            if (int.TryParse(args[0].ToString(), out money))
            {
                RAGE.Elements.Player.LocalPlayer.SetMoney(money);
            }
        }
    }
}
