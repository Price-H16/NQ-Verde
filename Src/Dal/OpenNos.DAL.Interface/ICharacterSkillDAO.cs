using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterSkillDAO
    {
        #region Methods

        DeleteResult Delete(long characterId, short skillVNum);

        DeleteResult Delete(Guid id);

        CharacterSkillDTO InsertOrUpdate(CharacterSkillDTO dto);

        IEnumerable<CharacterSkillDTO> InsertOrUpdate(IEnumerable<CharacterSkillDTO> dtos);

        IEnumerable<CharacterSkillDTO> LoadByCharacterId(long characterId);

        CharacterSkillDTO LoadById(Guid id);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}