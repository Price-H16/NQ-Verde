using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IMinigameLogDAO
    {
        #region Methods

        SaveResult InsertOrUpdate(ref MinigameLogDTO minigameLog);

        IEnumerable<MinigameLogDTO> LoadByCharacterId(long characterId);

        MinigameLogDTO LoadById(long minigameLogId);

        #endregion
    }
}