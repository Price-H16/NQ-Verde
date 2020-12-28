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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Event
{
    public static class ACT4SHIP
    {
        #region Methods

        public static void GenerateAct4Ship(byte faction)
        {
            EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(145)), EventActionType.NPCSEFFECTCHANGESTATE, true));
            DateTime result = Core.Extensions.TimeExtensions.RoundUp(DateTime.Now, TimeSpan.FromMinutes(5));
            Observable.Timer(result - DateTime.Now).Subscribe(X => Act4ShipThread.Run(faction));
        }

        #endregion
    }

    public static class Act4ShipThread
    {
        #region Methods

        public static void Run(byte faction)
        {
            MapInstance map = ServerManager.GenerateMapInstance(148, faction == 1 ? MapInstanceType.Act4ShipAngel : MapInstanceType.Act4ShipDemon, new InstanceBag());
            MapNpc mapNpc1 = new MapNpc
            {
                NpcVNum = 613,
                MapNpcId = map.GetNextNpcId(),
                Dialog = 434,
                MapId = 148,
                MapX = 8,
                MapY = 28,
                IsMoving = false,
                Position = 1,
                IsSitting = false
            };
            mapNpc1.Initialize(map);
            map.AddNPC(mapNpc1);
            MapNpc mapNpc2 = new MapNpc
            {
                NpcVNum = 540,
                MapNpcId = map.GetNextNpcId(),
                Dialog = 433,
                MapId = 148,
                MapX = 31,
                MapY = 28,
                IsMoving = false,
                Position = 3,
                IsSitting = false
            };
            mapNpc2.Initialize(map);
            map.AddNPC(mapNpc2);
            while (true)
            {
                openShip();
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 4), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 3), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 2), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_MINUTE"), 0));
                lockShip();
                Thread.Sleep(30 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 30), 0));
                Thread.Sleep(20 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 10), 0));
                Thread.Sleep(10 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_SETOFF"), 0));
                List<ClientSession> sessions = map.Sessions.Where(s => s?.Character != null).ToList();
                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(X => teleportPlayers(sessions));
            }
        }

        private static void lockShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(145)), EventActionType.NPCSEFFECTCHANGESTATE, true));

        private static void openShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(145)), EventActionType.NPCSEFFECTCHANGESTATE, false));

        private static void teleportPlayers(List<ClientSession> sessions)
        {
            foreach (ClientSession session in sessions)
            {
                if (ServerManager.Instance.IsAct4Online())
                {
                    switch (session.Character.Faction)
                    {
                        case FactionType.None:
                            ServerManager.Instance.ChangeMap(session.Character.CharacterId, 145, 51, 41);
                            session.SendPacket(UserInterfaceHelper.GenerateInfo("You need to be part of a faction to join Act 4"));
                            return;

                        case FactionType.Angel:
                            session.Character.MapId = 130;
                            session.Character.MapX = (short)(12 + ServerManager.RandomNumber(-2, 3));
                            session.Character.MapY = (short)(40 + ServerManager.RandomNumber(-3, 3));
                            break;

                        case FactionType.Demon:
                            session.Character.MapId = 131;
                            session.Character.MapX = (short)(12 + ServerManager.RandomNumber(-2, 3));
                            session.Character.MapY = (short)(40 + ServerManager.RandomNumber(-3, 3));
                            break;
                    }

                    session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP, ServerManager.Instance.Configuration.Act4Port, 1);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, 145, 51, 41);
                    session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("ACT4_OFFLINE")));
                }
            }
        }

        #endregion
    }
}