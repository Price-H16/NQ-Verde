namespace ChickenAPI.Enums.Game.Buffs
{
    public enum BuffType
    {
        Good = 0,
        Neutral = 1,
        Bad = 2,
        All = Good | Neutral | Bad,
        Basic = Good | Neutral
    }
}