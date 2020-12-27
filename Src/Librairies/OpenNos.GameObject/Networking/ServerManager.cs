using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Event.ACT6;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Extensions;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.RainbowBattle;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using OpenNos.XMLModel.Models.Quest;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using ChickenAPI.Enums;
using ChickenAPI.Enums.Game.BCard;
using ChickenAPI.Enums.Game.Buffs;
using ChickenAPI.Enums.Game.Character;
using CharacterState = OpenNos.Domain.CharacterState;
using FactionType = OpenNos.Domain.FactionType;
using GenderType = OpenNos.Domain.GenderType;
using HairColorType = OpenNos.Domain.HairColorType;
using HairStyleType = OpenNos.Domain.HairStyleType;

namespace OpenNos.GameObject.Networking
{
    public class ServerManager : BroadcastableBase
    {
        #region Instantiation

        private ServerManager()
        {
        }

        #endregion

        #region Members

        public bool InShutdown;
        public bool ShutdownStop;
        public ThreadSafeSortedList<long, Group> ThreadSafeGroupList;

        public static List<Card> Cards { get; set; }

        private static readonly ConcurrentDictionary<Guid, MapInstance> _mapinstances =
            new ConcurrentDictionary<Guid, MapInstance>();

        private static readonly List<Map> Maps = new List<Map>();
        private static readonly CryptoRandom _random = new CryptoRandom();
        private static readonly List<Skill> Skills = new List<Skill>();
        private static readonly List<Item> Items = new List<Item>();
        private static readonly List<NpcMonster> Npcs = new List<NpcMonster>();

        //Function to get a random number
        private static readonly Random random = new Random();
        private static readonly RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

        private static readonly object syncLock = new object();

        private static ServerManager _instance;

        private ConcurrentBag<NpcMonsterSkill> _allMonsterSkills;
        private List<DropDTO> _generalDrops;

        private bool _inRelationRefreshMode;

        private long _lastGroupId;

        private Dictionary<short, List<MapNpc>> _mapNpcs;

        private Dictionary<short, List<DropDTO>> _monsterDrops;

        private Dictionary<short, List<NpcMonsterSkill>> _monsterSkills;
        private ThreadSafeSortedList<int, RecipeListDTO> _recipeLists;

        private ThreadSafeSortedList<short, Recipe> _recipes;

        private Dictionary<int, List<ShopItemDTO>> _shopItems;

        private Dictionary<int, Shop> _shops;

        private Dictionary<int, List<ShopSkillDTO>> _shopSkills;

        private Dictionary<int, List<TeleporterDTO>> _teleporters;

        #endregion

        #region Properties

        public static ServerManager Instance => _instance ?? (_instance = new ServerManager());

        public List<RainbowBattleTeam> RainbowBattleMembers { get; set; } = new List<RainbowBattleTeam>();


        public Act4Stat Act4AngelStat { get; set; }

        public Act4Stat Act4DemonStat { get; set; }

        public DateTime Act4RaidStart { get; set; }

        public Act4Stat Act6Erenia { get; set; }

        public ConcurrentBag<ScriptedInstance> Act6Raids { get; set; }

        public Act4Stat Act6Zenas { get; set; }

        public MapInstance Act7Ship { get; private set; }

        public MapInstance ArenaInstance { get; private set; }

        public List<ArenaMember> ArenaMembers { get; set; } = new List<ArenaMember>();

        public List<ConcurrentBag<ArenaTeamMember>> ArenaTeams { get; set; } =
            new List<ConcurrentBag<ArenaTeamMember>>();

        public List<long> BannedCharacters { get; set; } = new List<long>();

        public ThreadSafeGenericList<BazaarItemLink> BazaarList { get; set; }

        public List<short> BossVNums { get; set; }

        public List<BoxItemDTO> BoxItems { get; set; } = new List<BoxItemDTO>();

        public int ChannelId { get; set; }

        public List<CharacterRelationDTO> CharacterRelations { get; set; }

        public ThreadSafeSortedList<long, ClientSession> CharacterScreenSessions { get; set; }

        public ThreadSafeGenericList<LogCommandsDTO> CommandsLogs { get; set; }

        public ConfigurationObject Configuration { get; set; }

        public bool EventInWaiting { get; set; }

        public MapInstance FamilyArenaInstance { get; private set; }

        public ThreadSafeSortedList<long, Family> FamilyList { get; set; }

        public long? FlowerQuestId { get; set; }
    

        public ThreadSafeGenericLockedList<Group> GroupList { get; set; } = new ThreadSafeGenericLockedList<Group>();

        public List<Group> Groups => ThreadSafeGroupList.GetAllItems();

        public bool IceBreakerInWaiting { get; set; }

        public bool InBazaarRefreshMode { get; set; }

        public long MaxBankGold { get; set; }

        public bool IsReboot { get; set; }

        public bool IsWorldServer => WorldId != Guid.Empty;

        public DateTime LastFCSent { get; set; }

        public MallAPIHelper MallApi { get; set; }

        public List<short> MapBossVNums { get; set; }

        public List<int> MateIds { get; internal set; } = new List<int>();

        public List<PenaltyLogDTO> PenaltyLogs { get; set; }

        public ThreadSafeSortedList<long, QuestModel> QuestList { get; set; }

        public List<Quest> Quests { get; set; }

        public ConcurrentBag<ScriptedInstance> Raids { get; set; }

        public Task RebootTask { get; set; }

        public List<Schedule> Schedules { get; set; }

        public string ServerGroup { get; set; }

        public List<MapInstance> SpecialistGemMapInstances { get; set; } = new List<MapInstance>();

        public List<EventType> StartedEvents { get; set; } = new List<EventType>();

        public Task TaskShutdown { get; set; }

        public ConcurrentBag<ScriptedInstance> TimeSpaces { get; set; }

        public List<CharacterDTO> TopComplimented { get; set; }

        public List<CharacterDTO> TopPoints { get; set; }

        public List<CharacterDTO> TopReputation { get; set; }

        public Guid WorldId { get; private set; }

        private DateTime LastMaintenanceAdvert { get; set; }

        public static bool IsUnderDebugMode => Debugger.IsAttached;

        #endregion

        #region Methods

        public static MapInstance GenerateMapInstance(short mapId, MapInstanceType type, InstanceBag mapclock,
            bool dropAllowed = false, bool isScriptedInstance = false)
        {
            var map = Maps.Find(m => m.MapId.Equals(mapId));
            if (map == null)
            {
                return null;
            }

            var guid = Guid.NewGuid();
            var mapInstance = new MapInstance(map, guid, false, type, mapclock, dropAllowed);
            if (!isScriptedInstance)
            {
                mapInstance.LoadMonsters();
                mapInstance.LoadNpcs();
                mapInstance.LoadPortals();
                foreach (var mapMonster in mapInstance.Monsters)
                {
                    mapMonster.MapInstance = mapInstance;
                    mapInstance.AddMonster(mapMonster);
                }

                foreach (var mapNpc in mapInstance.Npcs)
                {
                    mapNpc.MapInstance = mapInstance;
                    mapInstance.AddNPC(mapNpc);
                }
            }

            _mapinstances.TryAdd(guid, mapInstance);
            return mapInstance;
        }

        public static IEnumerable<Card> GetAllCard() => Cards;

        public static List<MapInstance> GetAllMapInstances() => _mapinstances.Values.ToList();

        public static IEnumerable<Skill> GetAllSkill() => Skills;

        public static Guid GetBaseMapInstanceIdByMapId(short mapId)
        {
            return _mapinstances.FirstOrDefault(s =>
                s.Value?.Map.MapId == mapId && s.Value.MapInstanceType == MapInstanceType.BaseMapInstance).Key;
        }

        public static Card GetCard(short? cardId)
        {
            return Cards.Find(m => m.CardId.Equals(cardId));
        }

        public static Item GetItem(short vnum)
        {
            return Items.FirstOrDefault(m => m.VNum.Equals(vnum));
        }

        public static MapInstance GetMapInstance(Guid id) => _mapinstances.ContainsKey(id) ? _mapinstances[id] : null;

        public static MapInstance GetMapInstanceByMapId(short mapId)
        {
            return _mapinstances.Values.FirstOrDefault(s => s.Map.MapId == mapId);
        }

        public static List<MapInstance> GetMapInstances(Func<MapInstance, bool> predicate) => _mapinstances.Values.Where(predicate).ToList();

        public static NpcMonster GetNpcMonster(short npcVNum)
        {
            return Npcs.FirstOrDefault(m => m.NpcMonsterVNum.Equals(npcVNum));
        }

        public static Skill GetSkill(short skillVNum)
        {
            return Skills.Find(m => m.SkillVNum.Equals(skillVNum));
        }

        //Function to get a random number 
        public static int RandomNumber(int min = 0, int max = 100)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        public static bool RandomProbabilityCheck(double probability)
        {
            if (probability == 0) return false;

            var randomNumber = TrueRandomNumber<int>(0, 100);

            if (randomNumber <= probability) return true;
            else return false;
        }

        public static T RandomNumber<T>(int min = 0, int max = 100) => (T)Convert.ChangeType(RandomNumber(min, max), typeof(T));

        public static void RemoveMapInstance(Guid mapId)
        {
            if (_mapinstances == null || mapId == null)
            {
                return;
            }

            if (_mapinstances.FirstOrDefault(s => s.Key == mapId) is KeyValuePair<Guid, MapInstance> map &&
                !map.Equals(default))
            {
                if (map.Value == null || map.Key == null) return;
                map.Value.Dispose();
                ((IDictionary)_mapinstances).Remove(map.Key);
            }
        }

        public static T TrueRandomNumber<T>(int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                rand.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (T)Convert.ChangeType((int)(min + (max - min) *
                (scale / (double)uint.MaxValue)), typeof(T));
        }

        public static MapInstance ResetMapInstance(MapInstance baseMapInstance)
        {
            if (baseMapInstance != null)
            {
                var mapinfo = new Map(baseMapInstance.Map.MapId, baseMapInstance.Map.GridMapId,
                    baseMapInstance.Map.Data)
                {
                    Music = baseMapInstance.Map.Music,
                    Name = baseMapInstance.Map.Name,
                    ShopAllowed = baseMapInstance.Map.ShopAllowed,
                    XpRate = baseMapInstance.Map.XpRate
                };
                var mapInstance = new MapInstance(mapinfo, baseMapInstance.MapInstanceId, baseMapInstance.ShopAllowed,
                    baseMapInstance.MapInstanceType, new InstanceBag(), baseMapInstance.DropAllowed);
                mapInstance.LoadMonsters();
                mapInstance.LoadNpcs();
                mapInstance.LoadPortals();
                foreach (var si in DAOFactory.ScriptedInstanceDAO.LoadByMap(mapInstance.Map.MapId).ToList())
                {
                    var siObj = new ScriptedInstance(si);
                    if (siObj.Type == ScriptedInstanceType.TimeSpace)
                    {
                        mapInstance.ScriptedInstances.Add(siObj);
                    }
                    else if (siObj.Type == ScriptedInstanceType.Raid)
                    {
                        var port = new Portal
                        {
                            Type = (byte)PortalType.Raid,
                            SourceMapId = siObj.MapId,
                            SourceX = siObj.PositionX,
                            SourceY = siObj.PositionY
                        };
                        mapInstance.Portals.Add(port);
                    }
                }

                foreach (var mapMonster in mapInstance.Monsters)
                {
                    mapMonster.MapInstance = mapInstance;
                    mapInstance.AddMonster(mapMonster);
                }

                foreach (var mapNpc in mapInstance.Npcs)
                {
                    mapNpc.MapInstance = mapInstance;
                    mapInstance.AddNPC(mapNpc);
                }

                RemoveMapInstance(baseMapInstance.MapInstanceId);
                _mapinstances.TryAdd(baseMapInstance.MapInstanceId, mapInstance);
                return mapInstance;
            }

            return null;
        }

        public static void Shout(string message, bool noAdminTag = false)
        {
            Instance.Broadcast(UserInterfaceHelper.GenerateSay(
                (noAdminTag ? "" : $"({Language.Instance.GetMessageFromKey("ADMINISTRATOR")})") + message, 10));
            Instance.Broadcast(UserInterfaceHelper.GenerateMsg(message, 2));
        }

        public void Act4Process()
        {
            if (ChannelId != 51)
            {
                return;
            }

            var angelMapInstance = GetMapInstance(GetBaseMapInstanceIdByMapId(132));
            var demonMapInstance = GetMapInstance(GetBaseMapInstanceIdByMapId(133));

            void SummonMukraju(MapInstance instance, byte faction)
            {
                var monster = new MapMonster
                {
                    MonsterVNum = 556,
                    MapY = faction == 1 ? (short)92 : (short)95,
                    MapX = faction == 1 ? (short)114 : (short)20,
                    MapId = (short)(131 + faction),
                    IsMoving = true,
                    MapMonsterId = instance.GetNextMonsterId(),
                    ShouldRespawn = false
                };
                monster.Initialize(instance);
                monster.Faction = (FactionType)faction == FactionType.Angel ? FactionType.Demon : FactionType.Angel;
                instance.AddMonster(monster);
                instance.Broadcast(monster.GenerateIn());

                Observable.Timer(TimeSpan.FromSeconds(faction == 1 ? Act4AngelStat.TotalTime : Act4DemonStat.TotalTime))
                    .Subscribe(s =>
                    {
                        if (instance.Monsters.ToList().Any(m => m.MonsterVNum == monster.MonsterVNum))
                        {
                            if (faction == 1)
                            {
                                Act4AngelStat.Mode = 0;
                            }
                            else
                            {
                                Act4DemonStat.Mode = 0;
                            }

                            instance.DespawnMonster(monster.MonsterVNum);
                            foreach (var sess in Sessions)
                            {
                                sess.SendPacket(sess.Character.GenerateFc());
                            }
                        }
                    });
            }

            int CreateRaid(byte faction)
            {
                var raidType = MapInstanceType.Act4Morcos;
                var rng = RandomNumber(1, 5);
                switch (rng)
                {
                    case 2:
                        raidType = MapInstanceType.Act4Hatus;
                        break;

                    case 3:
                        raidType = MapInstanceType.Act4Calvina;
                        break;

                    case 4:
                        raidType = MapInstanceType.Act4Berios;
                        break;
                }

                Act4Raid.GenerateRaid(raidType, faction);
                return rng;
            }

            if (Act4AngelStat.Percentage >= 10000)
            {
                Act4AngelStat.Mode = 1;
                Act4AngelStat.Percentage = 0;
                Act4AngelStat.TotalTime = 300;
                SummonMukraju(angelMapInstance, 1);
                foreach (var sess in Sessions)
                {
                    sess.SendPacket(sess.Character.GenerateFc());
                }
            }

            if (Act4AngelStat.Mode == 1 && !angelMapInstance.Monsters.Any(s => s.MonsterVNum == 556))
            {
                Act4AngelStat.Mode = 3;
                Act4AngelStat.TotalTime = 3600;

                switch (CreateRaid(1))
                {
                    case 1:
                        Act4AngelStat.IsMorcos = true;
                        break;

                    case 2:
                        Act4AngelStat.IsHatus = true;
                        break;

                    case 3:
                        Act4AngelStat.IsCalvina = true;
                        break;

                    case 4:
                        Act4AngelStat.IsBerios = true;
                        break;
                }

                foreach (var sess in Sessions)
                {
                    sess.SendPacket(sess.Character.GenerateFc());
                }
            }

            if (Act4DemonStat.Percentage >= 10000)
            {
                Act4DemonStat.Mode = 1;
                Act4DemonStat.Percentage = 0;
                Act4DemonStat.TotalTime = 300;
                SummonMukraju(demonMapInstance, 2);
                foreach (var sess in Sessions)
                {
                    sess.SendPacket(sess.Character.GenerateFc());
                }
            }

            if (Act4DemonStat.Mode == 1 && !demonMapInstance.Monsters.Any(s => s.MonsterVNum == 556))
            {
                Act4DemonStat.Mode = 3;
                Act4DemonStat.TotalTime = 3600;

                switch (CreateRaid(2))
                {
                    case 1:
                        Act4DemonStat.IsMorcos = true;
                        break;

                    case 2:
                        Act4DemonStat.IsHatus = true;
                        break;

                    case 3:
                        Act4DemonStat.IsCalvina = true;
                        break;

                    case 4:
                        Act4DemonStat.IsBerios = true;
                        break;
                }

                foreach (var sess in Sessions)
                {
                    sess.SendPacket(sess.Character.GenerateFc());
                }
            }

            if (DateTime.Now >= LastFCSent.AddMinutes(1))
            {
                foreach (var sess in Sessions)
                {
                    sess.SendPacket(sess.Character.GenerateFc());
                }

                LastFCSent = DateTime.Now;
            }
        }

