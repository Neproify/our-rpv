using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace roleplay_client.Items
{
    public class Items : Events.Script
    {
        RAGE.Ui.HtmlWindow window;
        public bool shouldBeVisible;

        public Items()
        {
            Events.Add("TogglePlayerItems", TogglePlayerItems);
            Events.Add("UseItem", UseItem);
        }

        private void UseItem(object[] args)
        {
            Events.CallRemote("UsePlayerItem", args);
        }

        private void TogglePlayerItems(object[] args)
        {
            shouldBeVisible = !shouldBeVisible;

            if (shouldBeVisible)
            {
                var items = args[0].ToString();
                window = new RAGE.Ui.HtmlWindow("package://static/items/items.html");
                window.Active = true;
                RAGE.Ui.Cursor.Visible = true;
                window.ExecuteJs(string.Format("loadItems('{0}');", items));
            }
            else
            {
                window.Active = false;
                window.Destroy();
                RAGE.Ui.Cursor.Visible = false;
            }
        }
    }
}
