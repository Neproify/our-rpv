using System.Collections.Generic;
using RAGE;

namespace roleplay_client.Items
{
    public class Items : Events.Script
    {
        RAGE.Ui.HtmlWindow window;
        public bool isVisible;
        public int framesWhenIsUp;

        public Items()
        {
            Events.Add("ShowPlayerItems", ShowPlayerItems);
            Events.Add("HidePlayerItems", HidePlayerItems);
            Events.Add("UseItem", UseItem);
            Events.Add("DropItem", DropItem);
            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if(Input.IsDown(0x50) && framesWhenIsUp > 60 && !Chat.Active)
            {
                if(isVisible)
                {
                    window.Active = false;
                    window.Destroy();
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

            window?.Destroy();

            window = new RAGE.Ui.HtmlWindow("package://static/items/items.html");
            window.ExecuteJs($"loadItems('{items}');");
            window.Active = true;
            RAGE.Ui.Cursor.Visible = true;
            isVisible = true;
        }

        private void HidePlayerItems(object[] args)
        {
            if(window != null)
            {
                window.Active = false;
                window.Destroy();
                RAGE.Ui.Cursor.Visible = false;
                isVisible = false;
            }
        }
    }
}
