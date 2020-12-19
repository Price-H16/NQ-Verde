using System;

namespace OpenNos.Data
{
    [Serializable]
    public class RollGeneratedItemDTO
    {
        #region Properties

        public bool IsRareRandom { get; set; }

        public short ItemGeneratedAmount { get; set; }

        public short ItemGeneratedDesign { get; set; }

        public short ItemGeneratedVNum { get; set; }

        public byte MaximumOriginalItemRare { get; set; }

        public byte MinimumOriginalItemRare { get; set; }

        public short OriginalItemDesign { get; set; }

        public short OriginalItemVNum { get; set; }

        public short Probability { get; set; }

        public short RollGeneratedItemId { get; set; }

        #endregion
    }
}