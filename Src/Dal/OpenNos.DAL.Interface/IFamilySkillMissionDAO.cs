using OpenNos.Data;
using System.Collections.Generic;
using OpenNos.Data.Enums;
using System;

namespace OpenNos.DAL.Interface
{
    public interface IFamilySkillMissionDAO
    {
        #region Methods
        DeleteResult Delete(long itemVNum, long familyId);

        SaveResult InsertOrUpdate(ref FamilySkillMissionDTO familyskillmission);

        IList<FamilySkillMissionDTO> LoadByFamilyId(long familyId);


        void DailyReset(FamilySkillMissionDTO fsm);

        IEnumerable<FamilySkillMissionDTO> LoadAll();
        #endregion
    }
}
