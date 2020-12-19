using System;

namespace OpenNos.Data
{
    [Serializable]
    public class RuneEffectsDTO
    {
        #region Properties

        public long RuneEffectId { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public byte Type { get; set; }

        public byte SubType { get; set; }

        public int FirstData { get; set; }

        public int SecondData { get; set; }

        public int ThirdDada { get; set; }

        #endregion
    }
}