using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Domain;

namespace OpenNos.DAL.Interface
{
    public interface IAccountDAO
    {
        #region Methods

        DeleteResult Delete(long accountId);

        SaveResult InsertOrUpdate(ref AccountDTO account);

        bool ContainsAccounts();

        void Insert(List<AccountDTO> account);

        AccountDTO LoadById(long accountId);

        AccountDTO LoadByName(string name);

        void WriteGeneralLog(long accountId, string ipAddress, long? characterId, GeneralLogType logType,
            string logData);

        #endregion
    }
}