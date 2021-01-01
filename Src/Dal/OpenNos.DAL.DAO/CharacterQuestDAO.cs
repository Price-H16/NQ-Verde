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
    public class CharacterQuestDAO : ICharacterQuestDAO
    {
        #region Methods

        public DeleteResult Delete(long characterId, long questId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var charQuest =
                        context.CharacterQuest.FirstOrDefault(i =>
                            i.CharacterId == characterId && i.QuestId == questId);
                    if (charQuest != null)
                    {
                        context.CharacterQuest.Remove(charQuest);
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

        public CharacterQuestDTO InsertOrUpdate(CharacterQuestDTO charQuest)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, charQuest);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return null;
            }
        }

        public IEnumerable<CharacterQuestDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterQuestDTO>();
                foreach (var charQuest in context.CharacterQuest.Where(s => s.CharacterId == characterId))
                {
                    var dto = new CharacterQuestDTO();
                    CharacterQuestMapper.ToCharacterQuestDTO(charQuest, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterQuest.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected static CharacterQuestDTO InsertOrUpdate(OpenNosContext context, CharacterQuestDTO dto)
        {
            var primaryKey = dto.Id;
            var entity = context.Set<CharacterQuest>().FirstOrDefault(c => c.Id == primaryKey);
            if (entity == null)
                return Insert(dto, context);
            return Update(entity, dto, context);
        }

        private static CharacterQuestDTO Insert(CharacterQuestDTO charQuest, OpenNosContext context)
        {
            var entity = new CharacterQuest();
            CharacterQuestMapper.ToCharacterQuest(charQuest, entity);
            context.CharacterQuest.Add(entity);
            context.SaveChanges();
            if (CharacterQuestMapper.ToCharacterQuestDTO(entity, charQuest)) return charQuest;

            return null;
        }

        private static CharacterQuestDTO Update(CharacterQuest entity, CharacterQuestDTO charQuest,
            OpenNosContext context)
        {
            if (entity != null)
            {
                CharacterQuestMapper.ToCharacterQuest(charQuest, entity);
                context.SaveChanges();
            }

            if (CharacterQuestMapper.ToCharacterQuestDTO(entity, charQuest)) return charQuest;

            return null;
        }

        #endregion
    }
}