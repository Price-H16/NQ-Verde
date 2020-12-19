namespace NosTale.Configuration.Configuration.Server
{
    public struct RateConfiguration
    {
        public long PartnerSpXp { get; set; }

        public int QuestDropRate { get; set; }

        public int RateDrop { get; set; }

        public int RateFairyXP { get; set; }

        public int RateGold { get; set; }

        public int RateGoldDrop { get; set; }

        public int RateHeroicXP { get; set; }

        public int RateReputation { get; set; }

        public int RateXP { get; set; }

        public byte GlacernonPercentRatePvm { get; set; }

        public byte GlacernonPercentRatePvp { get; set; }

        public byte CylloanPercentRate { get; set; }
    }
}