        public void Act6Process()
        {
            if (Act6Zenas.Percentage >= 10000 && Act6Zenas.Mode == 0)
            {
                Act6Raid.GenerateRaid(FactionType.Angel);
                Act6Zenas.TotalTime = 3600;
                Act6Zenas.Mode = 1;
                Act6Zenas.Percentage = 0;
            }
            else if (Act6Erenia.Percentage >= 10000 && Act6Erenia.Mode == 0)
            {
                Act6Raid.GenerateRaid(FactionType.Demon);
                Act6Erenia.TotalTime = 3600;
                Act6Erenia.Mode = 1;
                Act6Erenia.Percentage = 0;
            }

            if (Act6Erenia.CurrentTime <= 0 && Act6Erenia.Mode != 0)
            {
                Act6Erenia.KilledMonsters = 0;
                Act6Erenia.Mode = 0;
                Act6Erenia.TotalTime = 0;
            }
            else if (Act6Zenas.CurrentTime <= 0 && Act6Zenas.Mode != 0)
            {
                Act6Zenas.KilledMonsters = 0;
                Act6Zenas.Mode = 0;
                Act6Zenas.TotalTime = 0;
            }

            Parallel.ForEach(
                Sessions.Where(s =>
                    s?.Character != null && s.CurrentMapInstance?.Map.MapId == 236 || s?.CurrentMapInstance?.Map.MapId == 232), sess => sess.SendPacket(sess.Character.GenerateAct6()));

        }

        public void AddGroup(Group group)
        {
            ThreadSafeGroupList[group.GroupId] = group;
        }

        public IEnumerable<ClientSession> FindSameIpAddresses(List<ClientSession> sessions)
        {
            return sessions.Where(session => sessions.Count(s => s.ParsedAddress == session.ParsedAddress) > 3);
        }

        public void AskPvpRevive(long characterId)
        {
            var session = GetSessionByCharacterId(characterId);
            if (session?.HasSelectedCharacter == true)
            {
                if (session.Character.IsVehicled)
                {
                    session.Character.RemoveVehicle();
                }

                session.Character.DisableBuffs(BuffType.All);
                session.Character.BattleEntity.AdditionalHp = 0;
                session.Character.BattleEntity.AdditionalMp = 0;
                session.SendPacket(session.Character.GenerateAdditionalHpMp());
                session.SendPacket(session.Character.GenerateStat());
                session.SendPacket(session.Character.GenerateCond());
                session.SendPackets(UserInterfaceHelper.GenerateVb());

                session.Character.BattleEntity.RemoveOwnedMonsters();

                switch (session.CurrentMapInstance.MapInstanceType)
                {
                    case MapInstanceType.TalentArenaMapInstance:
                        var team = Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(o => o.Session == session));
                        var member = team?.FirstOrDefault(s => s.Session == session);
                        if (member != null)
                        {
                            if (member.LastSummoned == null && team.OrderBy(tm3 => tm3.Order)
                                    .FirstOrDefault(tm3 => tm3.ArenaTeamType == member.ArenaTeamType && !tm3.Dead)
                                    ?.Session == session)
                            {
                                session.CurrentMapInstance.InstanceBag.DeadList.Add(session.Character.CharacterId);
                                member.Dead = true;
                                team.ToList().Where(s => s.LastSummoned != null).ToList().ForEach(s =>
                                {
                                    s.LastSummoned = null;
                                    s.Session.Character.PositionX = s.ArenaTeamType == ArenaTeamType.ERENIA
                                        ? (short)120
                                        : (short)19;
                                    s.Session.Character.PositionY = s.ArenaTeamType == ArenaTeamType.ERENIA
                                        ? (short)39
                                        : (short)40;
                                    session.CurrentMapInstance.Broadcast(s.Session.Character.GenerateTp());
                                    s.Session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));

                                    var bufftodisable = new List<BuffType> { BuffType.Bad };
                                    s.Session.Character.DisableBuffs(bufftodisable);
                                    s.Session.Character.Hp = (int)s.Session.Character.HPLoad();
                                    s.Session.Character.Mp = (int)s.Session.Character.MPLoad();
                                });
                                var killer = team.OrderBy(s => s.Order)
                                    .FirstOrDefault(s => !s.Dead && s.ArenaTeamType != member.ArenaTeamType);
                                session.CurrentMapInstance.Broadcast(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("TEAM_WINNER_ARENA_ROUND"),
                                        killer?.Session.Character.Name, killer?.ArenaTeamType), 10));
                                session.CurrentMapInstance.Broadcast(UserInterfaceHelper.GenerateMsg(
                                    string.Format(Language.Instance.GetMessageFromKey("TEAM_WINNER_ARENA_ROUND"),
                                        killer?.Session.Character.Name, killer?.ArenaTeamType), 0));
                                session.CurrentMapInstance.Sessions
                                    .Except(team.Where(s => s.ArenaTeamType == killer?.ArenaTeamType)
                                        .Select(s => s.Session)).ToList().ForEach(o =>
                                    {
                                        if (killer?.ArenaTeamType == ArenaTeamType.ERENIA)
                                        {
                                            o.SendPacket(killer.Session.Character.GenerateTaM(2));
                                            o.SendPacket(killer.Session.Character.GenerateTaP(2, true));
                                        }
                                        else
                                        {
                                            o.SendPacket(member.Session.Character.GenerateTaM(2));
                                            o.SendPacket(member.Session.Character.GenerateTaP(2, true));
                                        }

                                        o.SendPacket($"taw_d {member.Session.Character.CharacterId}");
                                        o.SendPacket(member.Session.Character.GenerateSay(
                                            string.Format(Language.Instance.GetMessageFromKey("WINNER_ARENA_ROUND"),
                                                killer?.Session.Character.Name /*, killer?.ArenaTeamType*/,
                                                member.Session.Character.Name), 10));
                                        o.SendPacket(UserInterfaceHelper.GenerateMsg(
                                            string.Format(Language.Instance.GetMessageFromKey("WINNER_ARENA_ROUND"),
                                                killer?.Session.Character.Name /*, killer?.ArenaTeamType*/,
                                                member.Session.Character.Name), 0));
                                    });
                                team.Replace(friends => friends.ArenaTeamType == member.ArenaTeamType).ToList()
                                    .ForEach(friends =>
                                    {
                                        friends.Session.SendPacket(friends.Session.Character.GenerateTaFc(0));
                                    });
                            }
                            else
                            {
                                member.LastSummoned = null;
                                var tm = team.OrderBy(tm3 => tm3.Order).FirstOrDefault(tm3 =>
                                    tm3.ArenaTeamType == member.ArenaTeamType && !tm3.Dead);
                                team.Replace(friends => friends.ArenaTeamType == member.ArenaTeamType).ToList()
                                    .ForEach(friends =>
                                    {
                                        friends.Session.SendPacket(tm.Session.Character.GenerateTaFc(0));
                                    });
                            }

