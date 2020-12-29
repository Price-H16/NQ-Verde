using System;
using OpenNos.Domain;
using CharacterState = OpenNos.Domain.CharacterState;
using FactionType = OpenNos.Domain.FactionType;
using GenderType = OpenNos.Domain.GenderType;
using HairColorType = OpenNos.Domain.HairColorType;
using HairStyleType = OpenNos.Domain.HairStyleType;
using MinilandState = OpenNos.Domain.MinilandState;

namespace OpenNos.Data
{
    [Serializable]
    public class CharacterDTO
    {
        #region Properties

        public long AccountId { get; set; }

        public int Act4Dead { get; set; }

        public int Act4Kill { get; set; }

        public int Act4Points { get; set; }

        public int ArenaWinner { get; set; }

        public long RBBWin { get; set; }

        public long RBBLose { get; set; }

        public string Biography { get; set; }

        public bool BuffBlocked { get; set; }

        public long CharacterId { get; set; }

        public ClassType Class { get; set; }

        public short Compliment { get; set; }

        public float Dignity { get; set; }

        public bool EmoticonsBlocked { get; set; }

        public bool ExchangeBlocked { get; set; }

        public FactionType Faction { get; set; }

        public bool FamilyRequestBlocked { get; set; }

        public bool FriendRequestBlocked { get; set; }

        public GenderType Gender { get; set; }

        public long Gold { get; set; }

        public bool GroupRequestBlocked { get; set; }

        public HairColorType HairColor { get; set; }

        public HairStyleType HairStyle { get; set; }

        public bool HeroChatBlocked { get; set; }

        public byte HeroLevel { get; set; }

        public long HeroXp { get; set; }

        public int Hp { get; set; }

        public bool HpBlocked { get; set; }

        public bool IsChangeName { get; set; }

        public bool IsPartnerAutoRelive { get; set; }

        public bool IsPetAutoRelive { get; set; }

        public bool IsSeal { get; set; }

        public byte JobLevel { get; set; }

        public long JobLevelXp { get; set; }

        public long LastFamilyLeave { get; set; }

        public byte Level { get; set; }

        public long LevelXp { get; set; }

        public short MapId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public int MasterPoints { get; set; }

        public int MasterTicket { get; set; }

        public byte MaxMateCount { get; set; }

        public byte MaxPartnerCount { get; set; }

        public bool MinilandInviteBlocked { get; set; }

        public string MinilandMessage { get; set; }

        public short MinilandPoint { get; set; }

        public MinilandState MinilandState { get; set; }

        public bool MouseAimLock { get; set; }

        public int Mp { get; set; }

        public string Name { get; set; }

        public bool QuickGetUp { get; set; }

        public long RagePoint { get; set; }

        public long Reputation { get; set; }

        public byte Slot { get; set; }

        public int SpAdditionPoint { get; set; }

        public int SpPoint { get; set; }

        public CharacterState State { get; set; }

        public int TalentLose { get; set; }

        public int TalentSurrender { get; set; }

        public int TalentWin { get; set; }

        public bool WhisperBlocked { get; set; }

        public int MobKillCounter { get; set; }

        public byte UnlockedHLevel { get; set; }

        public int ArenaDeath { get; set; }

        public int ArenaKill { get; set; }

        public bool HideHat { get; set; }
        
        public bool UiBlocked { get; set; }

        public string LockCode { get; set; }

        public bool VerifiedLock { get; set; }

        public long ArenaDie { get; set; }

        public long ArenaTc { get; set; }

        public long LastFactionChange { get; set; }

        public int BattleTowerExp { get; set; }

        public byte BattleTowerStage { get; set; }

        public long GoldBank { get; set; }
        
        public int ItemShopShip { get; set; }
        
        #endregion
    }
}