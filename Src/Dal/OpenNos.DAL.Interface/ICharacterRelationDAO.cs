using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterRelationDAO
    {
        #region Methods

        DeleteResult Delete(long characterRelationId);

        SaveResult InsertOrUpdate(ref CharacterRelationDTO characterRelation);

        IEnumerable<CharacterRelationDTO> LoadAll();

        CharacterRelationDTO LoadById(long characterId);

        #endregion
    }
}