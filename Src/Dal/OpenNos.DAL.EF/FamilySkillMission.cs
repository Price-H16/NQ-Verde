using System;

namespace OpenNos.DAL.EF
{
    public class FamilySkillMission
    {
        #region Properties

        public short CurrentValue { get; set; }

        public DateTime Date { get; set; }

        public long FamilyId { get; set; }

        public long FamilySkillMissionId { get; set; }

        public short ItemVNum { get; set; }

        public int TotalValue { get; set; }

        #endregion
    }
}