namespace OpenNos.GameObject
{
    public class Gift
    {
        #region Instantiation

        public Gift(short vnum, short amount, short design = 0, bool isRareRandom = false)
        {
            VNum = vnum;
            Amount = amount;
            IsRandomRare = isRareRandom;
            Design = design;
        }

        #endregion

        #region Properties

        public short Amount { get; set; }

        public short Design { get; set; }

        public bool IsRandomRare { get; set; }

        public byte MaxTeamSize { get; set; }

        public byte MinTeamSize { get; set; }

        public short VNum { get; set; }

        #endregion
    }
}