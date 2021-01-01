namespace OpenNos.Domain
{
    /// <summary>
    /// Account basic groups
    /// </summary>
    public enum AuthorityType : short
    {
        Closed = -3,
        Banned = -2,
        Unconfirmed = -1,
        User = 0,
        Youtuber = 1,
        Donator = 2,
        Supporter = 20,
        DSGM = 30,
        MOD = 31,
        SMOD = 32,
        BA = 33,
        TGM = 50,
        GM = 51,
        SGM = 52,
        GA = 53,
        TM = 60,
        CM = 70,
        DEV = 80,
        BetaTester = 90,
        Administrator = 100
    }
}