                            team.ToList().ForEach(arenauser =>
                            {
                                if (arenauser?.Session?.Character != null)
                                {
                                    arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaP(2, true));
                                    arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaM(2));
                                }
                            });

                            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(s =>
                            {
                                if (member != null && member?.Session != null && member?.Session?.Character != null && member?.Session?.CurrentMapInstance != null)
                                {
                                    member.Session.Character.PositionX = member.ArenaTeamType == ArenaTeamType.ERENIA
                                        ? (short)120
                                        : (short)19;
                                    member.Session.Character.PositionY = member.ArenaTeamType == ArenaTeamType.ERENIA
                                        ? (short)39
                                        : (short)40;
                                    member.Session.CurrentMapInstance.Broadcast(member.Session,
                                        member.Session.Character.GenerateTp());
                                    member.Session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));
                                }
                            });

                            Observable.Timer(TimeSpan.FromSeconds(4)).Subscribe(s =>
                            {
                                if (session != null)
                                {
                                    session.Character.Hp = (int)session.Character.HPLoad();
                                    session.Character.Mp = (int)session.Character.MPLoad();
                                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateRevive());
                                    session.SendPacket(session.Character.GenerateStat());
                                }
                            });
                        }

                        break;

                    default:
                        if (session.CurrentMapInstance == ArenaInstance ||
                            session.CurrentMapInstance == FamilyArenaInstance)
                        {
                            session.Character.LeaveTalentArena(true);
                            session.SendPacket(UserInterfaceHelper.GenerateDialog(
                                $"#revival^2 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_PVP")}"));
                            Task.Factory.StartNew(async () =>
                            {
                                var revive = true;
                                for (var i = 1; i <= 30; i++)
                                {
                                    await Task.Delay(1000);
                                    if (session.Character.Hp <= 0)
                                    {
                                        continue;
                                    }

                                    revive = false;
                                    break;
                                }

                                if (revive)
                                {
                                    ReviveTask(session);
                                }
                            });
                        }
                        else
                        {
                            AskRevive(characterId);
                        }

                        break;
                }
            }
        }

        // PacketHandler -> with Callback?
        public void AskRevive(long characterId)
        {
            var session = GetSessionByCharacterId(characterId);
            if (session?.HasSelectedCharacter == true && session.HasCurrentMapInstance)
            {
                if (session.Character.IsVehicled)
                {
                    session.Character.RemoveVehicle();
                }

                session.Character.ClearLaurena();
                session.Character.DisableBuffs(BuffType.All);
                session.Character.BattleEntity.AdditionalHp = 0;
                session.Character.BattleEntity.AdditionalMp = 0;
                session.SendPacket(session.Character.GenerateAdditionalHpMp());
                session.SendPacket(session.Character.GenerateStat());
                session.SendPacket(session.Character.GenerateCond());
                session.SendPackets(UserInterfaceHelper.GenerateVb());

                switch (session.CurrentMapInstance.MapInstanceType)
                {
                    case MapInstanceType.BaseMapInstance:
                        if (session.Character.Level > 20 && ChannelId != 51)
                        {
                            session.Character.Dignity -= (short)(session.Character.Level < 50 ? session.Character.Level : 50);

                            if (session.Character.Dignity < -1000)
                            {
                                session.Character.Dignity = -1000;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("LOSE_DIGNITY"),
                                        (short)(session.Character.Level < 50 ? session.Character.Level : 50)), 11));
                            }

                            session.SendPacket(session.Character.GenerateFd());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);

                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);

                        }

                        session.SendPacket(UserInterfaceHelper.GenerateDialog(
                            $"#revival^0 #revival^1 {(session.Character.Level > 20 ? Language.Instance.GetMessageFromKey("ASK_REVIVE") : Language.Instance.GetMessageFromKey("ASK_REVIVE_FREE"))}"));
                        ReviveTask(session);
                        break;

                    case MapInstanceType.TimeSpaceInstance:
                        lock (session.CurrentMapInstance.InstanceBag.DeadList)
                        {
                            if (session.CurrentMapInstance.InstanceBag.Lives - session.CurrentMapInstance.InstanceBag
                                    .DeadList.ToList().Count(s => s == session.Character.CharacterId) < 0)
                            {
                                session.Character.Hp = 1;
                                session.Character.Mp = 1;
                            }
                            else
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    string.Format(Language.Instance.GetMessageFromKey("YOU_HAVE_LIFE"),
                                        session.CurrentMapInstance.InstanceBag.Lives -
                                        session.CurrentMapInstance.InstanceBag.DeadList.Count(e =>
                                            e == session.Character.CharacterId)), 0));
                                session.SendPacket(UserInterfaceHelper.GenerateDialog(
                                    $"#revival^1 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_TS")}"));
                                ReviveTask(session);
                            }
                        }

                        break;

                    case MapInstanceType.RaidInstance:
                        var save = session.CurrentMapInstance.InstanceBag.DeadList.ToList();
                        if (session.CurrentMapInstance.InstanceBag.Lives - save.Count < 0)
                        {
                            session.Character.Hp = 1;
                            session.Character.Mp = 1;
                            session.Character.Group?.Raid.End();
                        }
                        else if (3 - save.Count(s => s == session.Character.CharacterId) > 0)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateInfo(
                                string.Format(Language.Instance.GetMessageFromKey("YOU_HAVE_LIFE"),
                                    2 - session.CurrentMapInstance.InstanceBag.DeadList.Count(s =>
                                        s == session.Character.CharacterId))));

                            session.Character.Group?.Sessions.ForEach(grpSession =>
                            {
                                grpSession?.SendPacket(grpSession.Character.Group?.GeneraterRaidmbf(grpSession));
                                grpSession?.SendPacket(grpSession.Character.Group?.GenerateRdlst());
                            });
                            Task.Factory.StartNew(async () =>
                            {
                                await Task.Delay(20000).ConfigureAwait(false);
                                Instance.ReviveFirstPosition(session.Character.CharacterId);
                            });
                        }
                        else
                        {
                            var grp = session.Character?.Group;
                            session.Character.Hp = 1;
                            session.Character.Mp = 1;
                            ChangeMap(session.Character.CharacterId, session.Character.MapId, session.Character.MapX,
                                session.Character.MapY);
                            session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KICK_RAID"), 0));
                            if (grp != null)
                            {
                                grp.LeaveGroup(session);
                                grp.Sessions.ForEach(s =>
                                {
                                    s.SendPacket(grp.GenerateRdlst());
                                    s.SendPacket(s.Character.Group?.GeneraterRaidmbf(s));
                                    s.SendPacket(s.Character.GenerateRaid(0));
                                });
                            }

                            session.SendPacket(session.Character.GenerateRaid(1, true));
                            session.SendPacket(session.Character.GenerateRaid(2, true));
                        }

                        break;

                    case MapInstanceType.SealedVesselsMap:
                        if (session.Character.MapId == 9999)
                        {
                            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(s =>

                            session.SendPacket(UserInterfaceHelper.GenerateInfo("INFO: You can only use sealed vessels in this map.")));

                        }
                        break;

                    case MapInstanceType.LodInstance:
                        const int saver = 1211;
                        if (session.Character.Inventory.CountItem(saver) >= 1)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateDialog($"#revival^0 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_LOD")}"));
                            ReviveTask(session);
                        }
                        else
                        {
                            Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(o => ServerManager.Instance.ReviveFirstPosition(session.Character.CharacterId));
                        }
                        break;

                    case MapInstanceType.Act4Berios:
                    case MapInstanceType.Act4Calvina:
                    case MapInstanceType.Act4Hatus:
                    case MapInstanceType.Act4Morcos:
                        session.SendPacket(UserInterfaceHelper.GenerateDialog(
                            $"#revival^0 #revival^1 {string.Format(Language.Instance.GetMessageFromKey("ASK_REVIVE_ACT4RAID"), session.Character.Level * 10)}"));
                        ReviveTask(session);
                        break;

                    case MapInstanceType.CaligorInstance:
                        session.SendPacket(UserInterfaceHelper.GenerateDialog(
                            $"#revival^0 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_CALIGOR")}"));
                        ReviveTask(session);
                        break;

                    default:
                        Instance.ReviveFirstPosition(session.Character.CharacterId);
                        break;
                }
            }
        }

        public async void AutoReboot()
        {
            Shout(string.Format(Language.Instance.GetMessageFromKey("AUTOREBOOT"), 30, true));
            for (var i = 0; i < 30; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }

            Shout(string.Format(Language.Instance.GetMessageFromKey("AUTOREBOOT"), 10));
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }

            InShutdown = true;
            Instance.SaveAll();

            Instance.DisconnectAll();
            CommunicationServiceClient.Instance.UnregisterWorldServer(WorldId);
            if (IsReboot)
            {
                if (ChannelId == 51)
                {
                    await Task.Delay(16000).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay((ChannelId - 1) * 2000).ConfigureAwait(false);
                }

                Process.Start("OpenNos.World.exe",
                    $"--nomsg{(ChannelId == 51 ? $" --port {Configuration.Act4Port}" : "")}");
            }

            Environment.Exit(0);
        }

        public void BazaarRefresh(long bazaarItemId)
        {
            InBazaarRefreshMode = true;
            CommunicationServiceClient.Instance.UpdateBazaar(ServerGroup, bazaarItemId);
            SpinWait.SpinUntil(() => !InBazaarRefreshMode);
        }

        public void ChangeMap(long id, short? mapId = null, short? mapX = null, short? mapY = null)
        {
            var session = GetSessionByCharacterId(id);
            if (session?.Character != null)
            {

                if (mapId != null)
                {
                    MapInstance gotoMapInstance = GetMapInstanceByMapId(mapId.Value);
                    if (session.Character.Level < gotoMapInstance.MinLevel || session.Character.Level > gotoMapInstance.MaxLevel || session.Character.HeroLevel < gotoMapInstance.MinHeroLevel || session.Character.HeroLevel > gotoMapInstance.MaxHeroLevel)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("LOW_LVL_MAP"), gotoMapInstance.MinLevel, gotoMapInstance.MaxLevel)));
                        return;
                    }

                    session.Character.MapInstanceId = GetBaseMapInstanceIdByMapId((short)mapId);
                }
                ChangeMapInstance(id, session.Character.MapInstanceId, mapX, mapY);
            }
        }
  

        // Both partly
        public void ChangeMapInstance(long characterId, Guid mapInstanceId, int? mapX = null, int? mapY = null, bool noAggroLoss = false)
        {
            var session = GetSessionByCharacterId(characterId);

            if (session?.Character != null && !session.Character.IsChangingMapInstance)
            {
                session.Character.IsChangingMapInstance = true;

                session.Character.RemoveBuff(620);

                session.Character.WalkDisposable?.Dispose();
                SpinWait.SpinUntil(
                    () => session.Character.LastSkillUse.AddMilliseconds(500) <= DateTime.Now);
                try
                {
                    var gotoMapInstance = GetMapInstance(mapInstanceId);
                    /*if (session.Character.Level < gotoMapInstance.MinLevel || session.Character.Level > gotoMapInstance.MaxLevel)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("LOW_LVL_MAP"), gotoMapInstance.MinLevel, gotoMapInstance.MaxLevel)));
                        session.Character.IsChangingMapInstance = false;
                        return;
                    }*/

                    session.SendPacket(StaticPacketHelper.Cancel(2, characterId));

                    if (session.Character.InExchangeOrTrade)
                    {
                        session.Character.CloseExchangeOrTrade();
                    }

                    if (session.Character.HasShopOpened)
                    {
                        session.Character.CloseShop();
                    }

                    session.Character.BattleEntity.ClearOwnFalcon();
                    session.Character.BattleEntity.ClearEnemyFalcon();
                    if (!noAggroLoss)
                    {
                        session.CurrentMapInstance.RemoveMonstersTarget(session.Character.CharacterId);
                    }
                    session.Character.BattleEntity.RemoveOwnedMonsters();
                   
                    //if (gotoMapInstance.Map.MapId == 150 || gotoMapInstance.Map.MapId == 247 && gotoMapInstance.Sessions.Any())
                    //{
                    //new MapMonster().MonsterSpawn();
                    //}

                    if (gotoMapInstance.MapInstanceType.Equals(MapInstanceType.LodInstance))
                    {
                        //LOD MSG
                        if (gotoMapInstance.Map.MapId == 150)
                        {
                            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(s =>
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOD_TIP"), 0)));

                        }
                    }

                    if (gotoMapInstance.MapInstanceType.Equals(MapInstanceType.BaseMapInstance))
                    {

                        #region Interaction on map join

                        if (gotoMapInstance.Map.MapId == 54)
                        {
                            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(s =>
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAP_54_DIALOG"), 0)));
                        }

                        if (gotoMapInstance.Map.MapId == 9999)
                        {
                            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(s =>
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAP_9999_DIALOG"), 0)));
                        }

                        if (gotoMapInstance.Map.MapId == 179)
                        {
                            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(s =>
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAP_179_DIALOG"), 0)));

                        }

                        if (gotoMapInstance.Map.MapId == 260)
                        {
                            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(s =>
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAP_260_DIALOG"), 0)));

                        }
                        #endregion

                    }

                    static void RemoveAllPetInTeam(ClientSession session)
                    {
                        foreach (var mate in session.Character.Mates.Where(s => s.IsTeamMember)) mate.RemoveTeamMember();
                    }

                    if (gotoMapInstance.MapInstanceType.Equals(MapInstanceType.ArenaInstance))
                    {
                        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(s =>
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REMOVE_ARENABUFFS"), 0)));

                        if (gotoMapInstance.Map.MapId == 2006 || gotoMapInstance.Map.MapId == 2106)
                        {
                            //disable a4 pills on enter
                            session.Character.RemoveBuff(170);
                            session.Character.RemoveBuff(171);
                            session.Character.RemoveBuff(172);

                            session.Character.DisableBuffs(BuffType.All);
                            session.Character.ChargeValue = 0;                          
                            RemoveAllPetInTeam(session);
                            session.Character.IsCustomSpeed = false;
                            session.Character.LoadSpeed();
                            session.SendPacket(session.Character.GenerateCond());
                        }
                    }


                    //if (gotoMapInstance == CaligorRaid.CaligorMapInstance && session.Character.MapInstance != CaligorRaid.CaligorMapInstance)
                    //{
                    //    session.Character.OriginalFaction = (byte)session.Character.Faction;

                    //    //random??
                    //    if (CaligorRaid.CaligorMapInstance.Sessions.ToList().Count(s => s.Character?.Faction == FactionType.Angel) >
                    //        CaligorRaid.CaligorMapInstance.Sessions.ToList().Count(s => s.Character?.Faction == FactionType.Demon)
                    //        && session.Character.Faction != FactionType.Demon)
                    //    {
                    //        session.Character.Faction = FactionType.Demon;
                    //        session.SendPacket(session.Character.GenerateFaction());
                    //    }
                    //    else if (CaligorRaid.CaligorMapInstance.Sessions.ToList().Count(s => s.Character?.Faction == FactionType.Demon) >
                    //             CaligorRaid.CaligorMapInstance.Sessions.ToList().Count(s => s.Character?.Faction == FactionType.Angel)
                    //             && session.Character.Faction != FactionType.Angel)
                    //    {
                    //        session.Character.Faction = FactionType.Angel;
                    //        session.SendPacket(session.Character.GenerateFaction());
                    //    }
                    if (mapX <= 0 && mapY <= 0)
                    {
                        switch (session.Character.Faction)
                        {
                            case FactionType.Angel:
                                mapX = 58;
                                mapY = 164;
                                break;

                            case FactionType.Demon:
                                mapX = 121;
                                mapY = 164;
                                break;
                        }
                    }
                    //}
                    //else if (gotoMapInstance != CaligorRaid.CaligorMapInstance && session.Character.MapInstance == CaligorRaid.CaligorMapInstance)
                    //{
                    //    if (session.Character.OriginalFaction != -1 && (byte)session.Character.Faction != session.Character.OriginalFaction)

                    //    {
                    //        session.Character.Faction = (FactionType)session.Character.OriginalFaction;
                    //        session.SendPacket(session.Character.GenerateFaction());
                    //    }
                    //}
                    if (session.Character.IsExchanging)
                        session.Character.CloseExchangeOrTrade();

                    if (session.Character.HasShopOpened)
                        session.Character.CloseShop();
                    session.CurrentMapInstance.UnregisterSession(session.Character.CharacterId);
                    LeaveMap(session.Character.CharacterId);

                    // cleanup sending queue to avoid sending uneccessary packets to it
                    session.ClearLowPriorityQueue();

                    session.Character.IsSitting = false;
                    session.Character.MapInstanceId = mapInstanceId;
                    session.CurrentMapInstance = session.Character.MapInstance;

                    if (!session.Character.MapInstance.MapInstanceType.Equals(MapInstanceType.TimeSpaceInstance) && session.Character.Timespace != null)

                    {
                        session.Character.TimespaceRewardGotten = false;
                        session.Character.RemoveTemporalMates();
                        if (session.Character.Timespace.SpNeeded?[(byte)session.Character.Class] != 0)
                        {
                            var specialist = session.Character.Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);


                            if (specialist != null || specialist.ItemVNum == session.Character.Timespace.SpNeeded?[(byte)session.Character.Class])

                            {
                                Observable.Timer(TimeSpan.FromMilliseconds(300)).Subscribe(s => session.Character.RemoveSp(specialist.ItemVNum, true));

                            }
                        }

                        session.Character.Timespace = null;
                    }

                    if (session.Character.Hp <= 0 && !session.Character.IsSeal)
                    {
                        session.Character.Hp = 1;
                        session.Character.Mp = 1;
                    }

                    session.Character.LeaveTalentArena();

                    if (session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        session.Character.MapId = session.Character.MapInstance.Map.MapId;
                        if (mapX != null && mapY != null)
                        {
                            session.Character.MapX = (short)mapX.Value;
                            session.Character.MapY = (short)mapY.Value;
                        }
                    }

                    if (mapX != null && mapY != null)
                    {
                        session.Character.PositionX = (short)mapX.Value;
                        session.Character.PositionY = (short)mapY.Value;
                    }

                    if (gotoMapInstance != null && gotoMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        if (session.Character.CharacterVisitedMaps.All(x => x.MapId != gotoMapInstance.Map.MapId) 
                            && mapX != null && mapY != null)
                        {
                            session.Character.AddNewVisitedMap(gotoMapInstance.Map.MapId, mapX.Value, mapY.Value);
                        }
                    }

                    foreach (var mate in session.Character.Mates.Where(m =>
                        m.IsTeamMember && !session.Character.IsVehicled || m.IsTemporalMate))
                    {
                        mate.PositionX =
                            (short)(session.Character.PositionX +
                                     (mate.MateType == MateType.Partner ? -1 : 1));
                        mate.PositionY = (short)(session.Character.PositionY + 1);
                        if (session.Character.MapInstance.Map.IsBlockedZone(mate.PositionX, mate.PositionY))
                        {
                            mate.PositionX = session.Character.PositionX;
                            mate.PositionY = session.Character.PositionY;
                        }

                        mate.UpdateBushFire();
                    }

                    session.Character.UpdateBushFire();
                    session.CurrentMapInstance.RegisterSession(session);
                    session.Character.LoadSpeed();

                    if (gotoMapInstance.Map?.MapId != 2514)
                    {
                        session.Character.ClearLaurena();
                    }

                    session.SendPacket(session.Character.GenerateCInfo());
                    session.SendPacket(session.Character.GenerateCMode());
                    session.SendPacket(session.Character.GenerateEq());
                    session.SendPacket(session.Character.GenerateEquipment());
                    session.SendPacket(session.Character.GenerateLev());
                    session.SendPacket(session.Character.GenerateStat());
                    session.SendPacket(session.Character.GenerateAt());
                    session.SendPacket(session.Character.GenerateCond());
                    session.SendPacket(session.Character.GenerateCMap());
                    session.SendPackets(session.Character.GenerateStatChar());
                    session.SendPacket(session.Character.GeneratePairy());
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateTitInfo());
                    session.SendPacket(Character.GenerateAct());
                    session.SendPacket(session.Character.GenerateScpStc());
                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(s => { session.SendPacket(session.Character.GenerateFmp()); });
                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(s => { session.SendPacket(session.Character.GenerateFmi()); });
                    session.Character.GenerateAscrPacket();


                    if (session.CurrentMapInstance.OnSpawnEvents.Any())
                    {
                        session.CurrentMapInstance.OnSpawnEvents.ForEach(e =>
                                EventHelper.Instance.RunEvent(e, session));
                    }

                    if (ChannelId == 51)
                    {
                        session.SendPacket(session.Character.GenerateFc());

                        if (mapInstanceId == session.Character.Family?.Act4Raid?.MapInstanceId ||
                            mapInstanceId == session.Character.Family?.Act4RaidBossMap?.MapInstanceId)
                        {
                            session.SendPacket(session.Character.GenerateDG());
                        }
                    }

                    if (session.Character.Group?.Raid?.InstanceBag?.Lock == true)
                    {
                        session.SendPacket(session.Character.Group.GeneraterRaidmbf(session));

                        if (session.CurrentMapInstance.Monsters.Any(s => s.IsBoss))
                        {
                            session.Character.Group.Sessions?.Where(s => s?.Character != null).ForEach(s =>
                            {
                                if (!s.Character.IsChangingMapInstance &&
                                    s.CurrentMapInstance != session.CurrentMapInstance)
                                {
                                    ChangeMapInstance(s.Character.CharacterId,
                                            session.CurrentMapInstance.MapInstanceId, mapX, mapY);
                                }
                            });
                        }
                    }

                    if (session.Character.MapInstance == session.Character.Family?.Act4RaidBossMap)
                    {
                        session.Character.Family.Act4Raid.Sessions
                               .Where(s => !s.Character.IsChangingMapInstance).ToList().ForEach(s =>
                               {
                                   ChangeMapInstance(s.Character.CharacterId,
                                           session.CurrentMapInstance.MapInstanceId, mapX, mapY);
                               });
                    }

                    foreach (var visibleSession in session.CurrentMapInstance.Sessions.Where(s =>
                                                                   s.Character?.InvisibleGm == false &&
                                                                   s.Character.CharacterId != session.Character.CharacterId))
                    {
                        if (ChannelId != 51 ||
                            session.Character.Faction == visibleSession.Character.Faction)
                        {
                            session.SendPacket(visibleSession.Character.GenerateIn());
                            session.SendPacket(visibleSession.Character.GenerateGidx());
                            visibleSession.Character.Mates
                                          .Where(m => (m.IsTeamMember || m.IsTemporalMate) &&
                                                      m.CharacterId != session.Character.CharacterId)
                                          .ToList().ForEach(m => session.SendPacket(m.GenerateIn()));
                        }
                        else
                        {
                            session.SendPacket(
                                    visibleSession.Character.GenerateIn(true, session.Account.Authority));
                            visibleSession.Character.Mates
                                          .Where(m => (m.IsTeamMember || m.IsTemporalMate) &&
                                                      m.CharacterId != session.Character.CharacterId)
                                          .ToList().ForEach(m =>
                                                  session.SendPacket(m.GenerateIn(true, ChannelId == 51,
                                                          session.Account.Authority)));
                        }
                    }

                    session.SendPacket(session.CurrentMapInstance.GenerateMapDesignObjects());
                    session.SendPackets(session.CurrentMapInstance.GetMapDesignObjectEffects());

                    session.SendPackets(session.CurrentMapInstance.GetMapItems());

                    MapInstancePortalHandler
                        .GenerateMinilandEntryPortals(session.CurrentMapInstance.Map.MapId,
                            session.Character.Miniland.MapInstanceId)
                        .ForEach(p => session.SendPacket(p.GenerateGp()));
                    MapInstancePortalHandler.GenerateAct4EntryPortals(session.CurrentMapInstance.Map.MapId)
                        .ForEach(p => session.SendPacket(p.GenerateGp()));

                    if (session.CurrentMapInstance.InstanceBag?.Clock?.Enabled == true)
                    {
                        session.SendPacket(session.CurrentMapInstance.InstanceBag.Clock.GetClock());
                    }

                    if (session.CurrentMapInstance.Clock.Enabled)
                    {
                        session.SendPacket(session.CurrentMapInstance.Clock.GetClock());
                    }

                    // TODO: fix this
                    if (session.Character.MapInstance.Map.MapTypes.Any(m =>
                        m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness))
                    {
                        session.SendPacket("bc 0 0 0");
                    }

                    if (!session.Character.InvisibleGm)
                    {
                        foreach (var s in session.CurrentMapInstance.Sessions.Where(
                                s => s.Character != null))
                        {
                            if (ChannelId != 51 || session.Character.Faction == s.Character.Faction)
                            {
                                s.SendPacket(session.Character.GenerateIn());
                                s.SendPacket(session.Character.GenerateGidx());
                                session.Character.Mates.Where(m => m.IsTeamMember || m.IsTemporalMate)
                                       .ToList().ForEach(m =>
                                               s.SendPacket(m.GenerateIn(false, ChannelId == 51)));
                            }
                            else
                            {
                                s.SendPacket(session.Character.GenerateIn(true, s.Account.Authority));
                                session.Character.Mates.Where(m => m.IsTeamMember || m.IsTemporalMate)
                                       .ToList()
                                       .ForEach(m =>
                                               s.SendPacket(m.GenerateIn(true, ChannelId == 51,
                                                       s.Account.Authority)));
                            }

                            if (session.Character.GetBuff(BCardType.SpecialEffects,
                                                (byte)BCardSubTypes.SpecialEffects.ShadowAppears) is int[]
                                        EffectData && EffectData[0] != 0 &&
                                EffectData[1] != 0)
                            {
                                s.CurrentMapInstance.Broadcast(
                                        $"guri 0 {(short)UserType.Player} {session.Character.CharacterId} {EffectData[0]} {EffectData[1]}");
                            }

                            session.Character.Mates.Where(m => m.IsTeamMember || m.IsTemporalMate).ToList()
                                   .ForEach(m =>
                                   {
                                       if (session.Character.IsVehicled)
                                       {
                                           m.PositionX = session.Character.PositionX;
                                           m.PositionY = session.Character.PositionY;
                                       }

                                       if (m.GetBuff(BCardType.SpecialEffects,
                                                           (byte)BCardSubTypes.SpecialEffects.ShadowAppears) is int[]
                                                   MateEffectData && MateEffectData[0] != 0 &&
                                           MateEffectData[1] != 0)
                                       {
                                           s.CurrentMapInstance.Broadcast(
                                                   $"guri 0 {(short)UserType.Monster} {m.MateTransportId} {MateEffectData[0]} {MateEffectData[1]}");
                                       }
                                   });
                        }
                    }

                    session.SendPacket(session.Character.GeneratePinit());

                    if (session.Character.Mates.FirstOrDefault(s =>
                        (s.IsTeamMember || s.IsTemporalMate) && s.MateType == MateType.Partner &&
                        s.IsUsingSp) is Mate partner)
                    {
                        session.SendPacket(partner.Sp.GeneratePski());
                    }

                    session.Character.Mates.ForEach(s => session.SendPacket(s.GenerateScPacket()));
                    session.SendPackets(session.Character.GeneratePst());

                    if (session.Character.Size != 10)
                    {
                        session.SendPacket(session.Character.GenerateScal());
                    }

                    if (session.CurrentMapInstance?.IsDancing == true && !session.Character.IsDancing)
                    {
                        session.CurrentMapInstance?.Broadcast("dance 2");
                    }
                    else if (session.CurrentMapInstance?.IsDancing == false && session.Character.IsDancing)
                    {
                        session.Character.IsDancing = false;
                        session.CurrentMapInstance?.Broadcast("dance");
                    }

                    if (Groups != null)
                    {
                        foreach (var group in Groups)
                            foreach (var groupSession in @group.Sessions.GetAllItems())
                            {
                                var groupCharacterSession = Sessions.FirstOrDefault(s =>
                                        s.Character != null &&
                                        s.Character.CharacterId == groupSession.Character.CharacterId &&
                                        s.CurrentMapInstance == groupSession.CurrentMapInstance);

                                if (groupCharacterSession == null)
                                {
                                    continue;
                                }

                                groupSession.SendPacket(groupSession.Character.GeneratePinit());
                                groupSession.SendPackets(groupSession.Character.GeneratePst());
                            }
                    }

                    if (session.Character.Group?.GroupType == GroupType.Group)
                    {
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GeneratePidx(), ReceiverType.AllExceptMe);

                    }

                    if (session.CurrentMapInstance?.Map.MapTypes.All(s => s.MapTypeId != (short)MapTypeEnum.Act52) == true && session.Character.Buff.Any(s => s.Card.CardId == 339)) //Act5.2 debuff

                    {
                        session.Character.RemoveBuff(339);
                    }
                    else if (session.CurrentMapInstance?.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act52) == true && session.Character.Buff.All(s => s.Card.CardId != 339 && s.Card.CardId != 340))

                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO
                        {
                            CardId = 339,
                            CharacterId = session.Character.CharacterId,
                            RemainingTime = -1
                        },  true);
                    }

                    session.SendPacket(session.Character.GenerateMinimapPosition());
                    session.CurrentMapInstance.OnCharacterDiscoveringMapEvents.ForEach(e =>
                    {
                        if (!e.Item2.Contains(session.Character.CharacterId))
                        {
                            e.Item2.Add(session.Character.CharacterId);
                            EventHelper.Instance.RunEvent(e.Item1, session);
                        }
                    });
                    session.CurrentMapInstance.OnCharacterDiscoveringMapEvents = session.CurrentMapInstance
                        .OnCharacterDiscoveringMapEvents
                        .Where(s => s.Item1.EventActionType == EventActionType.SENDPACKET).ToList();
                    session.Character.LeaveIceBreaker();

                    session.Character.IsChangingMapInstance = false;
                }
                catch (Exception ex)
                {
                    Logger.Warn("Character changed while changing map. Do not abuse Commands.", ex);
                    session.Character.IsChangingMapInstance = false;
                }
            }
        }

        public void DisconnectAll()
        {
            foreach (var session in Sessions)
            {
                session?.Destroy();
            }
        }

        public void FamilyRefresh(long familyId, bool changeFaction = false)
        {
            CommunicationServiceClient.Instance.UpdateFamily(ServerGroup, familyId, changeFaction);
        }

        public List<Recipe> GetAllRecipes() => _recipes.GetAllItems();

        public Family GetBestFamily(bool isLevel)
        {
            if (isLevel)
            {
                return FamilyList.GetAllItems().OrderByDescending(
                        s => s.FamilyLevel).ToList().FirstOrDefault();
            }

            return FamilyList.GetAllItems().OrderByDescending(
                                      s => s.FamilyExperience).ToList().FirstOrDefault();
        }

        public Card GetCardByCardId(short cardId)
        {
            return Cards.Find(s => s.CardId == cardId);
        }

        public List<DropDTO> GetDropsByMonsterVNum(short monsterVNum) => _monsterDrops.ContainsKey(monsterVNum)
                ? _generalDrops.Concat(_monsterDrops[monsterVNum]).ToList()
                : _generalDrops.ToList();

        public Group GetGroupByCharacterId(long characterId)
        {
            return Groups?.SingleOrDefault(g => g.IsMemberOfGroup(characterId));
        }

        public List<MapNpc> GetMapNpcsByVNum(short npcVNum)
        {
            return GetAllMapInstances().Where(mapInstance =>
                    mapInstance != null && !mapInstance.IsScriptedInstance)
                .SelectMany(mapInstance => mapInstance.Npcs.Where(mapNpc => mapNpc?.NpcVNum == npcVNum))
                .ToList();
        }

        public long GetNextGroupId() => ++_lastGroupId;

        public int GetNextMobId()
        {
            var maxMobId = 0;
            foreach (var map in _mapinstances.Values.ToList())
            {
                if (map.Monsters.Count > 0 && maxMobId < map.Monsters.Max(m => m.MapMonsterId))
                {
                    maxMobId = map.Monsters.Max(m => m.MapMonsterId);
                }
            }

            return ++maxMobId;
        }

        public int GetNextNpcId()
        {
            var mapNpcId = 0;
            foreach (var map in _mapinstances.Values.ToList())
            {
                if (map.Npcs.Count > 0 && mapNpcId < map.Npcs.Max(m => m.MapNpcId))
                {
                    mapNpcId = map.Npcs.Max(m => m.MapNpcId);
                }
            }

            return ++mapNpcId;
        }

        public NpcMonsterSkill GetNpcMonsterSkill(short skillVnum)
        {
            return _allMonsterSkills.FirstOrDefault(s => s.SkillVNum == skillVnum);
        }

        public Quest GetQuest(long questId)
        {
            return Quests.FirstOrDefault(m => m.QuestId.Equals(questId));
        }

        public List<Recipe> GetRecipesByItemVNum(short itemVNum)
        {
            var recipes = new List<Recipe>();
            foreach (var recipeList in _recipeLists.Where(r => r.ItemVNum == itemVNum))
            {
                recipes.Add(_recipes[recipeList.RecipeId]);
            }

            return recipes;
        }

        public List<Recipe> GetRecipesByMapNpcId(int mapNpcId)
        {
            var recipes = new List<Recipe>();
            foreach (var recipeList in _recipeLists.Where(r => r.MapNpcId == mapNpcId))
            {
                recipes.Add(_recipes[recipeList.RecipeId]);
            }

            return recipes;
        }

        public ClientSession GetSessionByCharacterName(string name)
        {
            return Sessions.SingleOrDefault(s => s.Character.Name == name);
        }

        public ClientSession GetSessionBySessionId(int sessionId)
        {
            return Sessions.SingleOrDefault(s => s.SessionId == sessionId);
        }

        public void GroupLeave(ClientSession session)
        {
            if (Groups != null && session?.CurrentMapInstance?.MapInstanceType != MapInstanceType.RainbowBattleInstance)
            {
                var grp = Instance.Groups.Find(s => s.IsMemberOfGroup(session.Character.CharacterId));
                if (grp != null)
                {
                    switch (grp.GroupType)
                    {
                        case GroupType.BigTeam:
                        case GroupType.GiantTeam:
                        case GroupType.Team:
                            if (grp.Raid?.InstanceBag.Lock == true)
                            {
                                grp.Raid.InstanceBag.DeadList.Add(session.Character.CharacterId);
                            }

                            if (grp.Sessions.ElementAt(0) == session && grp.SessionCount > 1)
                            {
                                Broadcast(session,
                                        UserInterfaceHelper.GenerateInfo(
                                                Language.Instance.GetMessageFromKey("NEW_LEADER")),
                                        ReceiverType.OnlySomeone, "",
                                        grp.Sessions.ElementAt(1)?.Character.CharacterId ?? 0);
                            }

                            grp.LeaveGroup(session);
                            session.SendPacket(session.Character.GenerateRaid(1, true));
                            session.SendPacket(session.Character.GenerateRaid(2, true));
                            foreach (var groupSession in grp.Sessions.GetAllItems())
                            {
                                groupSession.SendPacket(grp.GenerateRdlst());
                                groupSession.SendPacket(grp.GeneraterRaidmbf(groupSession));
                                groupSession.SendPacket(groupSession.Character.GenerateRaid(0));
                            }

                            if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                            {
                                ChangeMap(session.Character.CharacterId, session.Character.MapId,
                                        session.Character.MapX, session.Character.MapY);
                            }

                            session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("LEFT_RAID"), 0));
                            break;

                        /*case GroupType.GiantTeam:
                            ClientSession[] grpmembers = new ClientSession[40];
                            grp.Sessions.CopyTo(grpmembers);
                            foreach (ClientSession targetSession in grpmembers)
                            {
                                if (targetSession != null)
                                {
                                    targetSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_CLOSED"), 0));
                                    Broadcast(targetSession.Character.GeneratePidx(true));
                                    grp.LeaveGroup(targetSession);
                                    targetSession.SendPacket(targetSession.Character.GeneratePinit());
                                    targetSession.SendPackets(targetSession.Character.GeneratePst());
                                }
                            }
                            GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                            ThreadSafeGroupList.Remove(grp.GroupId);
                            break;*/

                        case GroupType.Group:
                            if (grp.Sessions.ElementAt(0) == session && grp.SessionCount > 1)
                            {
                                Broadcast(session,
                                        UserInterfaceHelper.GenerateInfo(
                                                Language.Instance.GetMessageFromKey("NEW_LEADER")),
                                        ReceiverType.OnlySomeone, "",
                                        grp.Sessions.ElementAt(1).Character.CharacterId);
                            }

                            grp.LeaveGroup(session);
                            if (grp.SessionCount == 1)
                            {
                                var targetSession = grp.Sessions.ElementAt(0);
                                if (targetSession != null)
                                {
                                    targetSession.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("GROUP_CLOSED"), 0));
                                    Broadcast(targetSession.Character.GeneratePidx(true));
                                    grp.LeaveGroup(targetSession);
                                    targetSession.SendPacket(targetSession.Character.GeneratePinit());
                                    targetSession.SendPackets(targetSession.Character.GeneratePst());
                                }
                            }
                            else
                            {
                                foreach (var groupSession in grp.Sessions.GetAllItems())
                                {
                                    groupSession.SendPacket(groupSession.Character.GeneratePinit());
                                    groupSession.SendPackets(session.Character.GeneratePst());
                                    groupSession.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        string.Format(Language.Instance.GetMessageFromKey("LEAVE_GROUP"),
                                            session.Character.Name), 0));
                                }
                            }

                            session.SendPacket(session.Character.GeneratePinit());
                            session.SendPackets(session.Character.GeneratePst());
                            Broadcast(session.Character.GeneratePidx(true));
                            session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("GROUP_LEFT"), 0));
                            break;

                        default:
                            return;
                    }

                    session.Character.Group = null;
                }
            }
        }

        public void Initialize()
        {
            InitAllProperty();
            LoadBossEntities();

            // Load Configuration
            LoadEvent();
            LoadItem();
            LoadBoxItem();
            LoadDropMonster();
            LoadMonsterSkill();
            LoadBazaar();
            LoadNpcMonsters();
            LoadRecipes();
            LoadRecipesList();
            LoadShopItems();
            LoadShopSkills();
            LoadShop();
            LoadTeleporter();
            LoadSkills();
            LoadCards();
            LoadQuest();
            LoadMapNpc();
            LoadMapsAndContent();
            LoadFamilies();
            LaunchEvents();
            RefreshRanking();
            LoadCharacterRelations();
            LoadAct7Ship();
            LoadGemStone();
            LoadNormalArena();
            LoadFamilyArena();
            LoadScriptedInstances();
            LoadBannedCharacters();
            FlushLogs(true);

            WorldId = Guid.NewGuid();

            if (DateTime.UtcNow.Day == 1 && DAOFactory.GeneralLogDAO.LoadByAccount(null).LastOrDefault(s => s.LogData == "RankingReward" && s.LogType == "World" && s.Timestamp.Date == DateTime.UtcNow.Date) == null)
            {
                DAOFactory.GeneralLogDAO.Insert(new GeneralLogDTO { LogData = "RankingReward", LogType = "World", Timestamp = DateTime.UtcNow });
                RewardByPopularity(true);
                RewardByReputation();
                RewardByScore(true);
            }
        }

        private void RewardByPopularity(bool reset)
        {
            List<CharacterDTO> characters = DAOFactory.CharacterDAO.GetTopCompliment();
            int rankspot = 1;
            foreach (CharacterDTO _ in characters)
            {
                CharacterDTO dto = _;
                switch (rankspot)
                {
                    case 1:
                        SendItemToMail(dto.CharacterId, 5993, 3, 0, 0);
                        break;
                    case 2:
                        SendItemToMail(dto.CharacterId, 5993, 2, 0, 0);
                        break;
                    case 3:
                        SendItemToMail(dto.CharacterId, 5993, 1, 0, 0);
                        break;
                }

                rankspot++;
                if (reset)
                {
                    dto.Compliment = 0;
                    DAOFactory.CharacterDAO.InsertOrUpdate(ref dto);
                }
            }
        }

        private void RewardByReputation()
        {
            List<CharacterDTO> characters = DAOFactory.CharacterDAO.GetTopReputation();
            int rankspot = 1;
            foreach (CharacterDTO _ in characters)
            {
                CharacterDTO dto = _;
                switch (rankspot)
                {
                    case 1:
                        SendItemToMail(dto.CharacterId, 5993, 5, 0, 0);
                        break;
                    case 2:
                        SendItemToMail(dto.CharacterId, 5993, 4, 0, 0);
                        break;
                    case 3:
                        SendItemToMail(dto.CharacterId, 5993, 3, 0, 0);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        SendItemToMail(dto.CharacterId, 5993, 2, 0, 0);
                        break;
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                        SendItemToMail(dto.CharacterId, 5993, 1, 0, 0);
                        break;
                }
                rankspot++;
            }
        }

        private void RewardByScore(bool reset)
        {
            List<CharacterDTO> characters = DAOFactory.CharacterDAO.GetTopPoints();
            int rankspot = 1;
            foreach (CharacterDTO _ in characters)
            {
                CharacterDTO dto = _;
                switch (rankspot)
                {
                    case 1:
                        SendItemToMail(dto.CharacterId, 5993, 5, 0, 0);
                        break;
                    case 2:
                        SendItemToMail(dto.CharacterId, 5993, 4, 0, 0);
                        break;
                    case 3:
                        SendItemToMail(dto.CharacterId, 5993, 3, 0, 0);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        SendItemToMail(dto.CharacterId, 5993, 1, 0, 0);
                        break;
                }
                rankspot++;
                if (reset)
                {
                    dto.Act4Points = 0;
                    DAOFactory.CharacterDAO.InsertOrUpdate(ref dto);
                }
            }
        }

        private void SendItemToMail(long id, short vnum, byte amount, sbyte rare, byte upgrade)
        {
            Item it = GetItem(vnum);

            if (it != null)
            {
                if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor && it.ItemType != ItemType.Specialist)
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

                // maximum size of the amount is 99
                if (amount > 99)
                {
                    amount = 99;
                }

                MailDTO mail = new MailDTO
                {
                    AttachmentAmount = it.Type == InventoryType.Etc || it.Type == InventoryType.Main ? amount : (byte) 1,
                    IsOpened = false,
                    Date = DateTime.UtcNow,
                    ReceiverId = id,
                    SenderId = id,
                    AttachmentRarity = (byte)rare,
                    AttachmentUpgrade = upgrade,
                    IsSenderCopy = false,
                    Title = "RankingReward",
                    AttachmentVNum = vnum,
                    SenderClass = CharacterClassType.Adventurer,
                    SenderGender = GenderType.Male,
                    SenderHairColor = HairColorType.Black,
                    SenderHairStyle = HairStyleType.NoHair,
                    EqPacket = string.Empty,
                    SenderMorphId = 0
                };
                MailServiceClient.Instance.SendMail(mail);
            }
        }

        public bool IsAct4Online() => CommunicationServiceClient.Instance.IsAct4Online(ServerGroup);
        
        public bool IsCharacterMemberOfGroup(long characterId)
        {
            return Groups?.Any(g => g.IsMemberOfGroup(characterId)) == true;
        }

        public bool IsCharactersGroupFull(long characterId)
        {
            return Groups?.Any(g =>
                       g.IsMemberOfGroup(characterId) &&
                       (g.SessionCount == (byte)g.GroupType || g.GroupType == GroupType.TalentArena)) ==
                   true;
        }

        public bool ItemHasRecipe(short itemVNum)
        {
            return _recipeLists.Any(r => r.ItemVNum == itemVNum);
        }
        public MapCell MinilandRandomPos() => new MapCell { X = (short)RandomNumber(5, 16), Y = (short)RandomNumber(3, 14) };

        public void JoinMiniland(ClientSession session, ClientSession minilandOwner)
        {
            if (session.Character.Miniland.MapInstanceId == minilandOwner.Character.Miniland.MapInstanceId)
            {
                foreach (var mate in session.Character.Mates)
                {
                    if (session.Character.Miniland.Map.IsBlockedZone(mate.PositionX, mate.PositionY))
                    {
                        MapCell newPos = MinilandRandomPos(); mate.MapX = newPos.X;
                        mate.MapY = newPos.Y;
                        mate.PositionX = mate.MapX;
                        mate.PositionY = mate.MapY;
                    }

                    if (!mate.IsAlive || mate.Hp <= 0)
                    {
                        mate.Hp = mate.MaxHp / 2;
                        mate.Mp = mate.MaxMp / 2;
                        mate.IsAlive = true;
                        mate.ReviveDisposable?.Dispose();
                    }
                }
            }

            ChangeMapInstance(session.Character.CharacterId, minilandOwner.Character.Miniland.MapInstanceId,
                5, 8);
            if (session.Character.Miniland.MapInstanceId != minilandOwner.Character.Miniland.MapInstanceId)
            {
                session.SendPacket(UserInterfaceHelper.GenerateMsg(minilandOwner.Character.MinilandMessage,
                    0));
                session.SendPacket(minilandOwner.Character.GenerateMlinfobr());
                minilandOwner.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = session.Account.AccountId,
                    CharacterId = session.Character.CharacterId,
                    IpAddress = session.IpAddress,
                    LogData = "Miniland",
                    LogType = "World",
                    Timestamp = DateTime.Now
                });
                session.SendPacket(minilandOwner.Character.GenerateMinilandObjectForFriends());
            }
            else
            {
                session.SendPacket(session.Character.GenerateMlinfo());
                session.SendPacket(minilandOwner.Character.GetMinilandObjectList());
            }

            minilandOwner.Character.Mates.Where(s => !s.IsTeamMember).ToList()
                .ForEach(s => session.SendPacket(s.GenerateIn()));
            session.SendPackets(minilandOwner.Character.GetMinilandEffects());
            session.SendPacket(session.Character.GenerateSay(
                string.Format(Language.Instance.GetMessageFromKey("MINILAND_VISITOR"),
                    session.Character.GeneralLogs.CountLinq(s =>
                        s.LogData == "Miniland" && s.Timestamp.Day == DateTime.Now.Day),
                    session.Character.GeneralLogs.CountLinq(s => s.LogData == "Miniland")), 10));
        }

        // Server
        public void Kick(string characterName)
        {
            var session = Sessions.FirstOrDefault(s => s.Character?.Name.Equals(characterName) == true);
            session?.Disconnect();
        }

        // Map
        public void LeaveMap(long id)
        {
            var session = GetSessionByCharacterId(id);
            if (session == null)
            {
                return;
            }

            session.SendPacket(UserInterfaceHelper.GenerateMapOut());
            if (!session.Character.InvisibleGm)
            {
                session.Character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                    session.CurrentMapInstance?.Broadcast(session,
                        StaticPacketHelper.Out(UserType.Npc, s.MateTransportId), ReceiverType.AllExceptMe));
                session.CurrentMapInstance?.Broadcast(session,
                    StaticPacketHelper.Out(UserType.Player, session.Character.CharacterId),
                    ReceiverType.AllExceptMe);
            }
        }

        public void CharacterSynchronizingAtSaveProcess(long characterId, bool lockOrUnlock)
        {
            CommunicationServiceClient.Instance.AddOrRemoveSavingCharacters(characterId, lockOrUnlock);
        }

        public bool IsCharacterSaving(long characterId)
        {
            return CommunicationServiceClient.Instance.IsCharacterSaving(characterId);
        }
        
        public void LoadBazaar()
        {
            BazaarList = new ThreadSafeGenericList<BazaarItemLink>();
            OrderablePartitioner<BazaarItemDTO> bazaarPartitioner = Partitioner.Create(DAOFactory.BazaarItemDAO.LoadAll(), EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(bazaarPartitioner, new ParallelOptions { MaxDegreeOfParallelism = 8 }, bazaarItem =>
            {
                BazaarItemLink item = new BazaarItemLink
                {
                    BazaarItem = bazaarItem
                };
                CharacterDTO chara = DAOFactory.CharacterDAO.LoadById(bazaarItem.SellerId);
                if (chara != null)
                {
                    item.Owner = chara.Name;
                    item.Item = new ItemInstance(DAOFactory.ItemInstanceDAO.LoadById(bazaarItem.ItemInstanceId));
                }
                BazaarList.Add(item);
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Load] Bazaar Itemlist: {BazaarList.Count} - successfully loaded");
        }

        public void LoadBoxItem()
        {
            var box = DAOFactory.BoxItemDAO.LoadAll();
            BoxItems = box;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Load] {BoxItems.Count} Box Items has been loaded");
        }

        private void LoadCards()
        {
            Cards = new List<Card>();

            var bcards = DAOFactory.BCardDAO.LoadAll().ToArray().Where(s => s.CardId.HasValue);
            IEnumerable<CardDTO> cards = DAOFactory.CardDAO.LoadAll().ToArray();
            foreach (var card in cards)
            {
                var tmp = new Card(card)
                {
                    BCards = new List<BCard>()
                };

                foreach (var bcard in bcards.Where(s => s.CardId == tmp.CardId))
                {
                    tmp.BCards.Add(new BCard(bcard));
                }

                Cards.Add(tmp);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Load] {Cards.Count} Cards has been loaded");
        }

        public void LoadDropMonster()
        {
            _monsterDrops = new Dictionary<short, List<DropDTO>>();
            foreach (var monsterDropGrouping in DAOFactory.DropDAO.LoadAll().GroupBy(d => d.MonsterVNum))
            {
                if (monsterDropGrouping.Key.HasValue)
                {
                    _monsterDrops[monsterDropGrouping.Key.Value] =
                            monsterDropGrouping.OrderBy(d => d.DropChance).ToList();
                }
                else
                {
                    _generalDrops = monsterDropGrouping.ToList();
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Load] {_monsterDrops.Sum(i => i.Value.Count)} Drops has been loaded");
        }

        public void LoadEvent()
        {
            Schedules = ConfigurationManager.GetSection("eventScheduler") as List<Schedule>;
            StartedEvents = new List<EventType>();
        }

        public void LoadItem()
        {
            var items = DAOFactory.ItemDAO.LoadAll();
            var bcards = DAOFactory.BCardDAO.LoadAll().Where(s => s.ItemVNum.HasValue)
                .GroupBy(s => s.ItemVNum).ToDictionary(s => s.Key, s => s.ToArray());
            var rollItems = DAOFactory.RollGeneratedItemDAO.LoadAll().GroupBy(s => s.OriginalItemVNum)
                .ToDictionary(s => s.Key, s => s.ToArray());
            var item = new Dictionary<short, Item>();
            foreach (var itemDto in items)
            {
                Item newItem;
                switch (itemDto.ItemType)
                {
                    case ItemType.Box:
                        newItem = new BoxItem(itemDto);
                        newItem.PluginType = ItemPluginType.Box;
                        break;

                    case ItemType.Fashion:
                    case ItemType.Jewelery:
                    case ItemType.Specialist:
                    case ItemType.Weapon:
                    case ItemType.Armor:
                        newItem = new WearableItem(itemDto);
                        newItem.PluginType = ItemPluginType.Wearable;
                        break;

                    case ItemType.Food:
                        newItem = new FoodItem(itemDto);
                        newItem.PluginType = ItemPluginType.Food;
                        break;

                    case ItemType.Special:
                        newItem = new SpecialItem(itemDto);
                        newItem.PluginType = ItemPluginType.Special;
                        break;

                    case ItemType.Magical:
                    case ItemType.Shell:
                    case ItemType.Event:
                        newItem = new MagicalItem(itemDto);
                        newItem.PluginType = ItemPluginType.Magical;
                        break;

                    case ItemType.Potion:
                        newItem = new PotionItem(itemDto);
                        newItem.PluginType = ItemPluginType.Potion;
                        break;

                    case ItemType.Production:
                        newItem = new ProduceItem(itemDto);
                        newItem.PluginType = ItemPluginType.Produce;
                        break;

                    case ItemType.Snack:
                        newItem = new SnackItem(itemDto);
                        newItem.PluginType = ItemPluginType.Snack;
                        break;

                    case ItemType.Teacher:
                        newItem = new TeacherItem(itemDto);
                        newItem.PluginType = ItemPluginType.Teacher;
                        break;

                    case ItemType.Upgrade:
                        newItem = new UpgradeItem(itemDto);
                        newItem.PluginType = ItemPluginType.Upgrade;
                        break;

                    case ItemType.Title:
                        newItem = new TitleItem(itemDto);
                        newItem.PluginType = ItemPluginType.Title;
                        break;

                    default:
                        newItem = new NoFunctionItem(itemDto);
                        newItem.PluginType = ItemPluginType.NoFunction;
                        break;
                }

                if (bcards.TryGetValue(newItem.VNum, out var bcardDtos))
                {
                    foreach (var b in bcardDtos)
                    {
                        newItem.BCards.Add(new BCard(b));
                    }
                }

                if (rollItems.TryGetValue(newItem.VNum, out var rolls))
                {
                    newItem.RollGeneratedItems.AddRange(rolls);
                }

                item[itemDto.VNum] = newItem;
            }

            Items.AddRange(item.Values);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Load] {Items.Count} Items has been loaded");
        }

        private static void LoadMapsAndContent()
        {
            try
            {
                var i = 0;
                var monstercount = 0;

                var monsters = DAOFactory.MapMonsterDAO.LoadAll().GroupBy(s => s.MapId)
                    .ToDictionary(s => s.Key, s => s.ToArray());
                var npcs = DAOFactory.MapNpcDAO.LoadAll().GroupBy(s => s.MapId)
                    .ToDictionary(s => s.Key, s => s.ToArray());
                var portals = DAOFactory.PortalDAO.LoadAll().GroupBy(s => s.SourceMapId)
                    .ToDictionary(s => s.Key, s => s.ToArray());
                var mapTypes = DAOFactory.MapTypeMapDAO.LoadAll().ToArray();
                var mapTypeMap = DAOFactory.MapTypeDAO.LoadAll().ToArray();
                var respawns = DAOFactory.RespawnMapTypeDAO.LoadAll();

                foreach (var map in DAOFactory.MapDAO.LoadAll().ToArray())
                {
                    var guid = Guid.NewGuid();
                    var mapinfo = new Map(map.MapId, map.GridMapId, map.Data)
                    {
                        Music = map.Music,
                        Name = map.Name,
                        ShopAllowed = map.ShopAllowed,
                        XpRate = map.XpRate
                    };
                    var newMap = new MapInstance(mapinfo, guid, map.ShopAllowed,
                        MapInstanceType.BaseMapInstance, new InstanceBag(), true);
                    _mapinstances.TryAdd(guid, newMap);

                    if (portals.TryGetValue(map.MapId, out var port))
                    {
                        newMap.LoadPortals(port);
                    }

                    if (npcs.TryGetValue(map.MapId, out var np))
                    {
                        newMap.LoadNpcs(np);
                    }

                    if (monsters.TryGetValue(map.MapId, out var monst))
                    {
                        newMap.LoadMonsters(monst);
                    }

                    foreach (var mapNpc in newMap.Npcs)
                    {
                        mapNpc.MapInstance = newMap;
                        newMap.AddNPC(mapNpc);
                    }

                    foreach (var mapMonster in newMap.Monsters)
                    {
                        mapMonster.MapInstance = newMap;
                        newMap.AddMonster(mapMonster);
                    }


                    monstercount += newMap.Monsters.Count;
                    Maps.Add(mapinfo);
                    i++;
                }

                Console.WriteLine($"[Load] {i} Maps has been loaded");
                Console.WriteLine($"[Load] {monstercount} Monsters has been loaded");
            }
            catch (Exception e)
            {
                Logger.Log.Error("General Error", e);
            }
        }

        public void LoadMapNpc()
        {
            _mapNpcs = new Dictionary<short, List<MapNpc>>();
            var npcs = DAOFactory.MapNpcDAO.LoadAll().GroupBy(t => t.MapId);
            foreach (var mapNpcGrouping in npcs)
            {
                _mapNpcs[mapNpcGrouping.Key] = mapNpcGrouping.Select(t => t as MapNpc).ToList();
            }
            Console.WriteLine($"[Load] {_mapNpcs.Sum(i => i.Value.Count)} Map-NPCs has been loaded");
        }

        public void LoadMonsterSkill()
        {
            _monsterSkills = new Dictionary<short, List<NpcMonsterSkill>>();
            _allMonsterSkills = new ConcurrentBag<NpcMonsterSkill>();
            DAOFactory.NpcMonsterSkillDAO.LoadAll()
                .ForEach(s => _allMonsterSkills.Add(new NpcMonsterSkill(s)));
            foreach (var monsterSkillGrouping in DAOFactory.NpcMonsterSkillDAO.LoadAll().ToArray()
                .GroupBy(n => n.NpcMonsterVNum))
            {
                _monsterSkills[monsterSkillGrouping.Key] =
                        monsterSkillGrouping.Select(n => new NpcMonsterSkill(n)).ToList();
            }
            Console.WriteLine($"[Load] {_monsterSkills.Sum(i => i.Value.Count)} Monsterskills has been loaded");
        }

        private void LoadQuest()
        {
            Quests = new List<Quest>();
            var questRewards = DAOFactory.QuestRewardDAO.LoadAll();
            var questObjectives = DAOFactory.QuestObjectiveDAO.LoadAll();
            foreach (var questdto in DAOFactory.QuestDAO.LoadAll().ToArray())
            {
                var quest = new Quest(questdto);
                quest.QuestRewards = questRewards.Where(s => s.QuestId == quest.QuestId).ToList();
                quest.QuestObjectives = questObjectives.Where(s => s.QuestId == quest.QuestId).ToList();
                Quests.Add(quest);
            }

            FlowerQuestId = Quests.Find(q => q.QuestType == (byte)QuestType.FlowerQuest)?.QuestId;

            Console.WriteLine($"[Load] {Quests.Count} Quest has been loaded");
        }

        public void LoadRecipes()
        {
            var recipes = DAOFactory.RecipeDAO.LoadAll();
            var recipeItems = DAOFactory.RecipeItemDAO.LoadAll();
            IEnumerable<RecipeItemDTO> recipeItemDtos = recipeItems.ToList();

            _recipes = new ThreadSafeSortedList<short, Recipe>();

            foreach (var recipeGrouping in recipes)
            {
                var recipe = new Recipe(recipeGrouping)
                {
                    Items = new List<RecipeItemDTO>()
                };
                recipe.Items.AddRange(recipeItemDtos.Where(s => s.RecipeId == recipe.RecipeId));
                _recipes[recipeGrouping.RecipeId] = recipe;
            }

            Console.WriteLine($"[Load] {_recipes.Count} Recipes has been loaded");
        }

        public void LoadRecipesList()
        {
            _recipeLists = new ThreadSafeSortedList<int, RecipeListDTO>();
            foreach (var recipeListGrouping in DAOFactory.RecipeListDAO.LoadAll())
            {
                _recipeLists[recipeListGrouping.RecipeListId] = recipeListGrouping;
            }
            Console.WriteLine($"[Load] {_recipeLists.Count} Recipe Lists has been loaded");
        }

        public void LoadScriptedInstances()
        {
            Raids = new ConcurrentBag<ScriptedInstance>();
            Act6Raids = new ConcurrentBag<ScriptedInstance>();
            TimeSpaces = new ConcurrentBag<ScriptedInstance>();
            foreach (var map in _mapinstances)
            {
                if (map.Value.MapInstanceType == MapInstanceType.BaseMapInstance)
                {
                    map.Value.ScriptedInstances.Clear();
                    map.Value.Portals.Clear();
                    foreach (var si in DAOFactory.ScriptedInstanceDAO.LoadByMap(map.Value.Map.MapId)
                                                 .ToList())
                    {
                        var siObj = new ScriptedInstance(si);
                        switch (siObj.Type)
                        {
                            case ScriptedInstanceType.TimeSpace:
                            case ScriptedInstanceType.QuestTimeSpace:
                                siObj.LoadGlobals();
                                if (siObj.Script != null)
                                {
                                    TimeSpaces.Add(siObj);
                                }

                                map.Value.ScriptedInstances.Add(siObj);
                                break;

                            case ScriptedInstanceType.Raid:
                                siObj.LoadGlobals();
                                if (siObj.Script != null)
                                {
                                    Raids.Add(siObj);
                                }

                                var port = new Portal
                                {
                                    Type = (byte)PortalType.Raid,
                                    SourceMapId = siObj.MapId,
                                    SourceX = siObj.PositionX,
                                    SourceY = siObj.PositionY
                                };
                                map.Value.Portals.Add(port);
                                break;
                        }
                    }

                    map.Value.LoadPortals();
                    map.Value.MapClear();
                }
            }
        }

        public void LoadShop()
        {
            _shops = new Dictionary<int, Shop>();
            foreach (var shopGrouping in DAOFactory.ShopDAO.LoadAll())
            {
                var shop = new Shop(shopGrouping);
                _shops[shopGrouping.MapNpcId] = shop;
                shop.Initialize();
            }

            Console.WriteLine($"[Load] {_shops.Count} Shops has been loaded");
        }

        private void LoadShopItems()
        {
            _shopItems = new Dictionary<int, List<ShopItemDTO>>();
            foreach (var shopItemGrouping in DAOFactory.ShopItemDAO.LoadAll().GroupBy(s => s.ShopId))
            {
                _shopItems[shopItemGrouping.Key] = shopItemGrouping.ToList();
            }
            Console.WriteLine($"[Load] {_shopItems.Sum(i => i.Value.Count)} Shop-Items has been loaded");
        }

        public void LoadShopSkills()
        {
            _shopSkills = new Dictionary<int, List<ShopSkillDTO>>();
            foreach (var shopSkillGrouping in DAOFactory.ShopSkillDAO.LoadAll().GroupBy(s => s.ShopId))
            {
                _shopSkills[shopSkillGrouping.Key] = shopSkillGrouping.ToList();
            }
        }

        private static void LoadSkills()
        {
            IEnumerable<ComboDTO> combos = DAOFactory.ComboDAO.LoadAll().ToArray();
            var bcards = DAOFactory.BCardDAO.LoadAll().ToArray().Where(s => s.SkillVNum.HasValue);
            foreach (var skillItem in DAOFactory.SkillDAO.LoadAll().ToArray())
            {
                var tmp = new Skill(skillItem);
                if (!(tmp is Skill skillObj))
                {
                    return;
                }

                skillObj.Combos.AddRange(combos.Where(s => s.SkillVNum == skillObj.SkillVNum).ToList());
                skillObj.BCards = new List<BCard>();

                foreach (var o in bcards.Where(s => s.SkillVNum == skillObj.SkillVNum))
                {
                    skillObj.BCards.Add(new BCard(o));
                }

                Skills.Add(skillObj);
            }
        }

        public void LoadTeleporter()
        {
            _teleporters = new Dictionary<int, List<TeleporterDTO>>();
            foreach (var teleporterGrouping in DAOFactory.TeleporterDAO.LoadAll().GroupBy(t => t.MapNpcId))
            {
                _teleporters[teleporterGrouping.Key] = teleporterGrouping.Select(t => t).ToList();
            }
            Console.WriteLine($"[Load] {_teleporters.Sum(i => i.Value.Count)} Teleporters has been loaded");
        }

        public bool MapNpcHasRecipe(int mapNpcId)
        {
            return _recipeLists.Any(r => r.MapNpcId == mapNpcId);
        }

        public void RefreshRanking()
        {
            TopComplimented = DAOFactory.CharacterDAO.GetTopCompliment();
            TopPoints = DAOFactory.CharacterDAO.GetTopPoints();
            TopReputation = DAOFactory.CharacterDAO.GetTopReputation();
        }

        public void RefreshDailyMissions()
        {
            foreach (var fsm in DAOFactory.FamilySkillMissionDAO.LoadAll())
            {
                if (!FamilySystemHelper.IsDaily(fsm.ItemVNum) && fsm.ItemVNum > 9603) continue;

                DAOFactory.FamilySkillMissionDAO.DailyReset(fsm);
            }
        }

        public void RelationRefresh(long relationId)
        {
            _inRelationRefreshMode = true;
            CommunicationServiceClient.Instance.UpdateRelation(ServerGroup, relationId);
            SpinWait.SpinUntil(() => !_inRelationRefreshMode);
        }

        // Map
        public void ReviveFirstPosition(long characterId)
        {
            var session = GetSessionByCharacterId(characterId);
            if (session?.Character.Hp <= 0)
            {
                if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance ||
                    session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    session.Character.Hp = (int)session.Character.HPLoad();
                    session.Character.Mp = (int)session.Character.MPLoad();
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                }
                else
                {
                    if (ChannelId == 51)
                    {
                        if (session.CurrentMapInstance.MapInstanceId ==
                            session.Character.Family?.Act4RaidBossMap?.MapInstanceId)
                        {
                            session.Character.Hp = 1;
                            session.Character.Mp = 1;

                            switch (session.Character.Family.Act4Raid.MapInstanceType)
                            {
                                case MapInstanceType.Act4Morcos:
                                    Instance.ChangeMapInstance(session.Character.CharacterId,
                                        session.Character.Family.Act4Raid.MapInstanceId, 43, 179);
                                    break;

                                case MapInstanceType.Act4Hatus:
                                    Instance.ChangeMapInstance(session.Character.CharacterId,
                                        session.Character.Family.Act4Raid.MapInstanceId, 15, 9);
                                    break;

                                case MapInstanceType.Act4Calvina:
                                    Instance.ChangeMapInstance(session.Character.CharacterId,
                                        session.Character.Family.Act4Raid.MapInstanceId, 24, 6);
                                    break;

                                case MapInstanceType.Act4Berios:
                                    Instance.ChangeMapInstance(session.Character.CharacterId,
                                        session.Character.Family.Act4Raid.MapInstanceId, 20, 20);
                                    break;
                            }
                        }
                        else
                        {
                            session.Character.Hp = (int)session.Character.HPLoad();
                            session.Character.Mp = (int)session.Character.MPLoad();
                            var x = (short)(39 + RandomNumber(-2, 3));
                            var y = (short)(42 + RandomNumber(-2, 3));
                            if (session.Character.Faction == FactionType.Angel)
                            {
                                ChangeMap(session.Character.CharacterId, 130, x, y);
                            }
                            else if (session.Character.Faction == FactionType.Demon)
                            {
                                ChangeMap(session.Character.CharacterId, 131, x, y);
                            }
                        }
                    }
                    else
                    {
                        session.Character.Hp = 1;
                        session.Character.Mp = 1;
                        if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                        {
                            var resp = session.Character.Respawn;
                            var x = (short)(resp.DefaultX + RandomNumber(-3, 3));
                            var y = (short)(resp.DefaultY + RandomNumber(-3, 3));
                            ChangeMap(session.Character.CharacterId, resp.DefaultMapId, x, y);
                        }
                        else
                        {
                            Instance.ChangeMap(session.Character.CharacterId, session.Character.MapId,
                                session.Character.MapX, session.Character.MapY);
                        }
                    }

                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                }
            }
        }

        public void SaveAll()
        {
            try
            {
                CommunicationServiceClient.Instance.CleanupOutdatedSession();
                foreach (ClientSession sess in Sessions)
                {
                    sess.Character?.Save();
                }
                DAOFactory.BazaarItemDAO.RemoveOutDated();
            }
            finally
            {
                Logger.LogEvent("SAVING COMPLETED!", null);
            }
        }

        public async Task ShutdownTaskAsync(int Time = 5)
        {
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_MIN"), Time));
            if (Time > 1)
            {
                for (int i = 0; i < 60 * (Time - 1); i++)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    if (Instance.ShutdownStop)
                    {
                        Instance.ShutdownStop = false;
                        return;
                    }
                }
                Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_MIN"), 1));
            }
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 30));
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 10));
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                if (Instance.ShutdownStop)
                {
                    Instance.ShutdownStop = false;
                    return;
                }
            }
            InShutdown = true;
            foreach (ClientSession sess in Sessions)
            {
                sess.Character?.Dispose();
            }
            Instance.SaveAll();
            CommunicationServiceClient.Instance.UnregisterWorldServer(WorldId);
            if (IsReboot)
            {
                if (ChannelId == 51)
                {
                    await Task.Delay(16000).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay((ChannelId - 1) * 2000).ConfigureAwait(false);
                }
                Process.Start("OpenNos.World.exe", $"--nomsg{( ChannelId == 51 ? $" --port {Configuration.Act4Port}" : "")}");
            }
            Environment.Exit(0);
        }

        public void SynchronizeSheduling()
        {
            if (Schedules.FirstOrDefault(s => s.Event == EventType.TALENTARENA)?.Time is TimeSpan arenaOfTalentsTime
                && IsTimeBetween(DateTime.Now, arenaOfTalentsTime, arenaOfTalentsTime.Add(new TimeSpan(4, 0, 0))))
            {
                EventHelper.GenerateEvent(EventType.TALENTARENA);
            }
            Schedules.Where(s => s.Event == EventType.LOD).ToList().ForEach(lodSchedule =>
            {
                if (IsTimeBetween(DateTime.Now, lodSchedule.Time, lodSchedule.Time.Add(new TimeSpan(2, 0, 0))))
                {
                    EventHelper.GenerateEvent(EventType.LOD);
                }
            });
        }

        //private void InitAllProperty()
        //{
        //    Act4RaidStart = DateTime.Now;
        //    Act4AngelStat = new Act4Stat();
        //    Act4DemonStat = new Act4Stat();
        //    Act6AngelStat = new Act6Stat();
        //    Act6DemonStat = new Act6Stat();
        //    ChatLogs = new ThreadSafeGenericList<ChatLogDTO>();
        //    CommandsLogs = new ThreadSafeGenericList<LogCommandsDTO>();
        //    LastFCSent = DateTime.Now;
        //    CharacterScreenSessions = new ThreadSafeSortedList<long, ClientSession>();
        //}

        public void TeleportOnRandomPlaceInMap(ClientSession session, Guid guid)
        {
            var map = GetMapInstance(guid);
            if (guid != default)
            {
                var pos = map.Map.GetRandomPosition();
                if (pos == null)
                {
                    return;
                }

                ChangeMapInstance(session.Character.CharacterId, guid, pos.X, pos.Y);
            }
        }

        // Server
        public void UpdateGroup(long charId)
        {
            try
            {
                if (Groups != null)
                {
                    var myGroup = Groups.Find(s => s.IsMemberOfGroup(charId));
                    if (myGroup == null)
                    {
                        return;
                    }

                    var groupMembers = Groups.Find(s => s.IsMemberOfGroup(charId))?.Sessions;
                    if (groupMembers != null)
                    {
                        foreach (var session in groupMembers.GetAllItems())
                        {
                            session.SendPacket(session.Character.GeneratePinit());
                            session.SendPackets(session.Character.GeneratePst());
                            session.SendPacket(session.Character.GenerateStat());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void StopServer()
        {
            Instance.ShutdownStop = true;
            Instance.TaskShutdown = null;
            Instance.SaveAll();
        }

        internal List<NpcMonsterSkill> GetNpcMonsterSkillsByMonsterVNum(short npcMonsterVNum) => _monsterSkills.ContainsKey(npcMonsterVNum)
                ? _monsterSkills[npcMonsterVNum]
                : new List<NpcMonsterSkill>();

        internal Shop GetShopByMapNpcId(int mapNpcId) => _shops.ContainsKey(mapNpcId) ? _shops[mapNpcId] : null;

        internal List<ShopItemDTO> GetShopItemsByShopId(int shopId) => _shopItems.ContainsKey(shopId) ? _shopItems[shopId] : new List<ShopItemDTO>();

        internal List<ShopSkillDTO> GetShopSkillsByShopId(int shopId) => _shopSkills.ContainsKey(shopId) ? _shopSkills[shopId] : new List<ShopSkillDTO>();

        internal List<TeleporterDTO> GetTeleportersByNpcVNum(int npcMonsterVNum)
        {
            if (_teleporters?.ContainsKey(npcMonsterVNum) == true)
            {
                return _teleporters[npcMonsterVNum];
            }

            return new List<TeleporterDTO>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // _monsterDrops.Dispose();
                ThreadSafeGroupList.Dispose();

                //_monsterSkills.Dispose();
                //_shopSkills.Dispose();
                //_shopItems.Dispose();
                //_shops.Dispose();
                _recipes.Dispose();
                //_mapNpcs.Dispose();
                //_teleporters.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        private static void Act4StatProcess()
        {
            if (Instance.ChannelId != 51)
            {
                return;
            }

            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = Instance.WorldId,
                Message =
                            $"[A4 Status] Angels: {Instance.Act4AngelStat.Percentage / 100} % Demons: {Instance.Act4DemonStat.Percentage / 100} %",
                Type = MessageType.Shout
            });
        }

        // Server
        //private static void BotProcess()
        //{
        //    try
        //    {
        //        Shout(Language.Instance.GetMessageFromKey($"BOT_MESSAGE_{RandomNumber(0, 4)}"));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e);
        //    }
        //}

        private static void LoadNpcMonsters()
        {
            var bcards = DAOFactory.BCardDAO.LoadAll().ToArray().Where(s => s.NpcMonsterVNum.HasValue);
            foreach (var npcMonster in DAOFactory.NpcMonsterDAO.LoadAll().ToArray())
            {
                var tmp = new NpcMonster(npcMonster);

                if (!(tmp is NpcMonster monster))
                {
                    continue;
                }

                // TODO: remove that after
                monster.Initialize();
                monster.BCards = new List<BCard>();

                foreach (var s in bcards.Where(s =>
                    s.NpcMonsterVNum == (monster.OriginalNpcMonsterVNum > 0
                        ? npcMonster.OriginalNpcMonsterVNum
                        : monster.NpcMonsterVNum)))
                {
                    monster.BCards.Add(new BCard(s));
                }

                Npcs.Add(monster);
            }

            Logger.Info(
                string.Format(Language.Instance.GetMessageFromKey("NPCMONSTERS_LOADED"), Npcs.Count));
        }

        private static void OnGlobalEvent(object sender, EventArgs e)
        {
            var tuple = (Tuple<EventType, byte>)sender;
            EventHelper.GenerateEvent(tuple.Item1, value: tuple.Item2);
        }

        private static void OnRestart(object sender, EventArgs e)
        {
            if (Instance.TaskShutdown != null)
            {
                Instance.IsReboot = false;
                Instance.ShutdownStop = true;
                Instance.TaskShutdown = null;
            }
            else
            {
                Instance.IsReboot = true;
                Instance.TaskShutdown = Instance.ShutdownTaskAsync((int)sender);
            }
        }

        private static void OnShutdown(object sender, EventArgs e)
        {
            if (Instance.TaskShutdown != null)
            {
                Instance.ShutdownStop = true;
                Instance.TaskShutdown = null;
            }
            else
            {
                Instance.TaskShutdown = Instance.ShutdownTaskAsync();
                Instance.TaskShutdown.Start();
            }
        }

        private static void ReviveTask(ClientSession session)
        {
            Task.Factory.StartNew(async () =>
            {
                var revive = true;
                for (var i = 1; i <= 30; i++)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    if (session.Character.Hp > 0)
                    {
                        revive = false;
                        break;
                    }
                }

                if (revive)
                {
                    Instance.ReviveFirstPosition(session.Character.CharacterId);
                }
            });
        }

        private void Act4FlowerProcess()
        {
            foreach (var map in GetAllMapInstances().Where(s =>
                s.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4) &&
                s.Npcs.Count(o => o.NpcVNum == 2004 && o.IsOut) < s.Npcs.Count(n => n.NpcVNum == 2004)))
                foreach (var i in map.Npcs.Where(s => s.IsOut && s.NpcVNum == 2004))
                {
                    var randomPos = map.Map.GetRandomPosition();
                    i.MapX = randomPos.X;
                    i.MapY = randomPos.Y;
                    i.MapInstance.Broadcast(i.GenerateIn());
                }
        }

        private void FlushLogs(bool init = false)
        {
            if (init)
            {
                Logger.Info(Language.Instance.GetMessageFromKey("LOG_INITIALIZED"));
            }

            Observable.Interval(TimeSpan.FromMinutes(10)).Subscribe(observer =>
            {
                LogHelper.Instance.InsertAllLogs();
                LogHelper.Instance.ClearAllList();
            });
        }

        private void GroupProcess()
        {
            try
            {
                if (Groups != null)
                {
                    foreach (var grp in Groups)
                        foreach (var session in grp.Sessions.GetAllItems())
                        {
                            if (grp.GroupType == GroupType.Group)
                            {
                                session.SendPackets(grp.GeneratePst(session));
                            }
                            else
                            {
                                session.SendPacket(grp.GenerateRdlst());
                            }
                        }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void InitAllProperty()
        {
            Act4RaidStart = DateTime.Now;
            Act4AngelStat = new Act4Stat();
            Act4DemonStat = new Act4Stat();
            Act6Erenia = new Act4Stat();
            Act6Zenas = new Act4Stat();
            //ChatLogs = new ThreadSafeGenericList<ChatLogDTO>();
            CommandsLogs = new ThreadSafeGenericList<LogCommandsDTO>();
            LastFCSent = DateTime.Now;
            CharacterScreenSessions = new ThreadSafeSortedList<long, ClientSession>();
        }

        private bool IsTimeBetween(DateTime dateTime, TimeSpan start, TimeSpan end)
        {
            var now = dateTime.TimeOfDay;

            return start < end ? start <= now && now <= end : !(end < now && now < start);
        }

        private void LaunchEvents()
        {
            ThreadSafeGroupList = new ThreadSafeSortedList<long, Group>();

            Observable.Interval(TimeSpan.FromMinutes(5)).Subscribe(x => SaveAllProcess());
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(x => Act4Process());
            Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => GroupProcess());
            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(x => Act4FlowerProcess());

            //Observable.Interval(TimeSpan.FromHours(3)).Subscribe(x => BotProcess()); activate it later
            Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(x => MaintenanceProcess());

            Observable.Interval(TimeSpan.FromMinutes(30)).Subscribe(x => Act4StatProcess());

            EventHelper.Instance.RunEvent(new EventContainer(
                GetMapInstance(GetBaseMapInstanceIdByMapId(98)), EventActionType.NPCSEFFECTCHANGESTATE,
                true));
            foreach (var schedule in Schedules)
            {
                Observable.Timer(
                        TimeSpan.FromSeconds(EventHelper.GetMilisecondsBeforeTime(schedule.Time).TotalSeconds),
                        TimeSpan.FromDays(1)).Subscribe(e =>
                {
                    if (schedule.DayOfWeek == "" || schedule.DayOfWeek == DateTime.Now.DayOfWeek.ToString())
                    {
                        EventHelper.GenerateEvent(schedule.Event, schedule.LvlBracket);
                    }
                });
            }

            EventHelper.GenerateEvent(EventType.ACT4SHIP);

            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(x => RemoveItemProcess());
            Observable.Interval(TimeSpan.FromMilliseconds(400)).Subscribe(x =>
            {
                foreach (var map in _mapinstances)
                {
                    foreach (var npc in map.Value.Npcs)
                    {
                        npc.StartLife();
                    }

                    foreach (var monster in map.Value.Monsters)
                    {
                        monster.StartLife();
                    }
                }
            });

            CommunicationServiceClient.Instance.SessionKickedEvent += OnSessionKicked;
            CommunicationServiceClient.Instance.MessageSentToCharacter += OnMessageSentToCharacter;
            CommunicationServiceClient.Instance.FamilyRefresh += OnFamilyRefresh;
            CommunicationServiceClient.Instance.RelationRefresh += OnRelationRefresh;
            CommunicationServiceClient.Instance.StaticBonusRefresh += OnStaticBonusRefresh;
            CommunicationServiceClient.Instance.BazaarRefresh += OnBazaarRefresh;
            CommunicationServiceClient.Instance.PenaltyLogRefresh += OnPenaltyLogRefresh;
            CommunicationServiceClient.Instance.GlobalEvent += OnGlobalEvent;
            CommunicationServiceClient.Instance.ShutdownEvent += OnShutdown;
            CommunicationServiceClient.Instance.RestartEvent += OnRestart;
            ConfigurationServiceClient.Instance.ConfigurationUpdate += OnConfiguratinEvent;
            MailServiceClient.Instance.MailSent += OnMailSent;
            _lastGroupId = 1;
        }

        private void LoadAct7Ship()
        {
            if (DAOFactory.MapDAO.LoadById(2629) == null)
            {
                return;
            }

            Act7Ship = GenerateMapInstance(2629, MapInstanceType.Act7Ship, new InstanceBag());
        }

        private void LoadBannedCharacters()
        {
            BannedCharacters.Clear();
            DAOFactory.CharacterDAO.LoadAll().ToList().ForEach(s =>
            {
                if (s.State != CharacterState.Active || DAOFactory.PenaltyLogDAO.LoadByAccount(s.AccountId)
                        .Any(c => c.DateEnd > DateTime.Now && c.Penalty == PenaltyType.Banned))
                {
                    BannedCharacters.Add(s.CharacterId);
                }
            });
        }

        private void LoadBossEntities()
        {
            BossVNums = new List<short>
            {
                580, //Perro infernal
                582, //Ginseng fuerte
                583, //Basilisco fuerte
                584, //Rey Tubérculo
                585, //Gran Rey Tubérculo
                586, //Rey Tubérculo monstruoso
                588, //Pollito gigante
                589, //Mandra milenaria
                590, //Perro guardián del infierno
                591, //Gallo de pelea gigante
                592, //Patata milenaria
                593, //Perro de Satanás
                594, //Barepardo gigante
                595, //Boing satélite
                596, //Raíz demoníaca
                597, //Slade musculoso
                598, //Rey de las tinieblas
                599, //Gólem slade
                600, //Mini Castra
                601, //Caballero de la muerte
                602, //Sanguijuela gorda
                603, //Ginseng milenario
                604, //Rey basilisco
                605, //Gigante guerrero
                606, //Árbol maldito
                607, //Rey cangrejo
                2529, //Castra Oscuro
                1904, //Sombra de Kertos
                796, //Rey Pollo
                388, //Rey Pollo
                774, //Reina Gallina
                2331, //Diablilla Hongbi
                2332, //Diablilla Cheongbi
                2309, //Vulpina
                2322, //Maru
                1381, //Jack O´Lantern
                2357, //Lola Cucharón
                533, //Cabeza grande de muñeco de nieve
                1500, //Capitán Pete O'Peng
                282, //Mamá cubi
                284, //Ginseng
                285, //Castra Oscuro
                289, //Araña negra gigante
                286, //Slade gigante
                587, //Lord Mukraju
                563, //Maestro Morcos
                629, //Lady Calvina
                624, //Lord Berios
                577, //Maestro Hatus
                557, //Ross Hisrant
                2305, //Caligor
                2326, //Bruja Laurena
                2327, //Bestia de Laurena
                2639, //Yertirán podrido
                1028, //Ibrahim
                2034, //Lord Draco
                2049, //Glacerus, el frío
                1044, //Valakus, Rey del Fuego
                1905, //Sombra de Valakus
                1046, //Kertos, el Perro Demonio
                1912, //Sombra de Kertos
                637, //Perro demonio Kertos fuerte
                1099, //Fantasma de Grenigas
                1058, //Grenigas, el Dios del Fuego
                2619, //Fafnir, el Codicioso
                2504, //Zenas
                2514, //Erenia
                2574, //Fernon incompleta
                679, //Guardián de los ángeles
                680, //Guardián de los demonios
                967, //Altar de los ángeles
                968, //Altar de los diablo
                533 // Huge Snowman Head
            };
            MapBossVNums = new List<short>
            {
                580, //Perro infernal
                582, //Ginseng fuerte
                583, //Basilisco fuerte
                584, //Rey Tubérculo
                585, //Gran Rey Tubérculo
                586, //Rey Tubérculo monstruoso
                588, //Pollito gigante
                589, //Mandra milenaria
                590, //Perro guardián del infierno
                591, //Gallo de pelea gigante
                592, //Patata milenaria
                593, //Perro de Satanás
                594, //Barepardo gigante
                595, //Boing satélite
                596, //Raíz demoníaca
                597, //Slade musculoso
                598, //Rey de las tinieblas
                599, //Gólem slade
                600, //Mini Castra
                601, //Caballero de la muerte
                602, //Sanguijuela gorda
                603, //Ginseng milenario
                604, //Rey basilisco
                605, //Gigante guerrero
                606, //Árbol maldito
                607, //Rey cangrejo
                2529, //Castra Oscuro
                1904, //Sombra de Kertos
                1346, //Baúl de la banda de ladrones
                1347, //Baúl del tesoro del olvido
                1348, //Baúl extraño
                1384, //Halloween
                1906,
                1905,
                2350
            };
        }
        
        private void LoadCharacterRelations()
        {
            CharacterRelations = DAOFactory.CharacterRelationDAO.LoadAll().ToList();
            PenaltyLogs = DAOFactory.PenaltyLogDAO.LoadAll().ToList();
        }

        private void LoadFamilies()
        {
            FamilyList = new ThreadSafeSortedList<long, Family>();
            Parallel.ForEach(DAOFactory.FamilyDAO.LoadAll(), familyDto =>
            {
                Family family = new Family(familyDto)
                {
                    FamilyCharacters = new List<FamilyCharacter>()
                };
                foreach (FamilyCharacterDTO famchar in DAOFactory.FamilyCharacterDAO.LoadByFamilyId(family.FamilyId).ToList())
                {
                    family.FamilyCharacters.Add(new FamilyCharacter(famchar));
                }
                foreach (FamilySkillMissionDTO famskill in DAOFactory.FamilySkillMissionDAO.LoadByFamilyId(family.FamilyId).ToList())
                {
                    family.FamilySkillMissions.Add(new FamilySkillMission(famskill));
                }
                FamilyCharacter familyCharacter = family.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                if (familyCharacter != null)
                {
                    family.Warehouse = new Inventory(new Character(familyCharacter.Character));
                    foreach (ItemInstanceDTO inventory in DAOFactory.ItemInstanceDAO.LoadByCharacterId(familyCharacter.CharacterId).Where(s => s.Type == InventoryType.FamilyWareHouse).ToList())
                    {
                        inventory.CharacterId = familyCharacter.CharacterId;
                        family.Warehouse[inventory.Id] = new ItemInstance(inventory);
                    }
                }
                family.FamilyLogs = DAOFactory.FamilyLogDAO.LoadByFamilyId(family.FamilyId).ToList();
                FamilyList[family.FamilyId] = family;
            });
        }

        private void LoadFamilyArena()
        {
            if (DAOFactory.MapDAO.LoadById(2106) == null)
            {
                return;
            }

            FamilyArenaInstance =
                GenerateMapInstance(2106, MapInstanceType.ArenaInstance, new InstanceBag());
            FamilyArenaInstance.IsPVP = true;

            var portal = new Portal
            {
                SourceMapId = 2106,
                SourceX = 38,
                SourceY = 3,
                DestinationMapId = 1,
                DestinationX = 0,
                DestinationY = 0,
                Type = -1
            };

            FamilyArenaInstance.CreatePortal(portal);
        }

        // This is a function being called when the server starts
        // This function actually loads all the Instances of the gemstone maps, example:
        // SP1, SP2, SP3... You know these maps are the same map, but different gemstone inside of it
        private void LoadGemStone()
        {
            // If map 2107 doesn't exist in DB, don't do this
            if (DAOFactory.MapDAO.LoadById(2107) == null)
            {
                return;
            }

            // Instantiates the object of a portal, that will be in the mapinstance
            var portal = new Portal
            {
                SourceMapId = 2107,
                SourceX = 10,
                SourceY = 5,
                DestinationMapId = 1,
                DestinationX = 0,
                DestinationY = 0,
                Type = -1
            };

            // F
            void loadSpecialistGemMap(short npcVNum)
            {
                // 
                MapInstance specialistGemMapInstance;
                specialistGemMapInstance = GenerateMapInstance(2107, MapInstanceType.GemmeStoneInstance,
                    new InstanceBag());
                specialistGemMapInstance.Npcs.Where(s => s.NpcVNum != npcVNum).ToList()
                    .ForEach(s => specialistGemMapInstance.RemoveNpc(s));
                specialistGemMapInstance.CreatePortal(portal);
                SpecialistGemMapInstances.Add(specialistGemMapInstance);
            }

            loadSpecialistGemMap(932); // Pajama
            loadSpecialistGemMap(933); // SP 1
            loadSpecialistGemMap(934); // SP 2
            loadSpecialistGemMap(948); // SP 3
            loadSpecialistGemMap(954); // SP 4
            Console.WriteLine($"[Load] The Mysterious Soulgems has been loaded");
            loadSpecialistGemMap(958); // idk
        }

        private void LoadNormalArena()
        {
            if (DAOFactory.MapDAO.LoadById(2106) == null) //2006
            {
                return;
            }

            ArenaInstance = GenerateMapInstance(2106, MapInstanceType.ArenaInstance, new InstanceBag()); //2006
            ArenaInstance.IsPVP = true;

            var portal = new Portal
            {
                SourceMapId = 2106, // 2006
                SourceX = 38, // 37
                SourceY = 3, // 15
                DestinationMapId = 1,
                DestinationX = 0,
                DestinationY = 0,
                Type = -1
            };

            ArenaInstance.CreatePortal(portal);
        }

        private void MaintenanceProcess()
        {
            List<ClientSession> sessions = Sessions.Where(c => c.IsConnected).ToList();
            MaintenanceLogDTO maintenanceLog = DAOFactory.MaintenanceLogDAO.LoadFirst();
            if (maintenanceLog != null)
            {
                if (maintenanceLog.DateStart <= DateTime.Now)
                {
                    Logger.LogUserEvent("MAINTENANCE_STATE", "Caller: ServerManager", $"[Maintenance]{Language.Instance.GetMessageFromKey("MAINTENANCE_PLANNED")}");
                    sessions.Where(s => s.Account.Authority < AuthorityType.Administrator).ToList().ForEach(session => session.Disconnect());
                }
                else if (LastMaintenanceAdvert.AddMinutes(1) <= DateTime.Now && maintenanceLog.DateStart <= DateTime.Now.AddMinutes(5))
                {
                    int min = (maintenanceLog.DateStart - DateTime.Now).Minutes;
                    if (min != 0)
                    {
                        Shout($"Maintenance will begin in {min} minutes");
                    }
                    LastMaintenanceAdvert = DateTime.Now;
                }
            }
        }

        private void OnBazaarRefresh(object sender, EventArgs e)
        {
            long bazaarId = (long)sender;
            BazaarItemDTO bzdto = DAOFactory.BazaarItemDAO.LoadById(bazaarId);
            BazaarItemLink bzlink = BazaarList.Find(s => s.BazaarItem.BazaarItemId == bazaarId);
            lock (BazaarList)
            {
                if (bzdto != null)
                {
                    CharacterDTO chara = DAOFactory.CharacterDAO.LoadById(bzdto.SellerId);
                    if (bzlink != null)
                    {
                        BazaarList.Remove(bzlink);
                        bzlink.BazaarItem = bzdto;
                        bzlink.Owner = chara.Name;
                        bzlink.Item = new ItemInstance(DAOFactory.ItemInstanceDAO.LoadById(bzdto.ItemInstanceId));
                        BazaarList.Add(bzlink);
                    }
                    else
                    {
                        BazaarItemLink item = new BazaarItemLink
                        {
                            BazaarItem = bzdto
                        };
                        if (chara != null)
                        {
                            item.Owner = chara.Name;
                            item.Item = new ItemInstance(DAOFactory.ItemInstanceDAO.LoadById(bzdto.ItemInstanceId));
                        }
                        BazaarList.Add(item);
                    }
                }
                else if (bzlink != null)
                {
                    BazaarList.Remove(bzlink);
                }
            }
            InBazaarRefreshMode = false;
        }
        
        private void OnConfiguratinEvent(object sender, EventArgs e)
        {
            Configuration = (ConfigurationObject)sender;
        }

        private void OnFamilyRefresh(object sender, EventArgs e)
        {
            var tuple = (Tuple<long, bool>)sender;
            var familyId = tuple.Item1;
            var famdto = DAOFactory.FamilyDAO.LoadById(familyId);
            var fam = FamilyList[familyId];
            lock (FamilyList)
            {
                if (famdto != null)
                {
                    var newFam = new Family(famdto);
                    if (fam != null)
                    {
                        newFam.LandOfDeath = fam.LandOfDeath;
                        newFam.Act4Raid = fam.Act4Raid;
                        newFam.Act4RaidBossMap = fam.Act4RaidBossMap;
                    }

                    newFam.FamilyCharacters = new List<FamilyCharacter>();
                    foreach (var famchar in DAOFactory.FamilyCharacterDAO.LoadByFamilyId(famdto.FamilyId)
                        .ToList())
                    {
                        newFam.FamilyCharacters.Add(new FamilyCharacter(famchar));
                    }

                    foreach (FamilySkillMissionDTO famskill in DAOFactory.FamilySkillMissionDAO.LoadByFamilyId(famdto.FamilyId).ToList())
                    {
                        newFam.FamilySkillMissions.Add(new FamilySkillMission(famskill));
                    }

                    var familyHead = newFam.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                    if (familyHead != null)
                    {
                        newFam.Warehouse = new Inventory(new Character(familyHead.Character));
                        foreach (var inventory in DAOFactory.ItemInstanceDAO
                            .LoadByCharacterId(familyHead.CharacterId)
                            .Where(s => s.Type == InventoryType.FamilyWareHouse).ToList())
                        {
                            inventory.CharacterId = familyHead.CharacterId;
                            newFam.Warehouse[inventory.Id] = new ItemInstance(inventory);
                        }
                    }

                    newFam.FamilyLogs = DAOFactory.FamilyLogDAO.LoadByFamilyId(famdto.FamilyId).ToList();
                    FamilyList[familyId] = newFam;

                    foreach (var session in Sessions.Where(s =>
                        newFam.FamilyCharacters.Any(m => m.CharacterId == s.Character.CharacterId)))
                    {
                        if (session.Character.LastFamilyLeave < DateTime.Now.AddDays(-1).Ticks)
                        {
                            session.Character.Family = newFam;

                            if (tuple.Item2)
                            {
                                session.Character.ChangeFaction((FactionType)newFam.FamilyFaction);
                            }
                            session?.CurrentMapInstance?.Broadcast(session?.Character?.GenerateGidx());
                        }
                        session.Character.Family = newFam;

                        if (tuple.Item2)
                        {
                            session.Character.ChangeFaction((FactionType)newFam.FamilyFaction);
                        }

                        session?.CurrentMapInstance?.Broadcast(session?.Character?.GenerateGidx());
                        session?.SendPacket(session?.Character.GenerateFmi());
                        session?.SendPacket(session?.Character.GenerateFmp());
                    }
                }
                else if (fam != null)
                {
                    lock (FamilyList)
                    {
                        FamilyList.Remove(fam.FamilyId);
                    }

                    foreach (var sess in Sessions.Where(s => fam.FamilyCharacters.Any(f => f.CharacterId.Equals(s.Character.CharacterId))))
                    {
                        sess.Character.Family = null;
                        sess.SendPacket(sess.Character.GenerateGidx());
                        sess?.SendPacket(sess?.Character.GenerateFmi());
                        sess?.SendPacket(sess?.Character.GenerateFmp());
                    }
                }
            }
        }

        private void OnMailSent(object sender, EventArgs e)
        {
            var mail = (MailDTO)sender;

            var session = GetSessionByCharacterId(mail.IsSenderCopy ? mail.SenderId : mail.ReceiverId);
            if (session != null)
            {
                if (mail.AttachmentVNum != null)
                {
                    session.Character.MailList.Add(
                        (session.Character.MailList.Count > 0
                            ? session.Character.MailList.OrderBy(s => s.Key).Last().Key
                            : 0) + 1, mail);
                    session.SendPacket(session.Character.GenerateParcel(mail));

                    //session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ITEM_GIFTED"), GetItem(mail.AttachmentVNum.Value)?.Name, mail.AttachmentAmount), 12));
                }
                else
                {
                    session.Character.MailList.Add(
                        (session.Character.MailList.Count > 0
                            ? session.Character.MailList.OrderBy(s => s.Key).Last().Key
                            : 0) + 1, mail);
                    session.SendPacket(session.Character.GeneratePost(mail,
                        mail.IsSenderCopy ? (byte)2 : (byte)1));
                }
            }
        }

        private void OnMessageSentToCharacter(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var message = (SCSCharacterMessage)sender;

                var targetSession = Sessions.SingleOrDefault(s =>
                    s.Character.CharacterId == message.DestinationCharacterId);
                switch (message.Type)
                {
                    case MessageType.WhisperGM:
                    case MessageType.Whisper:
                        if (
                            targetSession ==
                            null /* || (message.Type == MessageType.WhisperGM && targetSession.Account.Authority != AuthorityType.GameMaster)*/
                        )
                        {
                            return;
                        }

                        if (targetSession.Character.GmPvtBlock)
                        {
                            if (message.DestinationCharacterId != null)
                            {
                                CommunicationServiceClient.Instance.SendMessageToCharacter(
                                        new SCSCharacterMessage
                                        {
                                            DestinationCharacterId = message.SourceCharacterId,
                                            SourceCharacterId = message.DestinationCharacterId.Value,
                                            SourceWorldId = WorldId,
                                            Message = targetSession.Character.GenerateSay(
                                                        Language.Instance.GetMessageFromKey("GM_CHAT_BLOCKED"), 10),
                                            Type = MessageType.Other
                                        });
                            }
                        }
                        else if (targetSession.Character.WhisperBlocked
                                 && DAOFactory.AccountDAO.LoadById(DAOFactory.CharacterDAO
                                     .LoadById(message.SourceCharacterId).AccountId).Authority <
                                 AuthorityType.DSGM)
                        {
                            if (message.DestinationCharacterId != null)
                            {
                                CommunicationServiceClient.Instance.SendMessageToCharacter(
                                        new SCSCharacterMessage
                                        {
                                            DestinationCharacterId = message.SourceCharacterId,
                                            SourceCharacterId = message.DestinationCharacterId.Value,
                                            SourceWorldId = WorldId,
                                            Message = UserInterfaceHelper.GenerateMsg(
                                                        Language.Instance.GetMessageFromKey("USER_WHISPER_BLOCKED"), 0),
                                            Type = MessageType.Other
                                        });
                            }
                        }
                        else
                        {
                            if (message.SourceWorldId != WorldId)
                            {
                                if (message.DestinationCharacterId != null)
                                {
                                    CommunicationServiceClient.Instance.SendMessageToCharacter(
                                            new SCSCharacterMessage
                                            {
                                                DestinationCharacterId = message.SourceCharacterId,
                                                SourceCharacterId = message.DestinationCharacterId.Value,
                                                SourceWorldId = WorldId,
                                                Message = targetSession.Character.GenerateSay(
                                                            string.Format(
                                                                    Language.Instance.GetMessageFromKey(
                                                                            "MESSAGE_SENT_TO_CHARACTER"),
                                                                    targetSession.Character.Name, ChannelId), 11),
                                                Type = MessageType.Other
                                            });
                                }

                                targetSession.SendPacket(
                                    $"{message.Message} <{Language.Instance.GetMessageFromKey("CHANNEL")}: {CommunicationServiceClient.Instance.GetChannelIdByWorldId(message.SourceWorldId)}>");
                            }
                            else
                            {
                                targetSession.SendPacket(message.Message);
                            }
                        }

                        break;

                    case MessageType.Shout:
                        Shout(message.Message);
                        break;

                    case MessageType.PrivateChat:
                        targetSession?.SendPacket(message.Message);
                        break;

                          case MessageType.FamilyChat:
                              if (message.DestinationCharacterId.HasValue && message.SourceWorldId != WorldId)
                              {
                                  Parallel.ForEach(Instance.Sessions, session =>
                                  {
                                      if (session.HasSelectedCharacter && session.Character.Family != null && session.Character.Family.FamilyId == message.DestinationCharacterId)
                                      {
                                          session.SendPacket($"say 1 0 6 <{Language.Instance.GetMessageFromKey("CHANNEL")}: {CommunicationServiceClient.Instance.GetChannelIdByWorldId(message.SourceWorldId)}>{message.Message}");
                                      }
                                  });
                              }
                              break;

                    case MessageType.Family:
                        if (message.DestinationCharacterId.HasValue)
                        {
                            Parallel.ForEach(Instance.Sessions, session =>
                            {
                                if (session.HasSelectedCharacter && session.Character.Family != null && session.Character.Family.FamilyId == message.DestinationCharacterId)
                                {
                                    session.SendPacket(message.Message);
                                }
                            });
                        }
                        break;

                    case MessageType.Other:
                        targetSession?.SendPacket(message.Message);
                        break;

                    case MessageType.Broadcast:
                        Parallel.ForEach(Instance.Sessions, session => session.SendPacket(message.Message));
                        break;
                }
            }
        }

        private void OnPenaltyLogRefresh(object sender, EventArgs e)
        {
            var relId = (int)sender;
            var reldto = DAOFactory.PenaltyLogDAO.LoadById(relId);
            var rel = PenaltyLogs.Find(s => s.PenaltyLogId == relId);
            if (reldto != null)
            {
                if (rel != null)
                {
                }
                else
                {
                    PenaltyLogs.Add(reldto);
                }
            }
            else if (rel != null)
            {
                PenaltyLogs.Remove(rel);
            }
        }

        private void OnRelationRefresh(object sender, EventArgs e)
        {
            _inRelationRefreshMode = true;
            var relId = (long)sender;
            lock (CharacterRelations)
            {
                var reldto = DAOFactory.CharacterRelationDAO.LoadById(relId);
                var rel = CharacterRelations.Find(s => s.CharacterRelationId == relId);
                if (reldto != null)
                {
                    if (rel != null)
                    {
                        CharacterRelations.Find(s => s.CharacterRelationId == rel.CharacterRelationId)
                                          .RelationType = reldto.RelationType;
                    }
                    else
                    {
                        CharacterRelations.Add(reldto);
                    }
                }
                else if (rel != null)
                {
                    CharacterRelations.Remove(rel);
                }
            }

            _inRelationRefreshMode = false;
        }

        private void OnSessionKicked(object sender, EventArgs e)
        {
            if (sender != null)
            {
                Tuple<long?, long?> kickedSession = (Tuple<long?, long?>)sender;
                if(!kickedSession.Item1.HasValue && !kickedSession.Item2.HasValue)
                {
                    return;
                }
                long? accId = kickedSession.Item1;
                long? sessId = kickedSession.Item2;

                ClientSession targetSession = CharacterScreenSessions.FirstOrDefault(s => s.SessionId == sessId || s.Account.AccountId == accId);
                targetSession?.Disconnect();
                targetSession = Sessions.FirstOrDefault(s => s.SessionId == sessId || s.Account.AccountId == accId);
                targetSession?.Disconnect();
            }
        }

        private void OnStaticBonusRefresh(object sender, EventArgs e)
        {
            var characterId = (long)sender;

            var sess = GetSessionByCharacterId(characterId);
            if (sess != null)
            {
                sess.Character.StaticBonusList =
                        DAOFactory.StaticBonusDAO.LoadByCharacterId(characterId).ToList();
            }
        }

        private void RemoveItemProcess()
        {
            try
            {
                foreach (var session in Sessions.Where(c => c.IsConnected))
                {
                    session.Character?.RefreshValidity();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        // Server
        private void SaveAllProcess()
        {
            try
            {
                Logger.Info(Language.Instance.GetMessageFromKey("SAVING_ALL"));
                SaveAll();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}