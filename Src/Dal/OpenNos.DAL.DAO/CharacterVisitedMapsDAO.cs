using OpenNos.DAL.DAO.Generic;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Mapper.Mappers;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class CharacterVisitedMapsDAO : GenericDAO<CharacterVisitedMapDTO, CharacterVisitedMaps>, ICharacterVisitedMapsDAO
    {
        #region Methods

        public void DeleteByCharacterId(long characterId)
        {
            throw new System.NotImplementedException();
        }

        public CharacterVisitedMapDTO InsertOrUpdate(CharacterVisitedMapDTO dto)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = InsertOrUpdate(context, dto, context.CharacterVisitedMaps, new CharacterVisitedMapsMapper(),
                    x => x.CharacterVisitedMapId == dto.CharacterVisitedMapId);
                context.SaveChanges();
                return result;
            }
        }

        public void InsertOrUpdateFromList(List<CharacterVisitedMapDTO> listMaps, long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                foreach (var dto in listMaps)
                {
                    if (dto.CharacterVisitedMapId == 0)
                    {
                        var newId = !context.CharacterVisitedMaps.Any() ? 1 : context.CharacterVisitedMaps.Max(x => x.CharacterVisitedMapId) + 1;

                        dto.CharacterVisitedMapId = newId;
                    }

                    dto.CharacterId = characterId;
                    InsertOrUpdate(context, dto, context.CharacterVisitedMaps, new CharacterVisitedMapsMapper(),
                        x => x.CharacterVisitedMapId == dto.CharacterVisitedMapId);
                    context.SaveChanges();
                }
            }
        }

        public List<CharacterVisitedMapDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<CharacterVisitedMapDTO>();
                foreach (var entity in context.CharacterVisitedMaps.Where(c => c.CharacterId == characterId))
                {
                    var dto = new CharacterVisitedMapDTO();
                    CharacterVisitedMapsMapper.ToDTOStatic(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        #endregion
    }
}