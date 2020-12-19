using System;
using System.Collections.Generic;
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
    public class RespawnDAO : IRespawnDAO
    {
        #region Methods

        public SaveResult InsertOrUpdate(ref RespawnDTO respawn)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var CharacterId = respawn.CharacterId;
                    var RespawnMapTypeId = respawn.RespawnMapTypeId;
                    var entity = context.Respawn.FirstOrDefault(c =>
                        c.RespawnMapTypeId.Equals(RespawnMapTypeId) && c.CharacterId.Equals(CharacterId));

                    if (entity == null)
                    {
                        respawn = insert(respawn, context);
                        return SaveResult.Inserted;
                    }

                    respawn.RespawnId = entity.RespawnId;
                    respawn = update(entity, respawn, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<RespawnDTO> LoadByCharacter(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<RespawnDTO>();
                foreach (var Respawnobject in context.Respawn.Where(i => i.CharacterId.Equals(characterId)))
                {
                    var dto = new RespawnDTO();
                    RespawnMapper.ToRespawnDTO(Respawnobject, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public RespawnDTO LoadById(long respawnId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new RespawnDTO();
                    if (RespawnMapper.ToRespawnDTO(context.Respawn.FirstOrDefault(s => s.RespawnId.Equals(respawnId)),
                        dto)) return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static RespawnDTO insert(RespawnDTO respawn, OpenNosContext context)
        {
            try
            {
                var entity = new Respawn();
                RespawnMapper.ToRespawn(respawn, entity);
                context.Respawn.Add(entity);
                context.SaveChanges();
                if (RespawnMapper.ToRespawnDTO(entity, respawn)) return respawn;

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static RespawnDTO update(Respawn entity, RespawnDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                RespawnMapper.ToRespawn(respawn, entity);
                context.SaveChanges();
            }

            if (RespawnMapper.ToRespawnDTO(entity, respawn)) return respawn;

            return null;
        }

        #endregion
    }
}