﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class BazaarItemDAO : IBazaarItemDAO
    {
        #region Methods

        public DeleteResult Delete(long bazaarItemId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var BazaarItem = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (BazaarItem != null)
                    {
                        context.BazaarItem.Remove(BazaarItem);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bazaarItemId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref BazaarItemDTO bazaarItem)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var bazaarItemId = bazaarItem.BazaarItemId;
                    var entity = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (entity == null)
                    {
                        bazaarItem = insert(bazaarItem, context);
                        return SaveResult.Inserted;
                    }

                    bazaarItem = update(entity, bazaarItem, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"BazaarItemId: {bazaarItem.BazaarItemId} Message: {e.Message}", e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<BazaarItemDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<BazaarItemDTO>();
                foreach (var bazaarItem in context.BazaarItem)
                {
                    var dto = new BazaarItemDTO();
                    BazaarItemMapper.ToBazaarItemDTO(bazaarItem, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public BazaarItemDTO LoadById(long bazaarItemId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new BazaarItemDTO();
                    if (BazaarItemMapper.ToBazaarItemDTO(
                        context.BazaarItem.FirstOrDefault(i => i.BazaarItemId.Equals(bazaarItemId)), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public void RemoveOutDated()
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    foreach (var entity in context.BazaarItem.Where(e =>
                        DbFunctions.AddDays(DbFunctions.AddHours(e.DateStart, e.Duration), e.MedalUsed ? 30 : 7) <
                        DateTime.Now))
                    {
                        context.BazaarItem.Remove(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static BazaarItemDTO insert(BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            var entity = new BazaarItem();
            BazaarItemMapper.ToBazaarItem(bazaarItem, entity);
            context.BazaarItem.Add(entity);
            context.SaveChanges();
            if (BazaarItemMapper.ToBazaarItemDTO(entity, bazaarItem))
            {
                return bazaarItem;
            }

            return null;
        }

        private static BazaarItemDTO update(BazaarItem entity, BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            if (entity != null)
            {
                BazaarItemMapper.ToBazaarItem(bazaarItem, entity);
                context.SaveChanges();
            }

            if (BazaarItemMapper.ToBazaarItemDTO(entity, bazaarItem))
            {
                return bazaarItem;
            }

            return null;
        }

        #endregion
    }
}