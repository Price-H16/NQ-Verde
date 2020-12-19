using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ILogsCommandsDAO
    {
        #region Methods

        void InsertOrUpdate(IEnumerable<LogCommandsDTO> logs);

        #endregion
    }
}