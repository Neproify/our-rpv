using RAGE;
using RAGE.Ui;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace roleplay_client.CharacterCustomization
{
    public class FaceOptionInfo
    {
        public int index;
        public string name;
        public float min;
        public float max;
        public float value;

        [JsonConstructor]
        public FaceOptionInfo(int index, string name, float min, float max, float value = 0)
        {
            this.index = index;
            this.name = name;
            this.min = min;
            this.max = max;
            this.value = value;
        }
    }

    public class ClothOptionInfo
    {
        public int index;
        public string name;
        public int maleMin;
        public int maleMax;
        public int femaleMin;
        public int femaleMax;
        public int value;

        [JsonConstructor]
        public ClothOptionInfo(int index, string name, int maleMin, int maleMax, int femaleMin, int femaleMax, int value = 0)
        {
            this.index = index;
            this.name = name;
            this.maleMin = maleMin;
            this.maleMax = maleMax;
            this.femaleMin = femaleMin;
            this.femaleMax = femaleMax;
            this.value = value;
        }

        public ClothOptionInfo(int index, string name, int min, int max, int value = 0)
        {
            this.index = index;
            this.name = name;
            maleMin = femaleMin = min;
            maleMax = femaleMax = max;
            this.value = value;
        }
    }

    public class PropOptionInfo
    {
        public int index;
        public string name;
        public int maleMin;
        public int maleMax;
        public int femaleMin;
        public int femaleMax;
        public int value;

        [JsonConstructor]
        public PropOptionInfo(int index, string name, int maleMin, int maleMax, int femaleMin, int femaleMax, int value = 0)
        {
            this.index = index;
            this.name = name;
            this.maleMin = maleMin;
            this.maleMax = maleMax;
            this.femaleMin = femaleMin;
            this.femaleMax = femaleMax;
            this.value = value;
        }

        public PropOptionInfo(int index, string name, int min, int max, int value)
        {
            this.index = index;
            this.name = name;
            maleMin = femaleMin = min;
            maleMax = femaleMax = max;
            this.value = value;
        }
    }

    public class FaceCustomizationPacket
    {
        public int index;
        public float value;
    }

    public class ClothCustomizationPacket
    {
        public int index;
        public int value;
    }

    public class PropCustomizationPacket
    {
        public int index;
        public int value;
    }

    public class CharacterCustomization : Events.Script
    {
        private bool isActive;

        public CharacterCustomization()
        {
            Events.Add("ShowCharacterCustomization", ShowCharacterCustomization);
            Events.Add("UpdateGender", UpdateGender);
            Events.Add("UpdateFaceOption", UpdateFaceOption);
            Events.Add("UpdateClothOption", UpdateClothOption);
            Events.Add("UpdatePropOption", UpdatePropOption);
            Events.Add("SaveCustomization", SaveCustomization);

            isActive = false;
            Events.Tick += Tick;
        }

        private void UpdateGender(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.Model = (string)args[0] == "male" ? 0x705E61F2 : 0x9C9EFFD8;
        }

        private void UpdateFaceOption(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.SetFaceFeature((int)args[0], (float)args[1]);
        }

        private void UpdateClothOption(object[] args)
        {
            RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)args[0], (int)args[1], 0, 2);
        }

        private void UpdatePropOption(object[] args)
        {
            if((int)args[1] == -1)
            {
                RAGE.Elements.Player.LocalPlayer.ClearProp((int)args[0]);
                return;
            }

            RAGE.Elements.Player.LocalPlayer.SetPropIndex((int)args[0], (int)args[1], 0, true);
        }

        private void SaveCustomization(object[] args)
        {
            UI.CallEvent("hideCharacterCustomizationWindow");
            Cursor.Visible = false;
            isActive = false;

            var faceOptionInfos = JsonConvert.DeserializeObject<List<FaceOptionInfo>>((string)args[1]);
            var clothOptionInfos = JsonConvert.DeserializeObject<List<ClothOptionInfo>>((string)args[2]);
            var propOptionInfos = JsonConvert.DeserializeObject<List<PropOptionInfo>>((string)args[3]);

            var gender = (string)args[0];

            var faceOptionsPacket = new List<FaceCustomizationPacket>();

            foreach(var faceOption in faceOptionInfos)
            {
                faceOptionsPacket.Add(new FaceCustomizationPacket { index = faceOption.index, value = faceOption.value });
            }

            var clothOptionsPacket = new List<ClothCustomizationPacket>();

            foreach(var clothOption in clothOptionInfos)
            {
                clothOptionsPacket.Add(new ClothCustomizationPacket { index = clothOption.index, value = clothOption.value });
            }

            var propOptionsPacket = new List<PropCustomizationPacket>();

            foreach(var propOption in propOptionInfos)
            {
                propOptionsPacket.Add(new PropCustomizationPacket { index = propOption.index, value = propOption.value });
            }

            Events.CallRemote("SaveCustomization", gender, JsonConvert.SerializeObject(faceOptionsPacket), 
                JsonConvert.SerializeObject(clothOptionsPacket), 
                JsonConvert.SerializeObject(propOptionsPacket));

            RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();
        }

        private void ShowCharacterCustomization(object[] args)
        {
            List<FaceOptionInfo> faceOptionInfos = new List<FaceOptionInfo>();
            faceOptionInfos.Add(new FaceOptionInfo(0, "Szerokość nosa", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(1, "Wysokość nosa", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(2, "Długość nosa", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(3, "Grzbiet nosa", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(4, "Czubek nosa", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(5, "Przesunięcie grzbietu nosa", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(6, "Wysokość brwi", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(7, "Szerokość brwi", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(8, "Wysokość kości policzkowej", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(9, "Szerokość kości policzkowej", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(10, "Szerokość policzków", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(11, "Oczy", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(12, "Usta", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(13, "Szerokość szczęki", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(14, "Wysokość szczęki", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(15, "Długość podbródka", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(16, "Pozycja podbródka", 0, 1));
            faceOptionInfos.Add(new FaceOptionInfo(17, "Szerokość podbródka", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(18, "Kształt podbródka", -1, 1));
            faceOptionInfos.Add(new FaceOptionInfo(19, "Szerokość szyi", -1, 1));

            var currentFaceOptions = JsonConvert.DeserializeObject<List<FaceCustomizationPacket>>((string)args[0]);

            if (currentFaceOptions != null)
            {
                foreach (var faceOption in faceOptionInfos)
                {
                    var currentFaceOption = currentFaceOptions.Find(x => x.index == faceOption.index);

                    if (currentFaceOption != null)
                    {
                        faceOption.value = currentFaceOption.value;
                    }
                }
            }

            string faceOptions = JsonConvert.SerializeObject(faceOptionInfos);

            List<ClothOptionInfo> clothOptionInfos = new List<ClothOptionInfo>();
            clothOptionInfos.Add(new ClothOptionInfo(2, "Włosy", 0, 73, 0, 77));
            clothOptionInfos.Add(new ClothOptionInfo(3, "Tors", 0, 167, 0, 208));
            clothOptionInfos.Add(new ClothOptionInfo(4, "Nogi", 0, 114, 0, 121));
            clothOptionInfos.Add(new ClothOptionInfo(6, "Buty", 0, 90, 0, 94));
            clothOptionInfos.Add(new ClothOptionInfo(7, "Akcesoria", 0, 131, 0, 101));
            clothOptionInfos.Add(new ClothOptionInfo(8, "Podkoszulek", 0, 143, 0, 184));
            clothOptionInfos.Add(new ClothOptionInfo(11, "Góra", 0, 289, 0, 302));

            var currentClothOptions = JsonConvert.DeserializeObject<List<ClothCustomizationPacket>>((string)args[1]);

            if (currentClothOptions != null)
            {
                foreach (var clothOption in clothOptionInfos)
                {
                    var currentClothOption = currentClothOptions.Find(x => x.index == clothOption.index);

                    if (currentClothOption != null)
                    {
                        clothOption.value = currentClothOption.value;
                    }
                }
            }

            string clothOptions = JsonConvert.SerializeObject(clothOptionInfos);

            List<PropOptionInfo> propOptionInfos = new List<PropOptionInfo>();
            propOptionInfos.Add(new PropOptionInfo(0, "Czapka", -1, 134, -1, 133));
            propOptionInfos.Add(new PropOptionInfo(1, "Okulary", -1, 27, -1, 29));
            propOptionInfos.Add(new PropOptionInfo(2, "Uszy", -1, 36, -1, 17));
            propOptionInfos.Add(new PropOptionInfo(6, "Zegarek", -1, 29, -1, 18));
            propOptionInfos.Add(new PropOptionInfo(7, "Nadgarstek", -1, 7, -1, 14));

            var currentPropOptions = JsonConvert.DeserializeObject<List<PropCustomizationPacket>>((string)args[2]);

            if (currentPropOptions != null)
            {
                foreach (var propOption in propOptionInfos)
                {
                    var currentPropOption = currentPropOptions.Find(x => x.index == propOption.index);

                    if (currentPropOption != null)
                    {
                        propOption.value = currentPropOption.value;
                    }
                }
            }

            string propOptions = JsonConvert.SerializeObject(propOptionInfos);

            UI.ExecuteJs($"{ UI.GetEventCaller() }('characterCustomizationLoaded', 0, {faceOptions}, {clothOptions}, {propOptions});");
            UI.CallEvent("showCharacterCustomizationWindow");
            Cursor.Visible = true;
            isActive = true;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (isActive)
            {
                RAGE.Elements.Player.LocalPlayer.SetHeading(RAGE.Elements.Player.LocalPlayer.GetHeading() + 0.5f);
            }
        }
    }
}