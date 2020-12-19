﻿using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class RollGeneratedItem
    {
        #region Properties

        public bool IsRareRandom { get; set; }

        public virtual Item ItemGenerated { get; set; }

        public short ItemGeneratedAmount { get; set; }

        public short ItemGeneratedDesign { get; set; }

        public short ItemGeneratedVNum { get; set; }

        public byte MaximumOriginalItemRare { get; set; }

        public byte MinimumOriginalItemRare { get; set; }

        public virtual Item OriginalItem { get; set; }

        public short OriginalItemDesign { get; set; }

        public short OriginalItemVNum { get; set; }

        public short Probability { get; set; }

        [Key] public short RollGeneratedItemId { get; set; }

        #endregion
    }
}