﻿using OpenNos.Domain;
using System;

namespace OpenNos.DAL.EF
{
    public class RuneEffect
    {
        public int RuneEffectId { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public BCardType.CardType Type { get; set; }

        public byte SubType { get; set; }

        public int FirstData { get; set; }

        public int SecondData { get; set; }

        public int ThirdData { get; set; }

        public bool IsPower { get; set; }
    }
}
