using System;
using OpenNos.Domain;

namespace OpenNos.Master.Library.Data
{
    [Serializable]
    public class MallStaticBonus
    {
        #region Properties

        public int Seconds { get; set; }

        public StaticBonusType StaticBonus { get; set; }

        #endregion
    }
}