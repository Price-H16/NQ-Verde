﻿using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class CellonOptionDTO
    {
        #region Properties

        public long CellonOptionId { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public byte Level { get; set; }

        public CellonOptionType Type { get; set; }

        public int Value { get; set; }

        #endregion
    }
}