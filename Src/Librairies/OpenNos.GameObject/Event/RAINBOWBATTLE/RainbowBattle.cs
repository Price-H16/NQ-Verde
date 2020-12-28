using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.RainbowBattle;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using ChickenAPI.Enums.Game.Buffs;

namespace OpenNos.GameObject.Event.RAINBOWBATTLE
{
    public static class ExtensionTimer
    {
        #region Methods

        public static void SendInMapAfter(this MapInstance map, double sec, string packet)
        {
            Observable.Timer(TimeSpan.FromSeconds(sec)).Subscribe(o => { map.Broadcast(packet); });
        }

        #endregion
    }

    public class RainbowBattle
    {
        #region Members

        // Team pvp 30 max ( 30 / 2 = 15 vs 15 ) ↓ 3v3
        private static readonly int GroupPlayer = 10;

        #endregion

        #region Methods

        public static void CheckAll(List<ClientSession> ses)
        {
            if (ses.Count() == GroupPlayer)
            {
                var map = ServerManager.GenerateMapInstance(2010, MapInstanceType.RainbowBattleInstance, new InstanceBag());
                foreach (var ss in ses)
                {
                    RemoveAllPets(ss);
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(ss, map.MapInstanceId);
                    ss.SendPacket(UserInterfaceHelper.GenerateBSInfo(2, 7, 0, 0));
                    ss.SendPacket("rsfp 0 0");
                }
                new RainbowThread().RunEvent(map);
                ses.Clear();
            }
        }

        public static void GenerateEvent()
        {
            SendShout();
            SendEvent();
        }

        public static void RemoveAllPets(ClientSession ses)
        {
            foreach (var mateTeam in ses.Character.Mates?.Where(s => s.IsTeamMember))
            {
                if (mateTeam == null) continue;
                mateTeam.RemoveTeamMember(true);
            }
        }

