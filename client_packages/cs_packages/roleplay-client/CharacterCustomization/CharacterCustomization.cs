using RAGE;
using RAGE.Ui;

namespace roleplay_client.CharacterCustomization
{
    public class CharacterCustomization : Events.Script
    {
        private RAGE.Ui.HtmlWindow window;

        public CharacterCustomization()
        {
            Events.Add("ShowCharacterCustomization", ShowCharacterCustomization);
        }

        private void ShowCharacterCustomization(object[] args)
        {
            window?.Destroy();

            window = new HtmlWindow("package://static/charactercustomization/charactercustomization.html");
        }
    }
}