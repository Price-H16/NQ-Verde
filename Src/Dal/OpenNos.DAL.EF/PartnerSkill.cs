using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class PartnerSkill
    {
        #region Properties

        [Key]
        public long PartnerSkillId { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public short SkillVNum { get; set; }

        public byte Level { get; set; }

        #endregion
    }
}