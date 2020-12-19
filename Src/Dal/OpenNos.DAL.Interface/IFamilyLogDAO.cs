using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IFamilyLogDAO
    {
        #region Methods

        DeleteResult Delete(long familyLogId);

        SaveResult InsertOrUpdate(ref FamilyLogDTO familyLog);

        IEnumerable<FamilyLogDTO> LoadByFamilyId(long familyId);

        #endregion
    }
}