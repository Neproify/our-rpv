using RAGE;

namespace roleplay_client.Auth
{
    public class Login : Events.Script
    {

        public Login()
        {
            Events.Add("OnLoginRequest", OnLoginRequest);
            Events.Add("LoginSuccessful", OnLoginSuccesful);
            Chat.PreventShowing = true;
            Chat.Show(false);
            RAGE.Ui.Cursor.Visible = true;
            UI.CallEvent("showLoginWindow");
        }


        private void OnLoginRequest(object[] args)
        {
            Events.CallRemote("OnLoginRequest", args);
        }

        private void OnLoginSuccesful(object[] args)
        {
            RAGE.Ui.Cursor.Visible = false;
            UI.CallEvent("hideLoginWindow");
            Events.CallRemote("LoadPlayerCharacters", null);
        }
    }
}
