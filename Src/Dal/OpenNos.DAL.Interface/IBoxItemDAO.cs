using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IBoxItemDAO
    {
        #region Methods

        List<BoxItemDTO> LoadAll();

        #endregion
    }
}