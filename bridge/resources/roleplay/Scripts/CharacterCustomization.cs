using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace roleplay.Scripts
{
    public class CharacterCustomization : Script
    {
        [Command("customization")]
        public void CustomizationCommand(Client client)
        {
#warning DELETE THIS AFTER WORK
            client.TriggerEvent("ShowCharacterCustomization");
        }

        [RemoteEvent("SaveCustomization")]
        public void SaveCustomization(Client client, object[] args)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

            if (!player.IsReady())
                return;

            var gender = (string)args[0];

            var faceFeatures = JsonConvert.DeserializeObject<List<FaceCustomizationPacket>>((string)args[1]);
            var clothOptions = JsonConvert.DeserializeObject<List<ClothCustomizationPacket>>((string)args[2]);
            var propOptions = JsonConvert.DeserializeObject<List<PropCustomizationPacket>>((string)args[3]);

            player.character.gender = gender;
            player.character.faceFeatures = faceFeatures;
            player.character.clothOptions = clothOptions;
            player.character.propOptions = propOptions;

            player.character.Save();

            player.LoadLook();
        }
    }
}
