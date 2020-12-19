using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IRollGeneratedItemDAO
    {
        #region Methods

        RollGeneratedItemDTO Insert(RollGeneratedItemDTO item);

        IEnumerable<RollGeneratedItemDTO> LoadAll();

        RollGeneratedItemDTO LoadById(short id);

        IEnumerable<RollGeneratedItemDTO> LoadByItemVNum(short vnum);

        #endregion
    }
}