﻿using System;
using System.Collections.Generic;
using System.Text;
using RAGE;

namespace roleplay_client
{
    public class NameTags : Events.Script
    {
        public NameTags()
        {
            Events.Tick += Tick;
            RAGE.Nametags.Enabled = false;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (nametags == null)
                return;

            float maxDistance = 25f;

            foreach(var nametag in nametags)
            {

                var formattedName = nametag.Player.Name.Replace("_", " ");

                var output = $"{formattedName}({nametag.Player.RemoteId})";
                float scale = 0.3f / (nametag.Distance / maxDistance);
                float positionX = 0;
                float positionZ = 0;

                // Not visible on screen
                if (!RAGE.Game.Graphics.GetScreenCoordFromWorldCoord(nametag.Player.Position.X, nametag.Player.Position.Y, nametag.Player.Position.Z, ref positionX, ref positionZ))
                    continue;

                RAGE.Game.UIText.Draw(output, new System.Drawing.Point((int)positionX, (int)positionZ), scale, System.Drawing.Color.White, RAGE.Game.Font.ChaletLondon, true);
            }
        }
    }
}