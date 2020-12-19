using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IItemDAO
    {
        #region Methods

        IEnumerable<ItemDTO> FindByName(string name);

        ItemDTO Insert(ItemDTO item);

        void Insert(IEnumerable<ItemDTO> items);

        IEnumerable<ItemDTO> LoadAll();

        ItemDTO LoadById(short vNum);

        #endregion
    }
}