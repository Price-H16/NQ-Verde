using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
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
    public class MateDAO : IMateDAO
    {
        #region Methods

        public DeleteResult Delete(long id)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var mate = context.Mate.FirstOrDefault(c => c.MateId.Equals(id));
                    if (mate != null)
                    {
                        context.Mate.Remove(mate);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_MATE_ERROR"), e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MateDTO mate)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var MateId = mate.MateId;
                    var entity = context.Mate.FirstOrDefault(c => c.MateId.Equals(MateId));

                    if (entity == null)
                    {
                        mate = insert(mate, context);
                        return SaveResult.Inserted;
                    }

                    mate = update(entity, mate, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), mate, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MateDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<MateDTO>();
                foreach (var mate in context.Mate.Where(s => s.CharacterId == characterId))
                {
                    var dto = new MateDTO();
                    MateMapper.ToMateDTO(mate, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        private static MateDTO insert(MateDTO mate, OpenNosContext context)
        {
            var entity = new Mate();
            MateMapper.ToMate(mate, entity);
            context.Mate.Add(entity);
            context.SaveChanges();
            if (MateMapper.ToMateDTO(entity, mate)) return mate;

            return null;
        }

        private static MateDTO update(Mate entity, MateDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                MateMapper.ToMate(character, entity);
                context.SaveChanges();
            }

            if (MateMapper.ToMateDTO(entity, character)) return character;

            return null;
        }

        #endregion
    }
}