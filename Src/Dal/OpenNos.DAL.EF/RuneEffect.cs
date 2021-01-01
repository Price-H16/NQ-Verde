using OpenNos.Domain;
using System;

namespace OpenNos.DAL.EF
{
    public class RuneEffect
    {
        #region Properties

        public Guid EquipmentSerialId { get; set; }

        public int FirstData { get; set; }

        public bool IsPower { get; set; }

        public int RuneEffectId { get; set; }

        public int SecondData { get; set; }

        public byte SubType { get; set; }

        public int ThirdData { get; set; }

        public BCardType.CardType Type { get; set; }

        #endregion
    }
}