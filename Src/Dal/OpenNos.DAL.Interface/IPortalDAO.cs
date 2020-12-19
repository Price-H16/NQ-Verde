using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IPortalDAO
    {
        #region Methods

        PortalDTO Insert(PortalDTO portal);

        void Insert(List<PortalDTO> portals);

        IEnumerable<PortalDTO> LoadByMap(short mapId);

        IEnumerable<PortalDTO> LoadAll();

        #endregion
    }
}