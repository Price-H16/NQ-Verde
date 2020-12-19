using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IMinilandObjectDAO
    {
        #region Methods

        DeleteResult DeleteById(long id);

        SaveResult InsertOrUpdate(ref MinilandObjectDTO obj);

        IEnumerable<MinilandObjectDTO> LoadByCharacterId(long characterId);

        #endregion
    }
}