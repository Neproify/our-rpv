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
        private HtmlWindow window;
        public RAGE.Elements.Ped ped;

        public CharacterCustomization()
        {
            Events.Add("ShowCharacterCustomization", ShowCharacterCustomization);
            Events.Add("UpdateGender", UpdateGender);
            Events.Add("UpdateFaceOption", UpdateFaceOption);
            Events.Add("UpdateClothOption", UpdateClothOption);
            Events.Add("UpdatePropOption", UpdatePropOption);
            Events.Add("SaveCustomization", SaveCustomization);
        }

        private void UpdateGender(object[] args)
        {
            ped.Model = (string)args[0] == "male" ? 0x705E61F2 : 0x9C9EFFD8;
        }

        private void UpdateFaceOption(object[] args)
        {
            ped.SetFaceFeature((int)args[0], (float)args[1]);
        }

        private void UpdateClothOption(object[] args)
        {
            ped.SetComponentVariation((int)args[0], (int)args[1], 0, 2);
        }

        private void UpdatePropOption(object[] args)
        {
            ped.SetPropIndex((int)args[0], (int)args[1], 0, true);
        }

        private void SaveCustomization(object[] args)
        {
            window?.Destroy();
            RAGE.Ui.Cursor.Visible = false;

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

            RAGE.Events.CallRemote("SaveCustomization", gender, JsonConvert.SerializeObject(faceOptionsPacket), 
                JsonConvert.SerializeObject(clothOptionsPacket), 
                JsonConvert.SerializeObject(propOptionsPacket));

            ped.Destroy();
            ped = null;
            RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();
        }

        private void ShowCharacterCustomization(object[] args)
        {
            window?.Destroy();

            window = new HtmlWindow("package://static/charactercustomization/charactercustomization.html") { Active = true };

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

            string faceOptions = JsonConvert.SerializeObject(faceOptionInfos);

            window.ExecuteJs($"vm.faceOptions = {faceOptions};");

            List<ClothOptionInfo> clothOptionInfos = new List<ClothOptionInfo>();
            clothOptionInfos.Add(new ClothOptionInfo(2, "Włosy", 0, 73, 0, 77));
            clothOptionInfos.Add(new ClothOptionInfo(3, "Tors", 0, 167, 0, 208));
            clothOptionInfos.Add(new ClothOptionInfo(4, "Nogi", 0, 114, 0, 121));
            clothOptionInfos.Add(new ClothOptionInfo(6, "Buty", 0, 90, 0, 94));
            clothOptionInfos.Add(new ClothOptionInfo(7, "Akcesoria", 0, 131, 0, 101));
            clothOptionInfos.Add(new ClothOptionInfo(8, "Podkoszulek", 0, 143, 0, 184));
#warning Change it after update. Or make workaround.
            //clothOptionInfos.Add(new ClothOptionInfo(11, "Góra", 0, 289, 0, 302));
            clothOptionInfos.Add(new ClothOptionInfo(11, "Góra", 0, 255, 0, 255));

            string clothOptions = JsonConvert.SerializeObject(clothOptionInfos);

            window.ExecuteJs($"vm.clothOptions = {clothOptions}");

            List<PropOptionInfo> propOptionInfos = new List<PropOptionInfo>();
            propOptionInfos.Add(new PropOptionInfo(0, "Czapka", 0, 134, 0, 133));
            propOptionInfos.Add(new PropOptionInfo(1, "Okulary", 0, 27, 0, 29));
            propOptionInfos.Add(new PropOptionInfo(2, "Uszy", 0, 36, 0, 17));
            propOptionInfos.Add(new PropOptionInfo(6, "Zegarek", 0, 29, 0, 18));
            propOptionInfos.Add(new PropOptionInfo(7, "Nadgarstek", 0, 7, 0, 14));

            string propOptions = JsonConvert.SerializeObject(propOptionInfos);

            window.ExecuteJs($"vm.propOptions = {propOptions};");

            Cursor.Visible = true;

            var position = RAGE.Elements.Player.LocalPlayer.Position;
            position.X += 5;

            ped = new RAGE.Elements.Ped(0x705E61F2, position);
            ped.FreezePosition(true);
            RAGE.Elements.Player.LocalPlayer.TaskLookAtEntity(ped.Handle, 9999, 0, 0);
        }
    }
}