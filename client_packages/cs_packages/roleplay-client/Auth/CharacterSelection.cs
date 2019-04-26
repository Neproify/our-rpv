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
            Events.Add("ShowCharacterCreator", ShowCharacterCreator);
            Events.Add("CreateNewCharacter", CreateNewCharacter);
        }

        private void SelectCharacter(object[] args)
        {
            Events.CallRemote("SelectCharacter", args);
        }

        private void OnPlayerCharactersLoaded(object[] args)
        {
            if (window != null)
                window.Destroy();

            string characters = args[0].ToString();
            window = new RAGE.Ui.HtmlWindow("package://static/auth/characterSelection.html") {Active = true};
            RAGE.Ui.Cursor.Visible = true;
            window.ExecuteJs($"LoadCharacters({characters});");
        }

        private void OnCharacterSelectionSuccessful(object[] args)
        {
            window.Destroy();
            RAGE.Ui.Cursor.Visible = false;
            Chat.PreventShowing = false;
            Chat.Show(true);
            Discord.Update("Gra na Our Role Play Developer", "W grze.");
        }

        private void ShowCharacterCreator(object[] args)
        {
            window.Url = "package://static/auth/characterCreation.html";
        }

        private void CreateNewCharacter(object[] args)
        {
            Events.CallRemote("CreateCharacter", args);
        }
    }
}
