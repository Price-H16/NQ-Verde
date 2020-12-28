using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Autofac;
using ChickenAPI.Enums.Game.BCard;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Exceptions;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject._gameEvent;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Event.ARENA;
using OpenNos.GameObject.Event.GAMES;
using OpenNos.GameObject.Event.WORLDBOSS;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Extensions;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using OpenNos.PathFinder;

namespace OpenNos.GameObject.Helpers
{
    public class EventHelper
    {
        #region Members

        private static EventHelper _instance;

        #endregion

        #region Properties

        public static EventHelper Instance => _instance ?? (_instance = new EventHelper());

        #endregion

        #region Methods

        public static int CalculateComboPoint(int n)
        {
            var a = 4;
            var b = 7;
            for (var i = 0; i < n; i++)
            {
                var temp = a;
                a = b;
                b = temp + b;
            }

            return a;
        }

        public static void GenerateEvent(EventType type, int LvlBracket = -1, byte value = 0)
        {
            if (type == EventType.ICEBREAKER && LvlBracket < 0)
            {
                return;
            }

            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();
            if (!ServerManager.Instance.StartedEvents.Contains(type))
            {
                Task.Factory.StartNew(() =>
                {
                    ServerManager.Instance.StartedEvents.Add(type);
                    switch (type)
                    {
                        case EventType.RANKINGREFRESH:
                            ServerManager.Instance.RefreshRanking();
                            ServerManager.Instance.StartedEvents?.Remove(type);
                            break;

                        case EventType.LOD:
                            if (ServerManager.Instance.ChannelId != 51)
                            {
                                LOD.GenerateLod();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents.Remove(type);
                                return;
                            }
                            break;

                        case EventType.DAILYMISSIONEXTENSIONREFRESH:
                            ServerManager.Instance.RefreshDailyMissions();
                            ServerManager.Instance.StartedEvents.Remove(type);
                            break;

                        case EventType.MINILANDREFRESHEVENT:
                            MinilandRefresh.GenerateMinilandEvent();
                            break;

                        case EventType.INSTANTBATTLE:
                            if (ServerManager.Instance.ChannelId != 51)
                            {
                                InstantBattle.GenerateInstantBattle();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents?.Remove(type);
                            }
                            break;

                        case EventType.SHEEPGAME:
                            if (ServerManager.Instance.ChannelId != 51)
                            {
                                SheepGame.GenerateSheepGames();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents.Remove(type);
                                return;
                            }
                            break;

                        case EventType.TOURNAMENT:
                            if (ServerManager.Instance.ChannelId != 51)
                            {
                                Tournament.GenerateTournament();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents.Remove(type);
                                return;
                            }
                            break;

                        case EventType.RAINBOWBATTLE:
                            if (ServerManager.Instance.ChannelId == 4)
                            {
                                Event.RAINBOWBATTLE.RainbowBattle.GenerateEvent();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents.Remove(type);
                                return;
                            }
                            break;

                        case EventType.ACT4SHIP:
                            ACT4SHIP.GenerateAct4Ship(1);
                            ACT4SHIP.GenerateAct4Ship(2);
                            break;

                        case EventType.TALENTARENA:
                            if (ServerManager.Instance.ChannelId != 51)
                            {
                                ArenaEvent.GenerateTalentArena();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents?.Remove(type);
                            }
                            break;

                        case EventType.CALIGOR:
                            if (ServerManager.Instance.ChannelId == 51)
                            {
                                CaligorRaid.Run();
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents?.Remove(type);
                            }
                            break;

                        case EventType.ICEBREAKER:
                            if (ServerManager.Instance.ChannelId != 51)
                            {
                                IceBreaker.GenerateIceBreaker(LvlBracket);
                            }
                            else
                            {
                                ServerManager.Instance.StartedEvents?.Remove(type);
                            }
                            break;

                        case EventType.TOWERGAME:
                            TowerGame.GenerateTowerGame();
                            break;

                        case EventType.FAFNIRBOSS: 
                            if (ServerManager.Instance.ChannelId == 1)
                            {
                                FafnirRad.Run();
                            }
                            break;

                        case EventType.YERTIRANDBOSS: 
                            if (ServerManager.Instance.ChannelId == 1)
                            {
                                YertirandRad.Run();
                            }
                            break;

                        case EventType.GRASSLINBOSS:
                            if (ServerManager.Instance.ChannelId == 1)
                            {
                                GrasslinRad.Run();
                            }
                            break;
                    }
                });
            }
        }

        public static TimeSpan GetMilisecondsBeforeTime(TimeSpan time)
        {
            var now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
            var timeLeftUntilFirstRun = time - now;
            if (timeLeftUntilFirstRun.TotalHours < 0)
            {
                timeLeftUntilFirstRun += new TimeSpan(24, 0, 0);
            }

            return timeLeftUntilFirstRun;
        }

