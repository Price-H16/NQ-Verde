using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IQuestLogDAO
    {
        #region Methods

        SaveResult InsertOrUpdate(ref QuestLogDTO questLog);

        IEnumerable<QuestLogDTO> LoadByCharacterId(long id);

        QuestLogDTO LoadById(long id);

        #endregion
    }
}