using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IQuicklistEntryDAO
    {
        #region Methods

        DeleteResult Delete(Guid id);

        QuicklistEntryDTO InsertOrUpdate(QuicklistEntryDTO dto);

        IEnumerable<QuicklistEntryDTO> InsertOrUpdate(IEnumerable<QuicklistEntryDTO> dtos);

        IEnumerable<QuicklistEntryDTO> LoadByCharacterId(long characterId);

        QuicklistEntryDTO LoadById(Guid id);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}