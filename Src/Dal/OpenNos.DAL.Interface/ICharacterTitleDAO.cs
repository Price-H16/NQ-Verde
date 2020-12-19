using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterTitleDAO
    {
        #region Methods

        DeleteResult Delete(long characterTitleId);

        SaveResult InsertOrUpdate(ref CharacterTitleDTO characterTitle);

        IEnumerable<CharacterTitleDTO> LoadByCharacterId(long characterId);

        #endregion
    }
}