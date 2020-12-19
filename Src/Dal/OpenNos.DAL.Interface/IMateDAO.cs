using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IMateDAO
    {
        #region Methods

        DeleteResult Delete(long id);

        SaveResult InsertOrUpdate(ref MateDTO mate);

        IEnumerable<MateDTO> LoadByCharacterId(long characterId);

        #endregion
    }
}