using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IMapNpcDAO
    {
        #region Methods

        DeleteResult DeleteById(int mapNpcId);

        bool DoesNpcExist(int mapNpcId);

        MapNpcDTO Insert(MapNpcDTO npc);

        void Insert(List<MapNpcDTO> npcs);

        IEnumerable<MapNpcDTO> LoadAll();

        MapNpcDTO LoadById(int mapNpcId);

        IEnumerable<MapNpcDTO> LoadFromMap(short mapId);

        SaveResult Update(ref MapNpcDTO mapNpc);

        #endregion
    }
}