using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using NosTale.Packets.Packets.ClientPackets;
using NosTale.Packets.Packets.ServerPackets;
using OpenNos.Core;
using OpenNos.Core.ConcurrencyExtensions;
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Extension.Inventory;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using OpenNos.PathFinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.GameObject._Event;
using static OpenNos.Domain.BCardType;
using OpenNos.GameObject.RainbowBattle;
using OpenNos.GameObject.Extensions;

namespace OpenNos.GameObject
{
    // Little test
    public class Character : CharacterDTO
    {
        #region Members

        public int OriginalFaction = -1;
        public int slhpbonus;
        private readonly object _syncObj = new object();

        private bool _isStaticBuffListInitial;

        private Random _random;
        private byte _speed;

        #endregion

        #region Instantiation

        public Character()
        {
            GroupSentRequestCharacterIds = new ThreadSafeGenericList<long>();
            FamilyInviteCharacters = new ThreadSafeGenericList<long>();
            TradeRequests = new ThreadSafeGenericList<long>();
            FriendRequestCharacters = new ThreadSafeGenericList<long>();
            MarryRequestCharacters = new ThreadSafeGenericList<long>();
            StaticBonusList = new List<StaticBonusDTO>();
            MinilandObjects = new List<MinilandObject>();
            Mates = new List<Mate>();
            LastMonsterAggro = DateTime.Now;
            LastPulse = DateTime.Now;
            LastFreeze = DateTime.Now;
            MTListTargetQueue = new ConcurrentStack<MTListHitTarget>();
            MeditationDictionary = new Dictionary<short, DateTime>();
            PVELockObject = new object();
            SpeedLockObject = new object();
            ShellEffectArmor = new ConcurrentBag<ShellEffectDTO>();
            ShellEffectMain = new ConcurrentBag<ShellEffectDTO>();
            RuneEffectMain = new ConcurrentBag<RuneEffectDTO>();
            ShellEffectSecondary = new ConcurrentBag<ShellEffectDTO>();
            Quests = new ConcurrentBag<CharacterQuest>();
            DamageList = new Dictionary<long, long>();
            Title = new List<CharacterTitleDTO>();
            EffectFromTitle = new ThreadSafeGenericList<BCard>();
        }

        public Character(CharacterDTO input) : this()
        {
            AccountId = input.AccountId;
            Act4Dead = input.Act4Dead;
            Act4Kill = input.Act4Kill;
            Act4Points = input.Act4Points;
            ArenaWinner = input.ArenaWinner;
            Biography = input.Biography;
            BuffBlocked = input.BuffBlocked;
            CharacterId = input.CharacterId;
            Class = input.Class;
            Compliment = input.Compliment;
            Dignity = input.Dignity;
            EmoticonsBlocked = input.EmoticonsBlocked;
            ExchangeBlocked = input.ExchangeBlocked;
            Faction = input.Faction;
            FamilyRequestBlocked = input.FamilyRequestBlocked;
            FriendRequestBlocked = input.FriendRequestBlocked;
            Gender = input.Gender;
            Gold = input.Gold;
            GroupRequestBlocked = input.GroupRequestBlocked;
            HairColor = input.HairColor;
            HairStyle = input.HairStyle;
            HeroChatBlocked = input.HeroChatBlocked;
            HeroLevel = input.HeroLevel;
            HeroXp = input.HeroXp;
            Hp = input.Hp;
            HpBlocked = input.HpBlocked;
            IsPetAutoRelive = input.IsPetAutoRelive;
            IsPartnerAutoRelive = input.IsPartnerAutoRelive;
            IsSeal = input.IsSeal;
            JobLevel = input.JobLevel;
            JobLevelXp = input.JobLevelXp;
            LastFamilyLeave = input.LastFamilyLeave;
            Level = input.Level;
            LevelXp = input.LevelXp;
            MapId = input.MapId;
            MapX = input.MapX;
            MapY = input.MapY;
            MasterPoints = input.MasterPoints;
            MasterTicket = input.MasterTicket;
            MaxMateCount = input.MaxMateCount;
            MaxPartnerCount = input.MaxPartnerCount;
            MinilandInviteBlocked = input.MinilandInviteBlocked;
            MinilandMessage = input.MinilandMessage;
            MinilandPoint = input.MinilandPoint;
            MinilandState = input.MinilandState;
            MouseAimLock = input.MouseAimLock;
            Mp = input.Mp;
            Name = input.Name;
            QuickGetUp = input.QuickGetUp;
            RagePoint = input.RagePoint;
            Reputation = input.Reputation;
            Slot = input.Slot;
            SpAdditionPoint = input.SpAdditionPoint;
            SpPoint = input.SpPoint;
            State = input.State;
            TalentLose = input.TalentLose;
            TalentSurrender = input.TalentSurrender;
            TalentWin = input.TalentWin;
            ArenaDeath = input.ArenaDeath;
            ArenaKill = input.ArenaKill;
            WhisperBlocked = input.WhisperBlocked;
            IsChangeName = input.IsChangeName;
            MobKillCounter = input.MobKillCounter;
            UnlockedHLevel = input.UnlockedHLevel;
            LockCode = input.LockCode;
            VerifiedLock = input.VerifiedLock;
            LastFactionChange = input.LastFactionChange;
            BattleTowerExp = input.BattleTowerExp;
            BattleTowerStage = input.BattleTowerStage;

        }

        #endregion

        #region Properties

        public AuthorityType Authority { get; set; }

        public BattleEntity BattleEntity { get; set; }

        public EventEntity Event { get; set; }

        public byte BeforeDirection { get; set; }

        public byte gameLifes = 3;

        public bool isFreezed { get; set; }

        public bool IsLocked { get; set; }
        public Node[][] BrushFireJagged { get; set; }
        public int SheepScore1 { get; set; }

        public bool IsWaitingForGift { get; set; }
        public int SheepScore2 { get; set; }

        public int SheepScore3 { get; set; }

        public string BubbleMessage { get; set; }

        public DateTime LastISort { get; set; }

        public byte BazarRequests { get; set; }

        public DateTime BubbleMessageEnd { get; set; }

        public ThreadSafeSortedList<short, Buff> Buff => BattleEntity.Buffs;

        public ThreadSafeSortedList<short, IDisposable> BuffObservables => BattleEntity.BuffObservables;

        public bool CanFight => !IsSitting && ExchangeInfo == null;

        public ThreadSafeGenericList<CellonOptionDTO> CellonOptions => BattleEntity.CellonOptions;

        public ServerManager Channel { get; set; }

        public List<CharacterRelationDTO> CharacterRelations
        {
            get
            {
                lock (ServerManager.Instance.CharacterRelations)
                {
                    return ServerManager.Instance.CharacterRelations == null ? new List<CharacterRelationDTO>() : ServerManager.Instance.CharacterRelations.Where(s => s.CharacterId == CharacterId || s.RelatedCharacterId == CharacterId).ToList();
                }
            }
        }

        public int ChargeValue { get; set; }

        public int ConvertedDamageToHP { get; set; }

        public short CurrentMinigame { get; set; }

        public IDictionary<long, long> DamageList { get; set; }

        public int DarkResistance { get; set; }

        public int Defence { get; set; }

        public int DefenceRate { get; set; }

        public byte Direction { get; set; }

        public int DistanceDefence { get; set; }

        public int DistanceDefenceRate { get; set; }

        public IDisposable DragonModeObservable { get; set; }

        public ThreadSafeGenericList<BCard> EffectFromTitle { get; set; }

        public byte Element { get; set; }

        public int ElementRate { get; set; }

        public int ElementRateSP { get; private set; }

        public ThreadSafeGenericLockedList<BCard> EquipmentBCards => BattleEntity.BCards;

        public ExchangeInfo ExchangeInfo { get; set; }

        public Family Family { get; set; }

        public FamilyCharacterDTO FamilyCharacter => Family?.FamilyCharacters.Find(s => s.CharacterId == CharacterId);

        public ThreadSafeGenericList<long> FamilyInviteCharacters { get; set; }

        public int FireResistance { get; set; }

        public int FoodAmount { get; set; }

        public int FoodHp { get; set; }

        public int FoodMp { get; set; }

        public ThreadSafeGenericList<long> FriendRequestCharacters { get; set; }

        public ThreadSafeGenericList<GeneralLogDTO> GeneralLogs { get; set; }

        public bool GmPvtBlock { get; set; }

        public Group Group { get; set; }

        public ThreadSafeGenericList<long> GroupSentRequestCharacterIds { get; set; }

        public bool HasGodMode { get; set; }

        public bool HasMagicalFetters => HasBuff(608);

        public bool HasMagicSpellCombo => HasBuff(617) && (LastComboCastId >= 11 && LastComboCastId <= 15);

        public bool HasShopOpened { get; set; }

        public int HitCriticalChance { get; set; }

        public int HitCriticalRate { get; set; }

        public int HitRate { get; set; }

        public bool InExchangeOrTrade => ExchangeInfo != null || Speed == 0;

        public Inventory Inventory { get; set; }

        public bool Invisible { get; set; }

        public bool InvisibleGm { get; set; }

        public int CurrentArenaKill { get; set; }

        public int CurrentArenaDeath { get; set; }

        public bool IsChangingMapInstance { get; set; }

        public bool IsCustomSpeed { get; set; }

        public bool IsDancing { get; set; }

        public bool IsDisposed { get; private set; }

        public DateTime LastBazaarBuy { get; set; }

        /// <summary>
        /// Defines if the Character Is currently sending or getting items thru exchange.
        /// </summary>
        public bool IsExchanging { get; set; }

        public bool IsMarried => CharacterRelations.Any(c => c.RelationType == CharacterRelationType.Spouse);

        public bool IsMorphed { get; set; }

        public bool IsShopping { get; set; }

        public bool IsSitting { get; set; }

        public byte IsUsingFairyBooster => _isStaticBuffListInitial ? (byte)(Buff.ContainsKey(131) ? 1 : Buff.ContainsKey(4045) ? 2 : 0)  : (byte)(DAOFactory.StaticBuffDAO.LoadByCharacterId(CharacterId).Any(s => s.CardId.Equals(131)) ? 1  : DAOFactory.StaticBuffDAO.LoadByCharacterId(CharacterId).Any(s => s.CardId.Equals(4045)) ? 2 : 0);

        public bool IsVehicled { get; set; }

        public bool IsWaitingForEvent { get; set; }

        public DateTime LastCMD { get; set; }

        public int LastComboCastId { get; set; }

        public DateTime LastCommand { get; set; }

        public DateTime LastBazaarInsert { get; set; }

        public DateTime LastBazaarModeration { get; set; }

        public DateTime LastDefence { get; set; }

        public DateTime LastDelay { get; set; }

        public DateTime LastDeposit { get; set; }

        public DateTime LastEffect { get; set; }

        public DateTime LastFreeze { get; set; }

        public DateTime LastFunnelUse { get; set; }

        public DateTime LastPotionUse { get; set; }

        public DateTime LastHealth { get; set; }

        public short LastItemVNum { get; set; }

        public DateTime FamilyBuff { get; set; }

        public DateTime LastLoyalty { get; set; }

        public DateTime LastMapObject { get; set; }

        public DateTime LastMonsterAggro { get; set; }

        public DateTime LastMove { get; set; }

        public int LastNpcMonsterId { get; set; }

        public int LastNRunId { get; set; }

        public DateTime LastPermBuffRefresh { get; set; }

        public double LastPortal { get; set; }

        public DateTime LastPotion { get; set; }

        public DateTime LastPulse { get; set; }

        public ClientSession LastPvPKiller { get; set; }

        public DateTime LastPVPRevive { get; set; }

        public DateTime LastQuest { get; set; }

        public DateTime LastQuestSummon { get; set; }

        public DateTime LastRepos { get; set; }

        public DateTime LastRoll { get; set; }

        public DateTime LastSchedule { get; set; }

        public DateTime LastSpeaker { get; set; }

        public DateTime LastSkillComboUse { get; set; }

        public DateTime LastSkillUse { get; set; }

        public double LastSp { get; set; }

        public DateTime LastSpeedChange { get; set; }

        public DateTime LastSpGaugeRemove { get; set; }

        public DateTime LastTransform { get; set; }

        public DateTime LastUnstuck { get; set; }

        public DateTime LastVessel { get; set; }

        public DateTime LastWarp { get; set; }

        public DateTime LastSort { get; set; }

        public DateTime LastWithdraw { get; set; }

        public IDisposable Life { get; set; }

        public int LightResistance { get; set; }

        public int MagicalDefence { get; set; }

        public IDictionary<int, MailDTO> MailList { get; set; }

        public MapInstance MapInstance => ServerManager.GetMapInstance(MapInstanceId);

        public Guid MapInstanceId { get; set; }

        public ThreadSafeGenericList<long> MarryRequestCharacters { get; set; }

        public List<Mate> Mates { get; set; }

        public int MaxFood { get; set; }

        public int MaxHit { get; set; }

        public int MaxSnack { get; set; }

        public Dictionary<short, DateTime> MeditationDictionary { get; set; }

        public int MessageCounter { get; set; }

        public int MinHit { get; set; }

        public MinigameLogDTO MinigameLog { get; set; }

        public MapInstance Miniland { get; private set; }

        public List<MinilandObject> MinilandObjects { get; set; }

        public int Morph { get; set; }

        public int MorphUpgrade { get; set; }

        public int MorphUpgrade2 { get; set; }

        public ConcurrentStack<MTListHitTarget> MTListTargetQueue { get; set; }

        public bool NoAttack { get; set; }

        public bool NoMove { get; set; }

        public List<EventContainer> OnDeathEvents => BattleEntity.OnDeathEvents;

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public int PreviousMorph { get; set; }

        public object PVELockObject { get; set; }

        public bool PyjamaDead { get; set; }

        public ConcurrentBag<CharacterQuest> Quests { get; internal set; }

        public List<QuicklistEntryDTO> QuicklistEntries { get; private set; }

        public RespawnMapTypeDTO Respawn
        {
            get
            {
                RespawnMapTypeDTO respawn = new RespawnMapTypeDTO
                {
                    DefaultX = 79,
                    DefaultY = 116,
                    DefaultMapId = 1,
                    RespawnMapTypeId = -1
                };

                if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
                {
                    long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].RespawnMapTypeId;
                    if (respawnmaptype != null)
                    {
                        RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                        if (resp == null)
                        {
                            RespawnMapTypeDTO defaultresp = Session.CurrentMapInstance.Map.DefaultRespawn;
                            if (defaultresp != null)
                            {
                                respawn.DefaultX = defaultresp.DefaultX;
                                respawn.DefaultY = defaultresp.DefaultY;
                                respawn.DefaultMapId = defaultresp.DefaultMapId;
                                respawn.RespawnMapTypeId = (long) respawnmaptype;
                            }
                        }
                        else
                        {
                            respawn.DefaultX = resp.X;
                            respawn.DefaultY = resp.Y;
                            respawn.DefaultMapId = resp.MapId;
                            respawn.RespawnMapTypeId = (long) respawnmaptype;
                        }
                    }
                }
                else if (Session.HasCurrentMapInstance)
                {
                    RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == 0);
                    if (resp != null)
                    {
                        respawn.DefaultX = resp.X;
                        respawn.DefaultY = resp.Y;
                        respawn.DefaultMapId = resp.MapId;
                        respawn.RespawnMapTypeId = (long) 1;
                    }
                }

