using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICardDAO
    {
        #region Methods

        CardDTO Insert(ref CardDTO card);

        void Insert(List<CardDTO> cards);

        SaveResult InsertOrUpdate(CardDTO card);

        IEnumerable<CardDTO> LoadAll();

        CardDTO LoadById(short cardId);

        #endregion
    }
}