        public static void SendEvent()
        {
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_MINUTES"), 5), 0));
#pragma warning disable 4014
            DiscordWebhookHelper.DiscordEventT($"ServerEvent: RainbowBattle will start in 5 minutes, are you ready?");
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_MINUTES"), 5), 1));
            Thread.Sleep(4 * 60 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_MINUTES"), 1), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_MINUTES"), 1), 1));
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_SECONDS"), 30), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_SECONDS"), 30), 1));
            Thread.Sleep(20 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_SECONDS"), 10), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOW_SECONDS"), 10), 1));
            Thread.Sleep(10 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAINBOW_STARTED"), 1));
            ServerManager.Instance.EventInWaiting = true;
            IEnumerable<ClientSession> checkMute = ServerManager.Instance.Sessions.Where(s => s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);
            checkMute = ServerManager.Instance.Sessions.Where(s => s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);
            foreach (ClientSession s in checkMute)
            {
                if (s.Character.IsMuted() == false)
                {
                    ServerManager.Instance.Sessions.Where(x => x.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance).ToList()
                        .ForEach(x => x.SendPacket($"qnaml 8 #guri^503 Do you want to do the Rainbow Battle ?"));
                }
            }
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAINBOW_STARTED"), 1));
            ServerManager.Instance.Sessions.ToList().ForEach(s => s.SendPacket("esf 0"));
            ServerManager.Instance.EventInWaiting = false;
            IEnumerable<ClientSession> sessions =
                ServerManager.Instance.Sessions.Where
                (s => s.Character?.IsWaitingForEvent == true &&
                s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);

            Tuple<List<ClientSession>, List<ClientSession>> a = new Tuple<List<ClientSession>, List<ClientSession>>(new List<ClientSession>(), new List<ClientSession>());

            // Create 3v3 RBB
            foreach (ClientSession s in sessions.OrderBy(s => Guid.NewGuid()).ToList())
            {
                if (s.Character.Level == 99)
                {
                    a.Item1.Add(s);
                }

                CheckAll(a.Item1);
            }

            sessions = ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == true && s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);

            foreach (ClientSession s in sessions)
            {
                s.SendPacket("msg 0 There is no team for the moment !");
            }

            ServerManager.Instance.Sessions.Where(s => s.Character != null).ToList().ForEach(s => s.Character.IsWaitingForEvent = false);
            ServerManager.Instance.StartedEvents.Remove(EventType.RAINBOWBATTLE);
        }

        public static void SendShout()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "The Rainbow Battle will start in 5 minutes on the channel 1 !",
                Type = MessageType.Shout
            });
        }

        #endregion

        #region Classes

        public class RainbowThread
        {
            #region Members

            private readonly int TimeRBBSec = 605;

            #endregion

            #region Methods

            public void CreateGroup(IEnumerable<ClientSession> session)
            {
                int team1 = 0;
                int team2 = 0;
                var group = new Group
                {
                    GroupType = GroupType.RBBBlue
                };
                ServerManager.Instance.AddGroup(group);
                var group2 = new Group
                {
                    GroupType = GroupType.RBBRed
                };
                ServerManager.Instance.AddGroup(group2);
                List<ClientSession> firstTeam = new List<ClientSession>();
                List<ClientSession> secondTeam = new List<ClientSession>();
                foreach (ClientSession ses in session)
                {
                    if (RainbowBattleManager.AreNotInMap(ses))
                    {
                        continue;
                    }

                    ses.Character.Group?.LeaveGroup(ses);

                    var value = team1 - team2;

                    if (value == 0)
                    {
                        team1++;
                        firstTeam.Add(ses);
                        group.JoinGroup(ses);
                    }
                    else
                    {
                        team2++;
                        secondTeam.Add(ses);
                        group2.JoinGroup(ses);
                    }

                    ServerManager.Instance.UpdateGroup(ses.Character.CharacterId);
                }

                var blueTeam = new RainbowBattleTeam(firstTeam, RainbowTeamBattleType.Blue, null);
                var redTeam = new RainbowBattleTeam(secondTeam, RainbowTeamBattleType.Red, blueTeam);
                blueTeam.SecondTeam = redTeam;

                ServerManager.Instance.RainbowBattleMembers.Add(blueTeam);
                ServerManager.Instance.RainbowBattleMembers.Add(redTeam);
            }
            private static int GetFlag(RainbowBattleTeam RainbowBattleTeam, RainbowNpcType type)
            {
                if (RainbowBattleTeam == null) return 0;
                return RainbowBattleTeam.TotalFlag.FindAll(s => s.Item2 == type).Count();
            }

            public void GenerateBattleRainbowPacket(ClientSession ee)
            {
                string rndm = string.Empty;
                string rndm2 = string.Empty;
                var RainbowTeam = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(ee));

                if (RainbowTeam == null)
                {
                    return;
                }

                foreach (var bb in RainbowTeam.Session)
                {
                    if (RainbowBattleManager.AreNotInMap(bb))
                    {
                        continue;
                    }
                    //fblst Level.Reputation.Class.Morph.Name.Gender.CharacterId.HeroLevel 
                    rndm += $"{bb.Character.CharacterId} ";
                    rndm2 +=
                        $"{bb.Character.Level}." +
                        $"{bb.Character.Reputation}." + 
                        $"{(byte)bb.Character.Class}." +
                        $"{bb.Character.Morph}." +
                        //$"0." +
                        $"{bb.Character.Name}." +
                        $"{(byte)bb.Character.Gender}." +
                        $"{bb.Character.CharacterId}." +
                        $"{bb.Character.HeroLevel} ";
                }

                var value = RainbowTeam.TeamEntity;

                if (RainbowBattleManager.AreNotInMap(ee))
                {
                    return;
                }

                ee.SendPacket("fbt 0 1");
                ee.SendPacket($"fbt 1 {rndm}");
                ee.SendPacket($"fblst {rndm2}");
                ee.SendPacket($"fbt 5 2 {(byte)value}");
                ee.SendPacket($"fbt 5 1 {TimeRBBSec}");
                ee.SendPacket($"msg 0 You are on the {value} team !");
                ee.SendPacket($"fbs {(byte)RainbowTeam.TeamEntity} {RainbowTeam.Session.Count()} {RainbowTeam.Score} {RainbowTeam.SecondTeam.Score} {GetFlag(RainbowTeam, RainbowNpcType.Small)} {GetFlag(RainbowTeam, RainbowNpcType.Second)} {GetFlag(RainbowTeam, RainbowNpcType.Large)} {RainbowTeam.TeamEntity}");
                ee?.SendPacket(ee?.Character?.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("TEAM_RBB"), value), 10));
            }

            public void RunEvent(MapInstance map)
            {
                CreateGroup(map.Sessions);
                foreach (ClientSession sess in map.Sessions)
                {
                    sess.Character.DisableBuffs(BuffType.All);
                    sess.Character.Hp = (int)sess.Character.HPLoad();
                    sess.Character.Mp = (int)sess.Character.MPLoad();
                    sess.SendPacket(sess.Character.GenerateStat());
                    GenerateBattleRainbowPacket(sess);
                    TeleportPlayerInSafeZone(sess);
                }
                SummonNpc(map);
                map.Broadcast("msg 0 Battle begins in 60 seconds.");
                map.SendInMapAfter(10, "msg 0 Battle begins in 50 seconds.");
                map.SendInMapAfter(20, "msg 0 Battle begins in 40 seconds.");
                map.SendInMapAfter(30, "msg 0 Battle begins in 30 seconds.");
                map.SendInMapAfter(40, "msg 0 Battle begins in 20 seconds.");
                map.SendInMapAfter(50, "msg 0 Battle begins in 10 seconds.");
                map.SendInMapAfter(60, "msg 0 The battle begins !");
                foreach (ClientSession sess in map.Sessions)
                {
                    sess.Character.NoMove = true;
                    sess.Character.NoAttack = false;
                    sess.SendPacket(sess.Character.GenerateCond());
                }
                IDisposable spawnsDisposable = null;
                Observable.Timer(TimeSpan.FromSeconds(60)).Subscribe(o =>
                {
                    foreach (ClientSession sess in map.Sessions)
                    {
                        sess.Character.NoMove = false;
                        sess.SendPacket(sess.Character.GenerateCond());
                    }
                    map.IsPVP = true;

                    spawnsDisposable = Observable.Interval(TimeSpan.FromSeconds(60)).Subscribe(s =>
                    {
                        RainbowBattleManager.GenerateScoreForAll();
                    });
                });

                Observable.Timer(TimeSpan.FromSeconds(TimeRBBSec)).Subscribe(o =>
                {
                    spawnsDisposable?.Dispose();
                    RainbowBattleManager.EndEvent(map.Sessions.Last(), map);
                });
            }

            public void SummonNpc(MapInstance map)
            {
                List<MapNpc> npc = new List<MapNpc>
                {
                    new MapNpc
                    {
                        NpcVNum = 922,
                        MapNpcId = map.GetNextNpcId(),
                        Dialog = 0,
                        MapId = map.Map.MapId,
                        MapX = 59,
                        MapY = 40,
                        IsMoving = false,
                        Position = 0,
                        IsSitting = false,
                        Effect = 3009,
                        Score = 3
                    },
                     new MapNpc
                    {
                        NpcVNum = 922,
                        MapNpcId = map.GetNextNpcId(),
                        Dialog = 0,
                        MapId = map.Map.MapId,
                        MapX = 85,
                        MapY = 4,
                        IsMoving = false,
                        Position = 0,
                        IsSitting = false,
                        Effect = 3009,
                        Score = 3
                    },
                    new MapNpc
                    {
                        NpcVNum = 922,
                        MapNpcId = map.GetNextNpcId(),
                        Dialog = 0,
                        MapId = map.Map.MapId,
                        MapX = 32,
                        MapY = 75,
                        IsMoving = false,
                        Position = 0,
                        IsSitting = false,
                        Effect = 3009,
                        Score = 3
                    },
                    new MapNpc
                    {
                        NpcVNum = 924,
                        MapNpcId = map.GetNextNpcId(),
                        Dialog = 0,
                        MapId = map.Map.MapId,
                        MapX = 15,
                        MapY = 40,
                        IsMoving = false,
                        Position = 0,
                        IsSitting = false,
                        Effect = 3009,
                        Score = 1
                    },
                    new MapNpc
                    {
                        NpcVNum = 924,
                        MapNpcId = map.GetNextNpcId(),
                        Dialog = 999,
                        MapId = map.Map.MapId,
                        MapX = 102,
                        MapY = 39,
                        IsMoving = false,
                        Position = 0,
                        IsSitting = false,
                        Effect = 3009,
                        Score = 1
                    },
                };

                List<MapMonster> monster = new List<MapMonster>
                {
                    new MapMonster
                    {
                        MonsterVNum = 2558,
                        MapX = 43,
                        MapY = 26,
                        MapId = map.Map.MapId,
                        Position = 1,
                        IsMoving = false,
                        MapMonsterId = map.GetNextMonsterId(),
                        ShouldRespawn = false,
                        IsHostile = true
                    },
                    new MapMonster
                    {
                        MonsterVNum = 2558,
                        MapX = 74,
                        MapY = 53,
                        MapId = map.Map.MapId,
                        Position = 6,
                        IsMoving = false,
                        MapMonsterId = map.GetNextMonsterId(),
                        ShouldRespawn = false,
                        IsHostile = true
                    },
                };

                foreach (var mob in monster)
                {
                    mob.Initialize(map);
                    map.AddMonster(mob);
                    map.Broadcast(mob.GenerateIn());
                }

                foreach (var Stone in npc)
                {
                    Stone.Dialog = 999;
                    Stone.Initialize(map);
                    map.AddNPC(Stone);
                    map.Broadcast(Stone.GenerateIn());
                    map.Broadcast($"fbt 6 {Stone.MapNpcId} {Stone.Score} 0");
                }
            }

            public void TeleportPlayerInSafeZone(ClientSession ses)
            {
                var rbb = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(ses));
                if (rbb == null) return;
                ses.Character.PositionX = rbb.TeamEntity == RainbowTeamBattleType.Red ? ServerManager.RandomNumber<short>(117, 117) : ServerManager.RandomNumber<short>(0, 0);
                ses.Character.PositionY = rbb.TeamEntity == RainbowTeamBattleType.Red ? ServerManager.RandomNumber<short>(27, 59) : ServerManager.RandomNumber<short>(20, 51);
                ses.CurrentMapInstance.Broadcast(ses.Character.GenerateTp());
                ses.Character.NoAttack = true;
                ses.Character.NoMove = true;
                ses?.SendPacket(ses.Character.GenerateCond());
            }

            #endregion

            //fbt 3 1.10.100 1285990.100.88
        }

        #endregion
    }
}