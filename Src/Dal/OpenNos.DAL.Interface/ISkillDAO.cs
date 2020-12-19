using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ISkillDAO
    {
        #region Methods

        SkillDTO Insert(SkillDTO skill);

        void Insert(List<SkillDTO> skills);

        SaveResult InsertOrUpdate(SkillDTO skill);

        IEnumerable<SkillDTO> LoadAll();

        SkillDTO LoadById(short skillId);

        #endregion
    }
}