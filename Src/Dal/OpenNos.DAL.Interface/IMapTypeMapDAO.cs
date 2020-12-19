using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IMapTypeMapDAO
    {
        #region Methods

        void Insert(List<MapTypeMapDTO> mapTypeMaps);

        IEnumerable<MapTypeMapDTO> LoadAll();

        MapTypeMapDTO LoadByMapAndMapType(short mapId, short maptypeId);

        IEnumerable<MapTypeMapDTO> LoadByMapId(short mapId);

        IEnumerable<MapTypeMapDTO> LoadByMapTypeId(short maptypeId);

        public short GetMapTypeIdByMapId(short mapId);

        #endregion
    }
}