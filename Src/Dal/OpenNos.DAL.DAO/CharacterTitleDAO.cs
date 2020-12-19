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
    public class CharacterTitleDAO : ICharacterTitleDAO
    {
        #region Methods

        public IEnumerable<CharacterTitleDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterTitleDTO>();
                foreach (var charQuest in context.CharacterTitle.Where(s => s.CharacterId == characterId))
                {
                    var dto = new CharacterTitleDTO();
                    CharacterTitleMapper.ToTitleDTO(charQuest, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public DeleteResult Delete(long CharacterTitleId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var relation =
                        context.CharacterTitle.SingleOrDefault(c => c.CharacterTitleId.Equals(CharacterTitleId));

                    if (relation != null)
                    {
                        context.CharacterTitle.Remove(relation);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), CharacterTitleId,
                        e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterTitleDTO CharacterTitle)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var characterId = CharacterTitle.CharacterTitleId;
                    var entity = context.CharacterTitle.FirstOrDefault(c => c.CharacterTitleId.Equals(characterId));

                    if (entity == null)
                    {
                        CharacterTitle = insert(CharacterTitle, context);
                        return SaveResult.Inserted;
                    }

                    CharacterTitle = update(entity, CharacterTitle, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("UPDATE_CHARACTERTITLE_ERROR"),
                        CharacterTitle.CharacterTitleId, e.Message), e);
                return SaveResult.Error;
            }
        }

        private static CharacterTitleDTO insert(CharacterTitleDTO relation, OpenNosContext context)
        {
            var entity = new CharacterTitle();
            CharacterTitleMapper.ToTitle(relation, entity);
            context.CharacterTitle.Add(entity);
            context.SaveChanges();
            if (CharacterTitleMapper.ToTitleDTO(entity, relation)) return relation;

            return null;
        }

        private static CharacterTitleDTO update(CharacterTitle entity, CharacterTitleDTO relation,
            OpenNosContext context)
        {
            if (entity != null)
            {
                CharacterTitleMapper.ToTitle(relation, entity);
                context.SaveChanges();
            }

            if (CharacterTitleMapper.ToTitleDTO(entity, relation)) return relation;

            return null;
        }

        #endregion
    }
}