        public void RunEvent(EventContainer evt, ClientSession session = null, MapMonster monster = null,
            MapNpc npc = null)
        {
            if (evt != null)
            {
                if (session != null)
                {
                    evt.MapInstance = session.CurrentMapInstance;
                    switch (evt.EventActionType)
                    {
                        #region EventForUser

                        case EventActionType.NPCDIALOG:
                            session.SendPacket(session.Character.GenerateNpcDialog((int)evt.Parameter));
                            break;

                        case EventActionType.SENDPACKET:
                            session.SendPacket((string)evt.Parameter);
                            break;

                            #endregion
                    }
                }

                if (evt.MapInstance != null)
                {
                    switch (evt.EventActionType)
                    {
                        #region EventForUser

                        case EventActionType.NPCDIALOG:
                        case EventActionType.SENDPACKET:
                            if (session == null)
                            {
                                evt.MapInstance.Sessions.ToList().ForEach(e => RunEvent(evt, e));
                            }

                            break;

                        #endregion

                        #region MapInstanceEvent

                        case EventActionType.REGISTEREVENT:
                            var even = (Tuple<string, List<EventContainer>>)evt.Parameter;
                            switch (even.Item1)
                            {
                                case "OnCharacterDiscoveringMap":
                                    even.Item2.ForEach(s =>
                                            evt.MapInstance.OnCharacterDiscoveringMapEvents.Add(
                                                    new Tuple<EventContainer, List<long>>(s, new List<long>())));
                                    break;

                                case "OnMoveOnMap":
                                    evt.MapInstance.OnMoveOnMapEvents.AddRange(even.Item2);
                                    break;

                                case "OnMapClean":
                                    evt.MapInstance.OnMapClean.AddRange(even.Item2);
                                    break;

                                case "OnLockerOpen":
                                    evt.MapInstance.UnlockEvents.AddRange(even.Item2);
                                    break;
                            }

                            break;

                        case EventActionType.REGISTERWAVE:
                            evt.MapInstance.WaveEvents.Add((EventWave)evt.Parameter);
                            break;

                        case EventActionType.SETAREAENTRY:
                            var even2 = (ZoneEvent)evt.Parameter;
                            evt.MapInstance.OnAreaEntryEvents.Add(even2);
                            break;

                        case EventActionType.REMOVEMONSTERLOCKER:
                            var evt2 = (EventContainer)evt.Parameter;
                            if (evt.MapInstance.InstanceBag.MonsterLocker.Current > 0)
                            {
                                evt.MapInstance.InstanceBag.MonsterLocker.Current--;
                            }

                            if (evt.MapInstance.InstanceBag.MonsterLocker.Current == 0 &&
                                evt.MapInstance.InstanceBag.ButtonLocker.Current == 0)
                            {
                                var UnlockEventsCopy = evt.MapInstance.UnlockEvents.ToList();
                                UnlockEventsCopy.ForEach(e => RunEvent(e));
                                evt.MapInstance.UnlockEvents.RemoveAll(s => s != null && UnlockEventsCopy.Contains(s));
                            }

                            break;

                        case EventActionType.REMOVEBUTTONLOCKER:
                            evt2 = (EventContainer)evt.Parameter;
                            if (evt.MapInstance.InstanceBag.ButtonLocker.Current > 0)
                            {
                                evt.MapInstance.InstanceBag.ButtonLocker.Current--;
                            }

                            if (evt.MapInstance.InstanceBag.MonsterLocker.Current == 0 &&
                                evt.MapInstance.InstanceBag.ButtonLocker.Current == 0)
                            {
                                var UnlockEventsCopy = evt.MapInstance.UnlockEvents.ToList();
                                UnlockEventsCopy.ForEach(e => RunEvent(e));
                                evt.MapInstance.UnlockEvents.RemoveAll(s => s != null && UnlockEventsCopy.Contains(s));
                            }

                            break;

                        case EventActionType.EFFECT:
                            {
                                var effectTuple = (Tuple<short, int>)evt.Parameter;

                                var effectId = effectTuple.Item1;
                                var delay = effectTuple.Item2;

                                Observable.Timer(TimeSpan.FromMilliseconds(delay)).Subscribe(obs =>
                                {
                                    if (monster != null)
                                    {
                                        monster.LastEffect = DateTime.Now;
                                        evt.MapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Monster,
                                                monster.MapMonsterId, effectId));
                                    }
                                    else
                                    {
                                        evt.MapInstance.Sessions.Where(s => s?.Character != null).ToList()
                                           .ForEach(s =>
                                                   s.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player,
                                                           s.Character.CharacterId, effectId)));
                                    }
                                });
                            }
                            break;

                        case EventActionType.CONTROLEMONSTERINRANGE:
                            if (monster != null)
                            {
                                var evnt = (Tuple<short, byte, List<EventContainer>>)evt.Parameter;
                                var MapMonsters =
                                        evt.MapInstance.GetMonsterInRangeList(monster.MapX, monster.MapY, evnt.Item2);
                                if (evnt.Item1 != 0)
                                {
                                    MapMonsters.RemoveAll(s => s.MonsterVNum != evnt.Item1);
                                }

                                MapMonsters.ForEach(s => evnt.Item3.ForEach(e => RunEvent(e, monster: s)));
                            }

                            break;

                        case EventActionType.ONTARGET:
                            if (monster.MoveEvent?.InZone(monster.MapX, monster.MapY) == true)
                            {
                                monster.MoveEvent = null;
                                monster.Path = new List<Node>();
                                ((List<EventContainer>)evt.Parameter).ForEach(s => RunEvent(s, monster: monster));
                            }

                            break;

