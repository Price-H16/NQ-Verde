using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IBazaarItemDAO
    {
        #region Methods

        DeleteResult Delete(long bazaarItemId);

        SaveResult InsertOrUpdate(ref BazaarItemDTO bazaarItem);

        IEnumerable<BazaarItemDTO> LoadAll();

        BazaarItemDTO LoadById(long bazaarItemId);

        void RemoveOutDated();

        #endregion
    }
}