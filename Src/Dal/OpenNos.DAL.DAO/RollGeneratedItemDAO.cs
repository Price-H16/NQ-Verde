using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class RollGeneratedItemDAO : IRollGeneratedItemDAO
    {
        #region Methods

        public RollGeneratedItemDTO Insert(RollGeneratedItemDTO item)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var entity = new RollGeneratedItem();
                    RollGeneratedItemMapper.ToRollGeneratedItem(item, entity);
                    context.RollGeneratedItem.Add(entity);
                    context.SaveChanges();
                    if (RollGeneratedItemMapper.ToRollGeneratedItemDTO(entity, item)) return item;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RollGeneratedItemDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<RollGeneratedItemDTO>();
                foreach (var item in context.RollGeneratedItem)
                {
                    var dto = new RollGeneratedItemDTO();
                    RollGeneratedItemMapper.ToRollGeneratedItemDTO(item, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public RollGeneratedItemDTO LoadById(short id)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new RollGeneratedItemDTO();
                    if (RollGeneratedItemMapper.ToRollGeneratedItemDTO(
                        context.RollGeneratedItem.FirstOrDefault(i => i.RollGeneratedItemId.Equals(id)), dto))
                        return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RollGeneratedItemDTO> LoadByItemVNum(short vnum)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<RollGeneratedItemDTO>();
                foreach (var item in context.RollGeneratedItem.Where(s => s.OriginalItemVNum == vnum))
                {
                    var dto = new RollGeneratedItemDTO();
                    RollGeneratedItemMapper.ToRollGeneratedItemDTO(item, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        #endregion
    }
}