using System.Collections.Generic;
using RAGE;

namespace roleplay_client.Items
{
    public class Items : Events.Script
    {
        public bool isVisible;
        public int framesWhenIsUp;

        public Items()
        {
            Events.Add("ShowPlayerItems", ShowPlayerItems);
            Events.Add("HidePlayerItems", HidePlayerItems);
            Events.Add("UseItem", UseItem);
            Events.Add("DropItem", DropItem);
            Events.Add("ReloadPlayerItems", ReloadPlayerItems);
            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if(Input.IsDown(0x50) && framesWhenIsUp > 60 && !Chat.Active)
            {
                if(isVisible)
                {
                    UI.CallEvent("hideItemsWindow");
                    RAGE.Ui.Cursor.Visible = false;
                    isVisible = false;
                }
                else
                {
                        Events.CallRemote("ShowPlayerItems");
                }
                framesWhenIsUp = 0;
            }

            if (Input.IsUp(0x50))
            {
                framesWhenIsUp += 1;
            }
        }

        private void UseItem(object[] args)
        {
            Events.CallRemote("UsePlayerItem", args);
        }

        private void DropItem(object[] args)
        {
            Events.CallRemote("DropPlayerItem", args);
        }

        private void ShowPlayerItems(object[] args)
        {
            var items = args[0].ToString();

            UI.ExecuteJs($"{ UI.GetEventCaller() }('onItemsLoaded', { items });");
            UI.CallEvent("showItemsWindow");
            RAGE.Ui.Cursor.Visible = true;
            isVisible = true;
        }

        private void HidePlayerItems(object[] args)
        {
            UI.CallEvent("hideItemsWindow");
        }

        private void ReloadPlayerItems(object[] args)
        {
            var items = args[0].ToString();
            UI.ExecuteJs($"{ UI.GetEventCaller() }('onItemsLoaded', { items });");

        }
    }
}
