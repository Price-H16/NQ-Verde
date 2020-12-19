using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IShopDAO
    {
        #region Methods

        DeleteResult DeleteByNpcId(int mapNpcId);

        ShopDTO Insert(ShopDTO shop);

        void Insert(List<ShopDTO> shops);

        IEnumerable<ShopDTO> LoadAll();

        ShopDTO LoadById(int shopId);

        ShopDTO LoadByNpc(int mapNpcId);

        SaveResult Update(ref ShopDTO shop);

        #endregion
    }
}