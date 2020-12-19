using System;

namespace OpenNos.Data
{
    [Serializable]
    public class BazaarItemDTO
    {
        #region Properties

        public short Amount { get; set; }

        public long BazaarItemId { get; set; }

        public DateTime DateStart { get; set; }

        public short Duration { get; set; }

        public bool IsPackage { get; set; }

        public Guid ItemInstanceId { get; set; }

        public bool MedalUsed { get; set; }

        public long Price { get; set; }

        public long SellerId { get; set; }

        #endregion
    }
}