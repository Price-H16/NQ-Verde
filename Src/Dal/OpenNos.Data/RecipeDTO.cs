﻿using System;

namespace OpenNos.Data
{
    [Serializable]
    public class RecipeDTO
    {
        #region Properties

        public short Amount { get; set; }

        public short ItemVNum { get; set; }

        public short RecipeId { get; set; }

        #endregion
    }
}