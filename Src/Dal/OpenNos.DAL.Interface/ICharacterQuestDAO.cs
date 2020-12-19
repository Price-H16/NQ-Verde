using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterQuestDAO
    {
        #region Methods

        DeleteResult Delete(long characterId, long questId);

        CharacterQuestDTO InsertOrUpdate(CharacterQuestDTO quest);

        IEnumerable<CharacterQuestDTO> LoadByCharacterId(long characterId);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}