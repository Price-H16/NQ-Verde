using OpenNos.DAL.EF;
using OpenNos.Data;
using OpenNos.Domain;
using CharacterState = OpenNos.Domain.CharacterState;
using FactionType = OpenNos.Domain.FactionType;

namespace OpenNos.Mapper.Mappers
{
    public static class CharacterMapper
    {
        #region Methods

        public static bool ToCharacter(CharacterDTO input, Character output)
        {
            if (input == null) return false;

            output.AccountId = input.AccountId;
            output.Act4Dead = input.Act4Dead;
            output.Act4Kill = input.Act4Kill;
            output.Act4Points = input.Act4Points;
            output.ArenaWinner = input.ArenaWinner;
            output.Biography = input.Biography;
            output.BuffBlocked = input.BuffBlocked;
            output.CharacterId = input.CharacterId;
            output.Class = (byte) input.Class;
            output.Compliment = input.Compliment;
            output.Dignity = (int) input.Dignity;
            output.EmoticonsBlocked = input.EmoticonsBlocked;
            output.ExchangeBlocked = input.ExchangeBlocked;
            output.Faction = (byte) input.Faction;
            output.FamilyRequestBlocked = input.FamilyRequestBlocked;
            output.FriendRequestBlocked = input.FriendRequestBlocked;
            output.Gender = input.Gender;
            output.Gold = input.Gold;
            output.GroupRequestBlocked = input.GroupRequestBlocked;
            output.HairColor = input.HairColor;
            output.HairStyle = input.HairStyle;
            output.HeroChatBlocked = input.HeroChatBlocked;
            output.HeroLevel = input.HeroLevel;
            output.HeroXp = input.HeroXp;
            output.Hp = input.Hp;
            output.HpBlocked = input.HpBlocked;
            output.IsPetAutoRelive = input.IsPetAutoRelive;
            output.IsPartnerAutoRelive = input.IsPartnerAutoRelive;
            output.IsSeal = input.IsSeal;
            output.JobLevel = input.JobLevel;
            output.JobLevelXp = input.JobLevelXp;
            output.LastFamilyLeave = input.LastFamilyLeave;
            output.Level = input.Level;
            output.LevelXp = input.LevelXp;
            output.MapId = input.MapId;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.MasterPoints = input.MasterPoints;
            output.MasterTicket = input.MasterTicket;
            output.MaxMateCount = input.MaxMateCount;
            output.MaxPartnerCount = input.MaxPartnerCount;
            output.MinilandInviteBlocked = input.MinilandInviteBlocked;
            output.MinilandMessage = input.MinilandMessage;
            output.MinilandPoint = input.MinilandPoint;
            output.MinilandState = input.MinilandState;
            output.MouseAimLock = input.MouseAimLock;
            output.Mp = input.Mp;
            output.Name = input.Name;
            output.QuickGetUp = input.QuickGetUp;
            output.RagePoint = input.RagePoint;
            output.Reputation = input.Reputation;
            output.Slot = input.Slot;
            output.SpAdditionPoint = input.SpAdditionPoint;
            output.SpPoint = input.SpPoint;
            output.State = (byte) input.State;
            output.TalentLose = input.TalentLose;
            output.TalentSurrender = input.TalentSurrender;
            output.TalentWin = input.TalentWin;
            output.ArenaDeath = input.ArenaDeath;
            output.ArenaKill = input.ArenaKill;
            output.WhisperBlocked = input.WhisperBlocked;
            output.IsChangeName = input.IsChangeName;
            output.MobKillCounter = input.MobKillCounter;
            output.UnlockedHLevel = input.UnlockedHLevel;
            output.HideHat = input.HideHat;
            output.UiBlocked = input.UiBlocked;
            output.LockCode = input.LockCode;
            output.VerifiedLock = input.VerifiedLock;
            output.RBBWin = input.RBBWin;
            output.RBBLose = input.RBBLose;
            output.ArenaDie = input.ArenaDie;
            output.ArenaTc = input.ArenaTc;
            output.LastFactionChange = input.LastFactionChange;
            output.BattleTowerExp = input.BattleTowerExp;
            output.BattleTowerStage = input.BattleTowerStage;
            output.GoldBank = input.GoldBank;
            output.ItemShopShip = input.ItemShopShip;



            return true;
        }

