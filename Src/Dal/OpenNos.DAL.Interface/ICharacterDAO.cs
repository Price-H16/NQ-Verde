using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterDAO
    {
        #region Methods

        DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot);

        List<CharacterDTO> GetTopCompliment();

        List<CharacterDTO> GetTopPoints();

        List<CharacterDTO> GetTopReputation();

        SaveResult InsertOrUpdate(ref CharacterDTO character);

        IEnumerable<CharacterDTO> LoadAll();

        IEnumerable<CharacterDTO> LoadAllByAccount(long accountId);

        IEnumerable<CharacterDTO> LoadByAccount(long accountId);

        CharacterDTO LoadById(long characterId);

        CharacterDTO LoadByName(string name);

        CharacterDTO LoadBySlot(long accountId, byte slot);

        #endregion
    }
}