                return respawn;
            }
        }

        public List<RespawnDTO> Respawns { get; set; }

        public RespawnMapTypeDTO Return
        {
            get
            {
                RespawnMapTypeDTO respawn = new RespawnMapTypeDTO();
                if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
                {
                    long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].ReturnMapTypeId;
                    if (respawnmaptype != null)
                    {
                        RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                        if (resp == null)
                        {
                            RespawnMapTypeDTO defaultresp = Session.CurrentMapInstance.Map.DefaultReturn;
                            if (defaultresp != null)
                            {
                                respawn.DefaultX = defaultresp.DefaultX;
                                respawn.DefaultY = defaultresp.DefaultY;
                                respawn.DefaultMapId = defaultresp.DefaultMapId;
                                respawn.RespawnMapTypeId = (long) respawnmaptype;
                            }
                        }
                        else
                        {
                            respawn.DefaultX = resp.X;
                            respawn.DefaultY = resp.Y;
                            respawn.DefaultMapId = resp.MapId;
                            respawn.RespawnMapTypeId = (long) respawnmaptype;
                        }
                    }
                }
                else if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                {
                    RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == 1);
                    if (resp != null)
                    {
                        respawn.DefaultX = resp.X;
                        respawn.DefaultY = resp.Y;
                        respawn.DefaultMapId = resp.MapId;
                        respawn.RespawnMapTypeId = 1;
                    }
                }

                return respawn;
            }
        }

        public ConcurrentBag<RuneEffectDTO> RuneEffectMain { get; set; }

        public MapCell SavedLocation { get; set; }

        public short SaveX { get; set; }

        public short SaveY { get; set; }

        public byte ScPage { get; set; }
        public long CurrentDie { get; set; }

        public long CurrentKill { get; set; }

        public long CurrentTc { get; set; }
        public IDisposable SealDisposable { get; set; }

        public int SecondWeaponCriticalChance { get; set; }

        public int SecondWeaponCriticalRate { get; set; }

        public int SecondWeaponHitRate { get; set; }

        public int SecondWeaponMaxHit { get; set; }

        public int SecondWeaponMinHit { get; set; }

        public ClientSession Session { get; private set; }

        public ConcurrentBag<ShellEffectDTO> ShellEffectArmor { get; set; }

        public ConcurrentBag<ShellEffectDTO> ShellEffectMain { get; set; }

        public ConcurrentBag<ShellEffectDTO> ShellEffectSecondary { get; set; }

        public int Size { get; set; } = 10;

        public byte SkillComboCount { get; set; }

        public ThreadSafeSortedList<int, CharacterSkill> Skills { get; private set; }

        public ThreadSafeSortedList<int, CharacterSkill> SkillsSp { get; set; }

        public int SnackAmount { get; set; }

        public int SnackHp { get; set; }

        public int SnackMp { get; set; }

        public int SpCooldown { get; set; }

        public byte Speed
        {
            get
            {
                if (_speed > 59)
                {
                    return 59;
                }

                return _speed;
            }

            set
            {
                LastSpeedChange = DateTime.Now;
                _speed = value > 59 ? (byte) 59 : value;
            }
        }

        public object SpeedLockObject { get; set; }

        public ItemInstance SpInstance => Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);

        public List<StaticBonusDTO> StaticBonusList { get; set; }

        public ScriptedInstance Timespace { get; set; }

        public bool TimespaceRewardGotten { get; set; }

        public int TimesUsed { get; set; }

        public List<CharacterTitleDTO> Title { get; set; }

        private List<CharacterVisitedMapDTO> _characterVisitedMaps { get; set; }

        public List<CharacterVisitedMapDTO> CharacterVisitedMaps
        {
            get
            {
                if (_characterVisitedMaps == null) _characterVisitedMaps = DAOFactory.CharacterVisitedMapsDAO.LoadByCharacterId(CharacterId);

                return _characterVisitedMaps;
            }
            set { _characterVisitedMaps = value; }
        }

        public ThreadSafeGenericList<long> TradeRequests { get; set; }

        public bool TriggerAmbush { get; set; }

        public int UltimatePoints { get; set; }

        public bool Undercover { get; set; }

        public bool UseSp { get; set; }

        public Item VehicleItem { get; set; }

        public byte VehicleSpeed { private get; set; }

        #region OBan by NQ
        public void Ban(DateTime dateEnd, string reason)
        {
            CharacterDTO characterDTO = DAOFactory.CharacterDAO.LoadByName(Name);
            if (characterDTO != null)
            {
                ServerManager.Instance.Kick(Name);

                PenaltyLogDTO penaltyLogDTO = new PenaltyLogDTO
                {
                    AccountId = characterDTO.AccountId,
                    Reason = reason?.Trim(),
                    Penalty = PenaltyType.Banned,
                    DateStart = DateTime.Now,
                    DateEnd = dateEnd,
                    AdminName = "WorldServer"
                };

                InsertOrUpdatePenalty(penaltyLogDTO);
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
        }
        public DateTime LastMinimumGoldCheck { get; set; }

        #endregion

        public IDisposable WalkDisposable { get; set; }

        public int WareHouseSize
        {
            get
            {
                MinilandObject mp = MinilandObjects.Where(s => s.ItemInstance.Item.ItemType == ItemType.House && s.ItemInstance.Item.ItemSubType == 2).OrderByDescending(s => s.ItemInstance.Item.MinilandObjectPoint).FirstOrDefault();
                if (mp != null)
                {
                    return mp.ItemInstance.Item.MinilandObjectPoint;
                }

                return 0;
            }
        }

        public int RBBWin { get; internal set; }
        
        public int RBBLose { get; internal set; }

        public int WaterResistance { get; set; }

        #endregion

        public DateTime LastBugSkill = DateTime.Now;
        
        public DateTime LastSkillUseNew = DateTime.Now;
        
        #region Methods

        public string GenerateEventIcon()
        {
            ConfigurationObject c = ServerManager.Instance.Configuration;
            return $"evtb " +
                $"{ServerManager.Instance.Configuration.EventLvlUpEq} " +
                $"{ServerManager.Instance.Configuration.EventRareUpEq} " +
                $"{ServerManager.Instance.Configuration.EventSpUp} " +
                $"{ServerManager.Instance.Configuration.EventSpPerfection} " +
                $"{ServerManager.Instance.Configuration.EventXPF} " +
                $"{ServerManager.Instance.Configuration.EventSealed} " +
                $"{ServerManager.Instance.Configuration.EventXp} " +
                $"{ServerManager.Instance.Configuration.EventGold} " +
                $"{ServerManager.Instance.Configuration.EventRep} " +
                $"{ServerManager.Instance.Configuration.EventDrop} " +
                $"{ServerManager.Instance.Configuration.EventRuneUp} " +
                $"{ServerManager.Instance.Configuration.EventTattoUp}";
        }

        public static string GenerateAct() => "act 6";
        public string GenerateAscr(AscrPacketType e)
        {
            if (e == AscrPacketType.Close)
            {
                return "ascr 0 0 0 0 0 0 0 0 -1";
            }
            long killGroup = 0;
            long dieGroup = 0;
            var topArena = "0 0 0";
            var packet = $"{CurrentKill} {CurrentDie} {CurrentTc} {ArenaKill} {ArenaDie} {ArenaTc}";
            if (e == AscrPacketType.Group)
            {
                if (Group == null)
                {
                    return $"ascr {packet} {killGroup} {dieGroup} {(long)e}";
                }

                if (Group.GroupType != GroupType.Group)
                {
                    return $"ascr {packet} {killGroup} {dieGroup} {(long)e}";
                }

                foreach (var character in Group.Sessions.GetAllItems())
                {
                    dieGroup += character.Character.ArenaDie;
                    killGroup += character.Character.ArenaKill;
                }
            }
            else if (e == AscrPacketType.Family)
            {
                if (Family == null)
                {
                    return $"ascr {packet} {killGroup} {dieGroup} {(long)e}";
                }

                foreach (var charac in Family.FamilyCharacters)
                {
                    dieGroup += charac.Character.ArenaDie;
                    killGroup += charac.Character.ArenaKill;
                }
            }

            return $"ascr {packet} {killGroup} {dieGroup} {(long)e}";
        }
        public string GenerateAscr() => $"ascr {ArenaKill} {ArenaDeath} 0 {CurrentArenaKill} {CurrentArenaDeath} 0 0 0 0 0";
        public static void BanMethod(ClientSession session)
        {
            if (session != null)
            {
                PenaltyLogDTO log = new PenaltyLogDTO
                {
                    AccountId = session.Account.AccountId,
                    Reason = "Bug Using",
                    Penalty = PenaltyType.Banned,
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now.AddYears(20),
                    AdminName = "BenSolo"
                };
                DAOFactory.PenaltyLogDAO.InsertOrUpdate(ref log);
                CommunicationServiceClient.Instance.RefreshPenalty(log.PenaltyLogId);
                session.Disconnect();
            }
        }
        public void GenerateAscrPacket()
        {
            if (Session.CurrentMapInstance.Map.MapId == 2006)
            {
                Session.SendPacket(GenerateAscr(Group == null ? AscrPacketType.Alone : AscrPacketType.Group));
                return;
            }
            if (Session.CurrentMapInstance.Map.MapId == 2106)
            {
                Session.SendPacket(GenerateAscr(AscrPacketType.Family));
                return;
            }
            Session.SendPacket(GenerateAscr(AscrPacketType.Close));
        }
        public static string GenerateRaidBf(byte type) => $"raidbf 0 {type} 25 ";

        public static void InsertOrUpdatePenalty(PenaltyLogDTO log)
        {
            DAOFactory.PenaltyLogDAO.InsertOrUpdate(ref log);
            CommunicationServiceClient.Instance.RefreshPenalty(log.PenaltyLogId);
        }

        public void AddBuff(Buff indicator, BattleEntity sender, bool noMessage = false, short x = 0, short y = 0) =>BattleEntity.AddBuff(indicator, sender, noMessage, x, y);

        public bool HasDoneQuestId(int questId)
        {
            return DAOFactory.QuestLogDAO.LoadByCharacterId(this.CharacterId).FirstOrDefault(x => x.QuestId == questId) != null;
        }

        public void EvolvePet()
        {
            Mate equipedMate = Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Pet);

            NpcMonster npcMonster = ServerManager.GetNpcMonster(equipedMate.NpcMonsterVNum);

            if (npcMonster.EvolvePet != 0)
            {
                equipedMate.RemoveTeamMember();
                Mates.Remove(equipedMate);
                MapInstance.Broadcast(equipedMate.GenerateOut());
                //equipedMate.Level += 1;
                Mate mate = new Mate(this, ServerManager.GetNpcMonster(npcMonster.EvolvePet), equipedMate.Level,MateType.Pet);
                mate.Experience += 1;
                mate.RefreshStats();
                AddPetWithSkill(mate);
                ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PET_EVOLVE"), 2));
                return;
            }
        }

        public bool AddPet(Mate mate)
        {
            if (CanAddMate(mate) || mate.IsTemporalMate)
            {
                Mates.Add(mate);
                MapInstance.Broadcast(mate.GenerateIn());
                if (!mate.IsTemporalMate)
                {
                    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("YOU_GET_PET"), mate.Name), 12));
                }

                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(GenerateScP());
                Session.SendPackets(GenerateScN());
                InventoryType newMateInventory = (InventoryType) (13 + mate.PetId);
                switch (mate.Monster.AttackClass)
                {
                    case 0:

                        // Partner Basic Weapon
                        mate.WeaponInstance = Inventory.AddNewToInventory(990, 1, newMateInventory).FirstOrDefault();

                        // Partner Basic Armor
                        mate.ArmorInstance = Inventory.AddNewToInventory(997, 1, newMateInventory).FirstOrDefault();
                        break;

                    case 1:

                        // Partner Basic Weapon
                        mate.WeaponInstance = Inventory.AddNewToInventory(991, 1, newMateInventory).FirstOrDefault();

                        // Partner Basic Armor
                        mate.ArmorInstance = Inventory.AddNewToInventory(996, 1, newMateInventory).FirstOrDefault();
                        break;

                    case 2:

                        // Partner Basic Weapon
                        mate.WeaponInstance = Inventory.AddNewToInventory(992, 1, newMateInventory).FirstOrDefault();

                        // Partner Basic Armor
                        mate.ArmorInstance = Inventory.AddNewToInventory(995, 1, newMateInventory).FirstOrDefault();
                        break;
                }

                Session.SendPackets(GenerateScN());
                mate.RefreshStats();
                return true;
            }

            return false;
        }

        #region Get Bob, Tom, Kliff

        public void AddBob(ClientSession session)
        {
            Mate equipedMate = session.Character.Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);
            if (equipedMate != null)
            {
                equipedMate.RemoveTeamMember();
                session.Character.MapInstance.Broadcast(equipedMate.GenerateOut());
            }

            Mate mate = new Mate(session.Character, ServerManager.GetNpcMonster(317), 24, MateType.Partner);
            session.Character.Mates?.Add(mate);
            mate.RefreshStats();
            session.SendPacket($"ctl 2 {mate.PetId} 3");
            session.Character.MapInstance.Broadcast(mate.GenerateIn());
            session.SendPacket(UserInterfaceHelper.GeneratePClear());
            session.SendPackets(session.Character.GenerateScP());
            session.SendPackets(session.Character.GenerateScN());
            session.SendPacket(session.Character.GeneratePinit());
            session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember).OrderBy(s => s.MateType).Select(s => s.GeneratePst()));

            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOT_BOB"), 0));
        }

        public void AddTom(ClientSession session)
        {
            Mate equipedMate = session.Character.Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);
            if (equipedMate != null)
            {
                equipedMate.RemoveTeamMember();
                session.Character.MapInstance.Broadcast(equipedMate.GenerateOut());
            }

            Mate mate = new Mate(session.Character, ServerManager.GetNpcMonster(318), 31, MateType.Partner);
            session.Character.Mates?.Add(mate);
            mate.RefreshStats();
            session.SendPacket($"ctl 2 {mate.PetId} 3");
            session.Character.MapInstance.Broadcast(mate.GenerateIn());
            session.SendPacket(UserInterfaceHelper.GeneratePClear());
            session.SendPackets(session.Character.GenerateScP());
            session.SendPackets(session.Character.GenerateScN());
            session.SendPacket(session.Character.GeneratePinit());
            session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember).OrderBy(s => s.MateType).Select(s => s.GeneratePst()));

            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOT_TOM"), 0));
        }

        public void AddKliff(ClientSession session)
        {
            Mate equipedMate = session.Character.Mates?.SingleOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);
            if (equipedMate != null)
            {
                equipedMate.RemoveTeamMember();
                session.Character.MapInstance.Broadcast(equipedMate.GenerateOut());
            }

            Mate mate = new Mate(session.Character, ServerManager.GetNpcMonster(319), 48, MateType.Partner);
            session.Character.Mates?.Add(mate);
            mate.RefreshStats();
            session.SendPacket($"ctl 2 {mate.PetId} 3");
            session.Character.MapInstance.Broadcast(mate.GenerateIn());
            session.SendPacket(UserInterfaceHelper.GeneratePClear());
            session.SendPackets(session.Character.GenerateScP());
            session.SendPackets(session.Character.GenerateScN());
            session.SendPacket(session.Character.GeneratePinit());
            session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember).OrderBy(s => s.MateType).Select(s => s.GeneratePst()));

            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOT_KLIFF"), 0));
        }

        #endregion


        public void AddNewVisitedMap(int mapId, int mapX, int mapY)
        {
            var characterVisitedMap = new CharacterVisitedMapDTO
            {
                CharacterId = this.CharacterId,
                MapId = mapId,
                MapX = mapX,
                MapY = mapY
            };

            CharacterVisitedMaps.Add(characterVisitedMap);
        }

        public void AddPetWithSkill(Mate mate)
        {
            bool isUsingMate = true;
            if (!Mates.ToList().Any(s => s.IsTeamMember && s.MateType == mate.MateType))
            {
                isUsingMate = false;
                mate.IsTeamMember = true;
            }
            else
            {
                mate.BackToMiniland();
            }

            Session.SendPacket($"ctl 2 {mate.MateTransportId} 3");
            Mates.Add(mate);
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());
            Session.SendPackets(GenerateScP());
            Session.SendPackets(GenerateScN());
            if (!isUsingMate)
            {
                foreach (var sess in Session.CurrentMapInstance.Sessions.Where(s => s.Character != null))
                {
                    if (ServerManager.Instance.ChannelId != 51 || Session.Character.Faction == sess.Character.Faction)
                    {
                        sess.SendPacket(mate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                    }
                    else
                    {
                        sess.SendPacket(mate.GenerateIn(true, ServerManager.Instance.ChannelId == 51,sess.Account.Authority));
                    }
                }

                Session.SendPacket(GeneratePinit());
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(GenerateScP());
                Session.SendPackets(GenerateScN());
                Session.SendPackets(GeneratePst());
            }
        }

        public void AddQuest(long questId, bool isMain = false)
        {
            if (Session.Character.Authority == AuthorityType.Administrator)
            {
                Session.SendPacket(Session.Character.GenerateSay($"[HELPER] QuestId: {questId}", 10));
            }

            var characterQuest = new CharacterQuest(questId, CharacterId);
            if (Quests.Any(q => q.QuestId == questId) || characterQuest.Quest == null || (isMain && Quests.Any(q => q.IsMainQuest)) || (Quests.Where(q => q.Quest.QuestType != (byte) QuestType.WinRaid).ToList().Count >= 5 && characterQuest.Quest.QuestType != (byte) QuestType.WinRaid && !isMain) || ((QuestType) characterQuest.Quest.QuestType == QuestType.FlowerQuest && Quests.Any(s => (QuestType) s.Quest.QuestType == QuestType.FlowerQuest)))
            {
                return;
            }

            //if (characterQuest.Quest.LevelMin > Level)
            //{
            //    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
            //    return;
            //}

            //if (ServerManager.Instance.Configuration.MaxLevel == 99 && characterQuest.Quest.LevelMax < Level)
            //{
            //    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_HIGH_LVL"), 0));
            //    return;
            //}

            #region GlacernonQuest

            bool GlaceQuest = false;

            if (questId >= 7525 && questId <= 7526) // Kill Character (quest a4)
            {
                GlaceQuest = true;
            }

            if (GlaceQuest)
            {
                if (Session.Character.Faction == FactionType.Angel && questId == 7526)
                {
                    return;
                }

                if (Session.Character.Faction == FactionType.Demon && questId == 7525)
                {
                    return;
                }

                if (!characterQuest.Quest.IsDaily && !characterQuest.IsMainQuest)
                {
                    if (DAOFactory.QuestLogDAO.LoadByCharacterId(CharacterId).Any(s => s.QuestId == questId))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE"),0));
                        return;
                    }
                }
                else if (characterQuest.Quest.IsDaily)
                {
                    if (DAOFactory.QuestLogDAO.LoadByCharacterId(CharacterId).Any(s =>s.QuestId == questId && s.LastDaily != null && s.LastDaily.Value.Date == DateTime.Today))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE_TODAY"), 0));
                        return;
                    }
                }
            }

            #endregion GlacernonQuest

            #region SPQuest

            bool isSpQuest = false;

            if ((questId >= 2000 && questId <= 2007) // Pajama
                || (questId >= 2008 && questId <= 2012) // SP 1 2013
                || (questId >= 2014 && questId <= 2020) // SP 2
                || (questId >= 2060 && questId <= 2095) // SP 3
                || (questId >= 2113 && questId <= 2134) // SP 4 2100
            )
            {
                isSpQuest = true;
            }

            #endregion SPQuest

            if (!isSpQuest)
            {
                if (!characterQuest.Quest.IsDaily && !characterQuest.IsMainQuest && (QuestType) characterQuest.Quest.QuestType != QuestType.FlowerQuest)
                {
                    if (DAOFactory.QuestLogDAO.LoadByCharacterId(CharacterId).Any(s => s.QuestId == questId))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE"),0));
                        return;
                    }
                }
                else if (characterQuest.Quest.IsDaily && (QuestType) characterQuest.Quest.QuestType != QuestType.FlowerQuest)
                {
                    if (DAOFactory.QuestLogDAO.LoadByCharacterId(CharacterId).Any(s =>s.QuestId == questId && s.LastDaily != null && s.LastDaily.Value.AddHours(15) >= DateTime.Now))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE_TODAY"), 0));
                        return;
                    }
                }
                else if (characterQuest.Quest.CanBeDoneOnlyOnce == true) //1-time quests
                {
                    if (DAOFactory.QuestLogDAO.LoadByCharacterId(CharacterId).Any(s =>s.QuestId == questId && s.LastDaily != null && s.LastDaily.Value.AddYears(2) >= DateTime.Now))
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ONE_TIME_QUEST_COMPLETED"), 0));
                        return;
                    }
                }
            }

            if (characterQuest.Quest.QuestType == (int) QuestType.TimesSpace && ServerManager.Instance.TimeSpaces.All(si => si.QuestTimeSpaceId !=(characterQuest.Quest.QuestObjectives.FirstOrDefault()?.SpecialData ?? -1)) || characterQuest.Quest.QuestType == (int) QuestType.Product || characterQuest.Quest.QuestType == (int) QuestType.Collect3 || characterQuest.Quest.QuestType == (int) QuestType.TransmitGold || characterQuest.Quest.QuestType == (int) QuestType.TsPoint || characterQuest.Quest.QuestType == (int) QuestType.NumberOfKill || characterQuest.Quest.QuestType == (int) QuestType.TargetReput || characterQuest.Quest.QuestType == (int) QuestType.Inspect || characterQuest.Quest.QuestType == (int) QuestType.Needed || characterQuest.Quest.QuestType == (int) QuestType.Collect5 || QuestHelper.Instance.SkipQuests.Any(q => q == characterQuest.QuestId))
            {
                Session.SendPacket(characterQuest.Quest.GetRewardPacket(this, true));
                AddQuest(characterQuest.Quest.NextQuestId ?? -1, isMain);
                return;
            }

            // Act 2 quest scene
            if (characterQuest.QuestId == 3000)
            {
                Session.SendPacket("scene 41");
            }

            // Act 3 quest scene
            if (characterQuest.QuestId == 3201)
            {
                Session.SendPacket("scene 42");
            }

            // Act 4 quest scene
            if (characterQuest.QuestId == 3341)
            {
                Session.SendPacket("scene 43");
            }

            // Act 5 quest scene
            if (characterQuest.QuestId == 5479)
            {
                Session.SendPacket("scene 44");
            }

            if (characterQuest.Quest.TargetMap != null)
            {
                Session.SendPacket(characterQuest.Quest.TargetPacket());
            }

            characterQuest.IsMainQuest = isMain;
            Quests.Add(characterQuest);
            Session.SendPacket(GenerateQuestsPacket(questId));
            if (characterQuest.Quest.QuestType == (int) QuestType.UnKnow)
            {
                Session.Character.IncrementObjective(characterQuest, isOver: true);
            }

            Session.SendPacket(GetSqst());
        }

        public void AddRelation(long characterId, CharacterRelationType Relation)
        {
            if (characterId == CharacterId)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("CANT_RELATION_YOURSELF"), 11));
            }

            CharacterRelationDTO addRelation = new CharacterRelationDTO
            {
                CharacterId = CharacterId,
                RelatedCharacterId = characterId,
                RelationType = Relation
            };

            DAOFactory.CharacterRelationDAO.InsertOrUpdate(ref addRelation);
            ServerManager.Instance.RelationRefresh(addRelation.CharacterRelationId);
            Session.SendPacket(GenerateFinit());
            ClientSession target = ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.CharacterId == characterId);
            target?.SendPacket(target?.Character.GenerateFinit());
        }

        public bool AddSkill(short skillVNum)
        {
            Skill skillinfo = ServerManager.GetSkill(skillVNum);
            if (skillinfo == null)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("SKILL_DOES_NOT_EXIST"), 11));
                return false;
            }

            if (skillinfo.SkillVNum < 200)
            {
                if (Skills.GetAllItems().Any(s => skillinfo.CastId == s.Skill.CastId && s.Skill.SkillVNum < 200 && s.Skill.UpgradeSkill > skillinfo.UpgradeSkill))
                {
                    // Character already has a better passive skill of the same type.
                    return false;
                }

                foreach (CharacterSkill skill in Skills.GetAllItems().Where(s => skillinfo.CastId == s.Skill.CastId && s.Skill.SkillVNum < 200))
                {
                    Skills.Remove(skill.SkillVNum);
                }
            }
            else
            {
                if (Skills.ContainsKey(skillVNum))
                {
                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("SKILL_ALREADY_EXIST"), 11));
                    return false;
                }

                if (skillinfo.UpgradeSkill != 0)
                {
                    CharacterSkill oldupgrade = Skills.FirstOrDefault(s =>s.Skill.UpgradeSkill == skillinfo.UpgradeSkill && s.Skill.UpgradeType == skillinfo.UpgradeType && s.Skill.UpgradeSkill != 0);
                    if (oldupgrade != null)
                    {
                        Skills.Remove(oldupgrade.SkillVNum);
                    }
                }
            }

            Skills[skillVNum] = new CharacterSkill
            {
                SkillVNum = skillVNum,
                CharacterId = CharacterId
            };

            Session.SendPackets(GenerateQuicklist());
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"),0));
            return true;
        }

        public void AddStaticBuff(StaticBuffDTO staticBuff, bool isPermaBuff = false)
        {
            Buff bf = new Buff(staticBuff.CardId, Level, isPermaBuff)
            {
                Start = DateTime.Now,
                StaticBuff = true
            };

            if (bf.Card == null)
            {
                return;
            }
            
            Buff oldbuff = Buff[staticBuff.CardId];
            if (oldbuff != null)
            {
                oldbuff.Card.BCards.Where(s => BattleEntity.BCardDisposables[s.BCardId] != null).ToList().ForEach(b => BattleEntity.BCardDisposables[b.BCardId].Dispose());
                oldbuff.StaticVisualEffect?.Dispose();
            }

            if (staticBuff.RemainingTime <= 0)
            {
                bf.RemainingTime = (int) (bf.Card.Duration * 0.6);
                Buff[bf.Card.CardId] = bf;
            }
            else if (staticBuff.RemainingTime > 0)
            {
                bf.RemainingTime = staticBuff.CardId == 340 ? staticBuff.RemainingTime + (oldbuff == null ? 0 : (int) (oldbuff.RemainingTime - (DateTime.Now - oldbuff.Start).TotalSeconds)) : staticBuff.RemainingTime;
                Buff[bf.Card.CardId] = bf;
            }
            else if (oldbuff != null)
            {
                Buff.Remove(bf.Card.CardId);
                int time = (int) ((oldbuff.Start.AddSeconds(oldbuff.Card.Duration * 6 / 10) - DateTime.Now).TotalSeconds / 10 * 6);
                bf.RemainingTime = (bf.Card.Duration * 6 / 10) + (time > 0 ? time : 0);
                Buff[bf.Card.CardId] = bf;
            }
            else
            {
                bf.RemainingTime = bf.Card.Duration * 6 / 10;
                Buff[bf.Card.CardId] = bf;
            }

            bf.Card.BCards.ForEach(c => c.ApplyBCards(BattleEntity, BattleEntity));
            if (BuffObservables.ContainsKey(bf.Card.CardId))
            {
                BuffObservables[bf.Card.CardId].Dispose();
                BuffObservables.Remove(bf.Card.CardId);
            }

            if (bf.RemainingTime > 0)
            {
                BuffObservables[bf.Card.CardId] = Observable.Timer(TimeSpan.FromSeconds(bf.RemainingTime)).Subscribe(o =>
                    {
                        RemoveBuff(bf.Card.CardId);
                        if (bf.Card.TimeoutBuff != 0 && ServerManager.RandomNumber() < bf.Card.TimeoutBuffChance)
                        {
                            AddBuff(new Buff(bf.Card.TimeoutBuff, Level), BattleEntity);
                        }
                    });
            }

            if (!_isStaticBuffListInitial)
            {
                _isStaticBuffListInitial = true;
            }

            Session.SendPacket($"vb {bf.Card.CardId} 1 {(bf.RemainingTime <= 0 ? -1 : bf.RemainingTime * 10)}");
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("UNDER_EFFECT"), bf.Card.Name), 12));

            // Visual Effects (eff packet)
            if (bf.Card.CardId == 319)
            {
                bf.StaticVisualEffect = Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(o =>
                {
                    if (!Invisible)
                    {
                        Session?.CurrentMapInstance?.Broadcast(GenerateEff(881));
                    }
                });
            }

            if (bf.Card.CardId == 4035) // Custom Buff
            {
                bf.StaticVisualEffect = Observable.Interval(TimeSpan.FromSeconds(4)).Subscribe(o =>
                {
                    if (!Invisible)
                    {
                        Session?.CurrentMapInstance?.Broadcast(GenerateEff(3039));
                    }
                });
            }

            if (bf.Card.CardId == 4036) // Last Breaths
            {
                bf.StaticVisualEffect = Observable.Interval(TimeSpan.FromSeconds(3)).Subscribe(o =>
                {
                    if (!Invisible)
                    {
                        Session?.CurrentMapInstance?.Broadcast(GenerateEff(6007));
                    }
                });
            }

            if (bf.Card.CardId == 244)
            {
                bf.StaticVisualEffect = Observable.Interval(TimeSpan.FromSeconds(3)).Subscribe(o =>
                {
                    if (!Invisible)
                    {
                        Session?.CurrentMapInstance?.Broadcast(GenerateEff(883));
                    }
                });
            }
        }

        public void AddUltimatePoints(short points)
        {
            UltimatePoints += points;

            if (UltimatePoints > 3000)
            {
                UltimatePoints = 3000;
            }

            Session.SendPacket(GenerateFtPtPacket());
            Session.SendPackets(GenerateQuicklist());
        }

        public void AddWolfBuffs()
        {
            if (UltimatePoints >= 1000 && !Buff.Any(s => s.Card.CardId == 727 || s.Card.CardId == 728 || s.Card.CardId == 729))
            {
                AddBuff(new Buff(727, 10, false), BattleEntity);
                RemoveBuff(728);
                RemoveBuff(729);
            }

            if (UltimatePoints >= 2000 && !Buff.Any(s => s.Card.CardId == 728 || s.Card.CardId == 729))
            {
                AddBuff(new Buff(728, 10, false), BattleEntity);
                RemoveBuff(727);
                RemoveBuff(729);
            }

            if (UltimatePoints >= 3000 && !Buff.Any(s => s.Card.CardId == 729))
            {
                AddBuff(new Buff(729, 10, false), BattleEntity);
                RemoveBuff(727);
                RemoveBuff(728);
            }
        }

        public bool CanAddMate(Mate mate) => mate.MateType == MateType.Pet   ? MaxMateCount > Mates.Count(s => s.MateType == MateType.Pet) : MaxPartnerCount > Mates.Count(s => s.MateType == MateType.Partner);

        public bool CanAttack() => !NoAttack && !HasBuff(CardType.SpecialAttack, (byte) AdditionalTypes.SpecialAttack.NoAttack) &&  !HasBuff(CardType.FrozenDebuff, (byte) AdditionalTypes.FrozenDebuff.EternalIce);

        public bool CanMove() => !NoMove && !HasBuff(CardType.Move, (byte) AdditionalTypes.Move.MovementImpossible) &&!HasBuff(CardType.FrozenDebuff, (byte) AdditionalTypes.FrozenDebuff.EternalIce);

        public bool CanUseNosBazaar()
        {
            if (MapInstance == null)
            {
                return false;
            }

            StaticBonusDTO medal = Session.Character.StaticBonusList.Find(s =>  s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);
            if (medal == null)
            {
                // Check if there is NosBazaar in Map
                if (!Session.Character.MapInstance.Npcs.Any(s => s.Dialog == 460) && !Session.Character.MapInstance.Npcs.Any(s => s.Dialog == 11030))
                {
                    return false;
                }
            }

            return true;
        }

        public void ChangeChannel(string ip, int port, byte mode, bool saveCharacter = true)
        {
            Session.SendPacket($"mz {ip} {port} {Slot}");
            Session.SendPacket($"it {mode}");
            Session.IsDisposing = true;
            CommunicationServiceClient.Instance.RegisterCrossServerAccountLogin(Session.Account.AccountId, Session.SessionId);

            try
            {
                Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            Session.Disconnect();
        }

        public void ChangeClass(ClassType characterClass, bool fromCommand)
        {
            if (!fromCommand)
            {
                JobLevel = 1;
                JobLevelXp = 0;
            }

            Session.SendPacket("npinfo 0");
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());

            if (characterClass == (byte) ClassType.Adventurer)
            {
                HairStyle = (byte) HairStyle > 1 ? 0 : HairStyle;
                if (JobLevel > 20)
                {
                    JobLevel = 20;
                }
            }

            LoadSpeed();
            Class = characterClass;
            Hp = (int) HPLoad();
            Mp = (int) MPLoad();
            Session.SendPacket(GenerateTit());
            Session.SendPacket(GenerateStat());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateEq());
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8),PositionX, PositionY);
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CLASS_CHANGED"),0));
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 196),PositionX, PositionY);

            ChangeFaction(Session.Character.Family == null ? (FactionType) ServerManager.RandomNumber(1, 2) : (FactionType) Session.Character.Family.FamilyFaction);

            Session.SendPacket(GenerateCond());
            Session.SendPacket(GenerateLev());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateCMode());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 6),PositionX, PositionY);
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 198),PositionX, PositionY);
            Session.Character.ResetSkills();

            foreach (QuicklistEntryDTO quicklists in DAOFactory.QuicklistEntryDAO.LoadByCharacterId(CharacterId).Where(quicklists => QuicklistEntries.Any(qle => qle.Id == quicklists.Id)))
            {
                DAOFactory.QuicklistEntryDAO.Delete(quicklists.Id);
            }

            QuicklistEntries = new List<QuicklistEntryDTO>
            {
                new QuicklistEntryDTO
                {
                    CharacterId = CharacterId,
                    Q1 = 0,
                    Q2 = 9,
                    Type = 1,
                    Slot = 3,
                    Pos = 1
                }
            };

            Session.SendPackets(GenerateQuicklist());

            if (ServerManager.Instance.Groups.Any(s => s.IsMemberOfGroup(Session) && s.GroupType == GroupType.Group))
            {
                Session.CurrentMapInstance?.Broadcast(Session, $"pidx 1 1.{CharacterId}", ReceiverType.AllExceptMe);
            }
        }

        public void ChangeFaction(FactionType faction)
        {
            if (Faction == faction)
            {
                return;
            }

            if (Channel.ChannelId == 51)
            {
                string connection = CommunicationServiceClient.Instance.RetrieveOriginWorld(AccountId);
                if (string.IsNullOrWhiteSpace(connection))
                {
                    return;
                }

                MapId = 145;
                MapX = 51;
                MapY = 41;
                int port = Convert.ToInt32(connection.Split(':')[1]);
                ChangeChannel(connection.Split(':')[0], port, 3);
            }

            Faction = faction;
            Act4Kill = 0;
            Act4Dead = 0;
            Act4Points = 0;
            Session.SendPacket("scr 0 0 0 0 0 0");
            Session.SendPacket(GenerateFaction());
            Session.SendPackets(GenerateStatChar());

            if (faction != FactionType.None)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{(int) Faction}"), 0));
                var effectId = 4799 + (int) faction;
                if (Family != null)
                {
                    effectId += 2;
                }

                Session.SendPacket(GenerateEff(effectId));
            }

            Session.SendPacket(GenerateCond());
            Session.SendPacket(GenerateLev());
        }

        public void ChangeSex()
        {
            Gender = Gender == GenderType.Female ? GenderType.Male : GenderType.Female;
            if (IsVehicled)
            {
                Morph = Gender == GenderType.Female ? Morph + 1 : Morph - 1;
            }

            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SEX_CHANGED"), 0));
            Session.SendPacket(GenerateEq());
            Session.SendPacket(GenerateGender());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
            Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 196),PositionX, PositionY);
        }

        public void CharacterLife()
        {
            if (Hp == 0 && LastHealth.AddSeconds(2) <= DateTime.Now)
            {
                Mp = 0;
                Session.SendPacket(GenerateStat());
                LastHealth = DateTime.Now;
            }
            else
            {
                #region Initial Buffs

                if (Level >= 1 && Level < 26)
                {
                    if (!HasBuff(4035))
                    {
                        Session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 4035 }, false);
                        //AddBuff(new Buff(3035, Level), BattleEntity);
                    }
                }
                else
                {
                    RemoveBuff(4035);
                }
                #endregion
                #region Battle Tower Level Up

                switch (BattleTowerExp)
                {
                    case 20:
                        if (Session.Character.BattleTowerStage == 0)
                        {
                            Session.Character.BattleTowerStage += 1;
                            Session.SendPacket("msg 4 You are now at Battle Tower Stage 1!");
                        }
                        break;

                    case 100:
                        if (Session.Character.BattleTowerStage == 0)
                        {
                            Session.Character.BattleTowerStage += 1;
                            Session.SendPacket("msg 4 You are now at Battle Tower Stage 2!");
                        }
                        break;

                    case 200:
                        if (Session.Character.BattleTowerStage == 0)
                        {
                            Session.Character.BattleTowerStage += 1;
                            Session.SendPacket("msg 4 You are now at Battle Tower Stage 3!");
                        }
                        break;

                    case 350:
                        if (Session.Character.BattleTowerStage == 0)
                        {
                            Session.Character.BattleTowerStage += 1;
                            Session.SendPacket("msg 4 You are now at Battle Tower Stage 4!");
                        }
                        break;

                    case 500:
                        if (Session.Character.BattleTowerStage == 0)
                        {
                            Session.Character.BattleTowerStage += 1;
                            Session.SendPacket("msg 4 You are now at Battle Tower Stage 5!");
                        }
                        break;

#warning TODO: Add Rewards
                }
                #endregion
                #region Family Buffs

                if (Session.Character.Family != null)
                {
                    if (FamilyBuff.AddSeconds(60) <= DateTime.Now)
                    {
                        FamilyBuff = DateTime.Now;

                        switch (Session.Character.Family.FamilyLevel)
                        {
                            case 2:
                            case 3:
                                Session.Character.AddBuff(new Buff(4051, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 4:
                            case 5:
                                Session.Character.AddBuff(new Buff(4052, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 6:
                            case 7:
                                Session.Character.AddBuff(new Buff(4053, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 8:
                            case 9:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 10:
                            case 11:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 12:
                            case 13:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 14:
                            case 15:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 16:
                            case 17:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 18:
                            case 19:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;

                            case 20:
                                Session.Character.AddBuff(new Buff(729, Session.Character.Level), Session.Character.BattleEntity);
                                break;
                        }
                    }
                }
                #endregion

                #region AntiHacks-Gold
                if (LastMinimumGoldCheck.AddSeconds(10) < DateTime.Now)
                {
                    LastMinimumGoldCheck = DateTime.Now;
                    if (Gold < 0 && Session.Account.Authority < AuthorityType.Administrator)
                    {
                        var oBanPacket = new NosTale.Packets.Packets.CommandPackets.BanPacket();
                        oBanPacket.CharacterName = Name;
                        oBanPacket.Duration = 5475; // Duration in Days
                        oBanPacket.Reason = "GoldExploit";
                        Ban(DateTime.Now.AddDays(oBanPacket.Duration), oBanPacket.Reason);

                        Session = null;
                        return;
                    }
                }
                #endregion

                if (BubbleMessage != null && BubbleMessageEnd <= DateTime.Now)
                {
                    BubbleMessage = null;
                }

                #region Server global family buffs
                if (ServerManager.Instance.Configuration.FamilyExpBuff && !HasBuff(360))
                {
                    AddStaticBuff(new StaticBuffDTO
                    {
                        CardId = 360,
                        CharacterId = CharacterId,
                        RemainingTime = (int)(ServerManager.Instance.Configuration.TimeExpBuff - DateTime.Now).TotalSeconds
                    });
                }

                if (ServerManager.Instance.Configuration.FamilyGoldBuff && !HasBuff(361))
                {
                    AddStaticBuff(new StaticBuffDTO
                    {
                        CardId = 361,
                        CharacterId = CharacterId,
                        RemainingTime = (int)(ServerManager.Instance.Configuration.TimeExpBuff - DateTime.Now).TotalSeconds
                    });
                }
                #endregion

                if (CurrentMinigame != 0 && LastEffect.AddSeconds(3) <= DateTime.Now)
                {
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, CurrentMinigame));
                    LastEffect = DateTime.Now;
                }

                if (LastEffect.AddMilliseconds(400) <= DateTime.Now && MessageCounter > 0)
                {
                    MessageCounter--;
                }

                if (MapInstance != null && HasBuff(CardType.FrozenDebuff, (byte) AdditionalTypes.FrozenDebuff.EternalIce) && LastFreeze.AddSeconds(1) <= DateTime.Now)
                {
                    LastFreeze = DateTime.Now;
                    MapInstance.Broadcast(GenerateEff(35));
                }

                if (MapInstance == Miniland && LastLoyalty.AddSeconds(10) <= DateTime.Now)
                {
                    LastLoyalty = DateTime.Now;
                    Mates.ForEach(m =>
                    {
                        m.Loyalty += 100;
                        if (m.Loyalty > 1000) m.Loyalty = 1000;
                    });
                    Session.SendPackets(GenerateScP());
                    Session.SendPackets(GenerateScN());
                }

                if (LastEffect.AddSeconds(5) <= DateTime.Now)
                {
                    if (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        Session.SendPacket(GenerateRaid(3));
                    }

                    ItemInstance ring = Inventory.LoadBySlotAndType((byte) EquipmentType.Ring, InventoryType.Wear);
                    ItemInstance bracelet = Inventory.LoadBySlotAndType((byte) EquipmentType.Bracelet, InventoryType.Wear);
                    ItemInstance necklace = Inventory.LoadBySlotAndType((byte) EquipmentType.Necklace, InventoryType.Wear);
                    CellonOptions.Clear();
                    if (ring != null)
                    {
                        CellonOptions.AddRange(ring.CellonOptions);
                    }

                    if (bracelet != null)
                    {
                        CellonOptions.AddRange(bracelet.CellonOptions);
                    }

                    if (necklace != null)
                    {
                        CellonOptions.AddRange(necklace.CellonOptions);
                    }

                    if (!Invisible)
                    {
                        ItemInstance amulet = Inventory.LoadBySlotAndType((byte) EquipmentType.Amulet, InventoryType.Wear);
                        if (amulet != null)
                        {
                            if (amulet.ItemVNum == 4503 || amulet.ItemVNum == 4504)
                            {
                                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, amulet.Item.EffectValue + (Class == ClassType.Adventurer ? 0 : (byte) Class - 1)), PositionX, PositionY);
                            }
                            else
                            {
                                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, amulet.Item.EffectValue), PositionX, PositionY);
                            }
                        }

                        if (Group != null && (Group.GroupType == GroupType.Team || Group.GroupType == GroupType.BigTeam || Group.GroupType == GroupType.GiantTeam))
                        {
                            try
                            {
                                Session.CurrentMapInstance?.Broadcast(Session,StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 828 + (Group.IsLeader(Session) ? 1 : 0)), ReceiverType.AllExceptGroup);
                                Session.CurrentMapInstance?.Broadcast(Session,StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 830 + (Group.IsLeader(Session) ? 1 : 0)), ReceiverType.Group);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        }

                        Mates.Where(s => s.CanPickUp).ToList().ForEach(s =>Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, s.MateTransportId, 3007)));
                        Mates.Where(s => s.IsTsProtected).ToList().ForEach(s =>Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, s.MateTransportId, 825)));
                        Mates.Where(s => s.MateType == MateType.Pet && s.Loyalty <= 0).ToList().ForEach(s =>Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Npc, s.MateTransportId, 5003)));
                    }

                    LastEffect = DateTime.Now;
                }

                foreach (Mate mate in Mates.Where(m => m.IsTeamMember))
                {
                    if (mate.LastHealth.AddSeconds(mate.IsSitting ? 1.5 : 2) <= DateTime.Now)
                    {
                        mate.LastHealth = DateTime.Now;
                        if (mate.LastDefence.AddSeconds(4) <= DateTime.Now &&
                            mate.LastSkillUse.AddSeconds(2) <= DateTime.Now && mate.Hp > 0)
                        {
                            mate.Hp += mate.Hp + mate.HealthHpLoad() < mate.HpLoad()? mate.HealthHpLoad() : mate.HpLoad() - mate.Hp;
                            mate.Mp += mate.Mp + mate.HealthMpLoad() < mate.MpLoad()? mate.HealthMpLoad() : mate.MpLoad() - mate.Mp;
                        }

                        Session.SendPackets(GeneratePst());
                    }
                }

                if (LastHealth.AddSeconds(2) <= DateTime.Now || (IsSitting && LastHealth.AddSeconds(1.5) <= DateTime.Now))
                {
                    LastHealth = DateTime.Now;

                    if (Session.HealthStop)
                    {
                        Session.HealthStop = false;
                        return;
                    }

                    if (LastDefence.AddSeconds(4) <= DateTime.Now && LastSkillUse.AddSeconds(2) <= DateTime.Now && Hp > 0)
                    {
                        bool change = false;

                        if (Hp + HealthHPLoad() < HPLoad())
                        {
                            change = true;
                            Hp += HealthHPLoad();
                        }
                        else
                        {
                            change |= Hp != (int) HPLoad();
                            Hp = (int) HPLoad();
                        }

                        if (Mp + HealthMPLoad() < MPLoad())
                        {
                            Mp += HealthMPLoad();
                            change = true;
                        }
                        else
                        {
                            change |= Mp != (int) MPLoad();
                            Mp = (int) MPLoad();
                        }

                        if (change)
                        {
                            Session.SendPacket(GenerateStat());
                        }
                    }
                }

                if (Session.Character.LastQuestSummon.AddSeconds(7) < DateTime.Now) // Quest in which you make monster spawn
                {
                    Session.Character.CheckHuntQuest();
                    Session.Character.LastQuestSummon = DateTime.Now;
                }

                if (MeditationDictionary.Count != 0)
                {
                    if (MeditationDictionary.ContainsKey(534) && MeditationDictionary[534] < DateTime.Now)
                    {
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 4344));
                        AddBuff(new Buff(534, Level), BattleEntity);
                        if (BuffObservables.ContainsKey(533))
                        {
                            BuffObservables[533].Dispose();
                            BuffObservables.Remove(533);
                        }

                        MeditationDictionary.Remove(534);
                    }
                    else if (MeditationDictionary.ContainsKey(533) && MeditationDictionary[533] < DateTime.Now)
                    {
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 4343));
                        AddBuff(new Buff(533, Level), BattleEntity);
                        if (BuffObservables.ContainsKey(532))
                        {
                            BuffObservables[532].Dispose();
                            BuffObservables.Remove(532);
                        }

                        MeditationDictionary.Remove(533);
                    }
                    else if (MeditationDictionary.ContainsKey(532) && MeditationDictionary[532] < DateTime.Now)
                    {
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 4343));
                        AddBuff(new Buff(532, Level), BattleEntity);
                        if (BuffObservables.ContainsKey(534))
                        {
                            BuffObservables[534].Dispose();
                            BuffObservables.Remove(534);
                        }

                        MeditationDictionary.Remove(532);
                    }
                }

                if (HasMagicSpellCombo)
                {
                    Session.SendPacket($"mslot {LastComboCastId} 0");
                }
                else if (SkillComboCount > 0 && LastSkillComboUse.AddSeconds(5) < DateTime.Now)
                {
                    SkillComboCount = 0;
                    Session.SendPackets(GenerateQuicklist());
                    Session.SendPacket($"mslot {LastComboCastId} 0");
                }

                if (LastPermBuffRefresh.AddSeconds(2) <= DateTime.Now)
                {
                    LastPermBuffRefresh = DateTime.Now;

                    foreach (BCard bcard in EquipmentBCards.Where(b =>b.Type.Equals(CardType.Buff) && new Buff((short) b.CardId, Level).Card?.BuffType == BuffType.Good))
                    {
                        bcard.ApplyBCards(BattleEntity, BattleEntity);
                    }

                    this.GetBuffFromSet();
                }

                if (UseSp)
                {
                    ItemInstance specialist = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                    if (specialist == null)
                    {
                        return;
                    }

                    if (LastSpGaugeRemove <= new DateTime(0001, 01, 01, 00, 00, 00))
                    {
                        LastSpGaugeRemove = DateTime.Now;
                    }

                    if (LastSkillUse.AddSeconds(15) >= DateTime.Now && LastSpGaugeRemove.AddSeconds(1) <= DateTime.Now)
                    {
                        byte spType = 0;

                        if ((specialist.Item.Morph > 1 && specialist.Item.Morph < 8) || (specialist.Item.Morph > 9 && specialist.Item.Morph < 16))
                        {
                            spType = 3;
                        }
                        else if (specialist.Item.Morph > 16 && specialist.Item.Morph < 29)
                        {
                            spType = 2;
                        }
                        else if (specialist.Item.Morph == 9)
                        {
                            spType = 1;
                        }

                        if (SpPoint >= spType)
                        {
                            SpPoint -= spType;
                        }
                        else if (SpPoint < spType && SpPoint != 0)
                        {
                            spType -= (byte) SpPoint;
                            SpPoint = 0;
                            SpAdditionPoint -= spType;
                        }
                        else if (SpPoint == 0 && SpAdditionPoint >= spType)
                        {
                            SpAdditionPoint -= spType;
                        }
                        else if (SpPoint == 0 && SpAdditionPoint < spType)
                        {
                            SpAdditionPoint = 0;

                            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;

                            if (UseSp)
                            {
                                LastSp = currentRunningSeconds;
                                if (Session?.HasSession == true)
                                {
                                    if (IsVehicled)
                                    {
                                        return;
                                    }

                                    UseSp = false;
                                    CharacterHelper.RemoveSpecialistWingsBuff(Session);
                                    LoadSpeed();
                                    Session.SendPacket(GenerateCond());
                                    Session.SendPacket(GenerateLev());
                                    SpCooldown = 30;
                                    if (SkillsSp != null)
                                    {
                                        foreach (CharacterSkill ski in SkillsSp.Where(s => !s.CanBeUsed()))
                                        {
                                            short time = ski.Skill.Cooldown;
                                            double temp = (ski.LastUse - DateTime.Now).TotalMilliseconds + (time * 100);
                                            temp /= 1000;
                                            SpCooldown = temp > SpCooldown ? (int) temp : SpCooldown;
                                        }
                                    }

                                    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("STAY_TIME"), SpCooldown),11));
                                    Session.SendPacket($"sd {SpCooldown}");
                                    Session.CurrentMapInstance?.Broadcast(GenerateCMode());
                                    Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, CharacterId), PositionX, PositionY);

                                    // ms_c
                                    Session.SendPacket(GenerateSki());
                                    Session.SendPackets(GenerateQuicklist());
                                    Session.SendPacket(GenerateStat());
                                    Session.SendPackets(GenerateStatChar());

                                    Logger.LogUserEvent("CHARACTER_SPECIALIST_RETURN", Session.GenerateIdentity(),$"SpCooldown: {SpCooldown}");

                                    Observable.Timer(TimeSpan.FromMilliseconds(SpCooldown * 1000)).Subscribe(o =>
                                    {
                                        Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORM_DISAPPEAR"), 11));
                                        Session.SendPacket("sd 0");
                                    });
                                }
                            }
                        }

                        Session.SendPacket(GenerateSpPoint());
                        LastSpGaugeRemove = DateTime.Now;
                    }
                }
            }
        }

        public byte SwitchLevel()
        {
            return Level;
        }

        public void CheckHuntQuest()
        {
            CharacterQuest quest = Quests?.FirstOrDefault(q =>q?.Quest?.QuestType == (int) QuestType.Hunt && q.Quest?.TargetMap == MapInstance?.Map?.MapId && Math.Abs(PositionX - q.Quest?.TargetX ?? 0) < 2 && Math.Abs(PositionY - q.Quest?.TargetY ?? 0) < 2);
            if (quest == null)
            {
                return;
            }

            if (MapInstance == null || MapInstance.Monsters == null || MapInstance.Monsters.Where(a => a != null).Any(s => s?.MonsterVNum == (short) (quest?.GetObjectiveByIndex(1)?.Data ?? -1) && Math.Abs(s?.MapX - quest?.Quest?.TargetX ?? 0) < 4 && Math.Abs(s?.MapY - quest?.Quest?.TargetY ?? 0) < 4))
            {
                return;
            }

            ConcurrentBag<MonsterToSummon> monsters = new ConcurrentBag<MonsterToSummon>();
            var monstersToSpawn = quest.GetObjectiveByIndex(1)?.Objective / 2 + 1;

            if (monstersToSpawn > 4)
            {
                monstersToSpawn = 4;
            }

            for (var a = 0; a < monstersToSpawn; a++)
            {
                monsters.Add(new MonsterToSummon((short) (quest.GetObjectiveByIndex(1)?.Data ?? -1),
                    new MapCell
                    {
                        X = (short) (PositionX + ServerManager.RandomNumber<int>(-2, 3)),
                        Y = (short) (PositionY + ServerManager.RandomNumber<int>(-2, 3))
                    }, this.BattleEntity, true));
            }

            EventHelper.Instance.RunEvent(new EventContainer(MapInstance, EventActionType.SPAWNMONSTERS,monsters.ToList()));
        }

        public void ClearLaurena()
        {
            if (IsLaurenaMorph())
            {
                IsMorphed = false;
                Morph = PreviousMorph;
                PreviousMorph = 0;
                MapInstance?.Broadcast(GenerateCMode());
            }

            RemoveBuff(477, true);
            RemoveBuff(478, true);
        }

        public void CloseExchangeOrTrade()
        {
            if (InExchangeOrTrade)
            {
                long? targetSessionId = ExchangeInfo?.TargetCharacterId;

                if (targetSessionId.HasValue && Session.HasCurrentMapInstance)
                {
                    ClientSession targetSession = Session.CurrentMapInstance.GetSessionByCharacterId(targetSessionId.Value);

                    if (targetSession == null)
                    {
                        return;
                    }

                    Session.SendPacket("exc_close 0");
                    targetSession.SendPacket("exc_close 0");
                    ExchangeInfo = null;
                    targetSession.Character.ExchangeInfo = null;
                }
            }
        }

        public void CloseShop()
        {
            if (HasShopOpened && Session.HasCurrentMapInstance)
            {
                KeyValuePair<long, MapShop> shop = Session.CurrentMapInstance.UserShops.FirstOrDefault(mapshop => mapshop.Value.OwnerId.Equals(CharacterId));
                if (!shop.Equals(default))
                {
                    Session.CurrentMapInstance.UserShops.Remove(shop.Key);

                    // declare that the shop cannot be closed
                    HasShopOpened = false;

                    Session.CurrentMapInstance?.Broadcast(GenerateShopEnd());
                    Session.CurrentMapInstance?.Broadcast(Session, GeneratePlayerFlag(0), ReceiverType.AllExceptMe);
                    IsSitting = false;
                    IsShopping = false; // close shop by character will always completely close the shop

                    LoadSpeed();
                    Session.SendPacket(GenerateCond());
                    Session.CurrentMapInstance?.Broadcast(GenerateRest());
                }
            }
        }

        public bool CustomQuestRewards(QuestType type, long questId)
        {
            switch (type)
            {
                case QuestType.FlowerQuest:
                    GetDignity(100);
                    if (ServerManager.RandomNumber() < 50)
                    {
                        RemoveBuff(379);
                        AddBuff(new Buff(378, Level), BattleEntity);
                    }
                    else
                    {
                        RemoveBuff(378);
                        AddBuff(new Buff(379, Level), BattleEntity);
                    }

                    return true;
            }

            switch (questId)
            {
                case 2255:
                    short[] possibleRewards = new short[] {1894, 1895, 1896, 1897, 1898, 1899, 1900, 1901, 1902, 1903};
                    GiftAdd(possibleRewards[ServerManager.RandomNumber(0, possibleRewards.Length - 1)], 1);
                    return true;

                case 307: //Title #1
                {
                    if (Session.Character.Class == ClassType.Swordsman || Session.Character.Class == ClassType.MartialArtist)
                    {
                        GiftAdd(9316, 1);
                    }

                    if (Session.Character.Class == ClassType.Archer)
                    {
                        GiftAdd(9313, 1);
                    }

                    if (Session.Character.Class == ClassType.Magician)
                    {
                        GiftAdd(9310, 1);
                    }
                }
                    break;

                case 306: // TItle #2
                {
                    if (Session.Character.Class == ClassType.Swordsman || Session.Character.Class == ClassType.MartialArtist)
                    {
                        GiftAdd(9317, 1);
                    }

                    if (Session.Character.Class == ClassType.Archer)
                    {
                        GiftAdd(9314, 1);
                    }

                    if (Session.Character.Class == ClassType.Magician)
                    {
                        GiftAdd(9311, 1);
                    }
                }
                    break;

                case 305: //Title #3
                {
                    if (Session.Character.Class == ClassType.Swordsman || Session.Character.Class == ClassType.MartialArtist)
                    {
                        GiftAdd(9318, 1);
                    }

                    if (Session.Character.Class == ClassType.Archer)
                    {
                        GiftAdd(9315, 1);
                    }

                    if (Session.Character.Class == ClassType.Magician)
                    {
                        GiftAdd(9312, 1);
                    }
                }
                    break;
            }

            return false;
        }
        public RsfiPacket GenerateRsfi()
        {
            return new RsfiPacket
            {
                Act = 1,
                ActPart = 1,
                Unknown1 = 0,
                Unknown2 = 9,
                Ts = 0,
                TsMax = 9
            };
        }
        public void Dance() => IsDancing = !IsDancing;

        public void DecreaseMp(int amount) => BattleEntity.DecreaseMp(amount);

        public Character DeepCopy() => (Character) MemberwiseClone();

        public void DeleteBlackList(long characterId)
        {
            CharacterRelationDTO chara = CharacterRelations.Find(s => s.RelatedCharacterId == characterId);
            if (chara != null)
            {
                long id = chara.CharacterRelationId;
                DAOFactory.CharacterRelationDAO.Delete(id);
                ServerManager.Instance.RelationRefresh(id);
                Session.SendPacket(GenerateBlinit());
            }
        }

        public void DeleteItem(InventoryType type, short slot)
        {
            if (Inventory != null)
            {
                Inventory.DeleteFromSlotAndType(slot, type);
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(type, slot));
            }
        }

        public void DeleteItemByItemInstanceId(Guid id)
        {
            if (Inventory != null)
            {
                Tuple<short, InventoryType> result = Inventory.DeleteById(id);
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(result.Item2, result.Item1));
            }
        }

        public void DeleteRelation(long characterId, CharacterRelationType relationType)
        {
            CharacterRelationDTO chara = CharacterRelations.Find(s => (s.RelatedCharacterId == characterId || s.CharacterId == characterId) && s.RelationType == relationType);
            if (chara != null)
            {
                long id = chara.CharacterRelationId;
                CharacterDTO charac = DAOFactory.CharacterDAO.LoadById(characterId);
                DAOFactory.CharacterRelationDAO.Delete(id);
                ServerManager.Instance.RelationRefresh(id);

                Session.SendPacket(GenerateFinit());
                if (charac != null)
                {
                    List<CharacterRelationDTO> lst = ServerManager.Instance.CharacterRelations.Where(s => s.CharacterId == characterId || s.RelatedCharacterId == characterId).ToList();
                    string result = "finit";
                    foreach (CharacterRelationDTO relation in lst.Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
                    {
                        long id2 = relation.RelatedCharacterId == charac.CharacterId ? relation.CharacterId : relation.RelatedCharacterId;
                        bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id2);
                        result += $" {id2}|{(short) relation.RelationType}|{(isOnline ? 1 : 0)}|{DAOFactory.CharacterDAO.LoadById(id2).Name}";
                    }

                    int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                        {
                            DestinationCharacterId = charac.CharacterId,
                            SourceCharacterId = CharacterId,
                            SourceWorldId = ServerManager.Instance.WorldId,
                            Message = result,
                            Type = MessageType.PrivateChat
                        });
                }
            }
        }

        public void DeleteTimeout()
        {
            if (Inventory == null)
            {
                return;
            }

            foreach (ItemInstance item in Inventory.GetAllItems())
            {
                if ((item.IsBound || item.Item.ItemType == ItemType.Box) && item.ItemDeleteTime != null && item.ItemDeleteTime < DateTime.Now)
                {
                    Inventory.DeleteById(item.Id);

                    EquipmentBCards.RemoveAll(o => o.ItemVNum == item.ItemVNum);

                    if (item.Type == InventoryType.Wear)
                    {
                        Session.SendPacket(GenerateEquipment());
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(item.Type, item.Slot));
                    }

                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                }
            }
        }

        public void DisableBuffs(BuffType type, int level = 100) => BattleEntity.DisableBuffs(type, level);

        public void DisableBuffs(List<BuffType> types, int level = 100) => BattleEntity.DisableBuffs(types, level);

        public void DisposeMap()
        {
            CloseShop();
            CloseExchangeOrTrade();
            GroupSentRequestCharacterIds.Clear();
            FamilyInviteCharacters.Clear();
            FriendRequestCharacters.Clear();
            WalkDisposable?.Dispose();
            SealDisposable?.Dispose();
            MarryRequestCharacters?.Clear();
            BattleEntity?.ClearOwnFalcon();
            BattleEntity?.ClearEnemyFalcon();
            BattleEntity?.ClearSacrificeBuff();
            BattleEntity?.RemoveOwnedMonsters();
            BattleEntity?.RemoveOwnedNpcs();
            RemoveTemporalMates();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                if (OriginalFaction != -1)
                {
                    Faction = (FactionType) OriginalFaction;
                }

                DisposeShopAndExchange();
                GroupSentRequestCharacterIds?.Clear();
                FamilyInviteCharacters?.Clear();
                FriendRequestCharacters?.Clear();
                Life?.Dispose();
                WalkDisposable?.Dispose();
                SealDisposable?.Dispose();
                MarryRequestCharacters?.Clear();

                Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                {
                    Session.CurrentMapInstance?.Broadcast(Session, s.GenerateOut(), ReceiverType.AllExceptMe);
                    s.ReviveDisposable?.Dispose();
                });
                Session.CurrentMapInstance?.Broadcast(Session, StaticPacketHelper.Out(UserType.Player, CharacterId), ReceiverType.AllExceptMe);

                if (Hp < 1)
                {
                    Hp = 1;
                }

                if (ServerManager.Instance.Groups != null)
                {
                    if (ServerManager.Instance.Groups.Any(s => s.IsMemberOfGroup(CharacterId)))
                    {
                        ServerManager.Instance.GroupLeave(Session);
                    }
                }

                LeaveTalentArena(true);
                LeaveIceBreaker();
                BattleEntity?.DisableBuffs(BuffType.All);
                BattleEntity?.RemoveOwnedMonsters();
                BattleEntity?.RemoveOwnedNpcs();
                RemoveTemporalMates();

                BattleEntity?.ClearOwnFalcon();
                BattleEntity?.ClearEnemyFalcon();
                BattleEntity?.ClearSacrificeBuff();

                if (MapInstance != null)
                {
                    if (MapInstance.MapInstanceId == Family?.Act4RaidBossMap?.MapInstanceId || MapInstance.MapInstanceId == Family?.Act4Raid?.MapInstanceId)
                    {
                        short x = (short) (39 + ServerManager.RandomNumber(-2, 3));
                        short y = (short) (42 + ServerManager.RandomNumber(-2, 3));
                        if (Faction == FactionType.Angel)
                        {
                            MapId = 130;
                            MapX = x;
                            MapY = y;
                        }
                        else if (Faction == FactionType.Demon)
                        {
                            MapId = 131;
                            MapX = x;
                            MapY = y;
                        }
                    }

                    if (MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance || MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        MapInstance.InstanceBag.DeadList.Add(CharacterId);
                        if (MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                        {
                            Group?.Sessions.ForEach(s =>
                            {
                                if (s != null)
                                {
                                    s.SendPacket(s.Character.Group.GeneraterRaidmbf(s));
                                    s.SendPacket(s.Character.Group.GenerateRdlst());
                                }
                            });
                        }
                    }

                    if (Miniland != null)
                    {
                        ServerManager.RemoveMapInstance(Miniland.MapInstanceId);
                    }
                }
            }
        }

        public void DisposeShopAndExchange()
        {
            CloseShop();
            CloseExchangeOrTrade();
        }

        public void EnterInstance(ScriptedInstance input)
        {
            ScriptedInstance instance = input.Copy();
            instance.LoadScript(MapInstanceType.TimeSpaceInstance, this);
            if (instance.FirstMap == null)
            {
                return;
            }

            if (Session.Character.Level < instance.LevelMinimum)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"),0));
                return;
            }

            if (Session.Character.Level > instance.LevelMaximum)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_HIGH_LVL"),0));
                return;
            }

            var entries = instance.DailyEntries - Session.Character.GeneralLogs.CountLinq(s => s.LogType == "InstanceEntry" && short.Parse(s.LogData) == instance.Id && s.Timestamp.Date == DateTime.Today);
            if (instance.DailyEntries == 0 || entries > 0)
            {
                foreach (Gift requiredItem in instance.RequiredItems)
                {
                    if (Session.Character.Inventory.CountItem(requiredItem.VNum) < requiredItem.Amount)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("NO_ITEM_REQUIRED"),ServerManager.GetItem(requiredItem.VNum).Name), 0));
                        return;
                    }

                    Session.Character.Inventory.RemoveItemAmount(requiredItem.VNum, requiredItem.Amount);
                }

                //if (Session.Character.TimespaceRewardGotten == true)
                //{

                //}

                Session?.SendPackets(instance.GenerateMinimap());
                Session?.SendPacket(instance.GenerateMainInfo());
                Session?.SendPacket(instance.FirstMap.InstanceBag.GenerateScore());
                if (instance.StartX != 0 || instance.StartY != 0)
                {
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                        instance.FirstMap.MapInstanceId, instance.StartX, instance.StartY);
                }
                else
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, instance.FirstMap.MapInstanceId);
                }

                instance.InstanceBag.CreatorId = Session.Character.CharacterId;
                Session.Character.Timespace = instance;
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TS_NO_MORE_ENTRIES"), 0));
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TS_NO_MORE_ENTRIES"), 10));
            }
        }

        public string GenerateAct6()
        {
            return $"act6 1 0 {ServerManager.Instance.Act6Zenas.Percentage / 100} " +
                   $"{Convert.ToByte(ServerManager.Instance.Act6Zenas.Mode)} " +
                   $"{ServerManager.Instance.Act6Zenas.CurrentTime} " +
                   $"{ServerManager.Instance.Act6Zenas.TotalTime} " +
                   $"{ServerManager.Instance.Act6Erenia.Percentage / 100} " +
                   $"{Convert.ToByte(ServerManager.Instance.Act6Erenia.Mode)} " +
                   $"{ServerManager.Instance.Act6Erenia.CurrentTime} " +
                   $"{ServerManager.Instance.Act6Erenia.TotalTime}";
        }

        public string GenerateAdditionalHpMp()
        {
            return $"guri 4 {Math.Round(BattleEntity.AdditionalHp)} {Math.Round(BattleEntity.AdditionalMp)}";
        }

        public string GenerateAt() =>
            $"at {CharacterId} {MapInstance.Map.GridMapId} {PositionX} {PositionY} {Direction} 0 {MapInstance?.InstanceMusic ?? 0} 2 -1";

        public string GenerateBfePacket(short effect, short time) => $"bf_e 1 {CharacterId} {effect} {time}";

        public string GenerateBlinit()
        {
            string result = "blinit";

            foreach (CharacterRelationDTO relation in CharacterRelations.Where(s => s.CharacterId == CharacterId && s.RelationType == CharacterRelationType.Blocked))
            {
                result += $" {relation.RelatedCharacterId}|{DAOFactory.CharacterDAO.LoadById(relation.RelatedCharacterId)?.Name}";
            }

            return result;
        }

        public string GenerateBubbleMessagePacket()
        {
            return $"csp {CharacterId} {BubbleMessage}";
        }

        public string GenerateCharge() => $"bf 1 {CharacterId} {ChargeValue}.0.{ChargeValue} {Level}";

        public string GenerateCInfo()
        {
            var morph = (UseSp && !IsVehicled && SpInstance.HasSkin ? SpInstance.Item.VNum == 903 ? 102 : SpInstance.Item.VNum == 913 ? 101 : SpInstance.Item.VNum == 902 ? 100 : UseSp || IsVehicled || IsMorphed ? Morph : 0 : UseSp || IsVehicled || IsMorphed ? Morph : 0);

            var name = (Authority > AuthorityType.User && !Undercover ? Authority == AuthorityType.Supporter ? $"[{Authority}]" + Name : Name : Name);
            return
                $"c_info {name} - -1 {(Family != null && FamilyCharacter != null && !Undercover ?$"{Family.FamilyId}.{this.GetFamilyNameType()} {Family.Name}" : "-1 -")} " +
                $"{CharacterId} {(Invisible && Authority >= AuthorityType.MOD ? 6 : 0)} " +
                $"{(byte) Gender} {(byte) HairStyle} " +
                $"{(byte) HairColor} {(byte) Class} " +
                $"{(GetDignityIco() == 1 ? GetReputationIco() : -GetDignityIco())} {( Authority > AuthorityType.User && !Undercover ? CharacterHelper.AuthorityColor(Authority) : Compliment)} " +
                $"{(morph)} {(Invisible ? 1 : 0)} " +
                $"{Family?.FamilyLevel ?? 0} {(UseSp ? MorphUpgrade : 0)} " +
                $"{ArenaWinner} 0 0";
        }

        public string GenerateCMap() => $"c_map 0 {MapInstance.Map.MapId} {(MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance ? 1 : 0)}";

        public string GenerateCMode()
        {
            var morph = (UseSp && !IsVehicled && SpInstance.HasSkin ? SpInstance.Item.VNum == 903 ? 102 : SpInstance.Item.VNum == 913 ? 101 : SpInstance.Item.VNum == 902 ? 100 : UseSp || IsVehicled || IsMorphed ? Morph : 0 : UseSp || IsVehicled || IsMorphed ? Morph : 0);

            ItemInstance item = Inventory.LoadBySlotAndType((byte) EquipmentType.Wings, InventoryType.Wear);

            return !IsSeal ? $"c_mode 1 {CharacterId} {morph} {(!IsLaurenaMorph() && UseSp ? MorphUpgrade : 0)} {(!IsLaurenaMorph() && UseSp ? MorphUpgrade2 : 0)} {ArenaWinner} {Size} {item?.Item.Morph ?? 0}" : "";
        }

        public string GenerateCond() => $"cond 1 {CharacterId} {(!IsLaurenaMorph() && !CanAttack() ? 1 : 0)} {(!CanMove() ? 1 : 0)} {Speed}";

        public string GenerateDG()
        {
            byte raidType = 0;

            if (ServerManager.Instance.Act4RaidStart.AddMinutes(60) < DateTime.Now)
            {
                ServerManager.Instance.Act4RaidStart = DateTime.Now;
            }

            double seconds = (ServerManager.Instance.Act4RaidStart.AddMinutes(60) - DateTime.Now).TotalSeconds;

            switch (Family?.Act4Raid?.MapInstanceType)
            {
                case MapInstanceType.Act4Morcos:
                    raidType = 1;
                    break;

                case MapInstanceType.Act4Hatus:
                    raidType = 2;
                    break;

                case MapInstanceType.Act4Calvina:
                    raidType = 3;
                    break;

                case MapInstanceType.Act4Berios:
                    raidType = 4;
                    break;
            }

            return $"dg {raidType} {(seconds > 1800 ? 1 : 2)} {(int) seconds} 0";
        }

        public void GenerateDignity(NpcMonster monsterinfo)
        {
            if (Level < monsterinfo.Level && Dignity < 100 && Level > 20)
            {
                Dignity += (float) 0.5;

                if (Dignity == (int) Dignity)
                {
                    Session.SendPacket(GenerateFd());
                    Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("RESTORE_DIGNITY"), 11));
                }
            }
        }

        public string GenerateDir() => $"dir 1 {CharacterId} {Direction}";

        public string GenerateDm(int dmg) => BattleEntity.GenerateDm(dmg);

        public EffectPacket GenerateEff(int effectid)
        {
            return new EffectPacket
            {
                EffectType = UserType.Player,
                CallerId = CharacterId,
                EffectId = effectid
            };
        }

        public string GenerateEq()
        {
            int color = (byte) HairColor;

            ItemInstance head = Inventory?.LoadBySlotAndType((byte) EquipmentType.Hat, InventoryType.Wear);

            if (head?.Item.IsColored == true)
            {
                color = head.Design;
            }

            return $"eq {CharacterId} {(Invisible && Authority >= AuthorityType.DSGM ? 6 : Undercover ? (byte) AuthorityType.User : Authority < AuthorityType.User ? (byte) AuthorityType.User : Authority >= AuthorityType.DSGM ? 2 : (byte) Authority)} {(byte) Gender} {(byte) HairStyle} {color} {(byte) Class} {GenerateEqListForPacket()} {(!InvisibleGm ? GenerateEqRareUpgradeForPacket() : null)}";

            //return $"eq {CharacterId} {(Invisible ? 6 : 0)} {(byte)Gender} {(byte)HairStyle} {color} {(byte)Class} {GenerateEqListForPacket()} {(!InvisibleGm ? GenerateEqRareUpgradeForPacket() : null)}";
        }

        public string GenerateEqListForPacket()
        {
            string[] invarray = new string[17];

            if (Inventory != null)
            {
                for (short i = 0; i < invarray.Length; i++)
                {
                    ItemInstance item = Inventory.LoadBySlotAndType(i, InventoryType.Wear);

                    if (item != null)
                    {
                        invarray[i] = item.ItemVNum.ToString();
                    }
                    else
                    {
                        invarray[i] = "-1";
                    }
                }
            }

            return $"{invarray[(byte) EquipmentType.Hat]}.{invarray[(byte) EquipmentType.Armor]}.{invarray[(byte) EquipmentType.MainWeapon]}.{invarray[(byte) EquipmentType.SecondaryWeapon]}.{invarray[(byte) EquipmentType.Mask]}.{invarray[(byte) EquipmentType.Fairy]}.{invarray[(byte) EquipmentType.CostumeSuit]}.{invarray[(byte) EquipmentType.CostumeHat]}.{invarray[(byte) EquipmentType.WeaponSkin]}.{invarray[(byte) EquipmentType.Wings]}";
        }

        public string GenerateEqRareUpgradeForPacket()
        {
            sbyte weaponRare = 0;
            byte weaponUpgrade = 0;
            sbyte armorRare = 0;
            byte armorUpgrade = 0;

            if (Inventory != null)
            {
                for (short i = 0; i < 16; i++)
                {
                    ItemInstance wearable = Inventory.LoadBySlotAndType(i, InventoryType.Wear);

                    if (wearable != null)
                    {
                        switch (wearable.Item.EquipmentSlot)
                        {
                            case EquipmentType.MainWeapon:
                                weaponRare = wearable.Rare;
                                weaponUpgrade = wearable.Upgrade;
                                break;

                            case EquipmentType.Armor:
                                armorRare = wearable.Rare;
                                armorUpgrade = wearable.Upgrade;
                                break;
                        }
                    }
                }
            }

            return $"{weaponUpgrade}{weaponRare} {armorUpgrade}{armorRare}";
        }

        public string GenerateEquipment()
        {
            string eqlist = "";

            EquipmentBCards.Lock(() =>
            {
                EquipmentBCards.Clear();
                ShellEffectArmor.Clear();
                ShellEffectMain.Clear();
                RuneEffectMain.Clear();
                ShellEffectSecondary.Clear();
                CellonOptions.Clear();

                if (Inventory != null)
                {
                    EquipmentBCards.AddRange(GetRunesInEquipment());

                    for (short i = 0; i < 17; i++)
                    {
                        ItemInstance item = Inventory.LoadBySlotAndType(i, InventoryType.Wear);
                        if (item != null)
                        {
                            if (item.Item.EquipmentSlot != EquipmentType.Sp)
                            {
                                EquipmentBCards.AddRange(item.Item.BCards);
                                switch (item.Item.ItemType)
                                {
                                    case ItemType.Armor:
                                        foreach (ShellEffectDTO dto in item.ShellEffects)
                                        {
                                            ShellEffectArmor.Add(dto);
                                        }

                                        break;

                                    case ItemType.Weapon:
                                        switch (item.Item.EquipmentSlot)
                                        {
                                            case EquipmentType.MainWeapon:
                                                foreach (ShellEffectDTO dto in item.ShellEffects.Where(s => !s.IsRune))
                                                {
                                                    ShellEffectMain.Add(dto);
                                                }

                                                foreach (RuneEffectDTO dto in item.RuneEffects)
                                                {
                                                    RuneEffectMain.Add(dto);
                                                }

                                                break;

                                            case EquipmentType.SecondaryWeapon:
                                                foreach (ShellEffectDTO dto in item.ShellEffects)
                                                {
                                                    ShellEffectSecondary.Add(dto);
                                                }

                                                break;
                                        }

                                        break;

                                    case ItemType.Jewelery: //Antidupe Cellon / Shell infinite
                                        foreach (CellonOptionDTO dto in item.CellonOptions)
                                        {
                                            CellonOptions.Add(dto);
                                        }
                                        break;
                                }
                            }

                            eqlist += $" {i}.{item.Item.VNum}.{item.Rare}.{(item.Item.IsColored ? item.Design : item.Upgrade)}.0.{item.RuneAmount}";
                        }
                    }
                }
                if (Family != null)
                {

                    foreach (FamilySkillMission famskill in Family.FamilySkillMissions)
                    {
                        Item effect = ServerManager.GetItem(famskill.ItemVNum);
                        EquipmentBCards.AddRange(effect.BCards);
                    }
                }
            });

            return $"equip {GenerateEqRareUpgradeForPacket()}{eqlist}";
        }

        public string GenerateExts()
        {
            var haveback = (HaveBackpack() ? 1 : 0) * 12;
            var haveext = (HaveExtension() ? 1 : 0) * 60;

            string tropDrole = string.Empty;

            for (byte i = 0; i != 3; i++)
            {
                tropDrole += $"{48 + haveback + haveext} ";
            }

            return $"exts 0 {tropDrole}";
        }

        public string GenerateFaction() => $"fs {(byte) Faction}";

        public void GenerateCommandInterface()
        {
            string Command = "";
            Session.SendPacket($"guri 10 1 579068 985 {Command}");
        }

        public string GenerateFamilyMember()
        {
            string str = "gmbr 0";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter TargetCharacter in Family?.FamilyCharacters)
                    {
                        bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, TargetCharacter.CharacterId);
                        str += $" {TargetCharacter.Character.CharacterId}|{Family.FamilyId}|{TargetCharacter.Character.Name}|{TargetCharacter.Character.Level}|{(byte) TargetCharacter.Character.Class}|{(byte) TargetCharacter.Authority}|{(byte) TargetCharacter.Rank}|{(isOnline ? 1 : 0)}|{TargetCharacter.Character.HeroLevel}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return str;
        }

        public string GenerateFamilyMemberExp()
        {
            string str = "gexp";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter TargetCharacter in Family?.FamilyCharacters)
                    {
                        str += $" {TargetCharacter.CharacterId}|{TargetCharacter.Experience}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return str;
        }

        public string GenerateFamilyMemberMessage()
        {
            string str = "gmsg";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter TargetCharacter in Family?.FamilyCharacters)
                    {
                        str += $" {TargetCharacter.CharacterId}|{TargetCharacter.DailyMessage}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return str;
        }

        public bool GenerateFamilyXp(int FXP, short InstanceId = -1)
        {
            if (!Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.BlockFExp && s.DateEnd > DateTime.Now) && Family != null && FamilyCharacter != null && (InstanceId == -1 || Session.Character.GeneralLogs.CountLinq(s => s.LogType == "InstanceEntry" && short.Parse(s.LogData) == InstanceId && s.Timestamp.Date == DateTime.Today) == 0))
            {
                FamilyCharacterDTO famchar = FamilyCharacter;
                FamilyDTO fam = Family;
                FXP += (FXP / 100) * ServerManager.Instance.Configuration.EventXPF;
                fam.FamilyExperience += FXP;
                famchar.Experience += FXP;
                if (CharacterHelper.LoadFamilyXPData(Family.FamilyLevel) <= fam.FamilyExperience)
                {
                    fam.FamilyExperience -= CharacterHelper.LoadFamilyXPData(Family.FamilyLevel);
                    fam.FamilyLevel++;
                    Family.AddMissionProgress((short)(9616 + fam.FamilyLevel), 1);
                    Family.InsertFamilyLog(FamilyLogType.FamilyLevelUp, level: fam.FamilyLevel);
                    CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = Family.FamilyId,
                        SourceCharacterId = CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("FAMILY_UP"), 0),
                        Type = MessageType.Family
                    });
                }

                DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref famchar);
                DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
                ServerManager.Instance.FamilyRefresh(Family.FamilyId);
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = Family.FamilyId,
                    SourceCharacterId = CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = "fhis_stc",
                    Type = MessageType.Family
                });
                if (FXP > 1000)
                {
                    int value = FXP - FXP % 1000;
                    Session.Character.Family.InsertFamilyLog(FamilyLogType.FamilyXP, Session.Character.Name, experience: value);
                }
                else if (famchar.Experience % 1000 == 0)
                {
                    Session.Character.Family.InsertFamilyLog(FamilyLogType.FamilyXP, Session.Character.Name, experience: 1000);
                }

                return true;
            }

            return false;
        }

        public string GenerateFc()
        {
            return
                $"fc {(byte) Faction} {ServerManager.Instance.Act4AngelStat.MinutesUntilReset} {ServerManager.Instance.Act4AngelStat.Percentage / 100} {ServerManager.Instance.Act4AngelStat.Mode}" +
                $" {ServerManager.Instance.Act4AngelStat.CurrentTime} {ServerManager.Instance.Act4AngelStat.TotalTime} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsMorcos)}" +
                $" {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsHatus)} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsCalvina)} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsBerios)}" +
                $" 0 {ServerManager.Instance.Act4DemonStat.Percentage / 100} {ServerManager.Instance.Act4DemonStat.Mode} {ServerManager.Instance.Act4DemonStat.CurrentTime} {ServerManager.Instance.Act4DemonStat.TotalTime}" +
                $" {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsMorcos)} {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsHatus)} {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsCalvina)} " +
                $"{Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsBerios)} 0";

            //return $"fc {Faction} 0 69 0 0 0 1 1 1 1 0 34 0 0 0 1 1 1 1 0";
        }

        public string GenerateFd() => $"fd {Reputation} {GetReputationIco()} {(int) Dignity} {Math.Abs(GetDignityIco())}";

        public string GenerateFinfo(long? relatedCharacterLoggedId, bool isConnected)
        {
            string result = "finfo";
            foreach (CharacterRelationDTO relation in CharacterRelations.Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
            {
                if (relatedCharacterLoggedId.HasValue && (relatedCharacterLoggedId.Value == relation.RelatedCharacterId || relatedCharacterLoggedId.Value == relation.CharacterId))
                {
                    if (DAOFactory.CharacterDAO.LoadById(relatedCharacterLoggedId.Value) is CharacterDTO character)
                    {
                        result += $" {relatedCharacterLoggedId}.{(isConnected ? 1 : 0)}.{character.Name}";
                    }
                }
            }

            return result;
        }

        public string GenerateFinit()
        {
            string result = "finit";
            foreach (CharacterRelationDTO relation in CharacterRelations.ToList().Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
            {
                long id = relation.RelatedCharacterId == CharacterId ? relation.CharacterId : relation.RelatedCharacterId;
                if (DAOFactory.CharacterDAO.LoadById(id) is CharacterDTO character)
                {
                    bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup,id);
                    result += $" {id}|{(short) relation.RelationType}|{(isOnline ? 1 : 0)}|{character.Name}";
                }
            }

            return result;
        }

        public void GenerateFmRank(byte type)
        {
            string packet = "fmrank_stc";
            int i = 1;
            List<Family> familyordered = null;
            if (Family == null)
            {
                return;
            }

            switch (type)
            {
                case 0:
                case 2:
                case 3:
                    packet += " 0";
                    break;

                case 4:
                case 6:
                case 7:
                    packet += " 1";
                    break;

                case 8:
                case 9:
                    packet += " 2";
                    break;

                default:
                    return;
            }

            switch (type)
            {
                case 0:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(s => s.FamilyExperience).ToList();
                    break;

                case 2:
                    if (Family.FamilyFaction != 1)
                    {
                        return;
                    }

                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 1).OrderByDescending(s => s.FamilyExperience).ToList();
                    break;

                case 3:
                    if (Family.FamilyFaction != 2)
                    {
                        return;
                    }

                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 2).OrderByDescending(s => s.FamilyExperience).ToList();
                    break;

                case 4:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(s => s.FamilyLogs.Where(l => l.FamilyLogType == FamilyLogType.FamilyXP && l.Timestamp.AddDays(30) < DateTime.Now).ToList().Sum(c => long.Parse(c.FamilyLogData.Split('|')[1]))).ToList();
                    break;

                case 6:
                    if (Family.FamilyFaction != 1)
                    {
                        return;
                    }

                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 1).OrderByDescending(s => s.FamilyLogs.Where(l => l.FamilyLogType == FamilyLogType.FamilyXP && l.Timestamp.AddDays(30) < DateTime.Now).ToList().Sum(c => long.Parse(c.FamilyLogData.Split('|')[1]))).ToList();
                    break;

                case 7:
                    if (Family.FamilyFaction != 2)
                    {
                        return;
                    }

                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 2).OrderByDescending( s => s.FamilyLogs.Where(l => l.FamilyLogType == FamilyLogType.FamilyXP && l.Timestamp.AddDays(30) < DateTime.Now).ToList().Sum(c => long.Parse(c.FamilyLogData.Split('|')[1]))).ToList();
                    break;

                case 8:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(s => s.FamilyExperience).ToList();
                    break;

                case 9:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(s => s.FamilyCharacters.Sum(c => c.Character.Reputation)).ToList();
                    break;
            }

            if (familyordered != null)
            {
                foreach (Family fam in familyordered.Take(100))
                {
                    if (fam != Family)
                    {
                        i++;
                        continue;
                    }

                    switch (type)
                    {
                        case 0:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyExperience}";
                            break;

                        case 2:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 3:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 4:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyExperience}";
                            break;

                        case 6:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 7:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 8:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyExperience}";
                            break;

                        case 9:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Reputation)}";
                            break;
                    }
                }
            }

            packet += $"|{Family.FamilyExperience}|{CharacterHelper.LoadFamilyXPData(Family.FamilyLevel)}";

            Session.SendPacket(packet);
        }

        public string GenerateFStashAll()
        {
            string stash = $"f_stash_all {Family.WarehouseSize}";
            foreach (ItemInstance item in Family.Warehouse.GetAllItems())
            {
                stash += $" {item.GenerateStashPacket()}";
            }

            return stash;
        }

        public string GenerateFtPtPacket() => $"ftpt {UltimatePoints} 3000";

        public string GenerateGender() => $"p_sex {(byte) Gender}";

        public string GenerateGExp()
        {
            string str = "gexp";
            foreach (FamilyCharacter familyCharacter in Family.FamilyCharacters)
            {
                str += $" {familyCharacter.CharacterId}|{familyCharacter.Experience}";
            }

            return str;
        }

        public string GenerateGidx() //Left Faction
        {
            if (Family == null || FamilyCharacter == null || Family.FamilySkillMissions == null)
            {
                return $"gidx 1 {CharacterId} -1 - 0 0|0|0";
            }

            int x = 0;

            switch (FamilyCharacter.Authority)
            {
                case FamilyAuthority.Member:
                    x = 918;
                    break;
                case FamilyAuthority.Head:
                    x = 915;
                    break;
                case FamilyAuthority.Familykeeper:
                    x = 916;
                    break;
                case FamilyAuthority.Familydeputy:
                    x = 917;
                    break;

            }

            return
                    $"gidx 1 " +
                    $"{CharacterId} " +
                    $"{Family.FamilyId}.{x} " +
                    $"{Family.Name} " +
                    $"{Family.FamilyLevel} " +
                    $"{(Family.FamilySkillMissions.Any(s => s.ItemVNum == 9600) ? 1 : 0)}|" +
                    $"{(Family.FamilySkillMissions.Any(s => s.ItemVNum == 9601) ? 1 : 0)}|" +
                    $"{(Family.FamilyFaction)}";
        }     

        public string GenerateGInfo()
        {
            if (Family != null)
            {
                try
                {
                    FamilyCharacter familyCharacter = Family.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                    if (familyCharacter != null)
                    {
                        return
                            $"ginfo {Family.Name} {familyCharacter.Character.Name} {(byte) Family.FamilyHeadGender} {Family.FamilyLevel} {Family.FamilyExperience} {CharacterHelper.LoadFamilyXPData(Family.FamilyLevel)} {Family.FamilyCharacters.Count} {Family.MaxSize} {(byte) FamilyCharacter.Authority} {(Family.ManagerCanInvite ? 1 : 0)} {(Family.ManagerCanNotice ? 1 : 0)} {(Family.ManagerCanShout ? 1 : 0)} {(Family.ManagerCanGetHistory ? 1 : 0)} {(byte) Family.ManagerAuthorityType} {(Family.MemberCanGetHistory ? 1 : 0)} {(byte) Family.MemberAuthorityType} {Family.FamilyMessage.Replace(' ', '^')}";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }

            return "";
        }

        public string GenerateGold() => $"gold {Gold} 0";

        public string GenerateIcon(int type, int value, short itemVNum) => $"icon {type} {CharacterId} {value} {itemVNum}";

        public string GenerateIdentity() => $"Character: {Name}";

        public string GenerateIn(bool foe = false, AuthorityType receiverAuthority = AuthorityType.User, int InEffect = 0)
        {
            string name = Name;

            if (receiverAuthority >= AuthorityType.DSGM)
            {
                foe = false;
                name = $"[{Faction}]{name}";
            }

            if (foe && Authority < AuthorityType.DSGM)
            {
                name = "!§$%&/()=?*+~#";
            }

            int faction = 0;

            if (ServerManager.Instance.ChannelId == 51)
            {
                faction = (byte) Faction + 2;
            }

            int color = HairStyle == HairStyleType.Hair8 ? 0 : (byte) HairColor;

            ItemInstance fairy = null;

            if (Inventory != null)
            {
                ItemInstance headWearable = Inventory.LoadBySlotAndType((byte) EquipmentType.Hat, InventoryType.Wear);

                if (headWearable?.Item.IsColored == true)
                {
                    color = headWearable.Design;
                }

                fairy = Inventory.LoadBySlotAndType((byte) EquipmentType.Fairy, InventoryType.Wear);
            }

            long tit = 0;
            if (Title.Find(s => s.Stat.Equals(3)) != null)
            {
                tit = Title.Find(s => s.Stat.Equals(3)).TitleVnum;
            }

            if (Title.Find(s => s.Stat.Equals(7)) != null)
            {
                tit = Title.Find(s => s.Stat.Equals(7)).TitleVnum;
            }

            var fLvl = (Family != null ? Family.FamilyLevel >= 5 ? Family.FamilyFaction == 0 ? "1" : Family.FamilyFaction.ToString() : "0" : "0");
            return $"in 1 " +
                   $"{(Authority > AuthorityType.User && !Undercover ? Authority == AuthorityType.Supporter ? $"[{Authority}]" + name : name : name)} " +
                   $"- {CharacterId} {PositionX} {PositionY} {Direction} " +
                   $"{(Undercover ? (byte) AuthorityType.User : Authority >= AuthorityType.MOD ? 2 : (byte) Authority)} {(byte) Gender} {(byte) HairStyle} {color} {(byte) Class} " +
                   $"{GenerateEqListForPacket()} {Math.Ceiling(Hp / HPLoad() * 100)} {Math.Ceiling(Mp / MPLoad() * 100)} {(IsSitting ? 1 : 0)} " +
                   $"{(Group?.GroupType == GroupType.Group ? (Group?.GroupId ?? -1) : -1)} {(fairy != null && !Undercover ? 4 : 0)} " +
                   $"{fairy?.Item.Element ?? 0} 0 {fairy?.Item.Morph ?? 0} {InEffect} {(UseSp || IsVehicled || IsMorphed ? Morph : 0)} " +
                   $"{GenerateEqRareUpgradeForPacket()} {(!Undercover ? (foe ? -1 : Family?.FamilyId ?? -1) : -1)} {(!Undercover ? (foe ? name : Family?.Name ?? "-") : "-")} " +
                   $"{(GetDignityIco() == 1 ? GetReputationIco() : -GetDignityIco())} {(Invisible ? 1 : 0)} {(UseSp ? MorphUpgrade : 0)} {faction} " +
                   $"{(UseSp ? MorphUpgrade2 : 0)} {Level} {Family?.FamilyLevel ?? 0} " +
                   $"{ArenaWinner} " +
                   $"{(IsFamilyTop(true) ? 1 : 0)}|" +
                   $"{(IsFamilyTop(false) ? 1 : 0)}|" +
                   $"{fLvl} " +
                   $"{Compliment} {Size} {HeroLevel} {tit}";
        }

        public string GenerateInvisible() => $"cl {CharacterId} {(Invisible ? 1 : 0)} {(InvisibleGm ? 1 : 0)}";

        public string GenerateSmemo(string message, byte type) => $"s_memo {type} {message}";

        public string GenerateGB(byte type) => $"gb {type} {Session.Character.GoldBank / 1000} {Gold} 0 0";

        public void GenerateKillBonus(MapMonster monsterToAttack, BattleEntity Killer)
        {
            RewardsHelper.Instance.MobKillRewards(Session);
            #region RBB

            if (Session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.RainbowBattleInstance && monsterToAttack?.MonsterVNum == 2558)
            {
                var rbb = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(Session));

                rbb.Score += 5;

                // Give buff mandra
                if (ServerManager.RandomNumber() < 90)
                {
                    Session.Character.AddBuff(new Buff(4, Level), BattleEntity, true);
                }
                else
                {
                    Session.Character.AddBuff(new Buff(5, Level), BattleEntity, true);
                }

                Session.CurrentMapInstance.Broadcast($"msg 0 {Session.Character.Name} killed The Mandra and won 5 points!");
                RainbowBattleManager.SendFbs(Session.CurrentMapInstance);
            }

            #endregion
            void _handleGoldDrop(DropDTO drop, long maxGold, long? dropOwner, short posX, short posY)
            {
                int amount = drop.Amount;
                if (ServerManager.Instance.Configuration.EventGold > 1)
                {
                    amount *= ServerManager.Instance.Configuration.EventGold;
                }
                Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                {
                    if (Session.HasCurrentMapInstance)
                    {
                        if (CharacterId == dropOwner && StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.AutoLoot))
                        {
                            double multiplier = 1 + (Session.Character.GetBuff(CardType.Item,(byte) AdditionalTypes.Item.IncreaseEarnedGold)[0] / 100D);
                            multiplier += (Session.Character.ShellEffectMain.FirstOrDefault(s => s.Effect == (byte) ShellWeaponEffectType.GainMoreGold)?.Value ?? 0) / 100D;

                            Gold += (int) (drop.Amount * multiplier);

                            if (Gold > maxGold)
                            {
                                Gold = maxGold;
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"),0));
                            }

                            Session.SendPacket(GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.GetItem(drop.ItemVNum).Name} x {drop.Amount}{(multiplier > 1 ? $" + {(int) (drop.Amount * multiplier) - drop.Amount}" : "")}",12));
                            Session.SendPacket(GenerateGold());
                        }
                        else
                        {
                            double multiplier = 1 + (Session.Character.GetBuff(CardType.Item,(byte) AdditionalTypes.Item.IncreaseEarnedGold)[0] / 100D);
                            multiplier +=(Session.Character.ShellEffectMain.FirstOrDefault(s =>s.Effect == (byte) ShellWeaponEffectType.GainMoreGold)?.Value ?? 0) / 100D;

                            Gold += (int) (drop.Amount * multiplier);

                            if (Gold > maxGold)
                            {
                                Gold = maxGold;
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"),0));
                            }

                            Session.SendPacket(GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.GetItem(drop.ItemVNum).Name} x {drop.Amount}{(multiplier > 1 ? $" + {(int) (drop.Amount * multiplier) - drop.Amount}" : "")}",12));
                            Session.SendPacket(GenerateGold());
                        }
                    }
                });
            }


            void _handleItemDrop(DropDTO drop, long? owner, short posX, short posY)
            {
                int amount = drop.Amount;
                if (ServerManager.Instance.Configuration.LockSystem)
                {
                    Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                    {
                        if (Session.HasCurrentMapInstance)
                        {
                            if (CharacterId == owner && StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.AutoLoot))
                            {
                                if (!Session.Character.VerifiedLock)
                                {
                                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THIS_ACTION_CHARACTER_IS_LOCKED"), 10));
                                    return;
                                }
                                GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                            }
                            else
                            {
                                if (!Session.Character.VerifiedLock)
                                {
                                    Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THIS_ACTION_CHARACTER_IS_LOCKED"), 10));
                                    return;
                                }
                                GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                            }
                        }
                    });
                }
                if (ServerManager.Instance.Configuration.EventDrop > 1)
                {
                    amount *= ServerManager.Instance.Configuration.EventDrop;
                }
                else
                {
                    Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                    {
                        if (CharacterId == owner &&
                            StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.AutoLoot))
                        {
                            GiftAdd(drop.ItemVNum, (byte) drop.Amount);
                        }

                        //int[] items = { 1012, 2098, 2102, 2010, 2117, 2118, 2114, 2099, 2116, 2046, 2042, 2038, 2035 };
                        //if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance || Session.Character.MapInstance.MapInstanceType == MapInstanceType.Act4Instance && items.Contains(drop.ItemVNum))
                        //{
                        //    GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                        //}

                        //int[] items2 = { 1013, 2900, 1031, 1032, 1033, 1034, 2118, 1029 };
                        //if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance && items.Contains(drop.ItemVNum))
                        //{
                        //    Session.CurrentMapInstance.DropItemByMonster(owner, drop, monsterToAttack.MapX, monsterToAttack.MapY, Quests.Any(q => (q.Quest.QuestType == (int)QuestType.Collect4 || q.Quest.QuestType == (int)QuestType.Collect2 || (q.Quest?.QuestType == (int)QuestType.Collect1 && MapInstance.Map.MapTypes.Any(s => s.MapTypeId != (short)MapTypeEnum.Act4))) && q.Quest.QuestObjectives.Any(qst => qst.Data == drop.ItemVNum)));
                        //}

                        else
                        {
                            Session.CurrentMapInstance.DropItemByMonster(owner, drop, monsterToAttack.MapX, monsterToAttack.MapY, Quests.Any(q =>(q.Quest.QuestType == (int) QuestType.Collect4 || q.Quest.QuestType == (int) QuestType.Collect2 || (q.Quest?.QuestType == (int) QuestType.Collect1 && MapInstance.Map.MapTypes.Any(s => s.MapTypeId != (short) MapTypeEnum.Act4))) && q.Quest.QuestObjectives.Any(qst => qst.Data == drop.ItemVNum)));
                        }
                    });
                }
            }

            lock (_syncObj)
            {
                if (monsterToAttack == null || monsterToAttack.IsAlive)
                {
                    return;
                }

                monsterToAttack.RunDeathEvent();

                // Not 100% sure if this covers all mob kills, have to look it later.
                if (Killer != null)
                {
                    if (Killer.Character != null && Killer.Character is Character chara) chara.MobKillCounter++;
                    if (Killer.Mate != null && Killer.Mate.Owner != null && Killer.Mate.Owner is Character charaMate) charaMate.MobKillCounter++;
                }

                if (monsterToAttack.GetBuff(CardType.SpecialEffects, (byte) AdditionalTypes.SpecialEffects.DecreaseKillerHP) is int[] DecreaseKillerHp)
                {
                    bool EffectResistance = false;
                    if (Killer.MapEntityId != CharacterId)
                    {
                        if (Killer.HasBuff(CardType.Buff, (byte) AdditionalTypes.Buff.EffectResistance))
                        {
                            if (ServerManager.RandomNumber() < 90)
                            {
                                EffectResistance = true;
                            }
                        }

                        if (!EffectResistance)
                        {
                            if (DecreaseKillerHp[0] > 0)
                            {
                                if (!HasGodMode)
                                {
                                    int DecreasedHp = 0;
                                    if (Killer.Hp - Killer.Hp * DecreaseKillerHp[0] / 100 > 1)
                                    {
                                        DecreasedHp = Killer.Hp * DecreaseKillerHp[0] / 100;
                                    }
                                    else
                                    {
                                        DecreasedHp = Killer.Hp - 1;
                                    }

                                    Killer.GetDamage(DecreasedHp, monsterToAttack.BattleEntity, true);
                                    Session.SendPacket(Killer.GenerateDm(DecreasedHp));
                                    if (Killer.Mate != null)
                                    {
                                        Session.SendPacket(Killer.Mate.GenerateStatInfo());
                                    }

                                    Session.SendPacket(new EffectPacket{EffectType = Killer.UserType, CallerId = Killer.MapEntityId, EffectId = 6007});
                                }
                            }
                        }
                    }
                    else
                    {
                        if (HasBuff(CardType.Buff, (byte) AdditionalTypes.Buff.EffectResistance))
                        {
                            if (ServerManager.RandomNumber() < 90)
                            {
                                EffectResistance = true;
                            }
                        }

                        if (!EffectResistance)
                        {
                            if (DecreaseKillerHp[0] > 0)
                            {
                                if (!HasGodMode)
                                {
                                    int DecreasedHp = 0;
                                    if (Hp - Hp * DecreaseKillerHp[0] / 100 > 1)
                                    {
                                        DecreasedHp = Hp * DecreaseKillerHp[0] / 100;
                                    }
                                    else
                                    {
                                        DecreasedHp = Hp - 1;
                                    }

                                    GetDamage(DecreasedHp, monsterToAttack.BattleEntity, true);
                                    Session.SendPacket(GenerateDm(DecreasedHp));
                                    Session.SendPacket(GenerateStat());
                                    Session.SendPacket(GenerateEff(6007));
                                }
                            }
                        }
                    }
                }

                Random random = new Random(DateTime.Now.Millisecond & monsterToAttack.MapMonsterId);

                long? dropOwner;

                lock (monsterToAttack.DamageList)
                {
                    dropOwner = monsterToAttack.DamageList.FirstOrDefault(s => s.Value > 0).Key?.MapEntityId ?? null;
                }

                Group group = null;
                if (dropOwner != null)
                {
                    group = ServerManager.Instance.Groups.Find(g =>
                        g.IsMemberOfGroup((long) dropOwner) && g.GroupType == GroupType.Group);
                }

                IncrementQuests(QuestType.Hunt, monsterToAttack.MonsterVNum);

                if (ServerManager.Instance.ChannelId == 51)
                {
                    if (ServerManager.Instance.Act4DemonStat.Mode == 0 && ServerManager.Instance.Act4AngelStat.Mode == 0 && !CaligorRaid.IsRunning)
                    {
                        if (Faction == FactionType.Angel)
                        {
                            ServerManager.Instance.Act4AngelStat.Percentage++;
                        }
                        else if (Faction == FactionType.Demon)
                        {
                            ServerManager.Instance.Act4DemonStat.Percentage++;
                        }
                    }

                    if (monsterToAttack.MonsterVNum == 556)
                    {
                        if (ServerManager.Instance.Act4AngelStat.Mode == 1 && Faction != FactionType.Angel)
                        {
                            ServerManager.Instance.Act4AngelStat.Mode = 0;
                        }

                        if (ServerManager.Instance.Act4DemonStat.Mode == 1 && Faction != FactionType.Demon)
                        {
                            ServerManager.Instance.Act4DemonStat.Mode = 0;
                        }
                    }
                }

                // end owner set
                if (Session.HasCurrentMapInstance &&
                    ((MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance ||
                      MapInstance.MapInstanceType == MapInstanceType.LodInstance) || MapInstance.DropAllowed))
                {
                    short[] explodeMonsters = new short[] {1348, 1906};

                    List<DropDTO> droplist = monsterToAttack.Monster.Drops.Where(s =>
                        (!explodeMonsters.Contains(monsterToAttack.MonsterVNum) &&
                         Session.CurrentMapInstance.Map.MapTypes.Any(m => m.MapTypeId == s.MapTypeId)) ||
                        s.MapTypeId == null).ToList();

                    int levelDifference = Session.Character.Level - monsterToAttack.Monster.Level;

                    #region Quest

                    Quests.Where(q =>
                            (q.Quest?.QuestType == (int) QuestType.Collect4 ||
                             q.Quest?.QuestType == (int) QuestType.Collect2 ||
                             (q.Quest?.QuestType == (int) QuestType.Collect1 &&
                              MapInstance.Map.MapTypes.Any(s => s.MapTypeId != (short) MapTypeEnum.Act4)))).ToList()
                        .ForEach(qst =>
                        {
                            qst.Quest.QuestObjectives.ForEach(d =>
                            {
                                if (d.SpecialData == monsterToAttack.MonsterVNum || d.SpecialData == null)
                                {
                                    droplist.Add(new DropDTO()
                                    {
                                        ItemVNum = (short) d.Data,
                                        Amount = 1,
                                        MonsterVNum = monsterToAttack.MonsterVNum,
                                        DropChance = (int) ((d.DropRate ?? 100) * 100 *
                                                            ServerManager.Instance.Configuration
                                                                .QuestDropRate) // Approx
                                    });
                                }
                            });
                        });

                    IncrementQuests(QuestType.FlowerQuest, monsterToAttack.Monster.Level);

                    #endregion

                    if (explodeMonsters.Contains(monsterToAttack.MonsterVNum) && ServerManager.RandomNumber() < 50)
                    {
                        MapInstance.Broadcast($"eff 3 {monsterToAttack.MapMonsterId} 3619");
                        if (Killer.MapEntityId != CharacterId)
                        {
                            if (!HasGodMode)
                            {
                                int DecreasedHp = 0;
                                if (Killer.Hp - Killer.Hp * 50 / 100 > 1)
                                {
                                    DecreasedHp = Killer.Hp * 50 / 100;
                                }
                                else
                                {
                                    DecreasedHp = Killer.Hp - 1;
                                }

                                Killer.GetDamage(DecreasedHp, monsterToAttack.BattleEntity, true);
                                if (Killer.Mate != null)
                                {
                                    Session.SendPacket(Killer.Mate.GenerateStatInfo());
                                }
                            }
                        }
                        else
                        {
                            if (!HasGodMode)
                            {
                                int DecreasedHp = 0;
                                if (Hp - Hp * 50 / 100 > 1)
                                {
                                    DecreasedHp = Hp * 50 / 100;
                                }
                                else
                                {
                                    DecreasedHp = Hp - 1;
                                }

                                GetDamage(DecreasedHp, monsterToAttack.BattleEntity, true);
                                Session.SendPacket(GenerateStat());
                            }
                        }

                        return;
                    }

                    if (monsterToAttack.Monster.MonsterType != MonsterType.Special)
                    {
                        #region item drop

                        int dropRate = (ServerManager.Instance.Configuration.RateDrop + MapInstance.DropRate);
                        int x = 0;
                        double rndamount = ServerManager.RandomNumber() * random.NextDouble();
                        foreach (DropDTO drop in droplist.OrderBy(s => random.Next()))
                        {
                            if (x < 4)
                            {
                                if (!explodeMonsters.Contains(monsterToAttack.MonsterVNum))
                                {
                                    rndamount = ServerManager.RandomNumber() * random.NextDouble();
                                }

                                bool divideRate = true;
                                if (MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (byte)MapTypeEnum.Act4)
                                    || MapInstance.Map.MapId == 20001 // Miniland
                                    || MapInstance.Map.MapId == 103
                                    || explodeMonsters.Contains(monsterToAttack.MonsterVNum))
                                {
                                    divideRate = false;
                                }

                                double divider = !divideRate ? 1D :
                                    levelDifference >= 20 ? (levelDifference - 19) * 1.2D :
                                    levelDifference <= -20 ? (levelDifference + 19) * 1.2D : 1D;
                                if (rndamount <= (double)drop.DropChance * dropRate / 1000.000 / divider)
                                {
                                    x++;
                                    if (Session.CurrentMapInstance != null)
                                    {
                                        if (monsterToAttack.Monster.MonsterType == MonsterType.Elite)
                                        {
                                            List<long> alreadyGifted = new List<long>();
                                            List<BattleEntity> damagers;

                                            lock (monsterToAttack.DamageList)
                                            {
                                                damagers = monsterToAttack.DamageList.Keys.ToList();
                                            }

                                            foreach (BattleEntity damager in damagers)
                                            {
                                                if (!alreadyGifted.Contains(damager.MapEntityId))
                                                {
                                                    ClientSession giftsession =
                                                        ServerManager.Instance.GetSessionByCharacterId(
                                                            damager.MapEntityId);
                                                    giftsession?.Character.GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                                                    alreadyGifted.Add(damager.MapEntityId);
                                                }
                                            }
                                        }
                                        else if (Session.CurrentMapInstance.Map.MapTypes.Any(s =>
                                            s.MapTypeId == (short)MapTypeEnum.Act4))
                                        {
                                            List<long> alreadyGifted = new List<long>();
                                            List<Character> hitters;

                                            lock (monsterToAttack.DamageList)
                                            {
                                                hitters = monsterToAttack.DamageList
                                                    .Where(s => s.Key?.Character != null &&
                                                                s.Key.Character.MapInstance ==
                                                                monsterToAttack.MapInstance && s.Value > 0)
                                                    .Select(s => s.Key.Character).ToList();
                                            }

                                            foreach (Character hitter in hitters)
                                            {
                                                if (!alreadyGifted.Contains(hitter.CharacterId))
                                                {
                                                    hitter.GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                                                    alreadyGifted.Add(hitter.CharacterId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (group?.GroupType == GroupType.Group)
                                            {
                                                if (group.SharingMode == (byte)GroupSharingType.ByOrder)
                                                {
                                                    dropOwner = group.GetNextOrderedCharacterId(this);
                                                    if (dropOwner.HasValue)
                                                    {
                                                        group.Sessions.ForEach(s =>
                                                            s.SendPacket(s.Character.GenerateSay(
                                                                string.Format(
                                                                    Language.Instance
                                                                        .GetMessageFromKey("ITEM_BOUND_TO"),
                                                                    ServerManager.GetItem(drop.ItemVNum).Name,
                                                                    group.Sessions.Single(c =>
                                                                            c.Character.CharacterId == (long)dropOwner)
                                                                        .Character.Name, drop.Amount), 10)));
                                                    }
                                                }
                                                else
                                                {
                                                    group.Sessions.ForEach(s =>
                                                        s.SendPacket(s.Character.GenerateSay(
                                                            string.Format(
                                                                Language.Instance.GetMessageFromKey("DROPPED_ITEM"),
                                                                ServerManager.GetItem(drop.ItemVNum).Name, drop.Amount),
                                                            10)));
                                                }
                                            }

                                            _handleItemDrop(drop, dropOwner, monsterToAttack.MapX,
                                                monsterToAttack.MapY);
                                        }
                                    }

                                    if (explodeMonsters.Contains(monsterToAttack.MonsterVNum))
                                    {
                                        break;
                                    }
                                }
                                else if (explodeMonsters.Contains(monsterToAttack.MonsterVNum))
                                {
                                    rndamount -= (double)drop.DropChance * dropRate / 1000.000 / divider;
                                }

                            }
                        }

                        #endregion

                        #region gold drop

                        // gold calculation
                        int gold = GetGold(monsterToAttack);
                        gold *= ServerManager.Instance.Configuration.RateGold;
                        long maxGold = ServerManager.Instance.Configuration.MaxGold;
                        gold = gold > maxGold ? (int) maxGold : gold;
                        double randChance = ServerManager.RandomNumber() * random.NextDouble();

                        if (Session.CurrentMapInstance.MapInstanceType != MapInstanceType.LodInstance && gold > 0 &&
                            randChance <= (int) (ServerManager.Instance.Configuration.RateGoldDrop * 10 *
                                                 (Session.CurrentMapInstance.Map.MapTypes.Any(s =>
                                                     s.MapTypeId == (short) MapTypeEnum.Act4)
                                                     ? 1
                                                     : CharacterHelper.GoldPenalty(Level,
                                                         monsterToAttack.Monster.Level))))
                        {
                            DropDTO drop2 = new DropDTO
                            {
                                Amount = gold,
                                ItemVNum = 1046
                            };

                            if (Session.CurrentMapInstance != null)
                            {
                                if (Session.CurrentMapInstance.Map.MapTypes.Any(s =>
                                        s.MapTypeId == (short) MapTypeEnum.Act4) ||
                                    monsterToAttack.Monster.MonsterType == MonsterType.Elite)
                                {
                                    List<long> alreadyGifted = new List<long>();
                                    List<BattleEntity> damagers;

                                    lock (monsterToAttack.DamageList)
                                    {
                                        damagers = monsterToAttack.DamageList.Keys.ToList();
                                    }

                                    foreach (BattleEntity damager in damagers)
                                    {
                                        if (!alreadyGifted.Contains(damager.MapEntityId))
                                        {
                                            ClientSession session = ServerManager.Instance.GetSessionByCharacterId(damager.MapEntityId);
                                            if (session != null)
                                            {
                                                double multiplier = 1 + (GetBuff(CardType.Item,(byte) AdditionalTypes.Item.IncreaseEarnedGold)[0] / 100D);
                                                multiplier +=(ShellEffectMain.FirstOrDefault(s =>s.Effect == (byte) ShellWeaponEffectType.GainMoreGold) ?.Value ?? 0) / 100D;

                                                session.Character.Gold += (int) (drop2.Amount * multiplier);
                                                if (session.Character.Gold > maxGold)
                                                {
                                                    session.Character.Gold = maxGold;
                                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));
                                                }

                                                session.SendPacket(session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.GetItem(drop2.ItemVNum).Name} x {drop2.Amount}{(multiplier > 1 ? $" + {(int) (drop2.Amount * multiplier) - drop2.Amount}" : "")}",10));
                                                session.SendPacket(session.Character.GenerateGold());
                                            }

                                            alreadyGifted.Add(damager.MapEntityId);
                                        }
                                    }
                                }
                                else
                                {
                                    if (group != null && MapInstance.MapInstanceType != MapInstanceType.LodInstance)
                                    {
                                        if (group.SharingMode == (byte) GroupSharingType.ByOrder)
                                        {
                                            dropOwner = group.GetNextOrderedCharacterId(this);

                                            if (dropOwner.HasValue)
                                            {
                                                group.Sessions.ForEach(s =>s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ITEM_BOUND_TO"),ServerManager.GetItem(drop2.ItemVNum).Name,group.Sessions.Single(c =>c.Character.CharacterId == (long) dropOwner).Character.Name, drop2.Amount), 10)));
                                            }
                                        }
                                        else
                                        {
                                            group.Sessions.ForEach(s =>s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("DROPPED_ITEM"),ServerManager.GetItem(drop2.ItemVNum).Name, drop2.Amount),10)));
                                        }
                                    }

                                    _handleGoldDrop(drop2, maxGold, dropOwner, monsterToAttack.MapX,monsterToAttack.MapY);
                                }
                            }
                        }

                        #endregion
                    }
                }

                #region act.4 % from monsters

                //if (monsterToAttack.MapInstance?.MapInstanceType == MapInstanceType.Act4Instance)
                if (monsterToAttack.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (byte) MapTypeEnum.Act4 == true))
                {
                    if (ServerManager.Instance.Act4AngelStat.Mode == 0 && ServerManager.Instance.Act4DemonStat.Mode == 0 && ServerManager.Instance.ChannelId == 51)
                    {
                        switch (Faction)
                        {
                            case FactionType.Angel:
                                ServerManager.Instance.Act4AngelStat.Percentage += 10000 / (ServerManager.Instance.Configuration.GlacernonPercentRatePvm * 100);
                                break;

                            case FactionType.Demon:
                                ServerManager.Instance.Act4DemonStat.Percentage += 10000 / (ServerManager.Instance.Configuration.GlacernonPercentRatePvm * 100);
                                break;
                        }

                        ServerManager.Instance.Act4Process();
                    }
                }

                #endregion

                #region Act6Stats

                if (monsterToAttack.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (byte) MapTypeEnum.Act61 == true))
                {
                    if (monsterToAttack.MapInstance?.Map.MapId == 232 && ServerManager.Instance.Act6Zenas.Mode == 0)
                    {
                        ServerManager.Instance.Act6Zenas.Percentage += 10000 / (ServerManager.Instance.Configuration.CylloanPercentRate * 100);
                        ServerManager.Instance.Act6Process();
                    }

                    if (monsterToAttack.MapInstance?.Map.MapId == 236 && ServerManager.Instance.Act6Erenia.Mode == 0)
                    {
                        ServerManager.Instance.Act6Erenia.Percentage += 10000 / (ServerManager.Instance.Configuration.CylloanPercentRate * 100);
                        ServerManager.Instance.Act6Process();
                    }
                }

                #endregion Act6Stats

                #region EXP, Reputation and Dignity

                if (Hp > 0 && !monsterToAttack.BattleEntity.IsMateTrainer(monsterToAttack.MonsterVNum))
                {
                    // If the Halloween event is running then the EXP is disabled in NosVille. -- Is
                    // this official-like or VSalu bullshit?
                    if (!ServerManager.Instance.Configuration.HalloweenEvent || MapInstance.Map.MapId != 1)
                    {
                        GenerateXp(monsterToAttack);
                    }

                    //GenerateDignity(monsterToAttack.Monster);


                    //if (Session.HasCurrentMapInstance)
                    //{
                    //    if (Group?.GroupType == GroupType.Group)
                    //    {
                    //        foreach (ClientSession targetSession in Group.Sessions.Where(s =>
                    //            s.Character.MapInstanceId == MapInstanceId))
                    //        {
                    //            targetSession.Character.GetReputation(monsterToAttack.Monster.Level / 2);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        GetReputation(monsterToAttack.Monster.Level / 2);
                    //    }
                    //}
                }

                #endregion
            }
        }

        public string GenerateLev()
        {
            ItemInstance specialist = null;
            if (Inventory != null)
            {
                specialist = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
            }

            return
                $"lev {Level} {(int) (Level < 100 ? LevelXp : LevelXp / 100)} {(!UseSp || specialist == null ? JobLevel : specialist.SpLevel)} {(!UseSp || specialist == null ? JobLevelXp : specialist.XP)} {(int) (Level < 100 ? XpLoad() : XpLoad() / 100)} {(!UseSp || specialist == null ? JobXPLoad() : SpXpLoad())} {Reputation} {GetCP()} {(int) (HeroLevel < 100 ? HeroXp : HeroXp / 100)} {HeroLevel} {(int) (HeroLevel < 100 ? HeroXPLoad() : HeroXPLoad() / 100)} 0";
        }

        public string GenerateLevelUp()
        {
            Logger.LogUserEvent("LEVELUP", Session.GenerateIdentity(),
                $"Level: {Level} JobLevel: {JobLevel} SPLevel: {Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear)?.SpLevel} HeroLevel: {HeroLevel} MapId: {Session.CurrentMapInstance?.Map.MapId} MapX: {PositionX} MapY: {PositionY}");
            return $"levelup {CharacterId}";
        }

        public void GenerateMiniland()
        {
            if (Miniland == null)
            {
                Miniland = ServerManager.GenerateMapInstance(20001, MapInstanceType.NormalInstance, new InstanceBag(),true);
                foreach (MinilandObjectDTO obj in DAOFactory.MinilandObjectDAO.LoadByCharacterId(CharacterId))
                {
                    MinilandObject mapobj = new MinilandObject(obj);
                    if (mapobj.ItemInstanceId != null)
                    {
                        ItemInstance item = Inventory.GetItemInstanceById((Guid) mapobj.ItemInstanceId);
                        if (item != null)
                        {
                            mapobj.ItemInstance = item;
                            MinilandObjects.Add(mapobj);
                        }
                    }
                }
            }
        }

        public string GenerateMinilandObjectForFriends()
        {
            string mlobjstring = "mltobj";
            int i = 0;
            foreach (MinilandObject mp in MinilandObjects)
            {
                mlobjstring += $" {mp.ItemInstance.ItemVNum}.{i}.{mp.MapX}.{mp.MapY}";
                i++;
            }

            return mlobjstring;
        }

        public string GenerateMinilandPoint() => $"mlpt {MinilandPoint} 100";

        public string GenerateMinimapPosition() => MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance || MapInstance.MapInstanceType == MapInstanceType.RaidInstance ? $"rsfp {MapInstance.MapIndexX} {MapInstance.MapIndexY}" : "rsfp 0 -1";

        public string GenerateMlinfo() => $"mlinfo 3800 {MinilandPoint} 100 {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland) && s.Timestamp.Day == DateTime.Now.Day)} {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland))} 10 {(byte) MinilandState} {Language.Instance.GetMessageFromKey("WELCOME_MUSIC_INFO")} {MinilandMessage.Replace(' ', '^')}";

        public string GenerateMlinfobr() => $"mlinfobr 3800 {Name} {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland) && s.Timestamp.Day == DateTime.Now.Day)} {GeneralLogs.CountLinq(s => s.LogData == nameof(Miniland))} 25 {MinilandMessage.Replace(' ', '^')}";

        public string GenerateMloMg(MinilandObject mlobj, MinigamePacket packet) => $"mlo_mg {packet.MinigameVNum} {MinilandPoint} 0 0 {mlobj.ItemInstance.DurabilityPoint} {mlobj.ItemInstance.Item.MinilandObjectPoint}";

        public string GenerateNpcDialog(int value) => $"npc_req 1 {CharacterId} {value}";

        public string GeneratePairy()
        {
            ItemInstance fairy = null;
            if (Inventory != null)
            {
                fairy = Inventory.LoadBySlotAndType((byte) EquipmentType.Fairy, InventoryType.Wear);
            }

            ElementRate = 0;
            Element = 0;
            bool shouldChangeMorph = false;

            if (fairy != null)
            {
                //exclude magical fairy
                shouldChangeMorph = IsUsingFairyBooster != 0 && (fairy.Item.Morph > 4 && fairy.Item.Morph != 9 && fairy.Item.Morph != 14);
                ElementRate += fairy.ElementRate + fairy.Item.ElementRate + (IsUsingFairyBooster == 1 ? 30 : IsUsingFairyBooster == 2 ? 60 : 0) + GetStuffBuff(CardType.PixieCostumeWings,(byte) AdditionalTypes.PixieCostumeWings.IncreaseFairyElement)[0];
                Element = fairy.Item.Element;
            }

            return fairy != null
                ? $"pairy 1 {CharacterId} 4 {fairy.Item.Element} {fairy.ElementRate + fairy.Item.ElementRate} {fairy.Item.Morph + (shouldChangeMorph ? 5 : 0)}"
                : $"pairy 1 {CharacterId} 0 0 0 0";
        }


        public string GenerateParcel(MailDTO mail) => mail.AttachmentVNum != null ? $"parcel 1 1 {MailList.First(s => s.Value.MailId == mail.MailId).Key} {(mail.Title == "NOSMALL" ? 1 : 4)} 0 {mail.Date.ToString("yyMMddHHmm")} {mail.Title} {mail.AttachmentVNum} {mail.AttachmentAmount} {(byte) ServerManager.GetItem((short) mail.AttachmentVNum).Type}" : "";

        public string GeneratePetskill(int VNum = -1) => $"petski {VNum}";

        public string GenerateSMemo(int type, string msg)
        {
            return $"s_memo {type} {msg}";
        }

        public string GeneratePidx(bool isLeaveGroup = false)
        {
            if (!isLeaveGroup && Group != null)
            {
                string result = $"pidx {Group.GroupId}";
                foreach (ClientSession session in Group.Sessions.GetAllItems().Where(s => s.Character.CharacterId != CharacterId))
                {
                    if (session.Character != null)
                    {
                        result += $" {(Group.IsMemberOfGroup(CharacterId) ? 1 : 0)}.{session.Character.CharacterId} ";
                    }
                }

                foreach (ClientSession session in Group.Sessions.GetAllItems().Where(s => s.Character.CharacterId == CharacterId))
                {
                    if (session.Character != null)
                    {
                        result += $" {(Group.IsMemberOfGroup(CharacterId) ? 1 : 0)}.{session.Character.CharacterId} ";
                    }
                }

                return result;
            }

            return $"pidx -1 1.{CharacterId}";
        }

        public string GeneratePinit()
        {
            Group grp = ServerManager.Instance.Groups.Find(s => s.IsMemberOfGroup(CharacterId) && s.GroupType == GroupType.Group);

            List<Mate> mates = Mates.ToList();

            int count = 0;

            string str = "";

            if (mates != null)
            {
                foreach (Mate mate in mates.Where(s => s.IsTeamMember).OrderByDescending(s => s.MateType))
                {
                    if ((byte) mate.MateType == 1)
                    {
                        count++;
                    }

                    str += $" 2|{mate.MateTransportId}|{(short) mate.MateType}|{mate.Level}|{(mate.IsUsingSp ? mate.Sp.GetName() : mate.Name.Replace(' ', '^'))}|-1|{(mate.IsUsingSp && mate.Sp != null ? mate.Sp.Instance.Item.Morph : mate.Monster.NpcMonsterVNum)}|0";
                }
            }

            if (grp != null)
            {
                foreach (ClientSession groupSessionForId in grp.Sessions.GetAllItems().Where(s => s.Character.CharacterId != CharacterId))
                {
                    count++;
                    str += $" 1|{groupSessionForId.Character.CharacterId}|{count}|{groupSessionForId.Character.Level}|{groupSessionForId.Character.Name}|0|{(byte) groupSessionForId.Character.Gender}|{(byte) groupSessionForId.Character.Class}|{(groupSessionForId.Character.UseSp || groupSessionForId.Character.IsVehicled || groupSessionForId.Character.IsMorphed ? groupSessionForId.Character.Morph : 0)}|{groupSessionForId.Character.HeroLevel}";
                }

                foreach (ClientSession groupSessionForId in grp.Sessions.GetAllItems().Where(s => s.Character.CharacterId == CharacterId))
                {
                    count++;
                    str += $" 1|{groupSessionForId.Character.CharacterId}|{count}|{groupSessionForId.Character.Level}|{groupSessionForId.Character.Name}|0|{(byte) groupSessionForId.Character.Gender}|{(byte) groupSessionForId.Character.Class}|{(groupSessionForId.Character.UseSp || groupSessionForId.Character.IsVehicled || groupSessionForId.Character.IsMorphed ? groupSessionForId.Character.Morph : 0)}|{groupSessionForId.Character.HeroLevel}";
                }
            }

            return $"pinit {(grp != null ? count : mates.Count(s => s.IsTeamMember))} {str}";
        }

        public string GeneratePlayerFlag(long pflag) => $"pflag 1 {CharacterId} {pflag}";

        public string GeneratePost(MailDTO mail, byte type)
        {
            if (mail != null)
            {
                return $"post 1 {type} {(MailList?.FirstOrDefault(s => s.Value?.MailId == mail?.MailId))?.Key} 0 {(mail.IsOpened ? 1 : 0)} {mail.Date.ToString("yyMMddHHmm")} {(type == 2 ? DAOFactory.CharacterDAO.LoadById(mail.ReceiverId).Name : DAOFactory.CharacterDAO.LoadById(mail.SenderId).Name)} {mail.Title}";
            }

            return "";
        }

        public string GeneratePostMessage(MailDTO mailDTO, byte type)
        {
            CharacterDTO sender = DAOFactory.CharacterDAO.LoadById(mailDTO.SenderId);

            return $"post 5 {type} {MailList.First(s => s.Value == mailDTO).Key} 0 0 {(byte) mailDTO.SenderClass} {(byte) mailDTO.SenderGender} {mailDTO.SenderMorphId} {(byte) mailDTO.SenderHairStyle} {(byte) mailDTO.SenderHairColor} {mailDTO.EqPacket} {sender.Name} {mailDTO.Title} {mailDTO.Message}";
        }

        public List<string> GeneratePst() => Mates.Where(s => s.IsTeamMember).OrderByDescending(s => s.MateType).Select(mate => $"pst 2 {mate.MateTransportId} {(mate.MateType == MateType.Partner ? "0" : "1")} {(int) (mate.Hp / mate.MaxHp * 100)} {(int) (mate.Mp / mate.MaxMp * 100)} {mate.Hp} {mate.Mp} 0 0 0 {mate.Buff.GetAllItems().Aggregate("", (current, buff) => current + $" {buff.Card.CardId}")}").ToList();

        public string GeneratePStashAll()
        {
            string stash = $"pstash_all {(StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBackPack) ? 50 : 0)}";
            return Inventory.Where(s => s.Type == InventoryType.PetWarehouse).Aggregate(stash,(current, item) => current + $" {item.GenerateStashPacket()}");
        }

        public string GenerateQuestsPacket(long newQuestId = -1)
        {
            short a = 0;
            short b = 6;
            Quests.ToList().ForEach(qst =>
            {
                qst.QuestNumber = qst.IsMainQuest ? (short) 5 : (!qst.IsMainQuest && !qst.Quest.IsDaily || qst.Quest.QuestId >= 5000 ? b++ : a++);
            });
            return $"qstlist {Quests.Aggregate("", (current, quest) => current + $" {quest.GetInfoPacket(quest.QuestId == newQuestId)}")}";
        }

        public IEnumerable<string> GenerateQuicklist()
        {
            string[] pktQs = {"qslot 0", "qslot 1"};
            var morph = Morph;
            if (Class == ClassType.MartialArtist && Morph == 29 || Morph == 30)
            {
                morph = 30;
            }


            switch (Class)
            {
                case ClassType.MartialArtist when Morph == 31 && UseSp && SpInstance != null && SpInstance.SpLevel >= 20 && HasBuff(CardType.LotusSkills,(byte) AdditionalTypes.LotusSkills.ChangeLotusSkills):
                    GenerateQuickListSp2Am(ref pktQs);
                    break;

                case ClassType.MartialArtist when Morph == 33 && UseSp && SpInstance != null && SpInstance.SpLevel >= 20 && HasBuff(CardType.WolfMaster,(byte) AdditionalTypes.WolfMaster.CanExecuteUltimateSkills):
                    GenerateQuickListSp3Am(ref pktQs);
                    break;

                default:
                {
                    for (var i = 0; i < 30; i++)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            QuicklistEntryDTO qi = QuicklistEntries.Find(n =>
                                n.Q1 == j && n.Q2 == i && n.Morph == (UseSp ? SpInstance.Item.Morph : 0));
                            pktQs[j] += $" {qi?.Type ?? 7}.{qi?.Slot ?? 7}.{qi?.Pos.ToString() ?? "-1"}";
                        }
                    }

                    break;
                }
            }

            return pktQs;
        }

        public string GenerateRaid(int Type, bool exit = false)
        {
            string result = "";
            switch (Type)
            {
                case 0:
                    result = "raid 0";
                    Group?.Sessions?.ForEach(s => result += $" {s.Character?.CharacterId}");
                    break;

                case 2:
                    result = $"raid 2 {(exit ? "-1" : $"{Group?.Sessions?.FirstOrDefault().Character.CharacterId}")}";
                    break;

                case 1:
                    result = $"raid 1 {(exit ? 0 : 1)}";
                    break;

                case 3:
                    result = "raid 3"; 
                    Group?.Sessions?.ForEach(s => result += $" {s.Character?.CharacterId}.{Math.Ceiling(s.Character.Hp / s.Character.HPLoad() * 100)}.{Math.Ceiling(s.Character.Mp / s.Character.MPLoad() * 100)}");
                    break;

                case 4:
                    result = "raid 4";
                    break;

                case 5:
                    result = "raid 5 1";
                    break;
            }

            return result;
        }

        public string GenerateRc(int characterHealth) => BattleEntity.GenerateRc(characterHealth);

        public string GenerateRCSList(CSListPacket packet)
        {
            string list = "";
            BazaarItemLink[] billist = new BazaarItemLink[ServerManager.Instance.BazaarList.Count + 20];
            ServerManager.Instance.BazaarList.CopyTo(billist);
            foreach (BazaarItemLink bz in billist.Where(s => s != null && (s.BazaarItem.DateStart.AddHours(s.BazaarItem.Duration).AddDays(s.BazaarItem.MedalUsed ? 30 : 7) - DateTime.Now).TotalMinutes > 0 && s.BazaarItem.SellerId == CharacterId).Skip(packet.Index * 30).Take(30))
            {
                if (bz.Item != null)
                {
                    int soldedAmount = bz.BazaarItem.Amount - bz.Item.Amount;
                    int amount = bz.BazaarItem.Amount;
                    bool package = bz.BazaarItem.IsPackage;
                    bool isNosbazar = bz.BazaarItem.MedalUsed;
                    long price = bz.BazaarItem.Price;
                    long minutesLeft = (long)(bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration) - DateTime.Now).TotalMinutes;
                    byte Status = minutesLeft >= 0 ? (soldedAmount < amount ? (byte)BazaarType.OnSale : (byte)BazaarType.Solded) : (byte)BazaarType.DelayExpired;
                    if (Status == (byte)BazaarType.DelayExpired)
                    {
                        minutesLeft = (long)(bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration).AddDays(isNosbazar ? 30 : 7) - DateTime.Now).TotalMinutes;
                    }
                    string info = "";
                    if (bz.Item.Item.Type == InventoryType.Equipment)
                    {
                        // Dup shell
                        //bz.Item.ShellEffects.Clear();
                        //bz.Item.ShellEffects.AddRange(DAOFactory.ShellEffectDAO.LoadByEquipmentSerialId(bz.Item.EquipmentSerialId));
                        info = bz.Item?.GenerateEInfo().Replace(' ', '^').Replace("e_info^", "");
                    }
                    if (packet.Filter == 0 || packet.Filter == Status)
                    {
                        list += $"{bz.BazaarItem.BazaarItemId}|{bz.BazaarItem.SellerId}|{bz.Item.ItemVNum}|{soldedAmount}|{amount}|{(package ? 1 : 0)}|{price}|{Status}|{minutesLeft}|{(isNosbazar ? 1 : 0)}|0|{bz.Item.Rare}|{bz.Item.Upgrade}|0|0|{info} ";
                    }
                }
            }

            return $"rc_slist {packet.Index} {list}";
        }

        public string GenerateReqInfo()
        {
            ItemInstance fairy = null;
            ItemInstance armor = null;
            ItemInstance weapon2 = null;
            ItemInstance weapon = null;

            if (Inventory != null)
            {
                fairy = Inventory.LoadBySlotAndType((byte) EquipmentType.Fairy, InventoryType.Wear);
                armor = Inventory.LoadBySlotAndType((byte) EquipmentType.Armor, InventoryType.Wear);
                weapon2 = Inventory.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon, InventoryType.Wear);
                weapon = Inventory.LoadBySlotAndType((byte) EquipmentType.MainWeapon, InventoryType.Wear);
            }

            bool isPvpPrimary = false;
            bool isPvpSecondary = false;
            bool isPvpArmor = false;

            if (weapon != null && !string.IsNullOrEmpty(weapon.Item.Name) && weapon.Item.Name.Contains(": "))
            {
                isPvpPrimary = true;
            }

            isPvpSecondary |= weapon2?.Item.Name.Contains(": ") == true;
            isPvpArmor |= armor?.Item.Name.Contains(": ") == true;

            return $"tc_info {Name} ({Level}) {fairy?.Item.Element ?? 0} {ElementRate} {(byte) Class} {(byte) Gender} {(Family != null ? $"{Family.FamilyId} {Family.Name}({Language.Instance.GetMessageFromKey(FamilyCharacter.Authority.ToString().ToUpper())})" : "-1 -")} {GetReputationIco()} {GetDignityIco()} {(weapon != null ? 1 : 0)} {weapon?.Rare ?? 0} {weapon?.Upgrade ?? 0} {(weapon2 != null ? 1 : 0)} {weapon2?.Rare ?? 0} {weapon2?.Upgrade ?? 0} {(armor != null ? 1 : 0)} {armor?.Rare ?? 0} {armor?.Upgrade ?? 0} {Act4Kill} {Act4Dead} {Reputation} 0 0 0 {(UseSp ? Morph : 0)} {TalentWin} {TalentLose} {TalentSurrender} 0 {MasterPoints} {Compliment} {Act4Points} {(isPvpPrimary ? 1 : 0)} {(isPvpSecondary ? 1 : 0)} {(isPvpArmor ? 1 : 0)} {HeroLevel} {(string.IsNullOrEmpty(Biography) ? Language.Instance.GetMessageFromKey("NO_PREZ_MESSAGE") : Biography)}";
        }

        public string GenerateRest() => $"rest 1 {CharacterId} {(IsSitting ? 1 : 0)}";

        public string GenerateRevive()
        {
            int lives = MapInstance.InstanceBag.Lives - MapInstance.InstanceBag.DeadList.Count + 1;
            if (MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance)
            {
                lives = MapInstance.InstanceBag.Lives - MapInstance.InstanceBag.DeadList.ToList().Count(s => s == CharacterId) + 1;
            }

            return $"revive 1 {CharacterId} {(lives > 0 ? lives : 0)}";
        }

        public string GenerateSay(string message, int type, bool ignoreNickname = false) => $"say {(ignoreNickname ? 2 : 1)} {CharacterId} {type} {message}";


        public string GenerateSayItem(string message, int type, byte itemInventory, short itemSlot,
            bool ignoreNickname = false)
        {
            if (Inventory.LoadBySlotAndType(itemSlot, (InventoryType) itemInventory) is ItemInstance item)
            {
                return $"sayitem {(ignoreNickname ? 2 : 1)} {CharacterId} {type} {message.Replace(' ', '^')} {(item.Item.EquipmentSlot == EquipmentType.Sp ? item.GenerateSlInfo() : item.GenerateEInfo())}";
            }

            return "";
        }

        public string GenerateSayTime() => $"say 1 {CharacterId} 20  Date: {DateTime.Now.Day:00}/{DateTime.Now.Month:00}/{DateTime.Now.Year:00} - Hour: {DateTime.Now.Hour:00}:{DateTime.Now.Minute:00} Channel: {Channel.ChannelId}";

        public string GenerateScal() => $"char_sc 1 {CharacterId} {Size}";

        public List<string> GenerateScN()
        {
            List<string> list = new List<string>();
            byte i = 0;
            var partners = Mates.Where(s => s.MateType == MateType.Partner).ToList();

            foreach (var partner in partners)
            {
                partner.PetId = i;
                partner.LoadInventory();
                list.Add(partner.GenerateScPacket());
                i++;
            }

            return list;
        }

        public List<string> GenerateScP(byte page = 0)
        {
            List<string> list = new List<string>();

            byte i = 0;

            Mates.Where(s => s.MateType == MateType.Pet).Skip(page * 10).Take(10).ToList().ForEach(s =>
            {
                s.PetId = (byte) (page * 10 + i);
                list.Add(s.GenerateScPacket());
                i++;
            });

            return list;
        }

        public string GenerateScpStc() => $"sc_p_stc {(MaxMateCount - 10) / 10} {MaxPartnerCount - 3}";

        public string GenerateShop(string shopname) => $"shop 1 {CharacterId} 1 3 0 {shopname}";

        public string GenerateShopEnd() => $"shop 1 {CharacterId} 0 0";

        public string GenerateSki()
        {
            string ski = "ski";

            List<CharacterSkill> skills = GetSkills().OrderBy(s => s.Skill.CastId).OrderBy(s => s.SkillVNum < 200).ToList();

            if (skills.Count >= 2)
            {
                if (UseSp)
                {
                    ski += $" {skills[0].SkillVNum} {skills[0].SkillVNum}";
                }
                else
                {
                    ski += $" {skills[0].SkillVNum} {skills[1].SkillVNum}";
                }

                ski = skills.Aggregate(ski,(packet, characterSKill) => $"{packet} {(characterSKill.IsTattoo ? $"{characterSKill.SkillVNum}|{characterSKill.TattooLevel}" : $"{characterSKill.SkillVNum}")}");
            }

            return ski;
        }

        public string GenerateSpk(object message, int type) => $"spk 1 {CharacterId} {type} {Name} {message}";

        public string GenerateSpPoint() => $"sp {SpAdditionPoint} 1000000 {SpPoint} 10000";

        [Obsolete(
            "GenerateStartupInventory should be used only on startup, for refreshing an inventory slot please use GenerateInventoryAdd instead.")]
        public void GenerateStartupInventory()
        {
            string inv0 = "inv 0",
                inv1 = "inv 1",
                inv2 = "inv 2",
                inv3 = "inv 3",
                inv6 = "inv 6",
                inv7 = "inv 7"; // inv 3 used for miniland objects
            if (Inventory != null)
            {
                foreach (ItemInstance inv in Inventory.GetAllItems())
                {
                    switch (inv.Type)
                    {
                        case InventoryType.Equipment:
                            if (inv.Item.EquipmentSlot == EquipmentType.Sp)
                            {
                                inv0 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{inv.Upgrade}.{inv.SpStoneUpgrade}.0";
                            }
                            else
                            {
                                inv0 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{(inv.Item.IsColored ? inv.Design : inv.Upgrade)}.0.{inv.RuneAmount}";
                            }

                            break;

                        case InventoryType.Main:
                            inv1 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}.0";
                            break;

                        case InventoryType.Etc:
                            inv2 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}.0";
                            break;

                        case InventoryType.Miniland:
                            inv3 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}";
                            break;

                        case InventoryType.Specialist:
                            inv6 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{inv.Upgrade}.{inv.SpStoneUpgrade}";
                            break;

                        case InventoryType.Costume:
                            inv7 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Rare}.{inv.Upgrade}.0";
                            break;
                    }
                }
            }

            Session.SendPacket(inv0);
            Session.SendPacket(inv1);
            Session.SendPacket(inv2);
            Session.SendPacket(inv3);
            Session.SendPacket(inv6);
            Session.SendPacket(inv7);
            Session.SendPacket(GetMinilandObjectList());
        }

        public string GenerateStashAll()
        {
            string stash = $"stash_all {WareHouseSize}";
            foreach (ItemInstance item in Inventory.Where(s => s.Type == InventoryType.Warehouse))
            {
                stash += $" {item.GenerateStashPacket()}";
            }

            return stash;
        }

        public int GetTitleEffectValue(CardType type, byte subtype)
        {
            return EffectFromTitle?.Where(x => x.Type == (byte) type && x.SubType == subtype) ?.Sum(x => x.FirstData) ?? 0;
        }

        public string GenerateStat()
        {
            double option =
                (WhisperBlocked ? Math.Pow(2, (int) CharacterOption.WhisperBlocked - 1) : 0)
                + (FamilyRequestBlocked ? Math.Pow(2, (int) CharacterOption.FamilyRequestBlocked - 1) : 0)
                + (!MouseAimLock ? Math.Pow(2, (int) CharacterOption.MouseAimLock - 1) : 0)
                + (MinilandInviteBlocked ? Math.Pow(2, (int) CharacterOption.MinilandInviteBlocked - 1) : 0)
                + (ExchangeBlocked ? Math.Pow(2, (int) CharacterOption.ExchangeBlocked - 1) : 0)
                + (FriendRequestBlocked ? Math.Pow(2, (int) CharacterOption.FriendRequestBlocked - 1) : 0)
                + (EmoticonsBlocked ? Math.Pow(2, (int) CharacterOption.EmoticonsBlocked - 1) : 0)
                + (HpBlocked ? Math.Pow(2, (int) CharacterOption.HpBlocked - 1) : 0)
                + (BuffBlocked ? Math.Pow(2, (int) CharacterOption.BuffBlocked - 1) : 0)
                + (GroupRequestBlocked ? Math.Pow(2, (int) CharacterOption.GroupRequestBlocked - 1) : 0)
                + (HeroChatBlocked ? Math.Pow(2, (int) CharacterOption.HeroChatBlocked - 1) : 0)
                + (QuickGetUp ? Math.Pow(2, (int) CharacterOption.QuickGetUp - 1) : 0)
                + (HideHat ? Math.Pow(2, (int) CharacterOption.HideHat - 1) : 0)
                + (UiBlocked ? Math.Pow(2, (int) CharacterOption.UiBlocked - 1) : 0)
                + (!IsPetAutoRelive ? 64 : 0)
                + (!IsPartnerAutoRelive ? 128 : 0);
            return $"stat {Hp} {HPLoad()} {Mp} {MPLoad()} 0 {option}";
        }

        public List<string> GenerateStatChar()
        {
            int weaponUpgrade = 0;
            int secondaryUpgrade = 0;
            int armorUpgrade = 0;
            MinHit = CharacterHelper.MinHit(Class, Level);
            MaxHit = CharacterHelper.MaxHit(Class, Level);
            HitRate = CharacterHelper.HitRate(Class, Level);
            HitCriticalChance = CharacterHelper.HitCriticalRate(Class, Level);
            HitCriticalRate = CharacterHelper.HitCritical(Class, Level);
            SecondWeaponMinHit = CharacterHelper.MinDistance(Class, Level);
            SecondWeaponMaxHit = CharacterHelper.MaxDistance(Class, Level);
            SecondWeaponHitRate = CharacterHelper.DistanceRate(Class, Level);
            SecondWeaponCriticalChance = CharacterHelper.DistCriticalRate(Class, Level);
            SecondWeaponCriticalRate = CharacterHelper.DistCritical(Class, Level);
            FireResistance = 0;
            LightResistance = 0;
            WaterResistance = 0;
            DarkResistance = 0;
            Defence = CharacterHelper.Defence(Class, Level);
            DefenceRate = CharacterHelper.DefenceRate(Class, Level);
            ElementRate = 0;
            ElementRateSP = 0;
            DistanceDefence = CharacterHelper.DistanceDefence(Class, Level);
            DistanceDefenceRate = CharacterHelper.DistanceDefenceRate(Class, Level);
            MagicalDefence = CharacterHelper.MagicalDefence(Class, Level);
            if (UseSp)
            {
                // handle specialist
                ItemInstance specialist = Inventory?.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                if (specialist != null)
                {
                    MinHit += specialist.DamageMinimum + (specialist.SpDamage * 10);
                    MaxHit += specialist.DamageMaximum + (specialist.SpDamage * 10);
                    SecondWeaponMinHit += specialist.DamageMinimum + (specialist.SpDamage * 10);
                    SecondWeaponMaxHit += specialist.DamageMaximum + (specialist.SpDamage * 10);
                    HitCriticalChance += specialist.CriticalLuckRate;
                    HitCriticalRate += specialist.CriticalRate;
                    SecondWeaponCriticalChance += specialist.CriticalLuckRate;
                    SecondWeaponCriticalRate += specialist.CriticalRate;
                    HitRate += specialist.HitRate;
                    SecondWeaponHitRate += specialist.HitRate;
                    DefenceRate += specialist.DefenceDodge;
                    DistanceDefenceRate += specialist.DistanceDefenceDodge;
                    FireResistance += specialist.Item.FireResistance + specialist.SpFire;
                    WaterResistance += specialist.Item.WaterResistance + specialist.SpWater;
                    LightResistance += specialist.Item.LightResistance + specialist.SpLight;
                    DarkResistance += specialist.Item.DarkResistance + specialist.SpDark;
                    ElementRateSP += specialist.ElementRate + specialist.SpElement;
                    Defence += specialist.CloseDefence + (specialist.SpDefence * 10);
                    DistanceDefence += specialist.DistanceDefence + (specialist.SpDefence * 10);
                    MagicalDefence += specialist.MagicDefence + (specialist.SpDefence * 10);

                    ItemInstance mainWeapon = Inventory.LoadBySlotAndType((byte) EquipmentType.MainWeapon, InventoryType.Wear);
                    ItemInstance secondaryWeapon = Inventory.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon, InventoryType.Wear);
                    List<ShellEffectDTO> effects = new List<ShellEffectDTO>();
                    if (mainWeapon?.ShellEffects != null)
                    {
                        effects.AddRange(mainWeapon.ShellEffects);
                    }

                    if (secondaryWeapon?.ShellEffects != null)
                    {
                        effects.AddRange(secondaryWeapon.ShellEffects);
                    }

                    int GetShellWeaponEffectValue(ShellWeaponEffectType effectType)
                    {
                        return effects?.Where(s => s.Effect == (byte) effectType)?.OrderByDescending(s => s.Value) ?.FirstOrDefault()?.Value ?? 0;
                    }


                    int point = CharacterHelper.SlPoint(specialist.SlDamage, 0) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLDamage) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) + GetTitleEffectValue(CardType.IncreaseSlPoint,(byte) AdditionalTypes.IncreaseSlPoint.IncreaseDamage);
                    if (point > 100)
                    {
                        point = 100;
                    }

                    ;

                    int p = 0;
                    if (point <= 10)
                    {
                        p = point * 5;
                    }
                    else if (point <= 20)
                    {
                        p = 50 + ((point - 10) * 6);
                    }
                    else if (point <= 30)
                    {
                        p = 110 + ((point - 20) * 7);
                    }
                    else if (point <= 40)
                    {
                        p = 180 + ((point - 30) * 8);
                    }
                    else if (point <= 50)
                    {
                        p = 260 + ((point - 40) * 9);
                    }
                    else if (point <= 60)
                    {
                        p = 350 + ((point - 50) * 10);
                    }
                    else if (point <= 70)
                    {
                        p = 450 + ((point - 60) * 11);
                    }
                    else if (point <= 80)
                    {
                        p = 560 + ((point - 70) * 13);
                    }
                    else if (point <= 90)
                    {
                        p = 690 + ((point - 80) * 14);
                    }
                    else if (point <= 94)
                    {
                        p = 830 + ((point - 90) * 15);
                    }
                    else if (point <= 95)
                    {
                        p = 890 + 16;
                    }
                    else if (point <= 97)
                    {
                        p = 906 + ((point - 95) * 17);
                    }
                    else if (point > 97)
                    {
                        p = 940 + ((point - 97) * 20);
                    }

                    MaxHit += p;
                    MinHit += p;
                    SecondWeaponMaxHit += p;
                    SecondWeaponMinHit += p;

                    point = CharacterHelper.SlPoint(specialist.SlDefence, 1) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLDefence) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) + GetTitleEffectValue(CardType.IncreaseSlPoint,(byte) AdditionalTypes.IncreaseSlPoint.IncreaseDefence);
                    if (point > 100)
                    {
                        point = 100;
                    }

                    p = 0;
                    if (point <= 10)
                    {
                        p = point;
                    }
                    else if (point <= 20)
                    {
                        p = 10 + ((point - 10) * 2);
                    }
                    else if (point <= 30)
                    {
                        p = 30 + ((point - 20) * 3);
                    }
                    else if (point <= 40)
                    {
                        p = 60 + ((point - 30) * 4);
                    }
                    else if (point <= 50)
                    {
                        p = 100 + ((point - 40) * 5);
                    }
                    else if (point <= 60)
                    {
                        p = 150 + ((point - 50) * 6);
                    }
                    else if (point <= 70)
                    {
                        p = 210 + ((point - 60) * 7);
                    }
                    else if (point <= 80)
                    {
                        p = 280 + ((point - 70) * 8);
                    }
                    else if (point <= 90)
                    {
                        p = 360 + ((point - 80) * 9);
                    }
                    else if (point > 90)
                    {
                        p = 450 + ((point - 90) * 10);
                    }

                    Defence += p;
                    MagicalDefence += p;
                    DistanceDefence += p;

                    point = CharacterHelper.SlPoint(specialist.SlElement, 2) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLElement) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) + GetTitleEffectValue(CardType.IncreaseSlPoint,(byte) AdditionalTypes.IncreaseSlPoint.IncreaseEllement);
                    if (point > 100)
                    {
                        point = 100;
                    }

                    ;

                    p = point <= 50 ? point : 50 + ((point - 50) * 2);
                    ElementRateSP += p;

                    slhpbonus = GetShellWeaponEffectValue(ShellWeaponEffectType.SLHP) + GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) + GetTitleEffectValue(CardType.IncreaseSlPoint,(byte) AdditionalTypes.IncreaseSlPoint.IncreaseHPMP);
                }
            }

            // TODO: add base stats
            ItemInstance weapon = Inventory?.LoadBySlotAndType((byte) EquipmentType.MainWeapon, InventoryType.Wear);
            if (weapon != null)
            {
                weaponUpgrade = weapon.Upgrade;
                MinHit += weapon.DamageMinimum + weapon.Item.DamageMinimum;
                MaxHit += weapon.DamageMaximum + weapon.Item.DamageMaximum;
                HitRate += weapon.HitRate + weapon.Item.HitRate;
                HitCriticalChance += weapon.CriticalLuckRate + weapon.Item.CriticalLuckRate;
                HitCriticalRate += weapon.CriticalRate + weapon.Item.CriticalRate;

                // maxhp-mp
            }

            ItemInstance weapon2 = Inventory?.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon, InventoryType.Wear);
            if (weapon2 != null)
            {
                secondaryUpgrade = weapon2.Upgrade;
                SecondWeaponMinHit += weapon2.DamageMinimum + weapon2.Item.DamageMinimum;
                SecondWeaponMaxHit += weapon2.DamageMaximum + weapon2.Item.DamageMaximum;
                SecondWeaponHitRate += weapon2.HitRate + weapon2.Item.HitRate;
                SecondWeaponCriticalChance += weapon2.CriticalLuckRate + weapon2.Item.CriticalLuckRate;
                SecondWeaponCriticalRate += weapon2.CriticalRate + weapon2.Item.CriticalRate;

                // maxhp-mp
            }

            ItemInstance armor = Inventory?.LoadBySlotAndType((byte) EquipmentType.Armor, InventoryType.Wear);
            if (armor != null)
            {
                armorUpgrade = armor.Upgrade;
                Defence += armor.CloseDefence + armor.Item.CloseDefence;
                DefenceRate += armor.DefenceDodge + armor.Item.DefenceDodge;
                MagicalDefence += armor.MagicDefence + armor.Item.MagicDefence;
                DistanceDefence += armor.DistanceDefence + armor.Item.DistanceDefence;
                DistanceDefenceRate += armor.DistanceDefenceDodge + armor.Item.DistanceDefenceDodge;
            }

            ItemInstance fairy = Inventory?.LoadBySlotAndType((byte) EquipmentType.Fairy, InventoryType.Wear);
            if (fairy != null)
            {
                ElementRate += fairy.ElementRate + fairy.Item.ElementRate + (IsUsingFairyBooster == 1 ? 30 : IsUsingFairyBooster == 2 ? 60 : 0) + GetStuffBuff(CardType.PixieCostumeWings,(byte) AdditionalTypes.PixieCostumeWings.IncreaseFairyElement)[0];
            }

            for (short i = 1; i < 14; i++)
            {
                ItemInstance item = Inventory?.LoadBySlotAndType(i, InventoryType.Wear);
                if (item != null && item.Item.EquipmentSlot != EquipmentType.MainWeapon
                                 && item.Item.EquipmentSlot != EquipmentType.SecondaryWeapon
                                 && item.Item.EquipmentSlot != EquipmentType.Armor
                                 && item.Item.EquipmentSlot != EquipmentType.Sp)
                {
                    FireResistance += item.FireResistance + item.Item.FireResistance;
                    LightResistance += item.LightResistance + item.Item.LightResistance;
                    WaterResistance += item.WaterResistance + item.Item.WaterResistance;
                    DarkResistance += item.DarkResistance + item.Item.DarkResistance;
                    Defence += item.CloseDefence + item.Item.CloseDefence;
                    DefenceRate += item.DefenceDodge + item.Item.DefenceDodge;
                    MagicalDefence += item.MagicDefence + item.Item.MagicDefence;
                    DistanceDefence += item.DistanceDefence + item.Item.DistanceDefence;
                    DistanceDefenceRate += item.DistanceDefenceDodge + item.Item.DistanceDefenceDodge;
                }
            }

            //BCards
            int BCardFireResistance = GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.FireIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.AllIncreased)[0];
            int BCardLightResistance = GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.LightIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.AllIncreased)[0];
            int BCardWaterResistance = GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.WaterIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.AllIncreased)[0];
            int BCardDarkResistance = GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.DarkIncreased)[0] + GetStuffBuff(CardType.ElementResistance, (byte) AdditionalTypes.ElementResistance.AllIncreased)[0];

            int BCardHitCritical = GetStuffBuff(CardType.Critical, (byte) AdditionalTypes.Critical.DamageIncreased)[0] + GetStuffBuff(CardType.Critical,(byte) AdditionalTypes.Critical.DamageFromCriticalIncreased)[0];
            int BCardHitCriticalRate = GetStuffBuff(CardType.Critical, (byte) AdditionalTypes.Critical.InflictingIncreased)[0];

            int BCardHit = GetStuffBuff(CardType.AttackPower, (byte) AdditionalTypes.AttackPower.AllAttacksIncreased)[0];
            int BCardSecondHit = GetStuffBuff(CardType.AttackPower, (byte) AdditionalTypes.AttackPower.AllAttacksIncreased)[0];

            int BCardHitRate = GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.AllHitRateIncreased)[0];
            int BCardSecondHitRate = GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.AllHitRateIncreased)[0];

            int BCardMeleeDodge = GetStuffBuff(CardType.DodgeAndDefencePercent,(byte) AdditionalTypes.Target.AllHitRateIncreased)[0];
            int BCardRangeDodge = GetStuffBuff(CardType.DodgeAndDefencePercent,(byte) AdditionalTypes.Target.AllHitRateIncreased)[0];

            int BCardMeleeDefence = GetStuffBuff(CardType.Defence, (byte) AdditionalTypes.Defence.AllIncreased)[0] +GetStuffBuff(CardType.Defence, (byte) AdditionalTypes.Defence.MeleeIncreased)[0];
            int BCardRangeDefence = GetStuffBuff(CardType.Defence, (byte) AdditionalTypes.Defence.AllIncreased)[0] +GetStuffBuff(CardType.Defence, (byte) AdditionalTypes.Defence.RangedIncreased)[0];
            int BCardMagicDefence = GetStuffBuff(CardType.Defence, (byte) AdditionalTypes.Defence.AllIncreased)[0] +GetStuffBuff(CardType.Defence, (byte) AdditionalTypes.Defence.MagicalIncreased)[0];

            switch (Class)
            {
                case ClassType.Adventurer:
                case ClassType.Swordsman:
                    BCardHit += GetStuffBuff(CardType.AttackPower,(byte) AdditionalTypes.AttackPower.MeleeAttacksIncreased)[0];
                    BCardSecondHit += GetStuffBuff(CardType.AttackPower,(byte) AdditionalTypes.AttackPower.RangedAttacksIncreased)[0];
                    BCardHitRate += GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.MeleeHitRateIncreased)[0];
                    BCardSecondHitRate += GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.RangedHitRateIncreased)[0];
                    break;

                case ClassType.Archer:
                    BCardHit += GetStuffBuff(CardType.AttackPower,(byte) AdditionalTypes.AttackPower.RangedAttacksIncreased)[0];
                    BCardSecondHit += GetStuffBuff(CardType.AttackPower,(byte) AdditionalTypes.AttackPower.MeleeAttacksIncreased)[0];
                    BCardHitRate += GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.RangedHitRateIncreased)[0];
                    BCardSecondHitRate += GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.MeleeHitRateIncreased)[0];
                    break;

                case ClassType.Magician:
                    BCardHit += GetStuffBuff(CardType.AttackPower,(byte) AdditionalTypes.AttackPower.MagicalAttacksIncreased)[0];
                    BCardSecondHit += GetStuffBuff(CardType.AttackPower,(byte) AdditionalTypes.AttackPower.RangedAttacksIncreased)[0];
                    BCardHitRate += GetStuffBuff(CardType.Target,(byte) AdditionalTypes.Target.MagicalConcentrationIncreased)[0];
                    BCardSecondHitRate += GetStuffBuff(CardType.Target, (byte) AdditionalTypes.Target.RangedHitRateIncreased)[0];
                    break;
            }

            byte type = Class == ClassType.Adventurer ? (byte) 0 : (byte) (Class - 1);

            List<string> packets = new List<string>();
            packets.Add( $"sc {type} {(weaponUpgrade == 10 ? weaponUpgrade : weaponUpgrade + GetBuff(CardType.AttackPower, (byte) AdditionalTypes.AttackPower.AttackLevelIncreased)[0])} {MinHit + BCardHit} {MaxHit + BCardHit} {HitRate + BCardHitRate} {HitCriticalChance + BCardHitCriticalRate} {HitCriticalRate + BCardHitCritical} {(Class == ClassType.Archer ? 1 : 0)} {(secondaryUpgrade == 10 ? secondaryUpgrade : secondaryUpgrade + GetBuff(CardType.AttackPower, (byte) AdditionalTypes.AttackPower.AttackLevelIncreased)[0])} {SecondWeaponMinHit + BCardSecondHit} {SecondWeaponMaxHit + BCardSecondHit} {SecondWeaponHitRate + BCardSecondHitRate} {SecondWeaponCriticalChance + BCardHitCriticalRate} {SecondWeaponCriticalRate + BCardHitCritical} {(armorUpgrade == 10 ? armorUpgrade : armorUpgrade + GetBuff(CardType.Defence, (byte) AdditionalTypes.Defence.DefenceLevelIncreased)[0])} {Defence + BCardMeleeDefence} {DefenceRate + BCardMeleeDodge} {DistanceDefence + BCardRangeDefence} {DistanceDefenceRate + BCardRangeDodge} {MagicalDefence + BCardMagicDefence} {FireResistance + BCardFireResistance} {WaterResistance + BCardWaterResistance} {LightResistance + BCardLightResistance} {DarkResistance + BCardDarkResistance}");
            packets.AddRange(GenerateScN());
            packets.AddRange(GenerateScP());

            LoadSpeed();

            return packets;
        }

        public string GenerateStatInfo() => $"st 1 {CharacterId} {Level} {HeroLevel} {(int) (Hp / (float) HPLoad() * 100)} {(int) (Mp / (float) MPLoad() * 100)} {Hp} {Mp}{Buff.GetAllItems().Where(s => !s.StaticBuff || new short[] {339, 340}.Contains(s.Card.CardId)).Aggregate("", (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}";

        public string GenerateTaF(byte victoriousteam)
        {
            ConcurrentBag<ArenaTeamMember> tm = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(o => o.Session == Session));
            var score1 = 0;
            var score2 = 0;
            var life1 = 0;
            var life2 = 0;
            var call1 = 0;
            var call2 = 0;
            var atype = ArenaTeamType.ERENIA;
            if (tm == null)
            {
                return $"ta_f 0 {victoriousteam} {(byte) atype} {score1} {life1} {call1} {score2} {life2} {call2}";
            }

            var tmem = tm.FirstOrDefault(s => s.Session == Session);
            if (tmem == null)
            {
                return $"ta_f 0 {victoriousteam} {(byte) atype} {score1} {life1} {call1} {score2} {life2} {call2}";
            }

            atype = tmem.ArenaTeamType;
            IEnumerable<long> ids = tm.Replace(s => tmem.ArenaTeamType == s.ArenaTeamType).Select(s => s.Session.Character.CharacterId);
            ConcurrentBag<ArenaTeamMember> oposit = tm.Replace(s => tmem.ArenaTeamType != s.ArenaTeamType);
            ConcurrentBag<ArenaTeamMember> own = tm.Replace(s => tmem.ArenaTeamType == s.ArenaTeamType);
            score1 = 3 - MapInstance.InstanceBag.DeadList.Count(s => ids.Contains(s));
            score2 = 3 - MapInstance.InstanceBag.DeadList.Count(s => !ids.Contains(s));
            life1 = 3 - own.Count(s => s.Dead);
            life2 = 3 - oposit.Count(s => s.Dead);
            call1 = 5 - own.Sum(s => s.SummonCount);
            call2 = 5 - oposit.Sum(s => s.SummonCount);
            return $"ta_f 0 {victoriousteam} {(byte) atype} {score1} {life1} {call1} {score2} {life2} {call2}";
        }

        public string GenerateTaFc(byte type) => $"ta_fc {type} {CharacterId}";

        public TalkPacket GenerateTalk(string message)
        {
            return new TalkPacket
            {
                CharacterId = CharacterId,
                Message = message
            };
        }

        public string GenerateTaM(int type)
        {
            ConcurrentBag<ArenaTeamMember> tm = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(o => o.Session == Session));
            var score1 = 0;
            var score2 = 0;
            if (tm == null)
            {
                return $"ta_m {type} {score1} {score2} {(type == 3 ? MapInstance.InstanceBag.Clock.SecondsRemaining / 10 : 0)} 0";
            }

            var tmem = tm.FirstOrDefault(s => s.Session == Session);
            IEnumerable<long> ids = tm.Replace(s => tmem != null && tmem.ArenaTeamType != s.ArenaTeamType).Select(s => s.Session.Character.CharacterId);
            score1 = MapInstance.InstanceBag.DeadList.Count(s => ids.Contains(s));
            score2 = MapInstance.InstanceBag.DeadList.Count(s => !ids.Contains(s));
            return $"ta_m {type} {score1} {score2} {(type == 3 ? MapInstance.InstanceBag.Clock.SecondsRemaining / 10 : 0)} 0";
        }

        public string GenerateTaP(byte tatype, bool showOponent)
        {
            List<ArenaTeamMember> arenateam = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s != null && s.Any(o => o != null && o.Session == Session)) ?.OrderBy(s => s.ArenaTeamType).ToList();
            var type = ArenaTeamType.ERENIA;
            var groups = "";
            if (arenateam == null)
            {
                return $"ta_p {tatype} {(byte) type} {5} {5} {groups.TrimEnd(' ')}";
            }

            type = arenateam.FirstOrDefault(s => s.Session == Session)?.ArenaTeamType ?? ArenaTeamType.ERENIA;

            List<ArenaTeamMember> MyTeam = arenateam.Where(s => s.ArenaTeamType == type && s.Order != null).ToList();
            List<ArenaTeamMember> EnemyTeam = arenateam.Where(s => s.ArenaTeamType != type && s.Order != null).ToList();

            for (int i = 0; i < 3; i++)
            {
                if (MyTeam.Where(s => s.Order == i).FirstOrDefault() is ArenaTeamMember arenamember)
                {
                    groups += $"{(arenamember.Dead ? 0 : 1)}.{arenamember.Session.Character.CharacterId}.{(byte) arenamember.Session.Character.Class}.{(byte) arenamember.Session.Character.Gender}.{(byte) arenamember.Session.Character.Morph} ";
                }
                else
                {
                    groups += $"-1.-1.-1.-1.-1 ";
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (EnemyTeam.Where(s => s.Order == i).FirstOrDefault() is ArenaTeamMember arenamember && showOponent)
                {
                    groups += $"{(arenamember.Dead ? 0 : 1)}.{arenamember.Session.Character.CharacterId}.{(byte) arenamember.Session.Character.Class}.{(byte) arenamember.Session.Character.Gender}.{(byte) arenamember.Session.Character.Morph} ";
                }
                else
                {
                    groups += $"-1.-1.-1.-1.-1 ";
                }
            }

            return $"ta_p {tatype} {(byte) type} {5 - arenateam.Where(s => s.ArenaTeamType == type).Sum(s => s.SummonCount)} {5 - arenateam.Where(s => s.ArenaTeamType != type).Sum(s => s.SummonCount)} {groups.TrimEnd(' ')}";
        }

        public string GenerateTaPs()
        {
            List<ArenaTeamMember> arenateam = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s != null && s.Any(o => o?.Session == Session))?.OrderBy(s => s.ArenaTeamType).ToList();
            string groups = "";
            if (arenateam == null)
            {
                return $"ta_ps {groups.TrimEnd(' ')}";
            }

            ArenaTeamType type = arenateam.FirstOrDefault(s => s.Session == Session)?.ArenaTeamType ?? ArenaTeamType.ERENIA;

            List<ArenaTeamMember> MyTeam = arenateam.Where(s => s.ArenaTeamType == type && s.Order != null).ToList();
            List<ArenaTeamMember> EnemyTeam = arenateam.Where(s => s.ArenaTeamType != type && s.Order != null).ToList();

            for (int i = 0; i < 3; i++)
            {
                if (MyTeam.Where(s => s.Order == i).FirstOrDefault() is ArenaTeamMember arenamember)
                {
                    groups += $"{arenamember.Session.Character.CharacterId}.{(int) (arenamember.Session.Character.Hp / arenamember.Session.Character.HPLoad() * 100)}.{(int) (arenamember.Session.Character.Mp / arenamember.Session.Character.MPLoad() * 100)}.0 ";
                }
                else
                {
                    groups += $"-1.-1.-1.-1.-1 ";
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (EnemyTeam.Where(s => s.Order == i).FirstOrDefault() is ArenaTeamMember arenamember)
                {
                    groups += $"{arenamember.Session.Character.CharacterId}.{(int) (arenamember.Session.Character.Hp / arenamember.Session.Character.HPLoad() * 100)}.{(int) (arenamember.Session.Character.Mp / arenamember.Session.Character.MPLoad() * 100)}.0 ";
                }
                else
                {
                    groups += $"-1.-1.-1.-1.-1 ";
                }
            }

            return $"ta_ps {groups.TrimEnd(' ')}";
        }

        public string GenerateTit() => $"tit {Language.Instance.GetMessageFromKey(Class == (byte) ClassType.Adventurer ? nameof(ClassType.Adventurer).ToUpper() : Class == ClassType.Swordsman ? nameof(ClassType.Swordsman).ToUpper() : Class == ClassType.Archer ? nameof(ClassType.Archer).ToUpper() : nameof(ClassType.Magician).ToUpper())} {Name}";

        public string GenerateTitInfo()
        {
            long tit = 0;
            long eff = 0;
            if (Title.Find(s => s.Stat.Equals(3)) != null)
            {
                tit = Title.Find(s => s.Stat.Equals(3)).TitleVnum;
            }

            if (Title.Find(s => s.Stat.Equals(7)) != null)
            {
                tit = Title.Find(s => s.Stat.Equals(7)).TitleVnum;
            }

            if (Title.Find(s => s.Stat.Equals(5)) != null)
            {
                eff = Title.Find(s => s.Stat.Equals(5)).TitleVnum;
            }

            return $"titinfo 1 {CharacterId} {tit} {(Title.Find(s => s.Stat.Equals(7)) != null ? tit : eff)}";
        }

        public string GenerateTitle()
        {
            string tit = string.Empty;
            foreach (var t in Title.ToList())
            {
                tit += $"{t.TitleVnum - 9300}.{t.Stat} ";
            }

            return $"title {tit}";
        }

        public string GenerateTp() => BattleEntity.GenerateTp();

        public void GetAct4Points(int point)
        {
            //RefreshComplimentRankingIfNeeded();
            Act4Points += point;
        }

        public int[] GetBuff(CardType type, byte subtype) => BattleEntity.GetBuff(type, subtype);

        public int GetCP()
        {
            int cpmax = (Class > 0 ? 40 : 0) + (JobLevel * 2);
            int cpused = 0;
            foreach (CharacterSkill ski in Skills.GetAllItems())
            {
                cpused += ski.Skill.CPCost;
            }

            return cpmax - cpused;
        }

        public void GetDamage(int damage, BattleEntity damager, bool dontKill = false) => BattleEntity.GetDamage(damage, damager, dontKill);

        public void GetDignity(int amount)
        {
            Dignity += amount;

            if (Dignity > 100)
            {
                Dignity = 100;
            }

            Session.SendPacket(GenerateFd());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, GenerateGidx(), ReceiverType.AllExceptMe);
            Session.SendPacket(GenerateSay($"{Language.Instance.GetMessageFromKey("RESTORE_DIGNITY")} (+{amount})",11));
        }

        public int GetDignityIco()
        {
            int icoDignity = 1;

            if (Dignity <= -100)
            {
                icoDignity = 2;
            }

            if (Dignity <= -200)
            {
                icoDignity = 3;
            }

            if (Dignity <= -400)
            {
                icoDignity = 4;
            }

            if (Dignity <= -600)
            {
                icoDignity = 5;
            }

            if (Dignity <= -800)
            {
                icoDignity = 6;
            }

            return icoDignity;
        }

        public void GetDir(int pX, int pY, int nX, int nY)
        {
            BeforeDirection = Direction;
            if (pX == nX && pY < nY)
            {
                Direction = 2;
            }
            else if (pX > nX && pY == nY)
            {
                Direction = 3;
            }
            else if (pX == nX && pY > nY)
            {
                Direction = 0;
            }
            else if (pX < nX && pY == nY)
            {
                Direction = 1;
            }
            else if (pX < nX && pY < nY)
            {
                Direction = 6;
            }
            else if (pX > nX && pY < nY)
            {
                Direction = 7;
            }
            else if (pX > nX && pY > nY)
            {
                Direction = 4;
            }
            else if (pX < nX && pY > nY)
            {
                Direction = 5;
            }
        }

        public List<Portal> GetExtraPortal() => new List<Portal>(MapInstancePortalHandler.GenerateMinilandEntryPortals(MapInstance.Map.MapId, Miniland.MapInstanceId).Concat(Family?.Act4Raid != null ? (MapInstancePortalHandler.GenerateAct4EntryPortals(MapInstance.Map.MapId)) : new List<Portal>()));

        public List<string> GetFamilyHistory()
        {
            //TODO: Fix some bugs(missing history etc)
            if (Family != null)
            {
                const string packetheader = "ghis";
                List<string> packetList = new List<string>();
                string packet = "";
                int i = 0;
                int amount = 0;
                foreach (FamilyLogDTO log in Family.FamilyLogs.Take(100).OrderByDescending(l => l.Timestamp))
                {
                    packet +=
                        $" {(byte) log.FamilyLogType}|{log.FamilyLogData}|{(int) (DateTime.Now - log.Timestamp).TotalHours}";
                    i++;
                    if (i == 50)
                    {
                        i = 0;
                        packetList.Add(packetheader + (amount == 0 ? " 0 " : "") + packet);
                        amount++;
                    }
                    else if (i + (50 * amount) == Family.FamilyLogs.Count)
                    {
                        packetList.Add(packetheader + (amount == 0 ? " 0 " : "") + packet);
                    }
                }

                return packetList;
            }

            return new List<string>();
        }

        public void GetGold(long val, bool isQuest = false)
        {
            Session.Character.Gold += val;
            if (Session.Character.Gold > ServerManager.Instance.Configuration.MaxGold)
            {
                Session.Character.Gold = ServerManager.Instance.Configuration.MaxGold;
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));
            }

            Session.SendPacket(isQuest ? GenerateSay($"Quest reward: [ {ServerManager.GetItem(1046).Name} x {val} ]", 10) : Session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.GetItem(1046).Name} x {val}",10));
            Session.SendPacket(Session.Character.GenerateGold());
        }

        public void GetHXp(long val, bool applyRate = true)
        {
            if (HeroLevel >= ServerManager.Instance.Configuration.MaxHeroLevel)
            {
                return;
            }

            HeroXp += val * (applyRate ? ServerManager.Instance.Configuration.RateHeroicXP : 1) * (int) (1 + GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] / 100D +  GetBuff(CardType.Dracula, (byte) AdditionalTypes.Dracula.ExpHeroIncrease)[0] / 100D);

            GenerateLevelXpLevelUp();
            Session.SendPacket(GenerateLev());
        }

        public void GetJobExp(long val, bool applyRate = true)
        {
            if (ServerManager.Instance.Configuration.EventXp > 1)
            {
                val = val * ServerManager.Instance.Configuration.EventXp;
            }
            val *= (applyRate ? ServerManager.Instance.Configuration.RateXP : 1);
            ItemInstance SpInstance = null;
            if (Inventory != null)
            {
                SpInstance = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
            }

            if (UseSp && SpInstance != null)
            {
                if (SpInstance.SpLevel >= ServerManager.Instance.Configuration.MaxSPLevel)
                {
                    return;
                }

                int multiplier = SpInstance.SpLevel < 10 ? 10 : SpInstance.SpLevel < 19 ? 5 : 1;
                SpInstance.XP += (int) ((val * (multiplier + GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] /100D + GetBuff(CardType.Item, (byte) AdditionalTypes.Item.IncreaseSPXP)[0] / 100D)));
                GenerateSpXpLevelUp(SpInstance);
                return;
            }

            if (JobLevel >= ServerManager.Instance.Configuration.MaxJobLevel)
            {
                return;
            }

            JobLevelXp += (int) (val * (1 + GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] / 100D));
            GenerateJobXpLevelUp();
            Session.SendPacket(GenerateLev());
        }

        public IEnumerable<string> GetMinilandEffects() => MinilandObjects.Select(mp => mp.GenerateMinilandEffect(false)).ToList();

        public string GetMinilandObjectList()
        {
            string mlobjstring = "mlobjlst";
            foreach (ItemInstance item in Inventory.Where(s => s.Type == InventoryType.Miniland).OrderBy(s => s.Slot))
            {
                MinilandObject mp = MinilandObjects.Find(s => s.ItemInstanceId == item.Id);
                bool used = mp != null;
                mlobjstring += $" {item.Slot}.{(used ? 1 : 0)}.{(used ? mp.MapX : 0)}.{(used ? mp.MapY : 0)}.{(item.Item.Width != 0 ? item.Item.Width : 1)}.{(item.Item.Height != 0 ? item.Item.Height : 1)}.{(used ? mp.ItemInstance.DurabilityPoint : 0)}.100.0.1";
            }

            return mlobjstring;
        }

        public List<long> GetMTListTargetQueue_QuickFix(CharacterSkill ski, UserType entityType)
        {
            List<long> result = new List<long>();

            if (BattleEntity != null && MapInstance != null && ski?.Skill != null)
            {
                foreach (long targetId in MTListTargetQueue.Where(target => target.EntityType == entityType && (byte) target.TargetHitType == ski.Skill.HitType).Select(s => s.TargetId))
                {
                    switch (entityType)
                    {
                        case UserType.Player:
                        {
                            Character targetCharacter = MapInstance.GetCharacterById(targetId);

                            if (targetCharacter?.BattleEntity == null /* Invalid character  */
                                || targetCharacter.Hp < 1 /* Amen */
                                || !targetCharacter.IsInRange(PositionX, PositionY,ski.Skill.Range) /* Character not in range */
                                || !BattleEntity.CanAttackEntity(targetCharacter.BattleEntity) /* Try again later */
                            )
                            {
                                continue;
                            }
                        }
                            break;

                        case UserType.Monster:
                        {
                            MapMonster targetMonster = MapInstance.GetMonsterById(targetId);

                            if (targetMonster?.BattleEntity == null /* Invalid monster */
                                || !targetMonster.IsAlive /* Amen */
                                || targetMonster.CurrentHp < 1 /* Schrödinger's cat */
                                || !targetMonster.IsInRange(PositionX, PositionY,ski.Skill.Range) /* Monster not in range */
                                || !BattleEntity.CanAttackEntity(targetMonster.BattleEntity) /* Try again later */
                            )
                            {
                                continue;
                            }
                        }
                            break;
                    }

                    result.Add(targetId);
                }
            }

            return result;
        }

        public void GetReferrerReward()
        {
            long referrerId = Session.Account.ReferrerId;
            if (Level >= 70 && referrerId != 0 && !CharacterId.Equals(referrerId))
            {
                List<GeneralLogDTO> logs = DAOFactory.GeneralLogDAO.LoadByLogType("ReferralProgram", null).Where(g => g.IpAddress.Equals(Session.Account.RegistrationIP.Split(':')[1].Replace("//", ""))).ToList();
                if (logs.Count <= 5)
                {
                    CharacterDTO character = DAOFactory.CharacterDAO.LoadById(referrerId);
                    if (character == null || character.Level < 70)
                    {
                        return;
                    }

                    AccountDTO referrer = DAOFactory.AccountDAO.LoadById(character.AccountId);
                    if (referrer != null && !AccountId.Equals(character.AccountId))
                    {
                        Logger.LogUserEvent("REFERRERREWARD", Session.GenerateIdentity(),$"AccountId: {AccountId} ReferrerId: {referrerId}");
                        DAOFactory.AccountDAO.WriteGeneralLog(AccountId, Session.Account.RegistrationIP, CharacterId,GeneralLogType.ReferralProgram, $"ReferrerId: {referrerId}");

                        // send gifts like you want
                        //SendGift(CharacterId, 5910, 1, 0, 0, false);
                        //SendGift(referrerId, 5910, 1, 0, 0, false);
                    }
                }
            }
        }

        public void GetReputation(int amount, bool applyRate = true)
        {
            if (ServerManager.Instance.Configuration.EventRep > 1)
            {
                amount *= ServerManager.Instance.Configuration.EventRep;
            }
            long val2 = amount * (amount > 0 && applyRate ? ServerManager.Instance.Configuration.RateReputation : 1);
            int bonus = GetBuff(CardType.Dracula, (byte) AdditionalTypes.Dracula.ReputationIncrease)[0];
            double Last = val2 * (bonus * 0.01);

            int beforeReputIco = GetReputationIco();
            Reputation += HasBuff(CardType.Dracula, (byte) AdditionalTypes.Dracula.ReputationIncrease) ? (val2 + (long) Last) * 1 : val2;
            Session.SendPacket(GenerateFd());
            if (beforeReputIco != GetReputationIco())
            {
                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(InEffect: 1),ReceiverType.AllExceptMe);
            }

            Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
            if (amount > 0)
            {
                //Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_INCREASE"), amount), 12));
            }
            else if (amount < 0)
            {
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_DECREASE"), amount), 11));
            }
        }

        public int GetReputationIco()
        {
            if (Reputation >= 5000001)
            {
                switch (IsReputationHero())
                {
                    case 1:
                        return 28;

                    case 2:
                        return 29;

                    case 3:
                        return 30;

                    case 4:
                        return 31;

                    case 5:
                        return 32;
                }
            }

            if (Reputation <= 50)
            {
                return 1;
            }

            if (Reputation <= 150)
            {
                return 2;
            }

            if (Reputation <= 250)
            {
                return 3;
            }

            if (Reputation <= 500)
            {
                return 4;
            }

            if (Reputation <= 750)
            {
                return 5;
            }

            if (Reputation <= 1000)
            {
                return 6;
            }

            if (Reputation <= 2250)
            {
                return 7;
            }

            if (Reputation <= 3500)
            {
                return 8;
            }

            if (Reputation <= 5000)
            {
                return 9;
            }

            if (Reputation <= 9500)
            {
                return 10;
            }

            if (Reputation <= 19000)
            {
                return 11;
            }

            if (Reputation <= 25000)
            {
                return 12;
            }

            if (Reputation <= 40000)
            {
                return 13;
            }

            if (Reputation <= 60000)
            {
                return 14;
            }

            if (Reputation <= 85000)
            {
                return 15;
            }

            if (Reputation <= 115000)
            {
                return 16;
            }

            if (Reputation <= 150000)
            {
                return 17;
            }

            if (Reputation <= 190000)
            {
                return 18;
            }

            if (Reputation <= 235000)
            {
                return 19;
            }

            if (Reputation <= 285000)
            {
                return 20;
            }

            if (Reputation <= 350000)
            {
                return 21;
            }

            if (Reputation <= 500000)
            {
                return 22;
            }

            if (Reputation <= 1500000)
            {
                return 23;
            }

            if (Reputation <= 2500000)
            {
                return 24;
            }

            if (Reputation <= 3750000)
            {
                return 25;
            }

            return Reputation <= 5000000 ? 26 : 27;
        }

        public int GetShellArmor(ShellArmorEffectType effectType)
        {
            var armor = Inventory.LoadBySlotAndType((byte) EquipmentType.Armor, InventoryType.Wear);
            List<ShellEffectDTO> effects = new List<ShellEffectDTO>();
            if (armor == null)
            {
                return 0;
            }

            if (armor.ShellEffects == null)
            {
                return 0;
            }

            effects.AddRange(armor.ShellEffects);

            return effects.Where(s => s.Effect == (byte) effectType).OrderByDescending(s => s.Value).FirstOrDefault()?.Value ?? 0;
        }

        public CharacterSkill GetSkill(short skillVNum) => GetSkills()?.FirstOrDefault(s => s.SkillVNum == skillVNum);

        public CharacterSkill GetSkillByCastId(short castId) => GetSkills()?.FirstOrDefault(s => s.Skill?.CastId == castId);

        //public List<CharacterSkill> GetSkills()=> UseSp? SkillsSp.GetAllItems().Concat(Skills.Where(s => s.SkillVNum < 200).Concat(Skills.Where(s => s.IsTattoo))).ToList() : Skills.GetAllItems();
        public List<CharacterSkill> GetSkills()
        {
            var list = new List<CharacterSkill>();
            if (UseSp)
            {
                list.AddRange(SkillsSp.GetAllItems().Concat(Skills.Where(s => s.SkillVNum < 200)).ToList());
                list.AddRange(Skills.GetAllItems().Where(sd => sd.IsPartnerSkill).ToList());
                list.AddRange(Skills.GetAllItems().Where(sd => sd.IsTattoo).ToList());
            }
            else
            {
                list.AddRange(Skills.GetAllItems());
            }
            return list;
        }
        public string GetSqst()
        {
            List<QuestLogDTO> questLogs = DAOFactory.QuestLogDAO.LoadByCharacterId(CharacterId).ToList();
            List<CharacterQuest> quests = Quests.ToList();
            string sqst = "sqst  3 ";
            for (int i = 0; i < 250; i++)
            {
                string tempSqst = "}";

                //string tempSqst = "0";
                int count = 0;
                foreach (QuestLogDTO questLog in questLogs)
                {
                    if (i == ServerManager.Instance.GetQuest(questLog.QuestId).SqstPosition)
                    {
                        double test = ServerManager.Instance.GetQuest(questLog.QuestId).SqstPosition;
                        count = ServerManager.Instance.Quests.ToList().Where(s => !questLogs.Any(q => q.QuestId == s.QuestId) && s.SqstPosition == i).Count();

                    }
                }

                foreach (CharacterQuest quest in quests)
                {
                    if (i == ServerManager.Instance.GetQuest(quest.Quest.QuestId).SqstPosition)
                    {
                        double test = ServerManager.Instance.GetQuest(quest.Quest.QuestId).SqstPosition;
                        count = ServerManager.Instance.Quests.Where(s => s.SqstPosition == i && !questLogs.Any(q => q.QuestId == s.QuestId)).Count();
                    }
                }

                if (i == 233)
                {
                    tempSqst = "2";
                }

                sqst += tempSqst;
            }

            return sqst;
        }

        /// <summary>
        /// Get Stuff Buffs Useful for Stats for example
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public int[] GetStuffBuff(CardType type, byte subtype)
        {
            int[] result = new int[2] {0, 0};

            List<BCard> bcards = new List<BCard>();

            if (Skills != null)
            {
                List<BCard> passiveSkillBCards = PassiveSkillHelper.Instance.PassiveSkillToBCards(Skills.Where(s => s?.Skill?.SkillType == 0));

                if (passiveSkillBCards.Any())
                {
                    bcards.AddRange(passiveSkillBCards);
                }
            }

            List<BCard> equipmentBCards = EquipmentBCards.ToList();

            if (equipmentBCards.Any())
            {
                bcards.AddRange(equipmentBCards);
            }

            if (EffectFromTitle != null && EffectFromTitle.ToList().Any())
            {
                bcards.AddRange(EffectFromTitle.ToList());
            }

            foreach (BCard bcard in bcards.Where(s =>
                s?.Type == (byte) type && s.SubType == (byte) (subtype) && s.FirstData > 0))
            {
                result[0] += bcard.IsLevelScaled ? (bcard.FirstData * Level) : bcard.FirstData;
                result[1] += bcard.SecondData;
            }

            return result;
        }

        public void GetXp(long val, bool applyRate = true)
        {
            if (ServerManager.Instance.Configuration.EventXp > 1)
            {
                val = val * ServerManager.Instance.Configuration.EventXp;
            }
            if (Level >= ServerManager.Instance.Configuration.MaxLevel)
            {
                return;
            }

            LevelXp += val * (applyRate ? ServerManager.Instance.Configuration.RateXP : 1) * (int) (1 + GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] / 100D);
            GenerateLevelXpLevelUp();
            Session.SendPacket(GenerateLev());
        }

        public void GiftAdd(short itemVNum, short amount, byte rare = 0, byte upgrade = 0, short design = 0, bool forceRandom = false, byte minRare = 0)
        {
            if (Inventory != null)
            {
                lock (Inventory)
                {
                    ItemInstance newItem = Inventory.InstantiateItemInstance(itemVNum, CharacterId, amount);
                    if (newItem.Item == null)
                    {
                        Logger.LogEventError("GIFT_ADD_ERROR", $"Item VNum {itemVNum} doesn't exist");
                    }

                    if (newItem != null)
                    {
                        newItem.Design = design;

                        if (newItem.Item.ItemType == ItemType.Armor || newItem.Item.ItemType == ItemType.Weapon || newItem.Item.ItemType == ItemType.Shell || forceRandom)
                        {
                            if (rare != 0 && !forceRandom)
                            {
                                try
                                {
                                    newItem.RarifyItem(Session, RarifyMode.Drop, RarifyProtection.None,forceRare: (sbyte) rare);
                                    newItem.Upgrade = (byte) (newItem.Item.BasicUpgrade + upgrade);
                                    if (newItem.Upgrade > 10)
                                    {
                                        newItem.Upgrade = 10;
                                    }
                                }
                                catch
                                {
                                    throw;
                                }
                            }
                            else if (rare == 0 || forceRandom)
                            {
                                do
                                {
                                    try
                                    {
                                        newItem.RarifyItem(Session, RarifyMode.Drop, RarifyProtection.None);
                                        newItem.Upgrade = newItem.Item.BasicUpgrade;
                                        if (newItem.Rare >= minRare)
                                        {
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                } while (forceRandom);
                            }
                        }

                        if (newItem.Item.Type.Equals(InventoryType.Equipment) && rare != 0 && !forceRandom)
                        {
                            newItem.Rare = (sbyte) rare;
                            newItem.SetRarityPoint();
                        }

                        if (newItem.Item.ItemType == ItemType.Shell)
                        {
                            newItem.Upgrade = (byte) ServerManager.RandomNumber(50, 81);
                            Session.Character.SaveEq();
                        }

                        if (newItem.Item.EquipmentSlot == EquipmentType.Gloves || newItem.Item.EquipmentSlot == EquipmentType.Boots)
                        {
                            newItem.Upgrade = upgrade;
                            newItem.DarkResistance = (short) (newItem.Item.DarkResistance * upgrade);
                            newItem.LightResistance = (short) (newItem.Item.LightResistance * upgrade);
                            newItem.WaterResistance = (short) (newItem.Item.WaterResistance * upgrade);
                            newItem.FireResistance = (short) (newItem.Item.FireResistance * upgrade);
                        }

                        List<ItemInstance> newInv = Inventory.AddToInventory(newItem);
                        if (newInv.Count > 0)
                        {
                            if (newItem.Item.IsHeroic && newItem.Item.ItemType == ItemType.Armor || newItem.Item.ItemType == ItemType.Weapon && newItem.Rare > 0)
                            {
                                newItem.GenerateHeroicShell(RarifyProtection.RandomHeroicAmulet);
                                newItem.SetRarityPoint();
                                Session.Character.SaveEq();

                            }
                            Session.SendPacket(GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newItem.Item.Name} x {amount}",10));
                        }
                        else if (MailList.Count(s => s.Value.AttachmentVNum != null) < 40)
                        {
                            SendGift(CharacterId, itemVNum, amount, newItem.Rare, newItem.Upgrade, newItem.Design,false);
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PACKET_ARRIVED"),$"{newItem.Item.Name} x {amount}"), 0));
                        }
                    }
                }
            }
        }

        public bool HasBuff(short cardId) => BattleEntity.HasBuff(cardId);

        public bool HasBuff(CardType type, byte subtype) => BattleEntity.HasBuff(type, subtype);

        public bool HaveBackpack() => StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BackPack);

        public bool HaveExtension() => StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.Extension);

        public double HPLoad() => BattleEntity.HPLoad();

        public void IncrementQuests(QuestType type, int firstData = 0, int secondData = 0, int thirdData = 0, bool forGroupMember = false)
        {
            foreach (CharacterQuest quest in Quests.Where(q => q?.Quest?.QuestType == (int) type))
            {
                switch ((QuestType) quest.Quest.QuestType)
                {
                    case QuestType.Capture1:
                    case QuestType.Capture2:
                    case QuestType.WinRaid:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d => IncrementObjective(quest, d.ObjectiveIndex));
                        break;

                    case QuestType.Collect1:
                    case QuestType.Collect2:
                    case QuestType.Collect3:
                    case QuestType.Collect4:
                    case QuestType.Hunt:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d => IncrementObjective(quest, d.ObjectiveIndex));
                        if (!forGroupMember)
                        {
                            IncrementGroupQuest(type, firstData, secondData, thirdData);
                        }

                        break;

                    case QuestType.Product:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d => IncrementObjective(quest, d.ObjectiveIndex, secondData));
                        break;

                    case QuestType.Dialog1:
                    case QuestType.Dialog2:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d => IncrementObjective(quest, d.ObjectiveIndex, isOver: true));
                        break;

                    case QuestType.Wear:
                        if (quest.Quest.QuestObjectives.Any(q => q.SpecialData == firstData && (Session.Character.Inventory.Any(i => i.ItemVNum == q.Data && i.Type == InventoryType.Wear) || (quest.QuestId == 1541 || quest.QuestId == 1546) && Class != ClassType.Adventurer)))
                        {
                            IncrementObjective(quest, isOver: true);
                        }

                        break;

                    case QuestType.Brings:
                    case QuestType.Required:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d =>
                        {
                            if (Inventory.CountItem(d.SpecialData ?? -1) >= d.Objective)
                            {
                                Inventory.RemoveItemAmount(d.SpecialData ?? -1, d.Objective ?? 1);
                                IncrementObjective(quest, d.ObjectiveIndex, d.Objective ?? 1);
                            }
                        });
                        break;

                    case QuestType.GoTo:
                        if (quest.Quest.TargetMap == firstData && Math.Abs(secondData - quest.Quest.TargetX ?? 0) < 3 &&
                            Math.Abs(thirdData - quest.Quest.TargetY ?? 0) < 3)
                        {
                            IncrementObjective(quest, isOver: true);
                        }

                        break;

                    case QuestType.Use:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData && Mates.Any(m => m.NpcMonsterVNum == o.SpecialData && m.IsTeamMember)).ToList().ForEach(d => IncrementObjective(quest, d.ObjectiveIndex, d.Objective ?? 1));
                        break;

                    case QuestType.FlowerQuest:
                        if (firstData + 10 < Level)
                        {
                            continue;
                        }

                        IncrementObjective(quest, 1);
                        break;

                    case QuestType.GlacernonQuest:
                        quest.Quest.QuestObjectives.ToList().ForEach(d => IncrementObjective(quest, 1));
                        break;

                    case QuestType.TimesSpace:
                        quest.Quest.QuestObjectives.Where(o => o.SpecialData == firstData).ToList().ForEach(d => IncrementObjective(quest, d.ObjectiveIndex));
                        break;

                    //TODO : Later
                    case QuestType.TsPoint:
                    case QuestType.NumberOfKill:
                    case QuestType.Inspect:
                    case QuestType.Needed:
                    case QuestType.TargetReput:
                    case QuestType.TransmitGold:
                    case QuestType.Collect5:
                        break;
                }
            }
        }

        public void Initialize()
        {
            _random = new Random();
            ExchangeInfo = null;
            SpCooldown = 30;
            SaveX = 0;
            SaveY = 0;
            LastDefence = DateTime.Now.AddSeconds(-21);
            LastDelay = DateTime.Now.AddSeconds(-5);
            LastHealth = DateTime.Now;
            LastEffect = DateTime.Now;
            LastDeposit = DateTime.Now;
            LastRepos = DateTime.Now;
            LastWithdraw = DateTime.Now;
            LastBazaarInsert = DateTime.Now;
            LastBazaarModeration = DateTime.Now;
            LastISort = DateTime.Now;
            Session = null;
            MailList = new Dictionary<int, MailDTO>();
            BattleEntity = new BattleEntity(this, null);
            Group = null;
            GmPvtBlock = false;
            Event = new EventEntity(this);
        }

        public bool IsBlockedByCharacter(long characterId) => CharacterRelations.Any(b => b.RelationType == CharacterRelationType.Blocked && b.CharacterId.Equals(characterId) && characterId != CharacterId);

        public bool IsBlockingCharacter(long characterId) => CharacterRelations.Any(c => c.RelationType == CharacterRelationType.Blocked && c.RelatedCharacterId.Equals(characterId));

        public bool IsCoupleOfCharacter(long characterId) => CharacterRelations.Any(c => characterId != CharacterId && c.RelationType == CharacterRelationType.Spouse && (c.RelatedCharacterId.Equals(characterId) || c.CharacterId.Equals(characterId)));

        public bool IsFriendlistFull() => CharacterRelations.Where(s => s.RelationType == CharacterRelationType.Friend || s.RelationType == CharacterRelationType.Spouse).ToList().Count >= 80;

        public bool IsFriendOfCharacter(long characterId) => CharacterRelations.Any(c => characterId != CharacterId && (c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse) && (c.RelatedCharacterId.Equals(characterId) || c.CharacterId.Equals(characterId)));

        public bool IsFamilyTop(bool isLevel)
        {
            var family = ServerManager.Instance.GetBestFamily(isLevel);

            if (Family == null)
            {
                return false;
            }

            if (family == Family)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the current character is in range of the given position
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the object to check.</param>
        /// <param name="yCoordinate">The y coordinate of the object to check.</param>
        /// <param name="range">The range of the coordinates to be maximal distanced.</param>
        /// <returns>True if the object is in Range, False if not.</returns>
        public bool IsInRange(int xCoordinate, int yCoordinate, int range = 50)
        {
            return Map.GetDistance(new MapCell
            {
                X = (short) xCoordinate,
                Y = (short) yCoordinate
            }, new MapCell
            {
                X = PositionX,
                Y = PositionY
            }) <= range;
        }

        public bool IsLaurenaMorph() => Morph == 1000099 /* Hamster */ || Morph == 1000156 /* Bushtail */;

        public bool IsMuted() => Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.Muted && s.DateEnd > DateTime.Now);

        public int IsReputationHero()
        {
            int i = 0;

            foreach (CharacterDTO character in ServerManager.Instance.TopReputation)
            {
                i++;

                if (character.CharacterId == CharacterId)
                {
                    if (i == 1)
                    {
                        return 5;
                    }

                    if (i == 2)
                    {
                        return 4;
                    }

                    if (i == 3)
                    {
                        return 3;
                    }

                    if (i <= 13)
                    {
                        return 2;
                    }

                    if (i <= 43)
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }

        public int ReputationHeroPosition()
        {
            int i = 0;

            foreach (CharacterDTO character in ServerManager.Instance.TopReputation)
            {
                i++;

                if (character.CharacterId == CharacterId)
                {
                    return i;
                }
            }

            return 0;
        }

        public void LearnAdventurerSkills(bool isCommand = false)
        {
            if (Class == 0)
            {
                bool hasLearnedNewSkill = false;

                for (short skillVNum = 200; skillVNum <= 210; skillVNum++)
                {
                    Skill skill = ServerManager.GetSkill(skillVNum);

                    if (skill?.Class == 0 && JobLevel >= skill.LevelMinimum && !Skills.Any(s => s.SkillVNum == skillVNum))
                    {
                        hasLearnedNewSkill = true;

                        Skills[skillVNum] = new CharacterSkill
                        {
                            SkillVNum = skillVNum,
                            CharacterId = CharacterId
                        };
                    }
                }

                if (!isCommand && hasLearnedNewSkill)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"), 0));
                    Session.SendPacket(GenerateSki());
                    Session.SendPackets(GenerateQuicklist());
                }
            }
        }

        public void LearnSPSkill()
        {
            ItemInstance specialist = null;

            if (Inventory != null)
            {
                specialist = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
            }

            byte SkillSpCount = (byte) SkillsSp.Count;

            SkillsSp = new ThreadSafeSortedList<int, CharacterSkill>();

            foreach (Skill ski in ServerManager.GetAllSkill())
            {
                if (specialist != null && ski.UpgradeType == specialist.Item.Morph && ski.SkillType == (byte) SkillType.CharacterSKill && specialist.SpLevel >= ski.LevelMinimum)
                {
                    SkillsSp[ski.SkillVNum] = new CharacterSkill {SkillVNum = ski.SkillVNum, CharacterId = CharacterId};
                }
            }

            if (SkillsSp.Count != SkillSpCount)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"),0));
            }
        }

        public void LeaveIceBreaker()
        {
            if (IceBreaker.AlreadyFrozenPlayers != null && IceBreaker.AlreadyFrozenPlayers.Contains(Session))
            {
                IceBreaker.AlreadyFrozenPlayers.Remove(Session);
            }

            if (IceBreaker.FrozenPlayers != null && IceBreaker.FrozenPlayers.Contains(Session))
            {
                IceBreaker.FrozenPlayers.Remove(Session);
                NoMove = false;
                NoAttack = false;
                Session.SendPacket(GenerateCond());
            }
        }

        public void LeaveTalentArena(bool surrender = false)
        {
            lock (ServerManager.Instance.ArenaTeams)
            {
                var memb = ServerManager.Instance.ArenaMembers.ToList().FirstOrDefault(s => s.Session == Session);
                if (memb != null)
                {
                    if (memb.GroupId != null)
                    {
                        ServerManager.Instance.ArenaMembers.ToList().Where(s => s.GroupId == memb.GroupId).ToList().ForEach(s =>
                            {
                                if (ServerManager.Instance.ArenaMembers.ToList().Count(g => g.GroupId == memb.GroupId) == 2)
                                {
                                    s.GroupId = null;
                                }

                                s.Time = 300;
                                s.Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(1, 2, s.Time, 8));
                                s.Session.SendPacket(s.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ARENA_TEAM_LEAVE"), 11));
                            });
                    }

                    ServerManager.Instance.ArenaMembers.Remove(memb);
                    Session.SendPacket(UserInterfaceHelper.GenerateBSInfo(2, 2, 0, 0));
                }

                ConcurrentBag<ArenaTeamMember> tm = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(o => o.Session == Session));
                Session.SendPacket(Session.Character.GenerateTaM(1));
                if (tm == null)
                {
                    return;
                }

                var tmem = tm.FirstOrDefault(s => s.Session == Session);
                if (tmem != null)
                {
                    tmem.Dead = true;
                    if (surrender)
                    {
                        Session.Character.TalentSurrender++;
                    }

                    Session.SendPacket(Session.Character.GenerateTaP(1, true));
                    Session.SendPacket("ta_sv 1");
                    Session.SendPacket("taw_sv 1");
                }

                if (UseSp)
                {
                    SkillsSp.ForEach(c => c.LastUse = DateTime.Now.AddDays(-1));
                }
                else
                {
                    Skills.ForEach(c => c.LastUse = DateTime.Now.AddDays(-1));
                }

                Session.SendPacket(GenerateSki());
                Session.SendPackets(GenerateQuicklist());

                List<BuffType> bufftodisable = new List<BuffType> {BuffType.Bad};
                Session.Character.DisableBuffs(bufftodisable);
                Session.Character.RemoveBuff(491);

                Session.Character.Hp = (int) Session.Character.HPLoad();
                Session.Character.Mp = (int) Session.Character.MPLoad();
                ServerManager.Instance.ArenaTeams.Remove(tm);
                tm.RemoveWhere(s => s.Session != Session, out tm);
                if (tm.Any())
                {
                    ServerManager.Instance.ArenaTeams.Add(tm);
                }

                tm.ToList().ForEach(s =>
                {
                    if (s.ArenaTeamType == tmem.ArenaTeamType)
                    {
                        s.Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ARENA_TALENT_LEFT"),Session.Character.Name), 0));
                    }

                    s.Session.SendPacket(s.Session.Character.GenerateTaP(2, true));
                });
            }
        }

        public void LoadInventory()
        {
            IEnumerable<ItemInstanceDTO> inventories = DAOFactory.ItemInstanceDAO.LoadByCharacterId(CharacterId).Where(s => s.Type != InventoryType.FamilyWareHouse).ToList();
            IEnumerable<CharacterDTO> characters = DAOFactory.CharacterDAO.LoadAllByAccount(Session.Account.AccountId);
            IEnumerable<Guid> warehouseInventoryIds = new List<Guid>();
            foreach (CharacterDTO character in characters.Where(s => s.CharacterId != CharacterId))
            {
                IEnumerable<ItemInstanceDTO> characterWarehouseInventory = DAOFactory.ItemInstanceDAO.LoadByCharacterId(character.CharacterId).Where(s => s.Type == InventoryType.Warehouse).ToList();
                inventories = inventories.Concat(characterWarehouseInventory);
                warehouseInventoryIds = warehouseInventoryIds.Concat(characterWarehouseInventory.Select(i => i.Id).ToList());
            }

            DAOFactory.ItemInstanceDAO.DeleteGuidList(warehouseInventoryIds);

            Inventory = new Inventory(this);
            foreach (ItemInstanceDTO inventory in inventories)
            {
                inventory.CharacterId = CharacterId;
                Inventory[inventory.Id] = new ItemInstance(inventory);
                ItemInstance iteminstance = inventory as ItemInstance;
                iteminstance?.ShellEffects.Clear();
                iteminstance?.ShellEffects.AddRange(DAOFactory.ShellEffectDAO.LoadByEquipmentSerialId(iteminstance.EquipmentSerialId));
            }

            ItemInstance ring = Inventory.LoadBySlotAndType((byte)EquipmentType.Ring, InventoryType.Wear);
            ItemInstance bracelet = Inventory.LoadBySlotAndType((byte)EquipmentType.Bracelet, InventoryType.Wear);
            ItemInstance necklace = Inventory.LoadBySlotAndType((byte)EquipmentType.Necklace, InventoryType.Wear);
            CellonOptions.Clear();
            if (ring != null)
            {
                CellonOptions.AddRange(ring.CellonOptions);
            }
            if (bracelet != null)
            {
                CellonOptions.AddRange(bracelet.CellonOptions);
            }
            if (necklace != null)
            {
                CellonOptions.AddRange(necklace.CellonOptions);
            }
        }

        public void LoadMail()
        {
            int parcel = 0, letter = 0;
            foreach (MailDTO mail in DAOFactory.MailDAO.LoadSentToCharacter(CharacterId))
            {
                MailList.Add((MailList.Count > 0 ? MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);

                if (mail.AttachmentVNum != null)
                {
                    parcel++;
                    Session.SendPacket(GenerateParcel(mail));
                }
                else
                {
                    if (!mail.IsOpened)
                    {
                        letter++;
                    }

                    Session.SendPacket(GeneratePost(mail, 1));
                }
            }

            //if (parcel > 0)
            //{
            //    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("GIFTED"), parcel), 11));
            //}
            if (letter > 0)
            {
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NEW_MAIL"), letter),10));
            }
        }

        public void LoadQuicklists()
        {
            QuicklistEntries = new List<QuicklistEntryDTO>();
            IEnumerable<QuicklistEntryDTO> quicklistDTO = DAOFactory.QuicklistEntryDAO.LoadByCharacterId(CharacterId).ToList();
            foreach (QuicklistEntryDTO qle in quicklistDTO)
            {
                QuicklistEntries.Add(qle);
            }
        }

        public void LoadSentMail()
        {
            foreach (MailDTO mail in DAOFactory.MailDAO.LoadSentByCharacter(CharacterId))
            {
                MailList.Add((MailList.Count > 0 ? MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);

                Session.SendPacket(GeneratePost(mail, 2));
            }
        }

        public void LoadSkills()
        {
            Skills = new ThreadSafeSortedList<int, CharacterSkill>();
            IEnumerable<CharacterSkillDTO> characterskillDTO = DAOFactory.CharacterSkillDAO.LoadByCharacterId(CharacterId).ToList();
            foreach (CharacterSkillDTO characterskill in characterskillDTO.OrderBy(s => s.SkillVNum))
            {
                if (!Skills.ContainsKey(characterskill.SkillVNum))
                {
                    Skills[characterskill.SkillVNum] = new CharacterSkill(characterskill);
                }
            }
        }

        public void LoadSpeed()
        {
            lock (SpeedLockObject)
            {
                // only load speed if you dont use custom speed
                if (!IsVehicled && !IsCustomSpeed)
                {
                    Speed = CharacterHelper.SpeedData[(byte) Class];

                    if (UseSp)
                    {
                        ItemInstance specialist = Inventory?.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);

                        if (specialist?.Item != null)
                        {
                            Speed += specialist.Item.Speed;
                        }
                    }

                    byte fixSpeed = (byte) GetBuff(CardType.Move, (byte) AdditionalTypes.Move.SetMovement)[0];

                    if (fixSpeed != 0)
                    {
                        Speed = fixSpeed;
                    }
                    else
                    {
                        Speed += (byte) GetBuff(CardType.Move, (byte) AdditionalTypes.Move.MovementSpeedIncreased)[0];
                        Speed -= (byte) GetBuff(CardType.Move, (byte) AdditionalTypes.Move.MovementSpeedDecreased)[0];
                        Speed = (byte) (Speed + ((Speed / 100D) * (GetBuff(CardType.Move, (byte) AdditionalTypes.Move.MoveSpeedIncreased)[0])));
                        Speed = (byte) (Speed - ((Speed / 100D) * (GetBuff(CardType.Move, (byte) AdditionalTypes.Move.MoveSpeedDecreased)[0])));
                    }
                }

                if (IsShopping)
                {
                    Speed = 0;
                    IsCustomSpeed = false;
                    return;
                }

                // reload vehicle speed after opening an shop for instance
                if (IsVehicled && !IsCustomSpeed)
                {
                    Speed = VehicleSpeed;

                    if (VehicleItem != null)
                    {
                        if (MapInstance?.Map?.MapTypes != null && VehicleItem.MapSpeedBoost != null && VehicleItem.ActSpeedBoost != null)
                        {
                            Speed += VehicleItem.MapSpeedBoost[MapInstance.Map.MapId];
                            if (MapInstance.Map.MapTypes.Any(s => new[]
                            {
                                (short) MapTypeEnum.Act1, (short) MapTypeEnum.CometPlain, (short) MapTypeEnum.Mine1,
                                (short) MapTypeEnum.Mine2, (short) MapTypeEnum.MeadowOfMine,
                                (short) MapTypeEnum.SunnyPlain, (short) MapTypeEnum.Fernon, (short) MapTypeEnum.FernonF,
                                (short) MapTypeEnum.Cliff
                            }.Contains(s.MapTypeId)))
                            {
                                Speed += VehicleItem.ActSpeedBoost[1];
                            }
                            else if (MapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short) MapTypeEnum.Act2))
                            {
                                Speed += VehicleItem.ActSpeedBoost[2];
                            }
                            else if (MapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short) MapTypeEnum.Act3))
                            {
                                Speed += VehicleItem.ActSpeedBoost[3];
                            }
                            else if (MapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short) MapTypeEnum.Act4))
                            {
                                Speed += VehicleItem.ActSpeedBoost[4];
                            }
                            else if (MapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short) MapTypeEnum.Act51))
                            {
                                Speed += VehicleItem.ActSpeedBoost[51];
                            }
                            else if (MapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short) MapTypeEnum.Act52))
                            {
                                Speed += VehicleItem.ActSpeedBoost[52];
                            }
                        }

                        if (HasBuff(CardType.Move, (byte) AdditionalTypes.Move.TempMaximized))
                        {
                            Speed += VehicleItem.SpeedBoost;
                        }
                    }
                }
            }
        }

        public double MPLoad() => BattleEntity.MPLoad();

        public bool MuteMessage()
        {
            PenaltyLogDTO penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();

            if (IsMuted() && penalty != null)
            {
                Session.CurrentMapInstance?.Broadcast(Gender == GenderType.Female ? GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1) : GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"),(penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"),(penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                return true;
            }

            return false;
        }

        public string OpenFamilyWarehouse()
        {
            if (Family == null || Family.WarehouseSize == 0)
            {
                return UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_WAREHOUSE"));
            }

            return GenerateFStashAll();
        }

        public List<string> OpenFamilyWarehouseHist()
        {
            List<string> packetList = new List<string>();
            if (Family == null || !(FamilyCharacter.Authority == FamilyAuthority.Head || FamilyCharacter.Authority == FamilyAuthority.Familydeputy || (FamilyCharacter.Authority == FamilyAuthority.Member && Family.MemberCanGetHistory) || (FamilyCharacter.Authority == FamilyAuthority.Familykeeper && Family.ManagerCanGetHistory)))
            {
                packetList.Add(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_RIGHT")));
                return packetList;
            }

            //return GenerateFamilyWarehouseHist();
            return new List<string>();
        }

        public void RemoveBuff(short cardId, bool removePermaBuff = false) => BattleEntity.RemoveBuff(cardId, removePermaBuff);

        public void RemoveBuffByBCardTypeSubType(List<KeyValuePair<byte, byte>> bcardTypes)
        {
            bcardTypes.ForEach(bt => Buff.Where(b => b.Card.BCards.Any(s => s.Type.Equals((byte) bt.Key) && s.SubType.Equals((byte) (bt.Value)) && (s.CastType == 0 || b.Start.AddMilliseconds(b.Card.Delay * 100 + 1500) < DateTime.Now))).ToList().ForEach(a => RemoveBuff(a.Card.CardId)));
        }

        public void RemoveQuest(long questId, bool IsGivingUp = false)
        {
            CharacterQuest questToRemove = Quests.FirstOrDefault(q => q.QuestId == questId);

            if (questToRemove == null)
            {
                return;
            }

            if (questToRemove.Quest.TargetMap != null)
            {
                Session.SendPacket(questToRemove.Quest.RemoveTargetPacket());
            }

            Quests.RemoveWhere(s => s.QuestId != questId, out ConcurrentBag<CharacterQuest> tmp);
            Quests = tmp;

            Session.SendPacket(GenerateQuestsPacket());

            if (IsGivingUp)
            {
                return;
            }

            if (questToRemove.Quest.EndDialogId != null)
            {
                Session.SendPacket(GenerateNpcDialog((int) questToRemove.Quest.EndDialogId));
            }

            if (questToRemove.Quest.NextQuestId != null)
            {
                AddQuest((long) questToRemove.Quest.NextQuestId, questToRemove.IsMainQuest);
            }

            LogHelper.Instance.InsertQuestLog(CharacterId, Session.IpAddress, questToRemove.Quest.QuestId,DateTime.Now);

            Session.SendPacket(GetSqst());

            #region Custom rewards based on character class

            switch (questId)
            {
                case 1621: // Jerico #1
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(20, 1, 4, 5);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(34, 1, 4, 5);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(48, 1, 4, 5);
                            break;
                    }
                }
                    break;

                case 1622: // Jerico #2
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(156, 1, 4, 5);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(158, 1, 4, 5);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(160, 1, 4, 5);
                            break;
                    }
                }
                    break;

                case 1623: // Jerico #3
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(96, 1, 4, 5);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(109, 1, 4, 5);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(122, 1, 4, 5);
                            break;
                    }
                }
                    break;

                case 1632: // Talk to Mimi
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(136, 1, 4, 5);
                            Session.Character.GiftAdd(164, 1, 4, 5);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(143, 1, 4, 5);
                            Session.Character.GiftAdd(169, 1, 4, 5);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(175, 1, 4, 5);
                            Session.Character.GiftAdd(150, 1, 4, 5);
                            break;
                    }
                }
                    break;

                case 1684: // Friend and Foe
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(24, 1, 4, 5);
                            Session.Character.GiftAdd(101, 1, 4, 5);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(113, 1, 4, 5);
                            Session.Character.GiftAdd(38, 1, 4, 5);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(52, 1, 4, 5);
                            Session.Character.GiftAdd(126, 1, 4, 5);
                            break;
                    }
                }
                    break;
            }

            #endregion

            #region Specialist Card Quest Reward

            switch (questId)
            {
                case 2007: // Pajama
                {
                    Session.Character.GiftAdd(900, 1);
                }
                    break;

                case 2013: // SP 1
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(901, 1);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(903, 1);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(905, 1);
                            break;
                    }
                }
                    break;

                case 2020: // SP 2
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(902, 1);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(904, 1);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(906, 1);
                            break;
                    }
                }
                    break;

                case 2095: // SP 3
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(909, 1);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(911, 1);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(913, 1);
                            break;
                    }
                }
                    break;

                case 2134: // SP 4
                {
                    switch (Session.Character.Class)
                    {
                        case ClassType.Swordsman:
                            Session.Character.GiftAdd(910, 1);
                            break;

                        case ClassType.Archer:
                            Session.Character.GiftAdd(912, 1);
                            break;

                        case ClassType.Magician:
                            Session.Character.GiftAdd(914, 1);
                            break;
                    }
                }
                    break;
            }

            #endregion
        }

        public bool RemoveSp(short vnum, bool forced)
        {
            if (Session?.HasSession == true && (!IsVehicled || forced))
            {
                if (Buff.Any(s => s.Card.BuffType == BuffType.Bad) && !forced)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANT_UNTRASFORM_WITH_DEBUFFS"),0));
                    return false;
                }

                LastTransform = DateTime.Now;
                DisableBuffs(BuffType.All);

                EquipmentBCards.RemoveAll(s => s.ItemVNum.Equals(vnum));

                UseSp = false;
                CharacterHelper.RemoveSpecialistWingsBuff(Session);
                LoadSpeed();
                Session.SendPacket(GenerateCond());
                Session.SendPacket(GenerateLev());
                SpCooldown = 30;
                if (SkillsSp != null)
                {
                    foreach (CharacterSkill ski in SkillsSp.Where(s => !s.CanBeUsed()))
                    {
                        short time = ski.Skill.Cooldown;
                        double temp = (ski.LastUse - DateTime.Now).TotalMilliseconds + (time * 100);
                        temp /= 1000;
                        SpCooldown = temp > SpCooldown ? (int) temp : SpCooldown;
                    }
                }

                if (Authority >= AuthorityType.User || forced)
                {
                    SpCooldown = 10;
                }

                if (Authority >= AuthorityType.Administrator || forced)
                {
                    SpCooldown = 1;
                }

                if (SpCooldown > 0)
                {
                    Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("STAY_TIME"), SpCooldown), 11));
                    Session.SendPacket($"sd {SpCooldown}");
                }

                Session.CurrentMapInstance?.Broadcast(GenerateCMode());
                Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, CharacterId), PositionX, PositionY);

                // ms_c
                Session.SendPacket(GenerateSki());
                Session.SendPackets(GenerateQuicklist());
                Session.SendPacket(GenerateStat());
                Session.SendPackets(GenerateStatChar());
                BattleEntity.RemoveOwnedMonsters();
                Logger.LogUserEvent("CHARACTER_SPECIALIST_RETURN", Session.GenerateIdentity(), $"SpCooldown: {SpCooldown}");
                if (SpCooldown > 0)
                {
                    Observable.Timer(TimeSpan.FromMilliseconds(SpCooldown * 1000)).Subscribe(o =>
                    {
                        Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORM_DISAPPEAR"), 11));
                        Session.SendPacket("sd 0");
                    });
                }
            }

            return true;
        }

        public void RemoveTemporalMates()
        {
            Mates.Where(s => s.IsTemporalMate).ToList().ForEach(m =>
            {
                m.GetInventory().ForEach(s => { Inventory.Remove(s.Id); });
                Mates.Remove(m);
                byte i = 0;
                Mates.Where(s => s.MateType == MateType.Partner).ToList().ForEach(s =>
                {
                    s.GetInventory().ForEach(item => item.Type = (InventoryType) (13 + i));
                    s.PetId = i;
                    i++;
                });
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(GenerateScP());
                Session.SendPackets(GenerateScN());
                MapInstance.Broadcast(m.GenerateOut());
            });
        }

        public void RemoveUltimatePoints(short points)
        {
            UltimatePoints -= points;

            if (UltimatePoints < 0)
            {
                UltimatePoints = 0;
            }

            if (UltimatePoints < 3000)
            {
                RemoveBuff(729);
                RemoveBuff(727);
                AddBuff(new Buff(728, 10, false), BattleEntity);
            }

            if (UltimatePoints < 2000)
            {
                RemoveBuff(728);
                RemoveBuff(729);
                AddBuff(new Buff(727, 10, false), BattleEntity);
            }

            if (UltimatePoints < 1000)
            {
                RemoveBuff(727);
                RemoveBuff(728);
                RemoveBuff(729);
            }

            Session.SendPacket(GenerateFtPtPacket());
            Session.SendPackets(GenerateQuicklist());
        }

        public void RemoveVehicle()
        {
            RemoveBuff(336);
            ItemInstance sp = null;
            if (Inventory != null)
            {
                sp = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
            }

            IsVehicled = false;
            VehicleItem = null;
            LoadSpeed();
            if (UseSp)
            {
                if (sp != null)
                {
                    Morph = sp.Item.Morph;
                    MorphUpgrade = sp.Upgrade;
                    MorphUpgrade2 = sp.Design;
                }
            }
            else
            {
                Morph = 0;
            }

            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
            Session.SendPacket(GenerateCond());
            LastSpeedChange = DateTime.Now;
        }

        public void ResetSkills()
        {
            Skills.ClearAll();

            switch ((byte) Class)
            {
                case 0:
                {
                    LearnAdventurerSkills(true);
                }
                    break;

                case 1:
                {
                    Session.Character.AddSkill(220);
                    Session.Character.AddSkill(221);
                    Session.Character.AddSkill(235);
                }
                    break;

                case 2:
                {
                    Session.Character.AddSkill(240);
                    Session.Character.AddSkill(241);
                    Session.Character.AddSkill(236);
                }
                    break;

                case 3:
                {
                    Session.Character.AddSkill(260);
                    Session.Character.AddSkill(261);
                    Session.Character.AddSkill(237);
                }
                    break;

                case 4:
                {
                    Enumerable.Range(1525, 15).ToList().ForEach(skillVNum => Session.Character.AddSkill((short) skillVNum));
                    Session.Character.AddSkill(1565);
                }
                    break;
            }

            if (!Session.Character.UseSp)
            {
                Session.SendPacket(Session.Character.GenerateSki());
                Session.SendPackets(Session.Character.GenerateQuicklist());
            }
        }

        public void Rest()
        {
            if (LastSkillUse.AddSeconds(4) > DateTime.Now || LastDefence.AddSeconds(4) > DateTime.Now)
            {
                return;
            }

            if (!IsVehicled)
            {
                IsSitting = !IsSitting;
                Session.CurrentMapInstance?.Broadcast(GenerateRest());
            }
            else
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("IMPOSSIBLE_TO_USE"), 10));
            }
        }

        public void SaveEq()
        {
            try
            {
                AccountDTO account = Session.Account;
                DAOFactory.AccountDAO.InsertOrUpdate(ref account);

                CharacterDTO character = DeepCopy();
                DAOFactory.CharacterDAO.InsertOrUpdate(ref character);


                List<ItemInstance> inventories = Inventory.GetAllItems();

                List<ItemInstance> saveInventory = inventories.Where(s => s.Type != InventoryType.Bazaar && s.Type != InventoryType.FamilyWareHouse).ToList();

                if (Inventory != null)
                {
                    lock (Inventory)




                        foreach (ItemInstance itemInstance in saveInventory)
                        {
                            if (!(itemInstance is ItemInstance instance))
                            {
                                continue;
                            }
                            if (!instance.ShellEffects.Any())
                            {
                                continue;
                            }
                            if (itemInstance.EquipmentSerialId != null)
                            {
                                DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(itemInstance.EquipmentSerialId);
                                DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(itemInstance.ShellEffects, itemInstance.EquipmentSerialId);
                                instance.ShellEffects.ForEach(s =>
                                {
                                    s.EquipmentSerialId = instance.EquipmentSerialId;
                                    DAOFactory.ShellEffectDAO.InsertOrUpdate(s);
                                });
                            }
                        }
                }
            }

            catch (Exception e)
            {
                Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", e);
            }
        }
        public void Savemates()
        {
            IEnumerable<long> currentlySavedMates = DAOFactory.MateDAO.LoadByCharacterId(CharacterId).Select(s => s.MateId);

            foreach (long matesToDeleteId in currentlySavedMates.Except(Mates.Select(s => s.MateId)))
            {
                DAOFactory.MateDAO.Delete(matesToDeleteId);
            }

            foreach (Mate mate in Mates)
            {
                MateDTO matesave = mate;
                DAOFactory.MateDAO.InsertOrUpdate(ref matesave);
            }
        }
        public void PerformItemSave(ItemInstance it)
        {
            DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(it.ShellEffects, it.EquipmentSerialId);
            DAOFactory.CellonOptionDAO.InsertOrUpdateFromList(it.CellonOptions, it.EquipmentSerialId);
        }

        public void Save()
        {
            Logger.LogUserEvent("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "START");
            try
            {
                // ServerManager.Instance.CharacterSynchronizingAtSaveProcess(AccountId, true);
                AccountDTO account = Session.Account;
                DAOFactory.AccountDAO.InsertOrUpdate(ref account);

                CharacterDTO character = DeepCopy();
                DAOFactory.CharacterDAO.InsertOrUpdate(ref character);

                if (Inventory != null)
                {
                    // be sure that noone tries to edit while saving is currently editing
                    lock (Inventory)
                    {
                        // load and concat inventory with equipment
                        List<ItemInstance> inventories = Inventory.GetAllItems();
                        IEnumerable<Guid> currentlySavedInventoryIds = DAOFactory.ItemInstanceDAO.LoadSlotAndTypeByCharacterId(CharacterId);
                        IEnumerable<CharacterDTO> characters = DAOFactory.CharacterDAO.LoadByAccount(Session.Account.AccountId);
                        foreach (CharacterDTO characteraccount in characters.Where(s => s.CharacterId != CharacterId))
                        {
                            currentlySavedInventoryIds = currentlySavedInventoryIds.Concat(DAOFactory.ItemInstanceDAO.LoadByCharacterId(characteraccount.CharacterId).Where(s => s.Type == InventoryType.Warehouse).Select(i => i.Id).ToList());
                        }

                        IEnumerable<MinilandObjectDTO> currentlySavedMinilandObjectEntries = DAOFactory.MinilandObjectDAO.LoadByCharacterId(CharacterId).ToList();
                        foreach (MinilandObjectDTO mobjToDelete in currentlySavedMinilandObjectEntries.Except(MinilandObjects))
                        {
                            try
                            {
                                DAOFactory.MinilandObjectDAO.DeleteById(mobjToDelete.MinilandObjectId);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                            }
                        }

                        DAOFactory.ItemInstanceDAO.DeleteGuidList(currentlySavedInventoryIds.Except(inventories.Select(i => i.Id)));

                        // create or update all which are new or do still exist
                        List<ItemInstance> saveInventory = inventories.Where(s => s.Type != InventoryType.Bazaar && s.Type != InventoryType.FamilyWareHouse).ToList();

                        DAOFactory.ItemInstanceDAO.InsertOrUpdateFromList(saveInventory);

                        foreach (ItemInstance itemInstance in saveInventory)
                        {
                            if (!(itemInstance is ItemInstance instance))
                            {
                                continue;
                            }
                            if (instance.RuneEffects.Any())
                            {
                                DAOFactory.RuneEffectDAO.InsertOrUpdateFromList(itemInstance.RuneEffects, itemInstance.EquipmentSerialId);
                            }
                            if (instance.CellonOptions.Any())
                            {
                                DAOFactory.CellonOptionDAO.InsertOrUpdateFromList(itemInstance.CellonOptions, itemInstance.EquipmentSerialId);
                            }
                            if (instance.ShellEffects.Any())
                            {
                                DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(itemInstance.ShellEffects, itemInstance.EquipmentSerialId);
                            }
                        }
                    }
                }

                if (Skills != null)
                {
                    try
                    {
                        IEnumerable<Guid> currentlySavedCharacterSkills = DAOFactory.CharacterSkillDAO.LoadKeysByCharacterId(CharacterId).ToList();

                        foreach (Guid characterSkillToDeleteId in currentlySavedCharacterSkills.Except(Skills.Select(s => s.Id)))
                        {
                            DAOFactory.CharacterSkillDAO.Delete(characterSkillToDeleteId);
                        }

                        foreach (CharacterSkill characterSkill in Skills.GetAllItems())
                        {
                            DAOFactory.CharacterSkillDAO.InsertOrUpdate(characterSkill);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                if (Title != null)
                {
                    try
                    {
                        IEnumerable<long> currentlySavedTitles = DAOFactory.CharacterTitleDAO.LoadByCharacterId(CharacterId).Select(s => s.CharacterTitleId);

                        foreach (long TitleToDeleteId in currentlySavedTitles.Except(Title.Select(s => s.CharacterTitleId)))
                        {
                            DAOFactory.CharacterTitleDAO.Delete(TitleToDeleteId);
                        }

                        foreach (var tit in Title)
                        {
                            CharacterTitleDTO titsave = tit;
                            DAOFactory.CharacterTitleDAO.InsertOrUpdate(ref titsave);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                if (CharacterVisitedMaps != null)
                {
                    try
                    {
                        DAOFactory.CharacterVisitedMapsDAO.InsertOrUpdateFromList(CharacterVisitedMaps, CharacterId);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                IEnumerable<long> currentlySavedMates = DAOFactory.MateDAO.LoadByCharacterId(CharacterId).Select(s => s.MateId);

                foreach (long matesToDeleteId in currentlySavedMates.Except(Mates.Select(s => s.MateId)))
                {
                    try
                    {
                        DAOFactory.MateDAO.Delete(matesToDeleteId);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                foreach (Mate mate in Mates)
                {
                    try
                    {
                        MateDTO matesave = mate;
                        DAOFactory.MateDAO.InsertOrUpdate(ref matesave);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                IEnumerable<QuicklistEntryDTO> quickListEntriesToInsertOrUpdate = QuicklistEntries.ToList();

                try
                {
                    IEnumerable<Guid> currentlySavedQuicklistEntries =
                        DAOFactory.QuicklistEntryDAO.LoadKeysByCharacterId(CharacterId).ToList();
                    foreach (Guid quicklistEntryToDelete in currentlySavedQuicklistEntries.Except(
                        QuicklistEntries.Select(s => s.Id)))
                    {
                        DAOFactory.QuicklistEntryDAO.Delete(quicklistEntryToDelete);
                    }

                    foreach (QuicklistEntryDTO quicklistEntry in quickListEntriesToInsertOrUpdate)
                    {
                        DAOFactory.QuicklistEntryDAO.InsertOrUpdate(quicklistEntry);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                }

                foreach (MinilandObjectDTO mobjEntry in (IEnumerable<MinilandObjectDTO>) MinilandObjects.ToList())
                {
                    try
                    {
                        MinilandObjectDTO mobj = mobjEntry;
                        DAOFactory.MinilandObjectDAO.InsertOrUpdate(ref mobj);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                IEnumerable<short> currentlySavedBuff = DAOFactory.StaticBuffDAO.LoadByTypeCharacterId(CharacterId);
                foreach (short bonusToDelete in currentlySavedBuff.Except(Buff.Select(s => s.Card.CardId)))
                {
                    try
                    {
                        DAOFactory.StaticBuffDAO.Delete(bonusToDelete, CharacterId);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                if (_isStaticBuffListInitial)
                {
                    try
                    {
                        foreach (Buff buff in Buff.Where(s => s.StaticBuff).ToArray())
                        {
                            if (buff.Card.CardId == 360 || buff.Card.CardId == 361) //GLOBAL FAMILY BUFFS
                                continue;
                            StaticBuffDTO bf = new StaticBuffDTO
                            {
                                CharacterId = CharacterId,
                                RemainingTime = (int)(buff.RemainingTime - (DateTime.Now - buff.Start).TotalSeconds),
                                CardId = buff.Card.CardId
                            };
                            DAOFactory.StaticBuffDAO.InsertOrUpdate(ref bf);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                //Quest
                foreach (CharacterQuestDTO q in DAOFactory.CharacterQuestDAO.LoadByCharacterId(CharacterId).ToList())
                {
                    try
                    {
                        DAOFactory.CharacterQuestDAO.Delete(CharacterId, q.QuestId);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                foreach (CharacterQuest qst in Quests.ToList())
                {
                    try
                    {
                        CharacterQuestDTO qstDTO = new CharacterQuestDTO
                        {
                            CharacterId = qst.CharacterId,
                            QuestId = qst.QuestId,
                            FirstObjective = qst.FirstObjective,
                            SecondObjective = qst.SecondObjective,
                            ThirdObjective = qst.ThirdObjective,
                            FourthObjective = qst.FourthObjective,
                            FifthObjective = qst.FifthObjective,
                            IsMainQuest = qst.IsMainQuest
                        };
                        DAOFactory.CharacterQuestDAO.InsertOrUpdate(qstDTO);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                foreach (StaticBonusDTO bonus in StaticBonusList.ToArray())
                {
                    try
                    {
                        StaticBonusDTO bonus2 = bonus;
                        DAOFactory.StaticBonusDAO.InsertOrUpdate(ref bonus2);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                foreach (GeneralLogDTO general in GeneralLogs.GetAllItems())
                {
                    try
                    {
                        if (!DAOFactory.GeneralLogDAO.IdAlreadySet(general.LogId))
                        {
                            DAOFactory.GeneralLogDAO.Insert(general);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                foreach (RespawnDTO Resp in Respawns)
                {
                    try
                    {
                        RespawnDTO res = Resp;
                        if (Resp.MapId != 0 && Resp.X != 0 && Resp.Y != 0)
                        {
                            DAOFactory.RespawnDAO.InsertOrUpdate(ref res);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", ex);
                    }
                }

                Logger.LogUserEvent("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "FINISH");
                // ServerManager.Instance.CharacterSynchronizingAtSaveProcess(AccountId, false);
            }
            catch (Exception e)
            {
                Logger.LogUserEventError("CHARACTER_DB_SAVE", Session.GenerateIdentity(), "ERROR", e);
                // ServerManager.Instance.CharacterSynchronizingAtSaveProcess(AccountId, false);
            }
        }

        public void SendGift(long id, short vnum, short amount, sbyte rare, byte upgrade, short design, bool isNosmall)
        {
            Item it = ServerManager.GetItem(vnum);

            if (it != null)
            {
                if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor && it.ItemType != ItemType.Specialist && it.EquipmentSlot != EquipmentType.Gloves && it.EquipmentSlot != EquipmentType.Boots)
                {
                    upgrade = 0;
                }
                else if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor)
                {
                    rare = 0;
                }

                if (rare > 8 || rare < -2)
                {
                    rare = 0;
                }

                if (upgrade > 10 && it.ItemType != ItemType.Specialist)
                {
                    upgrade = 0;
                }
                else if (it.ItemType == ItemType.Specialist && upgrade > 15)
                {
                    upgrade = 0;
                }

                // maximum size of the amount is 32767
                if (amount > DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Inventory.MaxItemPerSlot)
                {
                    amount = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Inventory.MaxItemPerSlot;
                }

                MailDTO mail = new MailDTO
                {
                    AttachmentAmount = it.Type == InventoryType.Etc || it.Type == InventoryType.Main ? amount : (short) 1,
                    IsOpened = false,
                    Date = DateTime.Now,
                    ReceiverId = id,
                    SenderId = CharacterId,
                    AttachmentRarity = (byte) rare,
                    AttachmentUpgrade = upgrade,
                    AttachmentDesign = design,
                    IsSenderCopy = false,
                    Title = isNosmall ? "NOSMALL" : Name,
                    AttachmentVNum = vnum,
                    SenderClass = Class,
                    SenderGender = Gender,
                    SenderHairColor = HairColor,
                    SenderHairStyle = HairStyle,
                    EqPacket = GenerateEqListForPacket(),
                    SenderMorphId = Morph == 0 ? (short) -1 : (short) (Morph > short.MaxValue ? 0 : Morph)
                };
                MailServiceClient.Instance.SendMail(mail);
                Session.Character.SaveEq();
            }
        }

        public void SetInvisible(bool invisible)
        {
            Invisible = invisible;

            if (!Invisible)
            {
                Buff?.GetAllItems().Where(b => b.Card?.BCards != null && b.Card.BCards.Any(bc => bc.Type == (byte) CardType.SpecialActions && bc.SubType == (byte) AdditionalTypes.SpecialActions.Hide)).ToList().ForEach(b => RemoveBuff(b.Card.CardId));
            }

            if (MapInstance != null)
            {
                Mates?.Where(m => m.IsTeamMember).ToList().ForEach(m => MapInstance.Broadcast(Invisible ? m.GenerateOut() : m.GenerateIn()));
                MapInstance.Broadcast(GenerateInvisible());
            }
        }

        public void SetRespawnPoint(short mapId, short mapX, short mapY)
        {
            if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
            {
                long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].RespawnMapTypeId;
                if (respawnmaptype != null)
                {
                    RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                    if (resp == null)
                    {
                        resp = new RespawnDTO
                        {
                            CharacterId = CharacterId, MapId = mapId, X = mapX, Y = mapY,
                            RespawnMapTypeId = (long) respawnmaptype
                        };
                        Respawns.Add(resp);
                    }
                    else
                    {
                        resp.X = mapX;
                        resp.Y = mapY;
                        resp.MapId = mapId;
                    }
                }
            }
        }

        public void SetReturnPoint(short mapId, short mapX, short mapY)
        {
            if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.Map.MapTypes.Count > 0)
            {
                long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes[0].ReturnMapTypeId;
                if (respawnmaptype != null)
                {
                    RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == respawnmaptype);
                    if (resp == null)
                    {
                        resp = new RespawnDTO { CharacterId = CharacterId, MapId = mapId, X = mapX, Y = mapY, RespawnMapTypeId = (long)respawnmaptype };
                        Respawns.Add(resp);
                    }
                    else
                    {
                        resp.X = mapX;
                        resp.Y = mapY;
                        resp.MapId = mapId;
                    }
                }
            }
            else if (Session.HasCurrentMapInstance && Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
            {
                RespawnDTO resp = Respawns.Find(s => s.RespawnMapTypeId == 1);
                if (resp == null)
                {
                    resp = new RespawnDTO { CharacterId = CharacterId, MapId = mapId, X = mapX, Y = mapY, RespawnMapTypeId = 1 };
                    Respawns.Add(resp);
                }
                else
                {
                    resp.X = mapX;
                    resp.Y = mapY;
                    resp.MapId = mapId;
                }
            }
        }

        public void SetSeal()
        {
            Hp = 0;
            Mp = 0;
            MapInstance.Broadcast(GenerateRevive());
            MapInstance.Broadcast(Session, $"c_mode 1 {CharacterId} 1564 0 0 0");
            IsSeal = true;
            SealDisposable?.Dispose();
            SealDisposable = Observable.Timer(TimeSpan.FromMilliseconds(5000)).Subscribe(o =>
            {
                short x = (short) (39 + ServerManager.RandomNumber(-2, 3));
                short y = (short) (42 + ServerManager.RandomNumber(-2, 3));

                IsSeal = false;

                Hp = (int) HPLoad();
                Mp = (int) MPLoad();
                if (Faction == FactionType.Angel)
                {
                    ServerManager.Instance.ChangeMap(CharacterId, 130, x, y);
                }
                else if (Faction == FactionType.Demon)
                {
                    ServerManager.Instance.ChangeMap(CharacterId, 131, x, y);
                }
                else
                {
                    MapId = 145;
                    MapX = 51;
                    MapY = 41;
                    string connection = CommunicationServiceClient.Instance.RetrieveOriginWorld(Session.Account.AccountId);
                    if (string.IsNullOrWhiteSpace(connection))
                    {
                        return;
                    }

                    int port = Convert.ToInt32(connection.Split(':')[1]);
                    Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                    return;
                }

                MapInstance?.Broadcast(Session, GenerateTp());
                MapInstance?.Broadcast(GenerateRevive());
                Session.SendPacket(GenerateStat());
            });
        }

        public void StandUp()
        {
            if (!IsVehicled && IsSitting)
            {
                IsSitting = false;
                MapInstance?.Broadcast(GenerateRest());
            }
        }

        public void TeleportToDir(int Dir, int Distance)
        {
            WalkDisposable?.Dispose();
            short NewX = PositionX;
            short NewY = PositionY;
            bool BlockedZone = false;
            for (short i = 1; Map.GetDistance(new MapCell {X = PositionX, Y = PositionY}, new MapCell {X = NewX, Y = NewY}) < Math.Abs(Distance) && i < +Math.Abs(Distance) + 5 && !BlockedZone;i++)
            {
                switch (Dir)
                {
                    case 0:
                        if (!MapInstance.Map.IsBlockedZone(NewX, NewY - i))
                        {
                            NewX = PositionX;
                            NewY = (short) (PositionY - i);
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 1:
                        if (!MapInstance.Map.IsBlockedZone(NewX + i, NewY))
                        {
                            NewX = (short) (PositionX + i);
                            NewY = PositionY;
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 2:
                        if (!MapInstance.Map.IsBlockedZone(NewX, NewY + i))
                        {
                            NewX = PositionX;
                            NewY = (short) (PositionY + i);
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 3:
                        if (!MapInstance.Map.IsBlockedZone(NewX - i, NewY))
                        {
                            NewX = (short) (PositionX - i);
                            NewY = PositionY;
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 4:
                        if (!MapInstance.Map.IsBlockedZone(NewX - i, NewY - i))
                        {
                            NewX = (short) (PositionX - i);
                            NewY = (short) (PositionY - i);
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 5:
                        if (!MapInstance.Map.IsBlockedZone(NewX + i, NewY - i))
                        {
                            NewX = (short) (PositionX + i);
                            NewY = (short) (PositionY - i);
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 6:
                        if (!MapInstance.Map.IsBlockedZone(NewX + i, NewY + i))
                        {
                            NewX = (short) (PositionX + i);
                            NewY = (short) (PositionY + i);
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;

                    case 7:
                        if (!MapInstance.Map.IsBlockedZone(NewX - i, NewY + i))
                        {
                            NewX = (short) (PositionX - i);
                            NewY = (short) (PositionY + i);
                        }
                        else
                        {
                            BlockedZone = true;
                        }

                        break;
                }
            }

            PositionX = NewX;
            PositionY = NewY;
            MapInstance.Broadcast(GenerateTp());
        }

        public void UpdateBushFire() => BattleEntity.UpdateBushFire();

        public bool WeaponLoaded(CharacterSkill ski)
        {
            if (ski != null)
            {
                switch (Class)
                {
                    default:
                        return false;

                    case ClassType.Adventurer:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance wearable = Inventory.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon,InventoryType.Wear);
                            if (wearable != null)
                            {
                                if (wearable.Ammo > 0)
                                {
                                    wearable.Ammo--;
                                    return true;
                                }

                                if (Inventory.CountItem(2081) < 1)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_AMMO_ADVENTURER"), 10));
                                    return false;
                                }

                                Inventory.RemoveItemAmount(2081);
                                wearable.Ammo = 100;
                                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_ADVENTURER"), 10));
                                return true;
                            }

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                            return false;
                        }

                        return true;

                    case ClassType.Swordsman:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance inv = Inventory.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon,InventoryType.Wear);
                            if (inv != null)
                            {
                                if (inv.Ammo > 0)
                                {
                                    inv.Ammo--;
                                    return true;
                                }

                                if (Inventory.CountItem(2082) < 1)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_AMMO_SWORDSMAN"), 10));
                                    return false;
                                }

                                Inventory.RemoveItemAmount(2082);
                                inv.Ammo = 100;
                                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_SWORDSMAN"), 10));
                                return true;
                            }

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                            return false;
                        }

                        return true;

                    case ClassType.Archer:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance inv = Inventory.LoadBySlotAndType((byte) EquipmentType.MainWeapon, InventoryType.Wear);
                            if (inv != null)
                            {
                                if (inv.Ammo > 0)
                                {
                                    inv.Ammo--;
                                    return true;
                                }

                                if (Inventory.CountItem(2083) < 1)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_AMMO_ARCHER"), 10));
                                    return false;
                                }

                                Inventory.RemoveItemAmount(2083);
                                inv.Ammo = 100;
                                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_ARCHER"), 10));
                                return true;
                            }

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                            return false;
                        }

                        return true;

                    case ClassType.Magician:
                        if (ski.Skill.Type == 1 && Inventory != null)
                        {
                            ItemInstance inv = Inventory.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon,InventoryType.Wear);
                            if (inv == null)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"),10));
                                return false;
                            }
                        }

                        return true;

                    case ClassType.MartialArtist:
                        return true;
                }
            }

            return false;
        }

        internal void NotifyRarifyResult(sbyte rare)
        {
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RARIFY_SUCCESS"), rare),12));
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RARIFY_SUCCESS"), rare), 0));
            MapInstance.Broadcast(GenerateEff(3005), PositionX, PositionY);
            Session.SendPacket("shop_end 1");
        }

        internal void RefreshValidity()
        {
            if (StaticBonusList.RemoveAll(
                s => s.StaticBonusType == StaticBonusType.BackPack && s.DateEnd < DateTime.Now) > 0)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                Session.SendPacket(GenerateExts());
            }

            if (StaticBonusList.RemoveAll(s => s.DateEnd < DateTime.Now) > 0)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
            }

            if (Inventory != null)
            {
                foreach (object suit in Enum.GetValues(typeof(EquipmentType)))
                {
                    ItemInstance item = Inventory.LoadBySlotAndType((byte) suit, InventoryType.Wear);
                    if (item?.DurabilityPoint > 0 && item.Item.EquipmentSlot != EquipmentType.Amulet)
                    {
                        item.DurabilityPoint--;
                        if (item.DurabilityPoint == 0)
                        {
                            Inventory.DeleteById(item.Id);
                            Session.SendPackets(GenerateStatChar());
                            Session.CurrentMapInstance?.Broadcast(GenerateEq());
                            Session.SendPacket(GenerateEquipment());
                            Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                        }
                    }
                }
            }
        }

        internal void SetSession(ClientSession clientSession) => Session = clientSession;

        private void GenerateHeroXpLevelUp()
        {
            double t = HeroXPLoad();
            while (HeroXp >= t)
            {
                HeroXp -= (long) t;
                HeroLevel++;
                //RewardsHelper.Instance.GetHeroRewards(Session);
                t = HeroXPLoad();
                if (HeroLevel >= ServerManager.Instance.Configuration.MaxHeroLevel)
                {
                    HeroLevel = ServerManager.Instance.Configuration.MaxHeroLevel;
                    HeroXp = 0;
                }

                Hp = (int) HPLoad();
                Mp = (int) MPLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HERO_LEVELUP"),0));
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8),PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 198),PositionX, PositionY);
                if (Family != null)
                {
                    Family.InsertFamilyLog(FamilyLogType.HeroLevelUp, Name, level: HeroLevel);
                }
            }
        }

        private void GenerateJobXpLevelUp()
        {
            var t = JobXPLoad();
            while (JobLevelXp >= t)
            {
                JobLevelXp -= (long) t;
                JobLevel++;
                //RewardsHelper.Instance.GetJobRewards(Session);
                t = JobXPLoad();
                if (JobLevel >= 20 && Class == 0)
                {
                    JobLevel = 20;
                    JobLevelXp = 0;
                }
                else if (JobLevel >= ServerManager.Instance.Configuration.MaxJobLevel)
                {
                    JobLevel = ServerManager.Instance.Configuration.MaxJobLevel;
                    JobLevelXp = 0;
                }

                Hp = (int) HPLoad();
                Mp = (int) MPLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("JOB_LEVELUP"),0));
                LearnAdventurerSkills();
                Session.SendPackets(GenerateQuicklist());
                Session.CurrentMapInstance?.Broadcast(GenerateEff(8), PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
            }
        }

        private void GenerateLevelXpLevelUp()
        {
            var t = XpLoad();
            while (LevelXp >= t)
            {
                LevelXp -= (long) t;
                Level++;
                //RewardsHelper.Instance.GetLevelUpRewards(Session);
                //RewardsHelper.Instance.MartialRewards(Session);
                t = XpLoad();
                if (Level >= ServerManager.Instance.Configuration.MaxLevel)
                {
                    Level = ServerManager.Instance.Configuration.MaxLevel;
                    LevelXp = 0;
                }

                if (Level == ServerManager.Instance.Configuration.HeroicStartLevel && HeroLevel == 0)
                {
                    HeroLevel = 1;
                    HeroXp = 0;
                }

                Hp = (int) HPLoad();
                Mp = (int) MPLoad();
                Session.SendPacket(GenerateStat());

                if (Level == 88 && HeroLevel == 0)
                {
                    HeroLevel = 1;
                }

                if (Family != null)
                {
                    if (Level > 20 && (Level % 10) == 0)
                    {
                        Family.InsertFamilyLog(FamilyLogType.LevelUp, Name, level: Level);
                        GenerateFamilyXp(20 * Level);
                    }
                    else if (Level > 80)
                    {
                        Family.InsertFamilyLog(FamilyLogType.LevelUp, Name, level: Level);
                    }
                    else
                    {
                        ServerManager.Instance.FamilyRefresh(Family.FamilyId);
                        CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
                        {
                            DestinationCharacterId = Family.FamilyId,
                            SourceCharacterId = CharacterId,
                            SourceWorldId = ServerManager.Instance.WorldId,
                            Message = "fhis_stc",
                            Type = MessageType.Family
                        });
                    }
                }

                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LEVELUP"), 0));
                Session.CurrentMapInstance?.Broadcast(GenerateEff(6), PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
                ServerManager.Instance.UpdateGroup(CharacterId);

                //LevelRewards(Level);
            }
        }

        private void GenerateQuickListSp2Am(ref string[] pktQs)
        {
            var morph = Morph;
            if (Class == ClassType.MartialArtist && Morph == 30 || Morph == 29)
            {
                morph = 30;
            }

            for (var i = 0; i < 30; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    QuicklistEntryDTO qi = QuicklistEntries.Find(n => n.Q1 == j && n.Q2 == i && n.Morph == (UseSp ? morph : 0));
                    var pos = qi?.Pos;
                    if (pos >= 6 && pos <= 9)
                    {
                        pos += 5;
                    }

                    pktQs[j] += $" {qi?.Type ?? 7}.{qi?.Slot ?? 7}.{pos.ToString() ?? "-1"}";
                }
            }
        }

        private void GenerateQuickListSp3Am(ref string[] pktQs)
        {
            var morph = Morph;
            if (Class == ClassType.MartialArtist && Morph == 30 || Morph == 29)
            {
                morph = 30;
            }

            for (var i = 0; i < 30; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    QuicklistEntryDTO qi = QuicklistEntries.Find(n => n.Q1 == j && n.Q2 == i && n.Morph == (UseSp ? morph : 0));
                    short? pos = qi?.Pos;
                    if (pos.HasValue && pos == 3 && UltimatePoints >= 2000 || pos == 4 && UltimatePoints >= 1000 ||
                        pos == 5 && UltimatePoints >= 3000)
                    {
                        pos += 8;
                    }

                    if (pos.HasValue && pos == 10 && UltimatePoints >= 3000)
                    {
                        pos += 4;
                    }

                    pktQs[j] += $" {qi?.Type ?? 7}.{qi?.Slot ?? 7}.{pos.ToString() ?? "-1"}";
                }
            }
        }

        private void GenerateSpXpLevelUp(ItemInstance specialist)
        {
            double t = SpXpLoad();
            while (UseSp && specialist.XP >= t)
            {
                specialist.XP -= (long) t;
                specialist.SpLevel++;
                t = SpXpLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                if (specialist.SpLevel >= ServerManager.Instance.Configuration.MaxSPLevel)
                {
                    specialist.SpLevel = ServerManager.Instance.Configuration.MaxSPLevel;
                    specialist.XP = 0;
                }

                LearnSPSkill();
                SkillsSp.ForEach(s => s.LastUse = DateTime.Now.AddDays(-1));
                Session.SendPacket(GenerateSki());
                Session.SendPackets(GenerateQuicklist());

                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_LEVELUP"), 0));
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 8),PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 198),PositionX, PositionY);
            }
        }

        private void GenerateXp(MapMonster monster, Dictionary<BattleEntity, long> damageList = null, bool isGroupMember = false)
        {
            if (monster?.DamageList == null)
            {
                return;
            }

            bool isKiller = false;

            if (damageList == null)
            {
                damageList = new Dictionary<BattleEntity, long>();

                lock (monster.DamageList)
                {
                    // Deep copy monster.DamageList to damageList.

                    foreach (KeyValuePair<BattleEntity, long> keyValuePair in monster.DamageList)
                    {
                        damageList.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                isKiller = true;
            }

            Group grp = null;

            if (Group?.GroupType == GroupType.Group)
            {
                grp = Group;
            }

            bool checkMonsterOwner(long entityId, Group group)
            {
                if (damageList.FirstOrDefault(s => s.Value > 0).Key is BattleEntity monsterOwner)
                {
                    return monsterOwner.MapEntityId == entityId || monsterOwner.Mate?.Owner?.CharacterId == entityId || monsterOwner.MapMonster?.Owner?.MapEntityId == entityId || group != null && group.IsMemberOfGroup(monsterOwner.MapEntityId);
                }

                return false;
            }

            bool isMonsterOwner = checkMonsterOwner(CharacterId, grp);

            lock (monster.DamageList)
            {
                if (monster.DamageList.Any())
                {
                    monster.DamageList.Where(s => s.Key.MapEntityId == CharacterId).ToList().ForEach(s => monster.DamageList.Remove(s));

                    // Call GenerateXp() for group members.

                    if (grp?.Sessions != null && !isGroupMember)
                    {
                        foreach (ClientSession groupMember in grp.Sessions.GetAllItems().Where(g => g.Character != null && g.Character.CharacterId != CharacterId && g.Character.MapInstanceId == MapInstanceId).ToList())
                        {
                            try
                            {
                                groupMember.Character?.GenerateXp(monster, damageList, true);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                            }
                        }
                    }
                }

                // Call GenerateXp() for others.

                if (monster.DamageList.Any() && isKiller)
                {
                    try
                    {
                        monster.DamageList.Where(s => s.Value > 0 && s.Key.MapEntityId != BattleEntity.MapEntityId).ToList().ForEach(s => s.Key.Character?.GenerateXp(monster, damageList));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            }

            // Exp percent regarding the damge
            double totalDamage = damageList.Sum(s => s.Value);
            double damageByCharacterOrGroup = damageList.Where(s => s.Key != null && s.Key.MapEntityId == CharacterId || Mates.Any(m => m.MateTransportId == s.Key.MapEntityId) || grp != null && grp.IsMemberOfGroup(s.Key.MapEntityId)).Sum(s => s.Value);
            double expDamageRate = damageByCharacterOrGroup / totalDamage * (isMonsterOwner && damageList.Any(s => s.Key != null && s.Value > 0 && s.Key.MapEntityId != CharacterId && (grp == null || !grp.IsMemberOfGroup(s.Key.MapEntityId))) ? 1.2f : 1);

            if (double.IsNaN(expDamageRate))
            {
                expDamageRate = 0;
            }

            NpcMonster monsterInfo = monster.Monster;

            if (!Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.BlockExp && s.DateEnd > DateTime.Now))
            {
                if (Hp <= 0)
                {
                    return;
                }

                if ((int) (LevelXp / (XpLoad() / 10)) < (int) ((LevelXp + monsterInfo.XP * expDamageRate) / (XpLoad() / 10)))
                {
                    Hp = (int) HPLoad();
                    Mp = (int) MPLoad();
                    Session.SendPacket(GenerateStat());
                    Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 5));
                }

                ItemInstance specialist = null;

                if (Inventory != null)
                {
                    specialist = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                }

                int xp = (int) (GetXP(monster, grp) * expDamageRate * (isMonsterOwner ? 1 : 0.8f) * (1 + (GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] / 100D)) * (1 + (GetBuff(CardType.MartialArts, (byte) AdditionalTypes.MartialArts.IncreaseBattleAndJobExperience)[0] /100)));

                if (Level < ServerManager.Instance.Configuration.MaxLevel)
                {
                    LevelXp += xp;
                }

                foreach (Mate mate in Mates.Where(x => x.IsTeamMember && x.IsAlive))
                {
                    mate.GenerateXp(xp);

                    if (mate.IsUsingSp)
                    {
                        mate.Sp.AddXp(xp);
                        mate.Owner?.Session?.SendPacket(mate.GenerateScPacket());
                    }
                }

                if ((Class == 0 && JobLevel < 20) || (Class != 0 && JobLevel < ServerManager.Instance.Configuration.MaxJobLevel))
                {
                    if (specialist != null && UseSp && specialist.SpLevel < ServerManager.Instance.Configuration.MaxSPLevel && specialist.SpLevel > 19)
                    {
                        JobLevelXp += (int) (GetJXP(monster, grp) * expDamageRate * (isMonsterOwner ? 1 : 0.8f) / 2D * (1 + (GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] /100D)) * (1 + (GetBuff(CardType.MartialArts, (byte) AdditionalTypes.MartialArts.IncreaseBattleAndJobExperience)[0] / 100)));
                    }
                    else
                    {
                        JobLevelXp += (int) (GetJXP(monster, grp) * expDamageRate * (isMonsterOwner ? 1 : 0.8f) * (1 + (GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] /100D)) * (1 + (GetBuff(CardType.MartialArts, (byte) AdditionalTypes.MartialArts.IncreaseBattleAndJobExperience)[0] / 100)));
                    }
                }

                if (specialist != null && UseSp && specialist.SpLevel < ServerManager.Instance.Configuration.MaxSPLevel)
                {
                    int multiplier = specialist.SpLevel < 10 ? 10 : specialist.SpLevel < 19 ? 5 : 1;

                    specialist.XP += (int) (GetJXP(monster, grp) * expDamageRate * (multiplier + (GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] /100D + (GetBuff(CardType.Item, (byte) AdditionalTypes.Item.IncreaseSPXP)[0] /100D))));
                }

                if (HeroLevel > 0 && HeroLevel < ServerManager.Instance.Configuration.MaxHeroLevel)
                {
                    HeroXp += (int) ((GetHXP(monster, grp) * expDamageRate / 50) * (isMonsterOwner ? 1 : 0.8f) * (1 + (GetBuff(CardType.Item, (byte) AdditionalTypes.Item.EXPIncreased)[0] / 100D) + (GetBuff(CardType.Dracula, (byte) AdditionalTypes.Dracula.ExpHeroIncrease)[0] /100D)));
                }

                ItemInstance fairy = Inventory?.LoadBySlotAndType((byte) EquipmentType.Fairy, InventoryType.Wear);

                if (fairy != null)
                {
                    double experience = CharacterHelper.LoadFairyXPData(fairy.ElementRate + fairy.Item.ElementRate);
                    var fairyXpBoost = GetBuff(CardType.FairyXPIncrease,(byte) AdditionalTypes.FairyXPIncrease.IncreaseFairyXPPoints)[0];

                    if (fairy.ElementRate + fairy.Item.ElementRate < fairy.Item.MaxElementRate && Level <= monsterInfo.Level + 15 && Level >= monsterInfo.Level - 15)
                    {
                        fairy.XP += ServerManager.Instance.Configuration.RateFairyXP;
                        fairy.XP += (ServerManager.Instance.Configuration.RateFairyXP / 100) * fairyXpBoost;
                    }

                    while (fairy.XP >= experience)
                    {
                        fairy.XP -= (int) experience;
                        fairy.ElementRate++;

                        if (fairy.ElementRate + fairy.Item.ElementRate == fairy.Item.MaxElementRate)
                        {
                            fairy.XP = 0;

                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("FAIRYMAX"), fairy.Item.Name), 10));
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("FAIRY_LEVELUP"), fairy.Item.Name),10));
                        }

                        Session.SendPacket(GeneratePairy());
                    }
                }

                GenerateLevelXpLevelUp();
                GenerateJobXpLevelUp();

                if (specialist != null)
                {
                    GenerateSpXpLevelUp(specialist);
                }

                GenerateHeroXpLevelUp();

                Session.SendPacket(GenerateLev());
            }
        }

        private int GetGold(MapMonster mapMonster)
        {
            if (MapId == 2006 || MapId == 150 || MapId == 153)
            {
                return 0;
            }

            MapTypeEnum[] enum15 = new MapTypeEnum[]
            {
                MapTypeEnum.Act52,
                MapTypeEnum.Act61,
                MapTypeEnum.Act62
            };

            //int lowBaseGold = ServerManager.RandomNumber(6 * mapMonster.Monster?.Level ?? 1, 12 * mapMonster.Monster?.Level ?? 1);
            int lowBaseGold = ServerManager.RandomNumber(7 * mapMonster.Monster?.Level ?? 1, 14 * mapMonster.Monster?.Level ?? 1);
            float actMultiplier = Session?.CurrentMapInstance?.Map.MapTypes?.Any(s => enum15.Contains((MapTypeEnum)s.MapTypeId)) ?? false ? 2F : 1;

            //if (Session?.CurrentMapInstance?.Map.MapTypes?.Any(s => s.MapTypeId == (short)MapTypeEnum.Act61 || s.MapTypeId == (short)MapTypeEnum.Act61a || s.MapTypeId == (short)MapTypeEnum.Act61d || s.MapTypeId == (short)MapTypeEnum.Act52) == true)
            //{
            //    actMultiplier = 2;
            //}
            return (int) (lowBaseGold * actMultiplier);
        }

        private int GetHXP(MapMonster mapMonster, Group group)
        {
            if (HeroLevel >= ServerManager.Instance.Configuration.MaxHeroLevel || (HeroLevel >= 60 && HeroLevel >= UnlockedHLevel))
            {
                return 0;
            }

            NpcMonster npcMonster = mapMonster.Monster;

            int partySize = group?.GroupType == GroupType.Group ? group.Sessions.ToList().Count(s => s?.Character != null && s.Character.MapInstance == mapMonster.MapInstance && s.Character.HeroLevel > 0 && s.Character.HeroLevel < ServerManager.Instance.Configuration.MaxHeroLevel) : 1;

            if (partySize < 1)
            {
                partySize = 1;
            }

            //double sharedHXp = npcMonster.HeroXp / partySize;

            double sharedHXp = npcMonster.HeroXp * (-13 * partySize + 113) / 100;

            double memberHXp = sharedHXp * CharacterHelper.ExperiencePenalty(Level, npcMonster.Level) * ServerManager.Instance.Configuration.RateHeroicXP;
            if (ServerManager.Instance.Configuration.EventXp > 1)
            {
                memberHXp = memberHXp * ServerManager.Instance.Configuration.EventXp;
            }
            return (int) memberHXp;
        }

        private int GetJXP(MapMonster mapMonster, Group group)
        {
            NpcMonster npcMonster = mapMonster.Monster;

            int partySize = group?.GroupType != GroupType.Group ? 1 : group.Sessions.ToList().Count(s =>
                {
                    if (s?.Character == null || s.Character.MapInstance != mapMonster.MapInstance)
                    {
                        return false;
                    }

                    if (!s.Character.UseSp)
                    {
                        return s.Character.JobLevel < (s.Character.Class == 0 ? 20 : ServerManager.Instance.Configuration.MaxJobLevel);
                    }

                    ItemInstance sp = s.Character.Inventory?.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);

                    if (sp != null)
                    {
                        return sp.SpLevel < ServerManager.Instance.Configuration.MaxSPLevel;
                    }

                    return false;
                });

            if (partySize < 1)
            {
                partySize = 1;
            }

            double sharedJXp = (double) npcMonster.JobXP / partySize;

            double memberJxp = sharedJXp * CharacterHelper.ExperiencePenalty(Level, npcMonster.Level) * (ServerManager.Instance.Configuration.RateXP + MapInstance.XpRate);

            return (int) memberJxp;
        }

        private int GetShellArmorEffectValue(ShellArmorEffectType effectType)
        {
            return ShellEffectArmor.Where(s => s.Effect == (byte) effectType).FirstOrDefault()?.Value ?? 0;
        }

        private int GetShellMainWeaponEffectValue(ShellWeaponEffectType effectType)
        {
            return ShellEffectMain.Where(s => s.Effect == (byte) effectType).FirstOrDefault()?.Value ?? 0;
        }

        private int GetXP(MapMonster mapMonster, Group group)
        {
            NpcMonster npcMonster = mapMonster.Monster;

            int partySize = group?.GroupType == GroupType.Group ? group.Sessions.ToList().Count(s => s?.Character != null && s.Character.MapInstance == mapMonster.MapInstance && s.Character.Level < ServerManager.Instance.Configuration.MaxLevel) : 1;

            if (partySize < 1)
            {
                partySize = 1;
            }

            //double sharedXp = (double)npcMonster.XP / partySize;
            double sharedXp = npcMonster.XP * (-13 * partySize + 113) / 100;

            //if (npcMonster.Level >= 80)
            //{
            //    sharedXp *= 3;
            //}
            //else if (npcMonster.Level >= 75)
            //{
            //    sharedXp *= 2;
            //}

            int lvlDifference = Level - npcMonster.Level;

            double memberXp = (lvlDifference < 5 ? sharedXp : (sharedXp / 3 * 2)) * CharacterHelper.ExperiencePenalty(Level, npcMonster.Level) * (ServerManager.Instance.Configuration.RateXP + MapInstance.XpRate);

            if (Level <= 5 && lvlDifference < -4)
            {
                memberXp *= 1.5;
            }

            return (int) memberXp;
        }

        private int HealthHPLoad()
        {
            int naturalRecovery = 1;
            int regen = GetBuff(CardType.Recovery, (byte) AdditionalTypes.Recovery.HPRecoveryIncreased)[0] + CellonOptions.Where(s => s.Type == CellonOptionType.HPRestore).Sum(s => s.Value);
            if (Skills != null)
            {
                naturalRecovery += Skills.Where(s => s.Skill.SkillType == 0 && s.Skill.CastId == 10).Sum(s => s.Skill.UpgradeSkill);
            }

            if (IsSitting)
            {
                return (int) ((regen + CharacterHelper.HPHealth[(byte) Class] *
                    (1 + GetShellArmorEffectValue(ShellArmorEffectType.RecoveryHPOnRest) / 100D)));
            }

            return (DateTime.Now - LastDefence).TotalSeconds > 4 ? (int) ((regen + CharacterHelper.HPHealthStand[(byte) Class] * (1 + GetShellArmorEffectValue(ShellArmorEffectType.RecoveryHP) / 100D)) * naturalRecovery) : 0;
        }

        private int HealthMPLoad()
        {
            int naturalRecovery = 1;
            int regen = GetBuff(CardType.Recovery, (byte) AdditionalTypes.Recovery.MPRecoveryIncreased)[0] + CellonOptions.Where(s => s.Type == CellonOptionType.MPRestore).Sum(s => s.Value);
            if (Skills != null)
            {
                naturalRecovery += Skills.Where(s => s.Skill.SkillType == 0 && s.Skill.CastId == 10).Sum(s => s.Skill.UpgradeSkill);
            }

            if (IsSitting)
            {
                return (int) ((regen + CharacterHelper.MPHealth[(byte) Class] * (1 + GetShellArmorEffectValue(ShellArmorEffectType.RecoveryMPOnRest) / 100D)));
            }

            return (DateTime.Now - LastDefence).TotalSeconds > 4 ? (int) ((regen + CharacterHelper.MPHealthStand[(byte) Class] * (1 + GetShellArmorEffectValue(ShellArmorEffectType.RecoveryMP) / 100D)) * naturalRecovery): 0;
        }

        private double HeroXPLoad() => HeroLevel == 0 ? 1 : CharacterHelper.HeroXpData[HeroLevel - 1];

        private void IncrementGroupQuest(QuestType type, int firstData = 0, int secondData = 0, int thirdData = 0)
        {
            if (Group != null && Group.GroupType == GroupType.Group)
            {
                foreach (ClientSession groupMember in Group.Sessions.Where(s => s.Character.MapInstance == MapInstance && s.Character.CharacterId != CharacterId))
                {
                    groupMember.Character.IncrementQuests(type, firstData, secondData, thirdData, true);
                }
            }
        }

        private void IncrementObjective(CharacterQuest quest, byte index = 0, int amount = 1, bool isOver = false)
        {
            bool isFinish = isOver;
            Session.SendPacket(quest.GetProgressMessage(index, amount));
            quest.Incerment(index, amount);
            byte a = 1;
            if (quest.GetObjectives().All(q => quest.GetObjectiveByIndex(a) == null || q >= quest.GetObjectiveByIndex(a++).Objective))
            {
                isFinish = true;
            }

            Session.SendPacket($"qsti {quest.GetInfoPacket(false)}");
            if (!isFinish)
            {
                return;
            }

            LastQuest = DateTime.Now;
            if (CustomQuestRewards((QuestType) quest.Quest.QuestType, quest.Quest.QuestId))
            {
                RemoveQuest(quest.QuestId);
                return;
            }

            Session.SendPacket(quest.Quest.GetRewardPacket(this));
            RemoveQuest(quest.QuestId);
        }

        private double JobXPLoad() => Class == (byte) ClassType.Adventurer ? CharacterHelper.FirstJobXPData[JobLevel - 1] : CharacterHelper.SecondJobXPData[JobLevel - 1];

        private double SpXpLoad()
        {
            ItemInstance specialist = null;
            if (Inventory != null)
            {
                specialist = Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
            }

            return specialist != null ? CharacterHelper.SPXPData[specialist.SpLevel == 0 ? 0 : specialist.SpLevel - 1] : 0;
        }

        private double XpLoad() => CharacterHelper.XPData[Level - 1];

        #endregion

        #region Runes

        public List<BCard> GetRunesInEquipment()
        {
            var runes = new List<BCard>();
            // First of all, let's get main weapon runes
            var weapon = Inventory.LoadBySlotAndType((byte) EquipmentType.MainWeapon, InventoryType.Wear);
            if (weapon != null && weapon.RuneEffects != null && weapon.RuneEffects.Any())
            {
                var runeEffects = weapon.RuneEffects.ConvertAll(x => x.DeepCopy()).ToList();

                var rune = runeEffects.Select(x => new BCard
                {
                    Type = (byte) x.Type,
                    SubType = (byte) ((x.SubType * 10) + 11),
                    FirstData = x.FirstData,
                    SecondData = x.SecondData,
                    ThirdData = x.ThirdData
                });

                runes.AddRange(rune);
            }

            // Now, secondary weapon runes
            var secondaryWeapon = Inventory.LoadBySlotAndType((byte) EquipmentType.SecondaryWeapon, InventoryType.Wear);
            if (secondaryWeapon != null && secondaryWeapon.RuneEffects != null && secondaryWeapon.RuneEffects.Any())
            {
                var runeEffects = secondaryWeapon.RuneEffects.ToList();

                var rune = runeEffects.Select(x => new BCard
                {
                    Type = (byte) x.Type,
                    SubType = (byte) ((x.SubType * 10) + 11),
                    FirstData = x.FirstData,
                    SecondData = x.SecondData,
                    ThirdData = x.ThirdData
                });

                runes.AddRange(rune);
            }

            // Now, I'll purge runes so I can get the highest one from both weapons
            // I'm retreiving only 105 and 106 cards, I have to look for other values by researching info --> Delynith
            runes = PurgeBCardList(runes, CardType.A7Powers1, CardType.A7Powers2);

            return runes;
        }

        public List<BCard> PurgeBCardList(List<BCard> list, params CardType[] cards)
        {
            var listReturn = new List<BCard>();
            if (!list.Any()) return listReturn;

            foreach (var card in cards.ToList())
            {
                // I'll get the runes from the list that matches each card I sent as parameter
                var valueToAdd = list.Where(x => cards.Contains((CardType) x.Type) && x.Type == (byte) card).ToList();
                if (valueToAdd == null || !valueToAdd.Any()) continue;

                // I'll group the runes by SubType and get max value from each Data variable
                var grouped = (from x in valueToAdd
                        group x by new {x.SubType}
                        into y
                        select new BCard
                        {
                            Type = (byte) card,
                            SubType = y.Key.SubType,
                            FirstData = y.Max(x => x.FirstData),
                            SecondData = y.Max(x => x.SecondData),
                            ThirdData = y.Max(x => x.ThirdData)
                        }
                    ).ToList();

                listReturn.AddRange(grouped);
            }

            // This part will add all those runes that were not in the Enum "cards"
            // I won't add those whose type and subtype is already on the listReturn variable
            listReturn.AddRange(list.Where(x => !listReturn.Any(y => y.Type == x.Type && x.SubType == y.SubType)));

            return listReturn;
        }

        #endregion
    }
}