using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IFamilyCharacterDAO
    {
        #region Methods

        DeleteResult Delete(long characterId);

        SaveResult InsertOrUpdate(ref FamilyCharacterDTO character);

        FamilyCharacterDTO LoadByCharacterId(long characterId);

        IList<FamilyCharacterDTO> LoadByFamilyId(long familyId);

        FamilyCharacterDTO LoadById(long familyCharacterId);

        #endregion
    }
}