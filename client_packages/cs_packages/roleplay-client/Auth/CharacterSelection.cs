using RAGE;

namespace roleplay_client.Auth
{
    public class CharacterSelection : Events.Script
    {

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
            string characters = args[0].ToString();
            RAGE.Ui.Cursor.Visible = true;
            UI.ExecuteJs($"{ UI.GetEventCaller() }('characterSelectionLoaded', { characters });");
            UI.CallEvent("showCharacterSelectionWindow");
        }

        private void OnCharacterSelectionSuccessful(object[] args)
        {
            RAGE.Ui.Cursor.Visible = false;
            Chat.PreventShowing = false;
            Chat.Show(true);
            Discord.Update("Gra na Our Role Play Developer", "W grze.");
            UI.CallEvent("hideCharacterSelectionWindow");
        }

        private void ShowCharacterCreator(object[] args)
        {
            //window.Url = "package://static/auth/characterCreation.html";
        }

        private void CreateNewCharacter(object[] args)
        {
            //Events.CallRemote("CreateCharacter", args);
        }
    }
}
