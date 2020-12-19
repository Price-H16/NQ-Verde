using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface ICharacterVisitedMapsDAO
    {
        CharacterVisitedMapDTO InsertOrUpdate(CharacterVisitedMapDTO dto);
        void InsertOrUpdateFromList(List<CharacterVisitedMapDTO> listMaps, long characterId);
        void DeleteByCharacterId(long characterId);

        List<CharacterVisitedMapDTO> LoadByCharacterId(long characterId);
    }
}