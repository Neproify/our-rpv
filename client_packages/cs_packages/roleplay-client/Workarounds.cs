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
            if (int.TryParse(args[0].ToString(), out var money))
            {
                RAGE.Elements.Player.LocalPlayer.SetMoney(money);
            }
        }
    }
}
