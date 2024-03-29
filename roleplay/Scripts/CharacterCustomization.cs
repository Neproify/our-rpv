﻿using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace roleplay.Scripts
{
    public class CharacterCustomization : Script
    {
        [Command("customization")]
        public void CustomizationCommand(Player client)
        {
            var player = Managers.PlayerManager.Instance().GetByHandle(client);

#warning DELETE THIS AFTER WORK
            player.TriggerEvent("ShowCharacterCustomization", JsonConvert.SerializeObject(player.character.faceFeatures), 
                JsonConvert.SerializeObject(player.character.clothOptions), 
                JsonConvert.SerializeObject(player.character.propOptions));
        }

        [RemoteEvent("SaveCustomization")]
        public void SaveCustomization(Player client, object[] args)
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
