using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class CharacterVisitedMaps
    {
        [Key]
        public long CharacterVisitedMapId { get; set; }
        
        public virtual Character Character { get; set; }
        
        public long CharacterId { get; set; }
        
        public int MapId { get; set; }
        
        public int MapX { get; set; }
        
        public int MapY { get; set; }
    }
}