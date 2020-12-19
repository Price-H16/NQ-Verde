using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class FamilyDTO
    {
        #region Properties

        public int FamilyExperience { get; set; }

        public byte FamilyFaction { get; set; }

        public GenderType FamilyHeadGender { get; set; }

        public long FamilyId { get; set; }

        public byte FamilyLevel { get; set; }

        public string FamilyMessage { get; set; }

        public long LastFactionChange { get; set; }

        public FamilyAuthorityType ManagerAuthorityType { get; set; }

        public bool ManagerCanGetHistory { get; set; }

        public bool ManagerCanInvite { get; set; }

        public bool ManagerCanNotice { get; set; }

        public bool ManagerCanShout { get; set; }

        public short MaxSize { get; set; }

        public FamilyAuthorityType MemberAuthorityType { get; set; }

        public bool MemberCanGetHistory { get; set; }

        public string Name { get; set; }

        public byte WarehouseSize { get; set; }

        #endregion
    }
}