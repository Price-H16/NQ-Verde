using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IPenaltyLogDAO
    {
        #region Methods

        DeleteResult Delete(int penaltyLogId);

        SaveResult InsertOrUpdate(ref PenaltyLogDTO log);

        IEnumerable<PenaltyLogDTO> LoadAll();

        IEnumerable<PenaltyLogDTO> LoadByAccount(long accountId);

        PenaltyLogDTO LoadById(int penaltyLogId);

        IEnumerable<PenaltyLogDTO> LoadByIp(string ip);

        #endregion
    }
}