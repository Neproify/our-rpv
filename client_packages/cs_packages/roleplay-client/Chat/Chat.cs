using System.Collections.Generic;
using RAGE;

namespace roleplay_client
{
    public static class Chat
    {
        public static RAGE.Ui.HtmlWindow window;
        public static bool PreventShowing = false;

        private static bool _isActive;
        public static bool Active
        {
            get => _isActive;
            set => SetActive(value);
        }

        public static void InitializeWindow()
        {
            RAGE.Chat.SafeMode = false;
            RAGE.Chat.Show(false);

            window = new RAGE.Ui.HtmlWindow("package://static/chat/chat.html") {Active = true};
            window.MarkAsChat();

        }

        private static void SetActive(bool active)
        {
            if (!_isActive)
            {
                if (PreventShowing)
                    return;

                _isActive = true;
                window.ExecuteJs("enableChatInput(true);");
                RAGE.Ui.Cursor.Visible = true;
            }
            else
            {
                _isActive = false;
                window.ExecuteJs("enableChatInput(false);");
                RAGE.Ui.Cursor.Visible = false;
            }
        }

        public static void Show(bool toggle)
        {
            if(toggle)
            {
                if (PreventShowing)
                    return;

                window.ExecuteJs("showChat(true);");
            }
            else
            {
                window.ExecuteJs("showChat(false);");
            }
        }

        public static void SendMessage()
        {
            window.ExecuteJs("sendMessage();");
            Active = false;
        }
    }

    public class ChatScript : Events.Script
    {

        public ChatScript()
        {
            Chat.InitializeWindow();

            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (Input.IsDown(0x54))
            {
                if (!Chat.Active)
                {
                    Chat.Active = true;
                }
            }

            if (Input.IsDown(0x1B))
            {
                if(Chat.Active)
                {
                    Chat.Active = false;
                }
            }

            if(Input.IsDown(0x0D))
            {
                if(Chat.Active)
                {
                    Chat.SendMessage();
                }
            }
        }
    }
}
