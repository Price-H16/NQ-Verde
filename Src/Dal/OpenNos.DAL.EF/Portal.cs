namespace OpenNos.DAL.EF
{
    public class Portal
    {
        #region Properties

        public short DestinationMapId { get; set; }

        public short DestinationX { get; set; }

        public short DestinationY { get; set; }

        public short HeroLevelRequired { get; set; }

        public bool IsDisabled { get; set; }

        public short LevelRequired { get; set; }

        public virtual Map Map { get; set; }

        public virtual Map Map1 { get; set; }

        public string NomeOggetto { get; set; }

        public int PortalId { get; set; }

        public byte? RequiredClass { get; set; }

        public short RequiredItem { get; set; }

        public short SourceMapId { get; set; }

        public short SourceX { get; set; }

        public short SourceY { get; set; }

        public short Type { get; set; }

        #endregion
    }
}