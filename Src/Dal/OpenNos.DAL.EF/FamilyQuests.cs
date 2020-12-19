using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class FamilyQuests
    {
        [Key]
        public long FamilyQuestsId { get; set; }

        public long FamilyId { get; set; }

        public byte QuestType { get; set; }

        public short QuestId { get; set; }

        public bool Do { get; set; }

        public string Date { get; set; }

        public int Count { get; set; }
    }
}
