﻿using RAGE.Ui;

namespace roleplay_client
{
    public static class UI
    {
        private static RAGE.Ui.HtmlWindow browser;

        public static void Initialize()
        {
            browser = new HtmlWindow("package://UI/index.html");
        }

        public static void ExecuteJs(string code)
        {
            browser.ExecuteJs(code);
        }

        public static void CallEvent(string eventName)
        {
            ExecuteJs($"window.emitter.emit('{eventName}');");
        }
    }
}