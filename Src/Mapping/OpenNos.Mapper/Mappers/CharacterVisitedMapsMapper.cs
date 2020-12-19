using OpenNos.DAL.EF;
using OpenNos.DAL.Interface.PropertiesMapping;
using OpenNos.Data;
using OpenNos.Mapper.Props;

namespace OpenNos.Mapper.Mappers
{
    public class CharacterVisitedMapsMapper : ModuleMapper<CharacterVisitedMapDTO, CharacterVisitedMaps>, IModuleMapper<CharacterVisitedMapDTO, CharacterVisitedMaps>
    {
        public override bool ToDTO(CharacterVisitedMaps input, CharacterVisitedMapDTO output)
        {
            if (input == null)
            {
                return false;
            }
            
            output.CharacterVisitedMapId = input.CharacterVisitedMapId;
            output.CharacterId = input.CharacterId;
            output.MapId = input.MapId;
            output.MapX = input.MapX;
            output.MapY = input.MapY;

            return true;
        }

        public override bool ToEntity(CharacterVisitedMapDTO input, CharacterVisitedMaps output)
        {
            if (input == null)
            {
                return false;
            }
            
            output.CharacterVisitedMapId = input.CharacterVisitedMapId;
            output.CharacterId = input.CharacterId;
            output.MapId = input.MapId;
            output.MapX = input.MapX;
            output.MapY = input.MapY;

            return true;
        }

        public static void ToDTOStatic(CharacterVisitedMaps entity, CharacterVisitedMapDTO dto)
        {
            new CharacterVisitedMapsMapper().ToDTO(entity, dto);
        }
    }
}