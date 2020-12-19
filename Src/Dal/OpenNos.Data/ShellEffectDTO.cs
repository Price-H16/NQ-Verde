using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class ShellEffectDTO
    {
        #region Properties

        public byte Effect { get; set; }

        public ShellEffectLevelType? EffectLevel { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public bool IsRune { get; set; }

        public byte RuneUpgrade { get; set; }

        public long ShellEffectId { get; set; }

        public short Type { get; set; }

        public short Upgrade { get; set; }

        public short Value { get; set; }

        #endregion
    }
}