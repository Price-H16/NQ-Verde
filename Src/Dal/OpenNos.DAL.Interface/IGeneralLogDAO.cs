using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IGeneralLogDAO
    {
        #region Methods

        bool IdAlreadySet(long id);

        GeneralLogDTO Insert(GeneralLogDTO generalLog);

        SaveResult InsertOrUpdate(ref GeneralLogDTO generalLog);

        IEnumerable<GeneralLogDTO> LoadAll();

        IEnumerable<GeneralLogDTO> LoadByAccount(long? accountId);

        IEnumerable<GeneralLogDTO> LoadByIp(string ip);

        IEnumerable<GeneralLogDTO> LoadByLogType(string logType, long? characterId);

        IEnumerable<GeneralLogDTO> LoadByLogTypeAndAccountId(string logType, long? accountId);

        void SetCharIdNull(long? characterId);

        void WriteGeneralLog(long accountId, string ipAddress, long? characterId, string logType, string logData);

        #endregion
    }
}