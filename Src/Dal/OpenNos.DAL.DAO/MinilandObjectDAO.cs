using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class MinilandObjectDAO : IMinilandObjectDAO
    {
        #region Methods

        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var item = context.MinilandObject.First(i => i.MinilandObjectId.Equals(id));

                    if (item != null)
                    {
                        context.MinilandObject.Remove(item);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MinilandObjectDTO obj)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var id = obj.MinilandObjectId;
                    var entity = context.MinilandObject.FirstOrDefault(c => c.MinilandObjectId.Equals(id));

                    if (entity == null)
                    {
                        obj = insert(obj, context);
                        return SaveResult.Inserted;
                    }

                    obj.MinilandObjectId = entity.MinilandObjectId;
                    obj = update(entity, obj, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MinilandObjectDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<MinilandObjectDTO>();
                foreach (var obj in context.MinilandObject.Where(s => s.CharacterId == characterId))
                {
                    var dto = new MinilandObjectDTO();
                    MinilandObjectMapper.ToMinilandObjectDTO(obj, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        private static MinilandObjectDTO insert(MinilandObjectDTO obj, OpenNosContext context)
        {
            try
            {
                var entity = new MinilandObject();
                MinilandObjectMapper.ToMinilandObject(obj, entity);
                context.MinilandObject.Add(entity);
                context.SaveChanges();
                if (MinilandObjectMapper.ToMinilandObjectDTO(entity, obj))
                {
                    return obj;
                }

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static MinilandObjectDTO update(MinilandObject entity, MinilandObjectDTO respawn,
            OpenNosContext context)
        {
            if (entity != null)
            {
                MinilandObjectMapper.ToMinilandObject(respawn, entity);
                context.SaveChanges();
            }

            if (MinilandObjectMapper.ToMinilandObjectDTO(entity, respawn))
            {
                return respawn;
            }

            return null;
        }

        #endregion
    }
}