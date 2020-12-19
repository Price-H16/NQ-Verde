using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IMapDAO
    {
        #region Methods

        MapDTO Insert(MapDTO map);

        void Insert(List<MapDTO> maps);

        IEnumerable<MapDTO> LoadAll();

        MapDTO LoadById(short mapId);

        #endregion
    }
}