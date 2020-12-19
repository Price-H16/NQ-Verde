﻿using System;
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
    public class StaticBuffDAO : IStaticBuffDAO
    {
        #region Methods

        public static StaticBuffDTO LoadById(long sbId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new StaticBuffDTO();
                    if (StaticBuffMapper.ToStaticBuffDTO(
                        context.StaticBuff.FirstOrDefault(s => s.StaticBuffId.Equals(sbId)), dto)
                    ) //who the fuck was so retarded and set it to respawn ?!?
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

        public void Delete(short bonusToDelete, long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var bon = context.StaticBuff.FirstOrDefault(c =>
                        c.CardId == bonusToDelete && c.CharacterId == characterId);

                    if (bon != null)
                    {
                        context.StaticBuff.Remove(bon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bonusToDelete, e.Message), e);
            }
        }

        public SaveResult InsertOrUpdate(ref StaticBuffDTO staticBuff)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var id = staticBuff.CharacterId;
                    var cardid = staticBuff.CardId;
                    var entity = context.StaticBuff.FirstOrDefault(c => c.CardId == cardid && c.CharacterId == id);

                    if (entity == null)
                    {
                        staticBuff = insert(staticBuff, context);
                        return SaveResult.Inserted;
                    }

                    staticBuff.StaticBuffId = entity.StaticBuffId;
                    staticBuff = update(entity, staticBuff, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<StaticBuffDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<StaticBuffDTO>();
                foreach (var entity in context.StaticBuff.Where(i => i.CharacterId == characterId))
                {
                    var dto = new StaticBuffDTO();
                    StaticBuffMapper.ToStaticBuffDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public IEnumerable<short> LoadByTypeCharacterId(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return context.StaticBuff.Where(i => i.CharacterId == characterId).Select(qle => qle.CardId)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static StaticBuffDTO insert(StaticBuffDTO sb, OpenNosContext context)
        {
            try
            {
                var entity = new StaticBuff();
                StaticBuffMapper.ToStaticBuff(sb, entity);
                context.StaticBuff.Add(entity);
                context.SaveChanges();
                if (StaticBuffMapper.ToStaticBuffDTO(entity, sb)) return sb;

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static StaticBuffDTO update(StaticBuff entity, StaticBuffDTO sb, OpenNosContext context)
        {
            if (entity != null)
            {
                StaticBuffMapper.ToStaticBuff(sb, entity);
                context.SaveChanges();
            }

            if (StaticBuffMapper.ToStaticBuffDTO(entity, sb)) return sb;

            return null;
        }

        #endregion
    }
}