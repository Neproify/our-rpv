using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace roleplay_client.Auth
{
    public class Login : Events.Script
    {
        RAGE.Ui.HtmlWindow window;

        public Login()
        {
            Events.Add("OnLoginRequest", OnLoginRequest);
            Events.Add("LoginSuccessful", OnLoginSuccesful);
            window = new RAGE.Ui.HtmlWindow("package://static/login/login.html");
            window.Active = true;
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
        }
    }
}
