using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Domain;

namespace OpenNos.DAL.Interface
{
    public interface IItemInstanceDAO
    {
        #region Methods

        DeleteResult Delete(Guid id);

        DeleteResult DeleteFromSlotAndType(long characterId, short slot, InventoryType type);

        DeleteResult DeleteGuidList(IEnumerable<Guid> guids);

        ItemInstanceDTO InsertOrUpdate(ItemInstanceDTO dto);

        IEnumerable<ItemInstanceDTO> InsertOrUpdate(IEnumerable<ItemInstanceDTO> dtos);

        SaveResult InsertOrUpdateFromList(IEnumerable<ItemInstanceDTO> items);

        IEnumerable<ItemInstanceDTO> LoadByCharacterId(long characterId);

        ItemInstanceDTO LoadById(Guid id);

        ItemInstanceDTO LoadBySlotAndType(long characterId, short slot, InventoryType type);

        IEnumerable<ItemInstanceDTO> LoadByType(long characterId, InventoryType type);

        IList<Guid> LoadSlotAndTypeByCharacterId(long characterId);

        #endregion
    }
}