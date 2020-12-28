using System;

namespace OpenNos.Master.Library.Data
{
    [Serializable]
    public class ConfigurationObject
    {
        #region Properties

        public byte GlacernonPercentRatePvm { get; set; }

        public byte GlacernonPercentRatePvp { get; set; }

        public string Act4IP { get; set; }

        public int Act4Port { get; set; }

        public bool ChristmasEvent { get; set; }

        public bool HalloweenEvent { get; set; }

        public bool EstivalEvent { get; set; }

        public bool ValentineEvent { get; set; }

        public bool BCardsInArenaTalent { get; set; }

        public bool LunarNewYearEvent { get; set; }

        public bool EasterEvent { get; set; }

        public byte HeroicStartLevel { get; set; }

        public long MaxGold { get; set; }

        public long MaxGoldBank { get; set; }

        public byte MaxHeroLevel { get; set; }

        public byte MaxJobLevel { get; set; }

        public byte MaxLevel { get; set; }

        public byte MaxSPLevel { get; set; }

        public byte MaxUpgrade { get; set; }

        public long PartnerSpXp { get; set; }

        public int QuestDropRate { get; set; }

        public int RateDrop { get; set; }

        public int RateFairyXP { get; set; }

        public int RateGold { get; set; }

        public int RateGoldDrop { get; set; }

        public int RateHeroicXP { get; set; }

        public int RateReputation { get; set; }

        public int RateXP { get; set; }

        public bool SceneOnCreate { get; set; }

        public int SessionLimit { get; set; }

        public bool WorldInformation { get; set; }

        public byte CylloanPercentRate { get; set; }

        public bool LockSystem { get; set; }

        public int EventLvlUpEq { get; set; }

        public int EventRareUpEq { get; set; }

        public int EventSpUp { get; set; }

        public int EventSpPerfection { get; set; }

        public int EventXPF { get; set; }

        public int EventSealed { get; set; }

        public int EventXp { get; set; }

        public int EventGold { get; set; }

        public int EventRep { get; set; }

        public int EventDrop { get; set; }

        public int EventRuneUp { get; set; }

        public int EventTattoUp { get; set; }

        public bool FamilyExpBuff { get; set; } = false;

        public bool FamilyGoldBuff { get; set; } = false;

        public DateTime TimeExpBuff { get; set; } = DateTime.Now.AddHours(-2);

        public DateTime TimeGoldBuff { get; set; } = DateTime.Now.AddHours(-2);

        #endregion
    }
}