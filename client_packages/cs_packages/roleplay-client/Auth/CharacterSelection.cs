using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace roleplay_client.Auth
{
    public class CharacterSelection : Events.Script
    {
        RAGE.Ui.HtmlWindow window;

        public CharacterSelection()
        {
            Events.Add("OnPlayerCharactersLoaded", OnPlayerCharactersLoaded);
            Events.Add("SelectCharacter", SelectCharacter);
            Events.Add("OnCharacterSelectionSuccessful", OnCharacterSelectionSuccessful);
        }

        private void SelectCharacter(object[] args)
        {
            Events.CallRemote("SelectCharacter", args);
        }

        private void OnPlayerCharactersLoaded(object[] args)
        {
            string characters = args[0].ToString();
            window = new RAGE.Ui.HtmlWindow("package://static/auth/characterSelection.html");
            window.Active = true;
            RAGE.Ui.Cursor.Visible = true;
            window.ExecuteJs(string.Format("LoadCharacters({0});", characters));
        }

        private void OnCharacterSelectionSuccessful(object[] args)
        {
            window.Destroy();
            RAGE.Ui.Cursor.Visible = false;
            RAGE.Chat.Show(true);
        }
    }
}
