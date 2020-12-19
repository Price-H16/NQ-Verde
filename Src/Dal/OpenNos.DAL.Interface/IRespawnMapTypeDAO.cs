using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IRespawnMapTypeDAO
    {
        #region Methods

        void Insert(List<RespawnMapTypeDTO> respawnMapTypes);

        SaveResult InsertOrUpdate(ref RespawnMapTypeDTO respawnMapType);

        RespawnMapTypeDTO LoadById(long respawnMapTypeId);

        RespawnMapTypeDTO LoadByMapId(short mapId);

        IEnumerable<RespawnMapTypeDTO> LoadAll();

        #endregion
    }
}