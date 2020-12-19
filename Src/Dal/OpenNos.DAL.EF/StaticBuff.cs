﻿namespace OpenNos.DAL.EF
{
    public class StaticBuff
    {
        #region Properties

        public virtual Card Card { get; set; }

        public short CardId { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public int RemainingTime { get; set; }

        public long StaticBuffId { get; set; }

        #endregion
    }
}