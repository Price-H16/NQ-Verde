using NosTale.Configuration.Configuration.Server;

namespace NosTale.Configuration
{
    public class JsonGameConfiguration
    {
        // Here some Config ↓
        public ServerConfiguration Server { get; set; } = new ServerConfiguration
        {
            //Auth
            AuthentificationServiceAuthKey = "llTestServerAuth14031941",
            MasterAuthKey = "llMasterAuthzzz14031941",
            LogKey = "LogAuth147", // this isn't being used as far as i know

            // Config ip/port
            LogerPort = 6970,
            IPAddress = "161.97.146.36",
            MasterIP = "127.0.0.1",
            Act4Port = 5100,
            LoginPort = 4001,
            MasterPort = 6969,
            WorldPort = 5000,

            // Config Srv
            ServerGroupS1 = "NosQuest",
            Language = "uk",
            SessionLimit = 150,
            LagMode = false,
            SceneOnCreate = false,
            UseOldCrypto = false,
            WorldInformation = true,
            LockSystem = true,
            BCardsInArenaTalent = true
        };

        public RateConfiguration Rate { get; set; } = new RateConfiguration
        {
            CylloanPercentRate = 10,
            GlacernonPercentRatePvm = 5,
            GlacernonPercentRatePvp = 1,
            RateXP = 2,
            PartnerSpXp = 2,
            QuestDropRate = 1,
            RateDrop = 35,
            RateFairyXP = 10,
            RateGold = 35,
            RateGoldDrop = 2,
            RateHeroicXP = 7,
            RateReputation = 1
        };

        public EventConfiguration Event { get; set; } = new EventConfiguration
        {
            ChristmasEvent = false,
            HalloweenEvent = false,
            EstivalEvent = false,
            ValentineEvent = false,
            LunarNewYearEvent = false,
            EasterEvent = false,
            //Event icon-->
            //By percentage-->
            EventLvlUpEq = 0,
            EventRareUpEq = 0,
            EventSpUp = 0,
            EventSpPerfection = 0,
            EventXPF = 0,
            //Useless at the moment-->
            EventSealed = 0,
            //By multiplication-->
            EventXp = 0,
            EventGold = 0,
            EventRep = 0,
            EventDrop = 0,
            //Coming soon-->
            EventRuneUp = 0,
            EventTattoUp = 0
            //End event icon-->

        };

        public MaxConfiguration Max { get; set; } = new MaxConfiguration
        {
            HeroicStartLevel = 85,
            MaxGold = int.MaxValue,
            MaxGoldBank = 100000000000,
            MaxHeroLevel = 60,
            MaxJobLevel = 80,
            MaxLevel = 99,
            MaxSPLevel = 99,
            MaxUpgrade = 10
        };
    }
}