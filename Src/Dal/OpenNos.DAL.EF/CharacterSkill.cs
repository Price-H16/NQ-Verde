namespace OpenNos.DAL.EF
{
    public class CharacterSkill : SynchronizableBaseEntity
    {
        #region Properties

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public virtual Skill Skill { get; set; }

        public short SkillVNum { get; set; }

        public bool IsTattoo { get; set; }

        public byte TattooLevel { get; set; }

        public bool IsPartnerSkill { get; set; }


        #endregion
    }
}