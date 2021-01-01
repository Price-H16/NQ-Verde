using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Domain;
using OpenNos.Mapper.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class CharacterDAO : ICharacterDAO
    {
        #region Methods

        public DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    // actually a Character wont be deleted, it just will be disabled for future traces
                    var character = context.Character.SingleOrDefault(c =>
                        c.AccountId.Equals(accountId) && c.Slot.Equals(characterSlot) &&
                        c.State.Equals((byte)CharacterState.Active));

                    if (character != null)
                    {
                        character.State = (byte)CharacterState.Inactive;
                        character.Name = $"[DELETED]{character.Name}";
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), characterSlot,
                        e.Message), e);
                return DeleteResult.Error;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Compliment
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopCompliment()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterDTO>();
                foreach (var entity in context.Character
                    .Where(c => c.State == (byte)CharacterState.Active && c.Account.Authority == AuthorityType.User &&
                                !c.Account.PenaltyLog.Any(l =>
                                    l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now))
                    .OrderByDescending(c => c.Compliment).Take(30))
                {
                    var dto = new CharacterDTO();
                    CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Act4Points
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopPoints()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterDTO>();
                foreach (var entity in context.Character
                    .Where(c => c.State == (byte)CharacterState.Active && c.Account.Authority == AuthorityType.User &&
                                !c.Account.PenaltyLog.Any(l =>
                                    l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now))
                    .OrderByDescending(c => c.Act4Points).Take(30))
                {
                    var dto = new CharacterDTO();
                    CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        /// <summary>
        /// Returns first 30 occurences of highest Reputation
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopReputation()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterDTO>();
                foreach (var entity in context.Character
                    .Where(c => c.State == (byte)CharacterState.Active && c.Account.Authority == AuthorityType.User &&
                                !c.Account.PenaltyLog.Any(l =>
                                    l.Penalty == PenaltyType.Banned && l.DateEnd > DateTime.Now))
                    .OrderByDescending(c => c.Reputation).Take(43))
                {
                    var dto = new CharacterDTO();
                    CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterDTO character)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var characterId = character.CharacterId;
                    var entity = context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId));
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

        public IEnumerable<CharacterDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterDTO>();
                foreach (var chara in context.Character)
                {
                    var dto = new CharacterDTO();
                    CharacterMapper.ToCharacterDTO(chara, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public IEnumerable<CharacterDTO> LoadAllByAccount(long accountId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterDTO>();
                foreach (var entity in context.Character.Where(c => c.AccountId.Equals(accountId))
                    .OrderByDescending(c => c.Slot))
                {
                    var dto = new CharacterDTO();
                    CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public IEnumerable<CharacterDTO> LoadByAccount(long accountId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterDTO>();
                foreach (var entity in context.Character
                    .Where(c => c.AccountId.Equals(accountId) && c.State.Equals((byte)CharacterState.Active))
                    .OrderByDescending(c => c.Slot))
                {
                    var dto = new CharacterDTO();
                    CharacterMapper.ToCharacterDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public CharacterDTO LoadById(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new CharacterDTO();
                    if (CharacterMapper.ToCharacterDTO(
                        context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId)), dto)) return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public CharacterDTO LoadByName(string name)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new CharacterDTO();
                    if (CharacterMapper.ToCharacterDTO(context.Character.SingleOrDefault(c => c.Name.Equals(name)), dto)
                    ) return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public CharacterDTO LoadBySlot(long accountId, byte slot)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new CharacterDTO();
                    if (CharacterMapper.ToCharacterDTO(
                        context.Character.SingleOrDefault(c =>
                            c.AccountId.Equals(accountId) && c.Slot.Equals(slot) &&
                            c.State.Equals((byte)CharacterState.Active)), dto)) return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"There should be only 1 character per slot, AccountId: {accountId} Slot: {slot}", e);
                return null;
            }
        }

        private static CharacterDTO insert(CharacterDTO character, OpenNosContext context)
        {
            var entity = new Character();
            CharacterMapper.ToCharacter(character, entity);
            context.Character.Add(entity);
            context.SaveChanges();
            if (CharacterMapper.ToCharacterDTO(entity, character)) return character;
            return null;
        }

        private static CharacterDTO update(Character entity, CharacterDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                // State Updates should only occur upon deleting character, so outside of this method.
                var state = entity.State;
                CharacterMapper.ToCharacter(character, entity);
                entity.State = state;

                context.SaveChanges();
            }

            if (CharacterMapper.ToCharacterDTO(entity, character)) return character;

            return null;
        }

        #endregion
    }
}