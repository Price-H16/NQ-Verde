﻿using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class CardDTO
    {
        #region Properties

        public BuffType BuffType { get; set; }

        public short CardId { get; set; }

        public int Delay { get; set; }

        public int Duration { get; set; }

        public int EffectId { get; set; }

        public byte Level { get; set; }

        public string Name { get; set; }

        public byte Propability { get; set; }

        public short TimeoutBuff { get; set; }

        public byte TimeoutBuffChance { get; set; }

        #endregion
    }
}