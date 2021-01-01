namespace OpenNos.DAL.EF
{
    public class CharacterTitle
    {
        #region Properties

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public long CharacterTitleId { get; set; }

        public byte Stat { get; set; }

        public long TitleVnum { get; set; }

        #endregion
    }
}