        public static bool ToCharacterDTO(Character input, CharacterDTO output)
        {
            if (input == null) return false;

            output.AccountId = input.AccountId;
            output.Act4Dead = input.Act4Dead;
            output.Act4Kill = input.Act4Kill;
            output.Act4Points = input.Act4Points;
            output.ArenaWinner = input.ArenaWinner;
            output.Biography = input.Biography;
            output.BuffBlocked = input.BuffBlocked;
            output.CharacterId = input.CharacterId;
            output.Class = (ClassType) input.Class;
            output.Compliment = input.Compliment;
            output.Dignity = input.Dignity;
            output.EmoticonsBlocked = input.EmoticonsBlocked;
            output.ExchangeBlocked = input.ExchangeBlocked;
            output.Faction = (FactionType) input.Faction;
            output.FamilyRequestBlocked = input.FamilyRequestBlocked;
            output.FriendRequestBlocked = input.FriendRequestBlocked;
            output.Gender = input.Gender;
            output.Gold = input.Gold;
            output.GroupRequestBlocked = input.GroupRequestBlocked;
            output.HairColor = input.HairColor;
            output.HairStyle = input.HairStyle;
            output.HeroChatBlocked = input.HeroChatBlocked;
            output.HeroLevel = input.HeroLevel;
            output.HeroXp = input.HeroXp;
            output.Hp = input.Hp;
            output.HpBlocked = input.HpBlocked;
            output.IsPetAutoRelive = input.IsPetAutoRelive;
            output.IsPartnerAutoRelive = input.IsPartnerAutoRelive;
            output.IsSeal = input.IsSeal;
            output.JobLevel = input.JobLevel;
            output.JobLevelXp = input.JobLevelXp;
            output.LastFamilyLeave = input.LastFamilyLeave;
            output.Level = input.Level;
            output.LevelXp = input.LevelXp;
            output.MapId = input.MapId;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.MasterPoints = input.MasterPoints;
            output.MasterTicket = input.MasterTicket;
            output.MaxMateCount = input.MaxMateCount;
            output.MaxPartnerCount = input.MaxPartnerCount;
            output.MinilandInviteBlocked = input.MinilandInviteBlocked;
            output.MinilandMessage = input.MinilandMessage;
            output.MinilandPoint = input.MinilandPoint;
            output.MinilandState = input.MinilandState;
            output.MouseAimLock = input.MouseAimLock;
            output.Mp = input.Mp;
            output.Name = input.Name;
            output.QuickGetUp = input.QuickGetUp;
            output.RagePoint = input.RagePoint;
            output.Reputation = input.Reputation;
            output.Slot = input.Slot;
            output.SpAdditionPoint = input.SpAdditionPoint;
            output.SpPoint = input.SpPoint;
            output.State = (CharacterState) input.State;
            output.TalentLose = input.TalentLose;
            output.TalentSurrender = input.TalentSurrender;
            output.TalentWin = input.TalentWin;
            output.WhisperBlocked = input.WhisperBlocked;
            output.IsChangeName = input.IsChangeName;
            output.MobKillCounter = input.MobKillCounter;
            output.UnlockedHLevel = input.UnlockedHLevel;
            output.HideHat = input.HideHat;
            output.UiBlocked = input.UiBlocked;
            output.LockCode = input.LockCode;
            output.VerifiedLock = input.VerifiedLock;
            output.RBBWin = input.RBBWin;
            output.RBBLose = input.RBBLose;
            output.ArenaDie = input.ArenaDie;
            output.ArenaTc = input.ArenaTc;
            output.ArenaKill = input.ArenaKill;
            output.LastFactionChange = input.LastFactionChange;
            output.BattleTowerExp = input.BattleTowerExp;
            output.BattleTowerStage = input.BattleTowerStage;
            output.GoldBank = input.GoldBank;
            output.ItemShopShip = input.ItemShopShip;



            return true;
        }

        #endregion
    }
}