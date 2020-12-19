﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Domain;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class ItemInstanceDAO : IItemInstanceDAO
    {
        #region Methods

        public virtual DeleteResult Delete(Guid id)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var entity = context.Set<ItemInstance>().FirstOrDefault(i => i.Id == id);
                if (entity != null)
                {
                    context.Set<ItemInstance>().Remove(entity);
                    context.SaveChanges();
                }

                return DeleteResult.Deleted;
            }
        }

        public DeleteResult DeleteFromSlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                var dto = LoadBySlotAndType(characterId, slot, type);
                if (dto != null) return Delete(dto.Id);

                return DeleteResult.Unknown;
            }
            catch (Exception e)
            {
                Logger.Error($"characterId: {characterId} slot: {slot} type: {type}", e);
                return DeleteResult.Error;
            }
        }

        public DeleteResult DeleteGuidList(IEnumerable<Guid> guids)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                try
                {
                    foreach (var id in guids)
                    {
                        var entity = context.ItemInstance.FirstOrDefault(i => i.Id == id);
                        if (entity != null && entity.Type != InventoryType.FamilyWareHouse)
                            context.ItemInstance.Remove(entity);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.LogUserEventError("DELETEGUIDLIST_EXCEPTION", "Saving Process", "Items were not deleted!",
                        ex);
                    foreach (var id in guids)
                        try
                        {
                            Delete(id);
                        }
                        catch (Exception exc)
                        {
                            // TODO: Work on: statement conflicted with the REFERENCE constraint
                            // "FK_dbo.BazaarItem_dbo.ItemInstance_ItemInstanceId". The conflict
                            // occurred in database "opennos", table "dbo.BazaarItem", column 'ItemInstanceId'.
                            Logger.LogUserEventError("ONSAVEDELETION_EXCEPTION", "Saving Process",
                                $"FALLBACK FUNCTION FAILED! Detailed Item Information: Item ID = {id}", exc);
                        }
                }

                return DeleteResult.Deleted;
            }
        }

        public IEnumerable<ItemInstanceDTO> InsertOrUpdate(IEnumerable<ItemInstanceDTO> dtos)
        {
            try
            {
                IList<ItemInstanceDTO> results = new List<ItemInstanceDTO>();
                using (var context = DataAccessHelper.CreateContext())
                {
                    foreach (var dto in dtos) results.Add(InsertOrUpdate(context, dto));
                }

                return results;
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return Enumerable.Empty<ItemInstanceDTO>();
            }
        }

        public ItemInstanceDTO InsertOrUpdate(ItemInstanceDTO dto)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, dto);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return null;
            }
        }

        public SaveResult InsertOrUpdateFromList(IEnumerable<ItemInstanceDTO> items)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    void insert(ItemInstanceDTO iteminstance)
                    {
                        var _entity = new ItemInstance();
                        map(iteminstance, _entity);
                        context.ItemInstance.Add(_entity);
                        context.SaveChanges();
                        iteminstance.Id = _entity.Id;
                    }

                    void update(ItemInstance _entity, ItemInstanceDTO iteminstance)
                    {
                        if (_entity != null) map(iteminstance, _entity);
                    }

                    foreach (var item in items)
                    {
                        var entity = context.ItemInstance.FirstOrDefault(c => c.Id == item.Id);

                        if (entity == null)
                            insert(item);
                        else
                            update(entity, item);
                    }

                    context.SaveChanges();
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<ItemInstanceDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<ItemInstanceDTO>();
                foreach (var itemInstance in context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)))
                {
                    var output = new ItemInstanceDTO();
                    map(itemInstance, output);
                    result.Add(output);
                }

                return result;
            }
        }

        public ItemInstanceDTO LoadById(Guid id)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var ItemInstanceDTO = new ItemInstanceDTO();
                if (map(context.ItemInstance.FirstOrDefault(i => i.Id.Equals(id)), ItemInstanceDTO))
                    return ItemInstanceDTO;

                return null;
            }
        }

        public ItemInstanceDTO LoadBySlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var entity = context.ItemInstance.FirstOrDefault(i =>
                        i.CharacterId == characterId && i.Slot == slot && i.Type == type);
                    var output = new ItemInstanceDTO();
                    if (map(entity, output)) return output;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ItemInstanceDTO> LoadByType(long characterId, InventoryType type)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<ItemInstanceDTO>();
                foreach (var itemInstance in context.ItemInstance.Where(i =>
                    i.CharacterId == characterId && i.Type == type))
                {
                    var output = new ItemInstanceDTO();
                    map(itemInstance, output);
                    result.Add(output);
                }

                return result;
            }
        }

        public IList<Guid> LoadSlotAndTypeByCharacterId(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)).Select(i => i.Id)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected static ItemInstanceDTO Insert(ItemInstanceDTO dto, OpenNosContext context)
        {
            var entity = new ItemInstance();
            map(dto, entity);
            context.Set<ItemInstance>().Add(entity);
            context.SaveChanges();
            if (map(entity, dto)) return dto;

            return null;
        }

        protected static ItemInstanceDTO InsertOrUpdate(OpenNosContext context, ItemInstanceDTO dto)
        {
            try
            {
                var entity = context.ItemInstance.FirstOrDefault(c => c.Id == dto.Id);
                return entity == null ? Insert(dto, context) : Update(entity, dto, context);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected static ItemInstanceDTO Update(ItemInstance entity, ItemInstanceDTO inventory, OpenNosContext context)
        {
            if (entity != null)
            {
                map(inventory, entity);
                context.SaveChanges();
            }

            if (map(entity, inventory)) return inventory;
            return null;
        }

        // TODO: Implement Exists
        private static bool map(ItemInstance input, ItemInstanceDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }

            ItemInstanceMapper.ToItemInstanceDTO(input, output);
            if (output.EquipmentSerialId == Guid.Empty) output.EquipmentSerialId = Guid.NewGuid();
            return true;
        }

        private static bool map(ItemInstanceDTO input, ItemInstance output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }

            ItemInstanceMapper.ToItemInstance(input, output);
            if (output.EquipmentSerialId == Guid.Empty) output.EquipmentSerialId = Guid.NewGuid();
            return true;
        }

        #endregion
    }
}