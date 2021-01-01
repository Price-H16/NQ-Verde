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
    public class FamilyCharacterDAO : IFamilyCharacterDAO
    {
        #region Methods

        public DeleteResult Delete(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var familyCharacter = context.FamilyCharacter.FirstOrDefault(c => c.CharacterId == characterId);

                    if (familyCharacter == null) return DeleteResult.NotFound;

                    context.FamilyCharacter.Remove(familyCharacter);
                    context.SaveChanges();

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("DELETE_FAMILYCHARACTER_ERROR"), e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyCharacterDTO character)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var familyCharacterId = character.FamilyCharacterId;
                    var entity =
                        context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId));

                    if (entity == null)
                    {
                        character = insert(character, context);
                        return SaveResult.Inserted;
                    }

                    character = update(entity, character, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), character, e.Message),
                    e);
                return SaveResult.Error;
            }
        }

        public FamilyCharacterDTO LoadByCharacterId(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new FamilyCharacterDTO();
                    if (FamilyCharacterMapper.ToFamilyCharacterDTO(
                        context.FamilyCharacter.FirstOrDefault(c => c.CharacterId == characterId), dto)) return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IList<FamilyCharacterDTO> LoadByFamilyId(long familyId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<FamilyCharacterDTO>();
                foreach (var entity in context.FamilyCharacter.Where(fc => fc.FamilyId.Equals(familyId)))
                {
                    var dto = new FamilyCharacterDTO();
                    FamilyCharacterMapper.ToFamilyCharacterDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public FamilyCharacterDTO LoadById(long familyCharacterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new FamilyCharacterDTO();
                    if (FamilyCharacterMapper.ToFamilyCharacterDTO(
                        context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId)),
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

        private static FamilyCharacterDTO insert(FamilyCharacterDTO character, OpenNosContext context)
        {
            var entity = new FamilyCharacter();
            FamilyCharacterMapper.ToFamilyCharacter(character, entity);
            context.FamilyCharacter.Add(entity);
            context.SaveChanges();
            if (FamilyCharacterMapper.ToFamilyCharacterDTO(entity, character)) return character;

            return null;
        }

        private static FamilyCharacterDTO update(FamilyCharacter entity, FamilyCharacterDTO character,
            OpenNosContext context)
        {
            if (entity != null)
            {
                FamilyCharacterMapper.ToFamilyCharacter(character, entity);
                context.SaveChanges();
            }

            if (FamilyCharacterMapper.ToFamilyCharacterDTO(entity, character)) return character;

            return null;
        }

        #endregion
    }
}