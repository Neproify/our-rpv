using RAGE;

namespace roleplay_client.Auth
{
    public class Login : Events.Script
    {
        readonly RAGE.Ui.HtmlWindow window;

        public Login()
        {
            Events.Add("OnLoginRequest", OnLoginRequest);
            Events.Add("LoginSuccessful", OnLoginSuccesful);
            window = new RAGE.Ui.HtmlWindow("package://static/auth/login.html") {Active = true};
            Chat.PreventShowing = true;
            Chat.Show(false);
            RAGE.Ui.Cursor.Visible = true;
        }

        private void OnLoginRequest(object[] args)
        {
            Events.CallRemote("OnLoginRequest", args);
        }

        private void OnLoginSuccesful(object[] args)
        {
            window.Destroy();
            RAGE.Ui.Cursor.Visible = false;
            Events.CallRemote("LoadPlayerCharacters", null);
        }
    }
}