                        case EventActionType.MOVE:
                            var evt4 = (ZoneEvent)evt.Parameter;
                            if (monster != null)
                            {
                                monster.MoveEvent = evt4;
                                monster.Path = BestFirstSearch.FindPathJagged(
                                        new Node { X = monster.MapX, Y = monster.MapY }, new Node { X = evt4.X, Y = evt4.Y },
                                        evt.MapInstance?.Map.JaggedGrid);
                                monster.RunToX = evt4.X;
                                monster.RunToY = evt4.Y;
                            }
                            else if (npc != null)
                            {
                                //npc.MoveEvent = evt4;
                                npc.Path = BestFirstSearch.FindPathJagged(new Node { X = npc.MapX, Y = npc.MapY },
                                        new Node { X = evt4.X, Y = evt4.Y }, evt.MapInstance?.Map.JaggedGrid);
                                npc.RunToX = evt4.X;
                                npc.RunToY = evt4.Y;
                            }

                            break;

                        case EventActionType.STARTACT4RAIDWAVES:
                            IDisposable spawnsDisposable = null;
                            spawnsDisposable = Observable.Interval(TimeSpan.FromSeconds(60)).Subscribe(s =>
                            {
                                var count = evt.MapInstance.Sessions.Count();

                                if (count <= 0)
                                {
                                    spawnsDisposable.Dispose();
                                    return;
                                }

                                if (count > 5)
                                {
                                    count = 5;
                                }

                                var mobWave = new List<MonsterToSummon>();
                                for (var i = 0; i < count; i++)
                                {
                                    switch (evt.MapInstance.MapInstanceType)
                                    {
                                        case MapInstanceType.Act4Morcos:
                                            mobWave.Add(new MonsterToSummon(561,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(561,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(561,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(562,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(562,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(562,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(851,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, false));
                                            break;

                                        case MapInstanceType.Act4Hatus:
                                            mobWave.Add(new MonsterToSummon(574,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(574,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(575,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(575,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(576,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(576,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            break;

                                        case MapInstanceType.Act4Calvina:
                                            mobWave.Add(new MonsterToSummon(770,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(770,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(770,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(771,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(771,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(771,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            break;

                                        case MapInstanceType.Act4Berios:
                                            mobWave.Add(new MonsterToSummon(780,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(781,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(782,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(782,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(783,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            mobWave.Add(new MonsterToSummon(783,
                                                    evt.MapInstance.Map.GetRandomPosition(), null, true));
                                            break;
                                    }
                                }

                                evt.MapInstance.SummonMonsters(mobWave);
                            });
                            break;

                        case EventActionType.SETMONSTERLOCKERS:
                            evt.MapInstance.InstanceBag.MonsterLocker.Current = Convert.ToByte(evt.Parameter);
                            evt.MapInstance.InstanceBag.MonsterLocker.Initial = Convert.ToByte(evt.Parameter);
                            break;

                        case EventActionType.SETBUTTONLOCKERS:
                            evt.MapInstance.InstanceBag.ButtonLocker.Current = Convert.ToByte(evt.Parameter);
                            evt.MapInstance.InstanceBag.ButtonLocker.Initial = Convert.ToByte(evt.Parameter);
                            break;

                        case EventActionType.SCRIPTEND:
                            switch (evt.MapInstance.MapInstanceType)
                            {
                                case MapInstanceType.FafnirBossInstance:
                                    foreach (ClientSession sess in evt.MapInstance.Sessions)
                                    {
                                        if (sess?.Character != null)
                                        {
                                            if (FafnirRad.RemainingTime > 0)
                                            {
                                                sess.Character.GetReputation(5000, false); 
                                                sess.Character.GenerateFamilyXp(1000);
                                                sess.Character.GiftAdd(5724, 1);
                                                sess.Character.Gold += 500000;
                                                sess.SendPacket(sess.Character.GenerateGold());
                                                try
                                                {
                                                    Observable.Timer(TimeSpan.FromSeconds(15))
                                                    .Subscribe(observer =>
                                                    {
                                                        //SomeTime
                                                        ServerManager.Instance.ChangeMap(sess.Character.CharacterId, 1, 80, 116);
                                                        ServerManager.Instance.StartedEvents.Remove(EventType.FAFNIRBOSS);

                                                    });
                                                    foreach (Portal p in FafnirRad.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == FafnirRad.WorldMapinstance.MapInstanceId).ToList())
                                                    {
                                                        FafnirRad.UnknownLandMapInstance.Portals.Remove(p);
                                                        FafnirRad.UnknownLandMapInstance.Broadcast(p.GenerateGp());
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                        sess.Character.GenerateFamilyXp(100);
                                    }
                                    break;

                                case MapInstanceType.YertirandBossInstance:
                                    foreach (ClientSession sess in evt.MapInstance.Sessions)
                                    {
                                        if (sess?.Character != null)
                                        {
                                            if (YertirandRad.RemainingTime > 0)
                                            {
                                                sess.Character.GetReputation(5000, false);
                                                sess.Character.GenerateFamilyXp(2000);
                                                sess.Character.GiftAdd(5974, 1);
                                                sess.Character.Gold += 500000;
                                                sess.SendPacket(sess.Character.GenerateGold());
                                                try
                                                {
                                                    Observable.Timer(TimeSpan.FromSeconds(15))
                                                    .Subscribe(observer =>
                                                    {
                                                        //SomeTime
                                                        ServerManager.Instance.ChangeMap(sess.Character.CharacterId, 1, 80, 116);
                                                        ServerManager.Instance.StartedEvents.Remove(EventType.YERTIRANDBOSS);

                                                    });
                                                    foreach (Portal p in YertirandRad.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == YertirandRad.WorldMapinstance.MapInstanceId).ToList())
                                                    {
                                                        YertirandRad.UnknownLandMapInstance.Portals.Remove(p);
                                                        YertirandRad.UnknownLandMapInstance.Broadcast(p.GenerateGp());
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                        sess.Character.GenerateFamilyXp(100);
                                    }
                                    break;

                                case MapInstanceType.GrasslinBossInstance:
                                    foreach (ClientSession sess in evt.MapInstance.Sessions)
                                    {
                                        if (sess?.Character != null)
                                        {
                                            if (GrasslinRad.RemainingTime > 0)
                                            {
                                                sess.Character.GetReputation(5000, false);
                                                sess.Character.GenerateFamilyXp(2000);
                                                sess.Character.GiftAdd(5722, 1);
                                                sess.Character.Gold += 500000;
                                                sess.SendPacket(sess.Character.GenerateGold());
                                                try
                                                {
                                                    Observable.Timer(TimeSpan.FromSeconds(15))
                                                    .Subscribe(observer =>
                                                    {
                                                        //SomeTime
                                                        ServerManager.Instance.ChangeMap(sess.Character.CharacterId, 1, 80, 116);
                                                        ServerManager.Instance.StartedEvents.Remove(EventType.GRASSLINBOSS);

                                                    });
                                                    foreach (Portal p in YertirandRad.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == GrasslinRad.WorldMapinstance.MapInstanceId).ToList())
                                                    {
                                                        GrasslinRad.UnknownLandMapInstance.Portals.Remove(p);
                                                        GrasslinRad.UnknownLandMapInstance.Broadcast(p.GenerateGp());
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                        sess.Character.GenerateFamilyXp(100);
                                    }
                                    break;
                                case MapInstanceType.TimeSpaceInstance:
                                    evt.MapInstance.InstanceBag.Clock.StopClock();
                                    evt.MapInstance.Clock.StopClock();
                                    evt.MapInstance.InstanceBag.EndState = (byte)evt.Parameter;
                                    ClientSession client = evt.MapInstance.Sessions.ToList().Where(s => s.Character?.Timespace != null).FirstOrDefault();
                                    if (client != null && client.Character?.Timespace != null && evt.MapInstance.InstanceBag.EndState != 10)
                                    {
                                        Guid MapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(client.Character.MapId);
                                        ScriptedInstance si = ServerManager.Instance.TimeSpaces.FirstOrDefault(s => s.Id == client.Character.Timespace.Id);
                                        if (si == null)
                                        {
                                            return;
                                        }
                                        byte penalty = 0;
                                        if (penalty > (client.Character.Level - si.LevelMinimum) * 2)
                                        {
                                            penalty = penalty > 100 ? (byte)100 : penalty;
                                            client.SendPacket(client.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("TS_PENALTY"), penalty), 10));
                                        }
                                        int point = evt.MapInstance.InstanceBag.Point * (100 - penalty) / 100;
                                        string perfection = "";
                                        perfection += evt.MapInstance.InstanceBag.MonstersKilled >= si.MonsterAmount ? 1 : 0;
                                        perfection += evt.MapInstance.InstanceBag.NpcsKilled == 0 ? 1 : 0;
                                        perfection += evt.MapInstance.InstanceBag.RoomsVisited >= si.RoomAmount ? 1 : 0;
                                        foreach (MapInstance mapInstance in client.Character.Timespace._mapInstanceDictionary.Values)
                                        {
                                            mapInstance.Broadcast($"score  {evt.MapInstance.InstanceBag.EndState} {point} 27 47 18 {si.DrawItems?.Count ?? 0} {evt.MapInstance.InstanceBag.MonstersKilled} {si.NpcAmount - evt.MapInstance.InstanceBag.NpcsKilled} {evt.MapInstance.InstanceBag.RoomsVisited} {perfection} 1 1");
                                        }

                                        if (evt.MapInstance.InstanceBag.EndState == 5)
                                        {
                                            if (client.Character.Inventory.GetAllItems().FirstOrDefault(s => s.Item.ItemType == ItemType.Special && s.Item.Effect == 140 && s.Item.EffectValue == si.Id) is ItemInstance tsStone)
                                            {
                                                client.Character.Inventory.RemoveItemFromInventory(tsStone.Id);
                                            }
                                            ClientSession[] tsmembers = new ClientSession[40];
                                            client.Character.Timespace._mapInstanceDictionary.SelectMany(s => s.Value?.Sessions).ToList().CopyTo(tsmembers);
                                            foreach (ClientSession targetSession in tsmembers)
                                            {
                                                if (targetSession != null)
                                                {
                                                    targetSession.Character.IncrementQuests(QuestType.TimesSpace, si.QuestTimeSpaceId);
                                                }
                                            }
                                        }

                                        var ClientTimeSpace = client.Character.Timespace;
                                        Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(o =>
                                        {
                                            var tsmembers = new ClientSession[40];
                                            ClientTimeSpace._mapInstanceDictionary.SelectMany(s => s.Value?.Sessions).ToList().CopyTo(tsmembers);
                                            foreach (var targetSession in tsmembers)
                                            {
                                                if (targetSession != null)
                                                {
                                                    if (targetSession.Character.Hp <= 0)
                                                    {
                                                        targetSession.Character.Hp = 1;
                                                        targetSession.Character.Mp = 1;
                                                    }
                                                }
                                            }

                                            ClientTimeSpace._mapInstanceDictionary.Values.ToList().ForEach(m => m.Dispose());

                                        });
                                    }

                                    break;

                                case MapInstanceType.RaidInstance:
                                    {
                                        evt.MapInstance.InstanceBag.EndState = (byte)evt.Parameter;
                                        var owner = evt.MapInstance.Sessions.FirstOrDefault(s => s.Character.Group?.Raid?.InstanceBag.CreatorId == s.Character.CharacterId)?.Character;

                                        if (owner == null)
                                        {
                                            owner = evt.MapInstance.Sessions.FirstOrDefault(s => s.Character.Group?.Raid != null)?.Character;

                                        }

                                        var group = owner?.Group;

                                        if (@group?.Raid == null)
                                        {
                                            break;
                                        }

                                        var teamSize = @group.Raid.InstanceBag.Lives;

                                        if (evt.MapInstance.InstanceBag.EndState == 1 && evt.MapInstance.Monsters.Any(s => s.IsBoss))
                                        {
                                            foreach (var s in @group.Sessions.Where(s => s?.Character?.MapInstance?.Monsters.Any(e => e.IsBoss) == true))

                                            {
                                                s.SendPacket(StaticPacketHelper.Cancel(2, s.Character.CharacterId));

                                                if (@group.Sessions.CountLinq(b => b.IpAddress.GetIp().Equals(s.IpAddress.GetIp())) > 2)
                                                {
                                                    s.SendPacket(s.Character.GenerateSay("if you are doing the raid with more than 2 characters you don't receive any reward", 10));
                                                    continue;
                                                }

                                                foreach (var gift in @group.Raid.GiftItems)
                                                {
                                                    byte rare = (byte)(gift.IsRandomRare ? ServerManager.RandomNumber(0, 8) : 0);

                                                    if (s.Character.Level >= @group.Raid.LevelMinimum)
                                                    {
                                                        if (gift.MinTeamSize == 0 && gift.MaxTeamSize == 0 || teamSize >= gift.MinTeamSize && teamSize <= gift.MaxTeamSize)
                                                        {
                                                            s.Character.GiftAdd(gift.VNum, gift.Amount, rare, 0, gift.Design, gift.IsRandomRare);

                                                        }
                                                    }
                                                }

                                                s.Character.GetReputation(@group.Raid.Reputation);
                                                s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REP"), @group.Raid.Reputation), 10));

                                                if (s.Character.GenerateFamilyXp(@group.Raid.FamExp, @group.Raid.Id))
                                                {
                                                    s.SendPacket(s.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), @group.Raid.FamExp), 10));

                                                }

                                                s.Character.IncrementQuests(QuestType.WinRaid, @group.Raid.Id);
                                            }

                                            //FAMILY MISSIONS IN RAID
                                            foreach (var Family in FamilyExtensions.SessionsToFamilies(group.Sessions.Where(s => s?.Character?.MapInstance?.Monsters.Any(e => e.IsBoss) ?? false)))
                                            {
                                                foreach (var fsm in group.Raid.FamMissions)
                                                {
                                                    if (fsm == 0) continue;
                                                    Family.AddMissionProgress((short)fsm, (short)(group.Sessions.Where(s => (s?.Character?.MapInstance?.Monsters.Any(e => e.IsBoss) ?? false) && (s?.Character?.Family?.FamilyId.Equals(Family.FamilyId) ?? false)).Count()));
                                                }
                                            }


                                            foreach (var mapMonster in evt.MapInstance.Monsters)
                                            {
                                                if (mapMonster != null)
                                                {
                                                    mapMonster.SetDeathStatement();
                                                    evt.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, mapMonster.MapMonsterId));
                                                    evt.MapInstance.RemoveMonster(mapMonster);
                                                }
                                            }

                                            Logger.LogUserEvent("RAID_SUCCESS", owner.Name, $"RaidId: {@group.GroupId}");
                                            DiscordWebhookHelper.DiscordEventRaidEnd($"Team {owner.Name} Just Won Raid {@group.Raid.Label}");
                                            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAID_SUCCEED"),@group.Raid.Label, owner.Name), 0));

                                            foreach (var s in @group.Sessions.GetAllItems())
                                            {
                                                if (s.Account != null && s.Character?.Group?.Raid != null)
                                                {
                                                    s.Character.GeneralLogs?.Add(new GeneralLogDTO
                                                    {
                                                        AccountId = s.Account.AccountId,
                                                        CharacterId = s.Character.CharacterId,
                                                        IpAddress = s.IpAddress,
                                                        LogData = $"{s.Character.Group.Raid.Id}",
                                                        LogType = "InstanceEntry",
                                                        Timestamp = DateTime.Now
                                                    });
                                                }
                                            }
                                        }

                                        var dueTime =
                                                TimeSpan.FromSeconds(evt.MapInstance.InstanceBag.EndState == 1 ? 15 : 0);

                                        evt.MapInstance.Broadcast(
                                                Character.GenerateRaidBf(evt.MapInstance.InstanceBag.EndState));

                                        Observable.Timer(dueTime).Subscribe(o =>
                                        {
                                            evt.MapInstance.Sessions.Where(s =>
                                                       s.Character != null &&
                                                       s.Character.HasBuff(BCardType.FrozenDebuff,
                                                               (byte)BCardSubTypes.FrozenDebuff.EternalIce))
                                               .Select(s => s.Character).ToList().ForEach(c => { c.RemoveBuff(569); });

                                            var groupMembers = new ClientSession[@group.SessionCount];
                                            @group.Sessions.CopyTo(groupMembers);

                                            foreach (var groupMember in groupMembers)
                                            {
                                                if (groupMember.Character.Hp < 1)
                                                {
                                                    groupMember.Character.Hp = 1;
                                                    groupMember.Character.Mp = 1;
                                                }

                                                groupMember.SendPacket(groupMember.Character.GenerateRaid(1, true));
                                                groupMember.SendPacket(groupMember.Character.GenerateRaid(2, true));
                                                @group.LeaveGroup(groupMember);
                                            }

                                            ServerManager.Instance.GroupList.RemoveAll(s => s.GroupId == @group.GroupId);
                                            ServerManager.Instance.ThreadSafeGroupList.Remove(@group.GroupId);

                                            @group.Raid.Dispose();
                                        });
                                    }
                                    break;

                                case MapInstanceType.Act4Morcos:
                                case MapInstanceType.Act4Hatus:
                                case MapInstanceType.Act4Calvina:
                                case MapInstanceType.Act4Berios:
                                    client = evt.MapInstance.Sessions.FirstOrDefault(s =>
                                            s.Character?.Family?.Act4RaidBossMap == evt.MapInstance);
                                    if (client != null)
                                    {
                                        var fam = client.Character.Family;
                                        if (fam != null)
                                        {
                                            short destX = 38;
                                            short destY = 179;
                                            short rewardVNum = 882;
                                            short rewardVNum2 = 11142;
                                            switch (evt.MapInstance.MapInstanceType)
                                            {
                                                //Morcos is default
                                                case MapInstanceType.Act4Hatus:
                                                    destX = 18;
                                                    destY = 10;
                                                    rewardVNum = 185;
                                                    rewardVNum2 = 11142;
                                                    break;

                                                case MapInstanceType.Act4Calvina:
                                                    destX = 25;
                                                    destY = 7;
                                                    rewardVNum = 942;
                                                    rewardVNum2 = 11142;
                                                    break;

                                                case MapInstanceType.Act4Berios:
                                                    destX = 16;
                                                    destY = 25;
                                                    rewardVNum = 999;
                                                    rewardVNum2 = 11142;
                                                    break;
                                            }

                                            var count = evt.MapInstance.Sessions.Count(s => s?.Character != null);
                                            foreach (var sess in evt.MapInstance.Sessions)
                                            {
                                                if (sess?.Character != null)
                                                {
                                                    sess.Character.GiftAdd(rewardVNum, 1, forceRandom: true, minRare: 4, design: 255);
                                                    sess.Character.GiftAdd(rewardVNum2, 1); // XD
                                                    sess.Character.GenerateFamilyXp(15000 / count);
                                                    sess.SendPacket(sess.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), 15000 / count), 10));
                                                    //if (sess.Character.GenerateFamilyXp(20000 / count))
                                                    //{
                                                    //    sess.SendPacket(sess.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), 20000 / count), 10));

                                                    //}
                                                }
                                            }

                                            evt.MapInstance.Broadcast("dance 2");

                                            Logger.LogEvent("FAMILYRAID_SUCCESS",$"[fam.Name]FamilyRaidId: {evt.MapInstance.MapInstanceType.ToString()}");

                                            CommunicationServiceClient.Instance.SendMessageToCharacter(
                                                    new SCSCharacterMessage
                                                    {
                                                        DestinationCharacterId = fam.FamilyId,
                                                        SourceCharacterId = client.Character.CharacterId,
                                                        SourceWorldId = ServerManager.Instance.WorldId,
                                                        Message = UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("FAMILYRAID_SUCCESS"), 0),
                                                        Type = MessageType.Family
                                                    });

                                            //ServerManager.Instance.Broadcast(UserInterfaceHelper.Instance.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("FAMILYRAID_SUCCESS"), grp?.Raid?.Label, grp.Characters.ElementAt(0).Character.Name), 0));

                                            Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(o =>
                                            {
                                                foreach (var targetSession in evt.MapInstance.Sessions.ToArray())
                                                {
                                                    if (targetSession != null)
                                                    {
                                                        if (targetSession.Character.Hp <= 0)
                                                        {
                                                            targetSession.Character.Hp = 1;
                                                            targetSession.Character.Mp = 1;
                                                        }

                                                        ServerManager.Instance.ChangeMapInstance(
                                                                targetSession.Character.CharacterId,
                                                                fam.Act4Raid.MapInstanceId, destX, destY);

                                                        targetSession.SendPacket("dance");
                                                    }
                                                }

                                                evt.MapInstance.Dispose();
                                            });

                                            fam.InsertFamilyLog(FamilyLogType.RaidWon,
                                                    raidType: (int)evt.MapInstance.MapInstanceType - 7);
                                        }
                                    }

                                    break;

                                case MapInstanceType.CaligorInstance:

                                    var winningFaction = CaligorRaid.AngelDamage > CaligorRaid.DemonDamage ? FactionType.Angel : FactionType.Demon;

                                    foreach (var sess in evt.MapInstance.Sessions)
                                    {
                                        if (sess?.Character != null)
                                        {
                                            if (CaligorRaid.RemainingTime > 2400)
                                            {
                                                if (sess.Character.Faction == winningFaction)
                                                {
                                                    sess.Character.GiftAdd(5960, 1);
                                                    sess.Character.GenerateFamilyXp(10000);
                                                }
                                                else
                                                {
                                                    sess.Character.GiftAdd(5961, 1);
                                                }
                                            }
                                            else
                                            {
                                                if (sess.Character.Faction == winningFaction)
                                                {
                                                    sess.Character.GiftAdd(5961, 1);
                                                    sess.Character.GenerateFamilyXp(10000);
                                                }
                                                else
                                                {
                                                    sess.Character.GiftAdd(5958, 1);
                                                }
                                            }

                                            sess.Character.GiftAdd(5959, 1);
                                            sess.Character.GenerateFamilyXp(5000);
                                        }
                                    }

                                    evt.MapInstance.Broadcast(UserInterfaceHelper.GenerateCHDM(
                                            ServerManager.GetNpcMonster(2305).MaxHP, CaligorRaid.AngelDamage,
                                            CaligorRaid.DemonDamage, CaligorRaid.RemainingTime));
                                    break;
                            }

                            break;

                        case EventActionType.CLOCK:
                            evt.MapInstance.InstanceBag.Clock.TotalSecondsAmount = Convert.ToInt32(evt.Parameter);
                            evt.MapInstance.InstanceBag.Clock.SecondsRemaining = Convert.ToInt32(evt.Parameter);
                            break;

                        case EventActionType.MAPCLOCK:
                            evt.MapInstance.Clock.TotalSecondsAmount = Convert.ToInt32(evt.Parameter);
                            evt.MapInstance.Clock.SecondsRemaining = Convert.ToInt32(evt.Parameter);
                            break;

                        case EventActionType.STARTCLOCK:
                            var eve = (Tuple<List<EventContainer>, List<EventContainer>>)evt.Parameter;
                            evt.MapInstance.InstanceBag.Clock.StopEvents = eve.Item1;
                            evt.MapInstance.InstanceBag.Clock.TimeoutEvents = eve.Item2;
                            evt.MapInstance.InstanceBag.Clock.StartClock();
                            evt.MapInstance.Broadcast(evt.MapInstance.InstanceBag.Clock.GetClock());
                            break;

                        case EventActionType.STARTMAPCLOCK:
                            eve = (Tuple<List<EventContainer>, List<EventContainer>>)evt.Parameter;
                            evt.MapInstance.Clock.StopEvents = eve.Item1;
                            evt.MapInstance.Clock.TimeoutEvents = eve.Item2;
                            evt.MapInstance.Clock.StartClock();
                            evt.MapInstance.Broadcast(evt.MapInstance.Clock.GetClock());
                            break;

                        case EventActionType.STOPCLOCK:
                            evt.MapInstance.InstanceBag.Clock.StopClock();
                            evt.MapInstance.Broadcast(evt.MapInstance.InstanceBag.Clock.GetClock());
                            break;

                        case EventActionType.STOPMAPCLOCK:
                            evt.MapInstance.Clock.StopClock();
                            evt.MapInstance.Broadcast(evt.MapInstance.Clock.GetClock());
                            break;

                        case EventActionType.ADDCLOCKTIME:
                            evt.MapInstance.InstanceBag.Clock.AddTime((int)evt.Parameter);
                            evt.MapInstance.Broadcast(evt.MapInstance.InstanceBag.Clock.GetClock());
                            break;

                        case EventActionType.ADDMAPCLOCKTIME:
                            evt.MapInstance.Clock.AddTime((int)evt.Parameter);
                            evt.MapInstance.Broadcast(evt.MapInstance.Clock.GetClock());
                            break;

                        case EventActionType.TELEPORT:
                            var tp = (Tuple<short, short, short, short>)evt.Parameter;
                            var characters = evt.MapInstance.GetCharactersInRange(tp.Item1, tp.Item2, 5).ToList();
                            characters.ForEach(s =>
                            {
                                s.PositionX = tp.Item3;
                                s.PositionY = tp.Item4;
                                evt.MapInstance?.Broadcast(s.Session, s.GenerateTp());
                                foreach (var mate in s.Mates.Where(m => m.IsTeamMember && m.IsAlive))
                                {
                                    mate.PositionX = tp.Item3;
                                    mate.PositionY = tp.Item4;
                                    evt.MapInstance?.Broadcast(s.Session, mate.GenerateTp());
                                }
                            });
                            break;

                        case EventActionType.SPAWNPORTAL:
                            evt.MapInstance.CreatePortal((Portal)evt.Parameter);
                            break;

                        case EventActionType.REFRESHMAPITEMS:
                            evt.MapInstance.MapClear();
                            break;

                        case EventActionType.STOPMAPWAVES:
                            evt.MapInstance.WaveEvents.Clear();
                            break;

                        case EventActionType.NPCSEFFECTCHANGESTATE:
                            evt.MapInstance.Npcs.ForEach(s => s.EffectActivated = (bool)evt.Parameter);
                            break;

                        case EventActionType.CHANGEPORTALTYPE:
                            var param = (Tuple<int, PortalType>)evt.Parameter;
                            var portal = evt.MapInstance.Portals.Find(s => s.PortalId == param.Item1);
                            if (portal != null)
                            {
                                portal.IsDisabled = true;
                                evt.MapInstance.Broadcast(portal.GenerateGp());
                                portal.IsDisabled = false;

                                portal.Type = (short)param.Item2;
                                if ((PortalType)portal.Type == PortalType.Closed
                                 && (evt.MapInstance.MapInstanceType.Equals(MapInstanceType.Act4Berios)
                                  || evt.MapInstance.MapInstanceType.Equals(MapInstanceType.Act4Calvina)
                                  || evt.MapInstance.MapInstanceType.Equals(MapInstanceType.Act4Hatus)
                                  || evt.MapInstance.MapInstanceType.Equals(MapInstanceType.Act4Morcos)))
                                {
                                    portal.IsDisabled = true;
                                }

                                evt.MapInstance.Broadcast(portal.GenerateGp());
                            }

                            break;

                        case EventActionType.CHANGEDROPRATE:
                            evt.MapInstance.DropRate = (int)evt.Parameter;
                            break;

                        case EventActionType.CHANGEXPRATE:
                            evt.MapInstance.XpRate = (int)evt.Parameter;
                            break;

                        case EventActionType.CLEARMAPMONSTERS:
                            foreach (var mapMonster in evt.MapInstance.Monsters.ToList()
                                                          .Where(s => s.Owner?.Character == null && s.Owner?.Mate == null))
                            {
                                evt.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster,
                                        mapMonster.MapMonsterId));
                                mapMonster.SetDeathStatement();
                                evt.MapInstance.RemoveMonster(mapMonster);
                            }

                            break;

                        case EventActionType.DISPOSEMAP:
                            evt.MapInstance.Dispose();
                            break;

                        case EventActionType.SPAWNBUTTON:
                            evt.MapInstance.SpawnButton((MapButton)evt.Parameter);
                            break;

                        case EventActionType.UNSPAWNMONSTERS:
                            evt.MapInstance.DespawnMonster((int)evt.Parameter);
                            break;

                        case EventActionType.SPAWNMONSTER:
                            evt.MapInstance.SummonMonster((MonsterToSummon)evt.Parameter);
                            break;

                        case EventActionType.SPAWNMONSTERS:
                            evt.MapInstance.SummonMonsters((List<MonsterToSummon>)evt.Parameter);
                            break;

                        case EventActionType.REFRESHRAIDGOAL:
                            var cl = evt.MapInstance.Sessions.FirstOrDefault();
                            if (cl?.Character != null)
                            {
                                ServerManager.Instance.Broadcast(cl, cl.Character?.Group?.GeneraterRaidmbf(cl),
                                        ReceiverType.Group);
                                ServerManager.Instance.Broadcast(cl,
                                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NEW_MISSION"),
                                                0), ReceiverType.Group);
                            }

                            break;

                        case EventActionType.SPAWNNPC:
                            evt.MapInstance.SummonNpc((NpcToSummon)evt.Parameter);
                            break;

                        case EventActionType.SPAWNNPCS:
                            evt.MapInstance.SummonNpcs((List<NpcToSummon>)evt.Parameter);
                            break;

                        case EventActionType.DROPITEMS:
                            evt.MapInstance.DropItems((List<Tuple<short, int, short, short>>)evt.Parameter);
                            break;

                        case EventActionType.THROWITEMS:
                            var parameters = (Tuple<int, short, byte, int, int, short>)evt.Parameter;
                            if (monster != null)
                            {
                                parameters = new Tuple<int, short, byte, int, int, short>(monster.MapMonsterId,
                                        parameters.Item2, parameters.Item3, parameters.Item4, parameters.Item5,
                                        parameters.Item6);
                            }

                            evt.MapInstance.ThrowItems(parameters);
                            break;

                        case EventActionType.SPAWNONLASTENTRY:
                            var lastincharacter = evt.MapInstance.Sessions.OrderByDescending(s => s.RegisterTime).FirstOrDefault()?.Character;
                            var summonParameters = new List<MonsterToSummon>();
                            var hornSpawn = new MapCell
                            {
                                X = lastincharacter?.PositionX ?? 154,
                                Y = lastincharacter?.PositionY ?? 140
                            };
                            var hornTarget = lastincharacter?.BattleEntity ?? null;
                            summonParameters.Add(new MonsterToSummon(Convert.ToInt16(evt.Parameter), hornSpawn, hornTarget, true));
                            evt.MapInstance.SummonMonsters(summonParameters);
                            break;

                        case EventActionType.REMOVEAFTER:
                            {
                                Observable.Timer(TimeSpan.FromSeconds(Convert.ToInt16(evt.Parameter)))
                                          .Subscribe(o =>
                                          {
                                              if (monster != null)
                                              {
                                                  monster.SetDeathStatement();
                                                  evt.MapInstance.RemoveMonster(monster);
                                                  evt.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, monster.MapMonsterId));

                                              }
                                          });
                            }
                            break;

                        case EventActionType.REMOVELAURENABUFF:
                            {
                                Observable.Timer(TimeSpan.FromSeconds(1))
                                          .Subscribe(observer =>
                                          {
                                              if (evt.Parameter is BattleEntity battleEntity
                                               && evt.MapInstance?.Monsters != null
                                               && !evt.MapInstance.Monsters.ToList().Any(s => s.MonsterVNum == 2327))
                                              {
                                                  battleEntity.RemoveBuff(475);
                                              }
                                          });
                            }
                            break;

                            #endregion
                    }
                }
            }
        }

        public void ScheduleEvent(TimeSpan timeSpan, EventContainer evt)
        {
            Observable.Timer(timeSpan).Subscribe(x => RunEvent(evt));
        }

        #endregion
    }
}