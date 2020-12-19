/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Event
{
    public static class InstantBattle
    {
        #region Methods
        //EXaMPLE OF HOW IT IS DONE YOU WILL DO IT IN ALL DO IT IN ALL THE EVENTS

        public static void GenerateInstantBattle()
        {
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 5), 0));
#pragma warning disable 4014
            DiscordWebhookHelper.DiscordEventT($"ServerEvent: Instant Battle will start in 5 minutes, are you ready?");
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 5), 1));
            Thread.Sleep(4 * 60 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 1), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES"), 1), 1));
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 30), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 30), 1));
            Thread.Sleep(20 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 10), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS"), 10), 1));
            Thread.Sleep(10 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_STARTED"), 1));
            ServerManager.Instance.Sessions.Where(s => s.Character?.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance).ToList().ForEach(s => s.SendPacket($"qnaml 1 #guri^506 {Language.Instance.GetMessageFromKey("INSTANTBATTLE_QUESTION")}"));
            ServerManager.Instance.EventInWaiting = true;
            Thread.Sleep(30 * 1000);
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_STARTED"), 1));
            ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == false).ToList().ForEach(s => s.SendPacket("esf"));
            ServerManager.Instance.EventInWaiting = false;
            IEnumerable<ClientSession> sessions = ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == true && s.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);
            List<Tuple<MapInstance, byte>> maps = new List<Tuple<MapInstance, byte>>();
            MapInstance map = null;
            int i = -1;
            int level = 0;
            byte instancelevel = 1;
            foreach (ClientSession s in sessions.OrderBy(s => s.Character?.Level))
            {
                i++;
                if (s.Character.Level > 79 && level <= 79)
                {
                    i = 0;
                    instancelevel = 80;
                }
                else if (s.Character.Level > 69 && level <= 69)
                {
                    i = 0;
                    instancelevel = 70;
                }
                else if (s.Character.Level > 59 && level <= 59)
                {
                    i = 0;
                    instancelevel = 60;
                }
                else if (s.Character.Level > 49 && level <= 49)
                {
                    i = 0;
                    instancelevel = 50;
                }
                else if (s.Character.Level > 39 && level <= 39)
                {
                    i = 0;
                    instancelevel = 40;
                }
                if (i % 50 == 0)
                {
                    map = ServerManager.GenerateMapInstance(2004, MapInstanceType.NormalInstance, new InstanceBag());
                    maps.Add(new Tuple<MapInstance, byte>(map, instancelevel));
                }
                if (map != null)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(s, map.MapInstanceId);
                }

                level = s.Character.Level;
            }
            ServerManager.Instance.Sessions.Where(s => s.Character != null).ToList().ForEach(s => s.Character.IsWaitingForEvent = false);
            ServerManager.Instance.StartedEvents.Remove(EventType.INSTANTBATTLE);
            foreach (Tuple<MapInstance, byte> mapinstance in maps)
            {
                InstantBattleTask task = new InstantBattleTask();
                Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => InstantBattleTask.Run(mapinstance));
            }
        }

        #endregion

        #region Classes

        public class InstantBattleTask
        {
            #region Methods

            public static void Run(Tuple<MapInstance, byte> mapinstance)
            {
                long maxGold = ServerManager.Instance.Configuration.MaxGold;
                Thread.Sleep(10 * 1000);
                if (!mapinstance.Item1.Sessions.Skip(3 - 1).Any())
                {
                    mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList().ForEach(s => {
                        s.Character.RemoveBuffByBCardTypeSubType(new List<KeyValuePair<byte, byte>>()
                        {
                            new KeyValuePair<byte, byte>((byte)BCardType.CardType.SpecialActions, (byte)AdditionalTypes.SpecialActions.Hide),
                            new KeyValuePair<byte, byte>((byte)BCardType.CardType.FalconSkill, (byte)AdditionalTypes.FalconSkill.Hide),
                            new KeyValuePair<byte, byte>((byte)BCardType.CardType.FalconSkill, (byte)AdditionalTypes.FalconSkill.Ambush)
                        });
                        ServerManager.Instance.ChangeMap(s.Character.CharacterId, s.Character.MapId, s.Character.MapX, s.Character.MapY);
                    });
                }
                Observable.Timer(TimeSpan.FromMinutes(12)).Subscribe(X =>
                {
                    for (int d = 0; d < 180; d++)
                    {
                        if (!mapinstance.Item1.Monsters.Any(s => s.CurrentHp > 0))
                        {
                            EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SPAWNPORTAL, new Portal { SourceX = 47, SourceY = 33, DestinationMapId = 1 }));
                            mapinstance.Item1.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SUCCEEDED"), 0));
                            foreach (ClientSession cli in mapinstance.Item1.Sessions.Where(s => s.Character != null).ToList())
                            {
                                cli.Character.GenerateFamilyXp(cli.Character.Level * 20);
                                cli.Character.GetReputation(cli.Character.Level * 300);
                                cli.Character.Gold += cli.Character.Level * 7500;
                                cli.Character.Gold = cli.Character.Gold > maxGold ? maxGold : cli.Character.Gold;
                                cli.Character.SpAdditionPoint += cli.Character.Level * 1000;

                                if (cli.Character.Level >= 60)
                                {
                                    cli.Character.GiftAdd(2236, 1);
                                }
                                if (cli.Character.SpAdditionPoint > 1000000)
                                {
                                    cli.Character.SpAdditionPoint = 1000000;
                                }
                                
                                cli.SendPacket(cli.Character.GenerateSpPoint());
                                cli.SendPacket(cli.Character.GenerateGold());
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_MONEY"), cli.Character.Level * 7500), 10));
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_REPUT"), cli.Character.Level * 300), 10));
                                if (cli.Character.Family != null)
                                {
                                    cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_FXP"), cli.Character.Level * 20), 10));
                                }
                                cli.SendPacket(cli.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WIN_SP_POINT"), cli.Character.Level * 1000), 10));
                            }
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                });

                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(15), new EventContainer(mapinstance.Item1, EventActionType.DISPOSEMAP, null));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(3), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 12), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 10), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 5), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(11), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 4), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(12), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 3), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(13), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 2), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MINUTES_REMAINING"), 1), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS_REMAINING"), 30), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(14.5), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("INSTANTBATTLE_SECONDS_REMAINING"), 30), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromMinutes(0), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_INCOMING"), 0)));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_HERE"), 0)));

                for (int wave = 0; wave < 4; wave++)
                {
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(130 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_WAVE"), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(160 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_INCOMING"), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(170 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SENDPACKET, UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_MONSTERS_HERE"), 0)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(10 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance.Item2, wave)));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(140 + (wave * 160)), new EventContainer(mapinstance.Item1, EventActionType.DROPITEMS, getInstantBattleDrop(mapinstance.Item1.Map, mapinstance.Item2, wave)));
                }
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(650), new EventContainer(mapinstance.Item1, EventActionType.SPAWNMONSTERS, getInstantBattleMonster(mapinstance.Item1.Map, mapinstance.Item2, 4)));
            }

            private static IEnumerable<Tuple<short, int, short, short>> generateDrop(Map map, short vnum, int amountofdrop, int amount)
            {
                List<Tuple<short, int, short, short>> dropParameters = new List<Tuple<short, int, short, short>>();
                for (int i = 0; i < amountofdrop; i++)
                {
                    MapCell cell = map.GetRandomPosition();
                    dropParameters.Add(new Tuple<short, int, short, short>(vnum, amount, cell.X, cell.Y));
                }
                return dropParameters;
            }

            private static List<Tuple<short, int, short, short>> getInstantBattleDrop(Map map, short instantbattletype, int wave)
            {
                List<Tuple<short, int, short, short>> dropParameters = new List<Tuple<short, int, short, short>>();
                switch (instantbattletype)
                {
                    case 1:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 3500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 5, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 2035, 10, 1)); // Metallo normale
                                dropParameters.AddRange(generateDrop(map, 2038, 10, 1)); // Legno di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2042, 10, 1)); // Pelle di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2046, 10, 1)); // Stoffa di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 1013, 10, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1009, 7, 1)); // Medium recovery potion
                                dropParameters.AddRange(generateDrop(map, 2002, 5, 1)); // Lana
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 5750)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 10, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 2035, 5, 2)); // Metallo normale
                                dropParameters.AddRange(generateDrop(map, 2038, 5, 2)); // Legno di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2042, 5, 2)); // Pelle di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2046, 5, 2)); // Stoffa di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 1013, 15, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1009, 10, 1)); // Medium recovery potion
                                dropParameters.AddRange(generateDrop(map, 2002, 12, 1)); // Lana
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 8500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 10, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 2035, 5, 3)); // Metallo normale
                                dropParameters.AddRange(generateDrop(map, 2038, 5, 3)); // Legno di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2042, 5, 3)); // Pelle di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2046, 5, 3)); // Stoffa di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 1013, 20, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1009, 10, 1)); // Medium recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2002, 10, 1)); // Lana
                                dropParameters.AddRange(generateDrop(map, 2282, 10, 1)); // Ali d'angelo
                                break;

                            case 3:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 10500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 10, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 2035, 5, 3)); // Metallo normale
                                dropParameters.AddRange(generateDrop(map, 2038, 5, 3)); // Legno di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2042, 5, 3)); // Pelle di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 2046, 5, 3)); // Stoffa di bassa qualità
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1009, 10, 1)); // Medium recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2002, 10, 1)); // Lana
                                dropParameters.AddRange(generateDrop(map, 2282, 10, 1)); // Ali d'angelo
                                break;
                        }
                        break;

                    case 40:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 10500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 15, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 1)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 2282, 10, 1)); // Ali d'angelo
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 12500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 15, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 2282, 12, 1)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 1)); // Perla dell'oscurità
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 15000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 1028, 15, 1)); //cristallo di nuova luna
                                dropParameters.AddRange(generateDrop(map, 2282, 12, 1)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 1)); // Perla dell'oscurità
                                break;

                            case 3:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 15000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 1)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2094, 20, 1)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1029, 7, 1)); // Cristallo di mezza-luna
                                break;
                        }
                        break;

                    case 50:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 15000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 1)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 3, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 3, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 3, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2094, 10, 2)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1029, 7, 1)); // Cristallo di mezza-luna
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 17500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 7, 3)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 1246, 5, 1)); // Atk pot
                                dropParameters.AddRange(generateDrop(map, 1247, 5, 1)); // Def pot
                                dropParameters.AddRange(generateDrop(map, 1248, 5, 1)); // Energy pot
                                dropParameters.AddRange(generateDrop(map, 2094, 10, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1029, 5, 2)); // Cristallo di mezza-luna
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 17500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 7, 3)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 2094, 10, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1029, 5, 2)); // Cristallo di mezza-luna
                                break;

                            case 3:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 20500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 7, 3)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 2094, 10, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1029, 5, 2)); // Cristallo di mezza-luna
                                break;
                        }
                        break;

                    case 60:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 22500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 13, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1029, 10, 2)); // Cristallo di mezza-luna
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 25500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1010, 10, 1)); // Large recovery potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 5, 1)); // Cristallo di luna piena
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 27500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 5, 1)); // Cristallo di luna piena
                                break;

                            case 3:
                                dropParameters.AddRange(generateDrop(map, 1046, 20, 30000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 5, 1)); // Cristallo di luna piena
                                break;
                        }
                        break;

                    case 70:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 30000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 15, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 10, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 2307, 5, 2)); // Ice cubes
                                dropParameters.AddRange(generateDrop(map, 9020, 5, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 9021, 5, 1)); // Strong defence potion
                                dropParameters.AddRange(generateDrop(map, 9022, 5, 1)); // Strong energy potion
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 32500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 30, 1)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 10, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 2307, 5, 2)); // Ice cubes
                                dropParameters.AddRange(generateDrop(map, 9020, 5, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 9021, 5, 1)); // Strong defence potion
                                dropParameters.AddRange(generateDrop(map, 9022, 5, 1)); // Strong energy potion
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 35500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 25, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 10, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 2307, 5, 2)); // Ice cubes
                                dropParameters.AddRange(generateDrop(map, 9020, 5, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 9021, 5, 1)); // Strong defence potion
                                dropParameters.AddRange(generateDrop(map, 9022, 5, 1)); // Strong energy potion
                                break;

                            case 3:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 37500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 25, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 10, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 2307, 5, 2)); // Ice cubes
                                dropParameters.AddRange(generateDrop(map, 9020, 5, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 9021, 5, 1)); // Strong defence potion
                                dropParameters.AddRange(generateDrop(map, 9022, 5, 1)); // Strong energy potion
                                break;
                        }
                        break;

                    case 80:
                        switch (wave)
                        {
                            case 0:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 42500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 25, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 12, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 9020, 5, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 9021, 5, 1)); // Strong defence potion
                                dropParameters.AddRange(generateDrop(map, 9022, 5, 1)); // Strong energy potion
                                break;

                            case 1:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 45500)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 25, 2)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1011, 12, 2)); // Huge Recovery Potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 15, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 9020, 5, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 9021, 5, 1)); // Strong defence potion
                                dropParameters.AddRange(generateDrop(map, 9022, 5, 1)); // Strong energy potion
                                dropParameters.AddRange(generateDrop(map, 5863, 5, 1)); // Hellord Supply Ticket
                                dropParameters.AddRange(generateDrop(map, 5862, 5, 1)); // Mystic Heaven Supply Ticket
                                break;

                            case 2:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 50000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 25, 3)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1244, 15, 2)); // Divine recovery potion
                                dropParameters.AddRange(generateDrop(map, 2094, 15, 3)); // Perla dell'oscurità
                                dropParameters.AddRange(generateDrop(map, 1030, 15, 1)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 9020, 7, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 5863, 7, 1)); // Hellord Supply Ticket
                                dropParameters.AddRange(generateDrop(map, 5862, 7, 1)); // Mystic Heaven Supply Ticket
                                break;

                            case 3:
                                dropParameters.AddRange(generateDrop(map, 1046, 25, 75000)); //Oro 
                                dropParameters.AddRange(generateDrop(map, 2282, 30, 3)); //ala d'angelo
                                dropParameters.AddRange(generateDrop(map, 1013, 25, 1)); // Gillion
                                dropParameters.AddRange(generateDrop(map, 1244, 25, 2)); // Divine recovery potion
                                dropParameters.AddRange(generateDrop(map, 1030, 25, 2)); // Cristallo di luna piena
                                dropParameters.AddRange(generateDrop(map, 9020, 7, 1)); // Strong attack potion
                                dropParameters.AddRange(generateDrop(map, 5863, 12, 1)); // Hellord Supply Ticket
                                dropParameters.AddRange(generateDrop(map, 5862, 12, 1)); // Mystic Heaven Supply Ticket
                                break;
                        }
                        break;
                }
                return dropParameters;
            }

            private static List<MonsterToSummon> getInstantBattleMonster(Map map, short instantbattletype, int wave)
            {
                List<MonsterToSummon> SummonParameters = new List<MonsterToSummon>();

                switch (instantbattletype)
                {
                    case 1:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(1, 16, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(58, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(105, 16, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(107, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(108, 8, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(111, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(136, 15, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(194, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(114, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(99, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(39, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(2, 16, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(140, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(100, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(81, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(12, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(4, 16, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(115, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(112, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(110, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(14, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(5, 16, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(979, 1, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(167, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(137, 10, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(22, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(17, 8, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(16, 16, true, new List<EventContainer>()));
                                break;
                        }
                        break;

                    case 40:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(120, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(151, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(149, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(139, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(73, 16, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(152, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(147, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(104, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(62, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(8, 16, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(153, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(132, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(86, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(76, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(68, 16, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(134, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(91, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(133, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(70, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(89, 16, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(154, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(200, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(77, 8, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(217, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(724, 1, true, new List<EventContainer>()));
                                break;
                        }
                        break;

                    case 50:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(134, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(91, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(89, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(77, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(71, 16, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(217, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(200, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(154, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(92, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(79, 16, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(235, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(226, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(214, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(204, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(201, 15, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(249, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(236, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(227, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(218, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(202, 15, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(583, 1, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(400, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(255, 8, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(253, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(251, 10, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(205, 14, true, new List<EventContainer>()));
                                break;
                        }
                        break;

                    case 60:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(242, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(234, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(215, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(207, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(202, 13, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(402, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(253, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(237, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(216, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(205, 13, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(402, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(243, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(228, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(255, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(205, 13, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(268, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(255, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(254, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(174, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(172, 13, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(725, 1, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(407, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(272, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(261, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(256, 12, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(275, 13, true, new List<EventContainer>()));
                                break;
                        }
                        break;

                    case 70:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(402, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(253, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(237, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(216, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(205, 15, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(402, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(243, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(228, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(225, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(205, 15, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(255, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(254, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(251, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(174, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(172, 15, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(407, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(272, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(261, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(257, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(256, 15, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(748, 1, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(444, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(439, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(275, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(274, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(273, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(163, 13, true, new List<EventContainer>()));
                                break;
                        }
                        break;

                    case 80:
                        switch (wave)
                        {
                            case 0:
                                SummonParameters.AddRange(map.GenerateMonsters(1007, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1003, 15, false, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1002, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1001, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1000, 16, true, new List<EventContainer>()));
                                break;

                            case 1:
                                SummonParameters.AddRange(map.GenerateMonsters(1199, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1198, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1197, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1196, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1123, 16, true, new List<EventContainer>()));
                                break;

                            case 2:
                                SummonParameters.AddRange(map.GenerateMonsters(1305, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1304, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1303, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1302, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1194, 16, true, new List<EventContainer>()));
                                break;

                            case 3:
                                SummonParameters.AddRange(map.GenerateMonsters(1902, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1901, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1900, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1045, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1043, 15, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1042, 16, true, new List<EventContainer>()));
                                break;

                            case 4:
                                SummonParameters.AddRange(map.GenerateMonsters(637, 1, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1903, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1053, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1051, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1049, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1048, 13, true, new List<EventContainer>()));
                                SummonParameters.AddRange(map.GenerateMonsters(1047, 13, true, new List<EventContainer>()));
                                break;
                        }
                        break;
                }
                return SummonParameters;
            }

            #endregion
        }

        #endregion
    }
}