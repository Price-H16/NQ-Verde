using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IRespawnDAO
    {
        #region Methods

        SaveResult InsertOrUpdate(ref RespawnDTO respawn);

        IEnumerable<RespawnDTO> LoadByCharacter(long characterId);

        RespawnDTO LoadById(long respawnId);

        #endregion
    }
}