namespace OpenNos.Domain
{
    public enum MapInstanceType
    {
        BaseMapInstance,
        NormalInstance,
        LodInstance,
        TimeSpaceInstance,
        RaidInstance,
        FamilyRaidInstance,
        Act4ShipAngel,
        Act4ShipDemon,
        Act4Morcos,
        Act4Hatus,
        Act4Calvina,
        Act4Berios,
        EventGameInstance,
        CaligorInstance,
        IceBreakerInstance,
        ArenaInstance,
        GemmeStoneInstance,
        TalentArenaMapInstance,
        Act4Instance,
        RainbowBattleInstance,
        SheepGameInstance,
        RoundToRound,
        MapPvp = ArenaInstance | TalentArenaMapInstance | Act4Instance,
        Act7Ship = 32,
        SealedVesselsMap = 33,
        TowerInstanceType,
        FafnirBossInstance,
        YertirandBossInstance,
        GrasslinBossInstance
    }
}