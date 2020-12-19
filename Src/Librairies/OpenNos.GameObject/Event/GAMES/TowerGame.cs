using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Event.GAMES
{
    public static class TowerGame
    {
        #region Methods

        public static void GenerateTowerGame()
        {
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 5 Minutes will start the Tower Game", 0)); //5 minutes
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 5 Minutes will start the Tower Game", 1));
            Thread.Sleep(4 * 60 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 1 Minute will start the Tower Game", 0)); //1 minute
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 1 Minute will start the Tower Game", 1));
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 30 seconds will start the Tower Game", 0)); //30 seconds
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 30 seconds will start the Tower Game", 1));
            Thread.Sleep(20 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 10 seconds will start the Tower Game", 0)); //10 seconds
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 10 seconds will start the Tower Game", 1));
            Thread.Sleep(10 * 1000);
            ServerManager.Instance.Broadcast($"qnaml 1 #guri^506 Join the Tower Game?");
            ServerManager.Instance.EventInWaiting = true;
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 5 seconds will start the Tower Game", 0)); //5 seconds
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("In 5 seconds will start the Tower Game", 1));
            Thread.Sleep(5 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("Tower Game starts !", 0)); //started
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("Tower Game starts !", 1));
            ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == false).ToList().ForEach(s => s.SendPacket("esf"));
            ServerManager.Instance.EventInWaiting = false;
            IEnumerable<ClientSession> sessions = ServerManager.Instance.Sessions.Where(s => s.Character != null && s.Character.IsWaitingForEvent);
            MapInstance map = ServerManager.GenerateMapInstance(4100, MapInstanceType.TowerInstanceType, new InstanceBag());

            foreach (ClientSession session in sessions)
            {
                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId, map.MapInstanceId, 14, 6);
            }

            map.Broadcast(UserInterfaceHelper.GenerateMsg("In 15 seconds will appear the Boss of this Level", 0));
            Thread.Sleep(2 * 1000);
            map.Broadcast(UserInterfaceHelper.GenerateMsg("There are 9 Levels in this Tower", 0));
            Thread.Sleep(2 * 1000);
            map.Broadcast(UserInterfaceHelper.GenerateMsg("Killing a Boss Level will teleport all the players on the next Floor", 0));
            Thread.Sleep(2 * 1000);
            map.Broadcast(UserInterfaceHelper.GenerateMsg("Every Boss will give you a reward plus a random reward and some golds", 0));
            Thread.Sleep(2 * 1000);
            map.Broadcast(UserInterfaceHelper.GenerateMsg("Ready ? 3... 2... 1...", 0));
            Thread.Sleep(2 * 1000);

            ServerManager.Instance.StartedEvents.Remove(EventType.TOWERGAME);
            TowerGameThread Tower = new TowerGameThread();
            Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(X => Tower.Run(map));
        }

        #endregion

        public class TowerGameThread
        {
            #region Members

            private short[] bosses = { 505, 593, 2691, 2331, 2034, 2049, 2678, 2673, 2574 }; //insert bosses here

            private short[] rewardsKill = { 1, 1696, 1696, 1696, 1696, 1696, 5871, 5871, 5871, 5871 }; //insert rewards here

            private short[] rewards = { 2339, 392, 393, 394, 395, 396, 5871, 1696, 259, 5872 }; //insert rewards here

            private byte floor = 0;

            private MapCell cell;

            #endregion

            #region Methods

            public void Run(MapInstance instance)
            {
                byte time = (byte)(30 + floor);

                if (instance.Map.MapId != 4100)
                {
                    if (instance.Map.MapId == 4109)
                    {
                        int divider = instance.Sessions.Count();
                        foreach (ClientSession sess in instance.Sessions.Where(h => h.Character.Hp > 0))
                        {
                            sess.Character.GiftAdd(rewardsKill[floor], 1);
                            sess.Character.GiftAdd(2339, (byte)(divider / 2));
                        }
                        EventHelper.Instance.RunEvent(new EventContainer(instance, EventActionType.SPAWNPORTAL, new Portal { SourceX = 60, SourceY = 70, DestinationMapId = 190, Type = -1, PortalId = 0, SourceMapInstanceId = instance.MapInstanceId, DestinationMapInstanceId = ServerManager.GetMapInstanceByMapId(190).MapInstanceId }));
                        instance.Broadcast(UserInterfaceHelper.GenerateMsg("You killed all the bosses of the Tower ! Well done", 0));
                        Thread.Sleep(20 * 1000);
                        EventHelper.Instance.RunEvent(new EventContainer(instance, EventActionType.DISPOSEMAP, null));
                        return;
                    }
                    else
                    {
                        foreach (ClientSession client in instance.Sessions.Where(player => player.Character.Hp > 0))
                        {
                            client.Character.GiftAdd(rewardsKill[floor], 1);
                            client.Character.GiftAdd(2339, 5);
                            switch (instance.Map.MapId)
                            {
                                case 4103:
                                case 4105:
                                case 4107:
                                    client.Character.gameLifes += (byte)(client.Character.gameLifes == 3 ? 0 : 1);
                                    break;
                            }
                            ServerManager.Instance.ChangeMapInstance(client.Character.CharacterId, instance.MapInstanceId, 14, 6);
                        }
                        instance.Broadcast(UserInterfaceHelper.GenerateMsg("The boss will arrive in 10 seconds !", 0));
                    }
                }

                Thread.Sleep(10 * 1000);

                instance.Broadcast(UserInterfaceHelper.GenerateMsg("Boss has arrived !", 0));

                MonsterToSummon monster = SummonParameters(instance, bosses[floor], cell, rewards[floor], 20, 1);

                Inizialization(instance, monster);

                do
                {
                    Thread.Sleep(10 * 1000);
                    if (instance.Monsters.FirstOrDefault(m => m.MonsterVNum == monster.VNum) == null)
                    {
                        floor++;
                        time = 0;
                        instance.Map.MapId++;
                        instance.Broadcast(UserInterfaceHelper.GenerateMsg("You will be teleported to the next floor in 30 seconds", 0));
                        Thread.Sleep(30 * 1000);
                        Parallel.ForEach(instance.DroppedList.GetAllItems(), drop =>
                        {
                            instance.Broadcast(StaticPacketHelper.Out(UserType.Object, drop.TransportId));
                            instance.DroppedList.Remove(drop.TransportId);
                        });
                        Run(instance);
                        return;
                    }
                    else
                    {
                        time--;
                        switch (time)
                        {
                            case 30:
                                instance.Broadcast(UserInterfaceHelper.GenerateMsg("You still have 5 minutes to kill the boss", 0));
                                break;

                            case 24:
                                instance.Broadcast(UserInterfaceHelper.GenerateMsg("You still have 4 minutes to kill the boss", 0));
                                break;

                            case 18:
                                instance.Broadcast(UserInterfaceHelper.GenerateMsg("You still have 3 minutes to kill the boss", 0));
                                break;

                            case 12:
                                instance.Broadcast(UserInterfaceHelper.GenerateMsg("You still have 2 minutes to kill the boss", 0));
                                break;

                            case 6:
                                instance.Broadcast(UserInterfaceHelper.GenerateMsg("You still have 1 minute to kill the boss", 0));
                                break;

                            case 1:
                                instance.Broadcast(UserInterfaceHelper.GenerateMsg("You still have 10 seconds to kill the boss", 0));
                                break;
                        }
                    }
                } while (time > 0);

                instance.Broadcast(UserInterfaceHelper.GenerateMsg("U LOST !", 0));

                Thread.Sleep(1000);

                EventHelper.Instance.RunEvent(new EventContainer(instance, EventActionType.DISPOSEMAP, null));
            }

            private static MonsterToSummon SummonParameters(MapInstance map, short monster, MapCell cell, short reward, byte times, int quantity)
            {
                cell = map.Map.GetRandomPosition();

                cell.X = 14;
                cell.Y = 6;

                MonsterToSummon summon = new MonsterToSummon(monster, cell, null, true, false, false, true, true);

                List<EventContainer> death = new List<EventContainer>
                {
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, reward, times, quantity, (quantity + 1))),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 2282, 50, 15, 16)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 1030, 50, 10, 11)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 2339, 15, 2, 3)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 2282, 50, 15, 16)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 392, 10, 1, 2)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 393, 10, 1, 2)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 394, 10, 1, 2)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 395, 10, 1, 2)),
                    new EventContainer(map, EventActionType.THROWITEMS, new Tuple<int, short, byte, int, int>(monster, 396, 5, 1, 2)),
                };

                summon.DeathEvents = death;

                return summon;
            }

            private static List<EventContainer> ClearDrops(MapInstance map, List<EventContainer> events, MapMonster monster)
            {
                events.RemoveAll(a => a.EventActionType == EventActionType.THROWITEMS);

                monster.BattleEntity.OnDeathEvents = events;

                return events;
            }

            private static void Inizialization(MapInstance map, MonsterToSummon monster)
            {
                EventHelper.Instance.RunEvent(new EventContainer(map, EventActionType.SPAWNMONSTER, monster));
            }

            #endregion
        }
    }
}