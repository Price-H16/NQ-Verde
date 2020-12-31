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
            EventHelper.Instance.RunEvent(new EventContainer(
                ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(9)),
                EventActionType.NPCSEFFECTCHANGESTATE, true));
            var result = TimeExtensions.RoundUp(DateTime.Now, TimeSpan.FromMinutes(1)); //5
            Observable.Timer(result - DateTime.Now).Subscribe(X => Act4ShipThread.Run(faction));
        }

        #endregion
    }

    public static class Act4ShipThread
    {
        #region Methods

        public static void Run(byte faction)
        {
            var map = ServerManager.GenerateMapInstance(149,
                faction == 1 ? MapInstanceType.Act4ShipAngel : MapInstanceType.Act4ShipDemon, new InstanceBag());
            if (map == null) return;
            var mapNpc1 = new MapNpc
            {
                NpcVNum = 613,
                MapNpcId = map.GetNextNpcId(),
                Dialog = 434,
                MapId = 177,
                MapX = 76,
                MapY = 127,
                IsMoving = false,
                Position = 1,
                IsSitting = false
            };
            mapNpc1.Initialize(map);
            map.AddNPC(mapNpc1);
            var mapNpc2 = new MapNpc
            {
                NpcVNum = 540,
                MapNpcId = map.GetNextNpcId(),
                Dialog = 433,
                MapId = 177,
                MapX = 76,
                MapY = 127,
                IsMoving = false,
                Position = 3,
                IsSitting = false
            };
            mapNpc2.Initialize(map);
            map.AddNPC(mapNpc2);
            while (true)
            {
                openShip();
                /*Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 4), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 3), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SHIP_MINUTES"), 2), 0));
                Thread.Sleep(60 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_MINUTE"), 0));
                lockShip();
                Thread.Sleep(30 * 1000);*/
                map.Broadcast(
                    UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 30), 0));
                Thread.Sleep(20 * 1000);
                map.Broadcast(
                    UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("SHIP_SECONDS"), 10), 0));
                Thread.Sleep(10 * 1000);
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_SETOFF"), 0));
                var sessions = map.Sessions.Where(s => s?.Character != null).ToList();
                Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(X => teleportPlayers(sessions));
            }
        }

        //private static void lockShip() => EventHelper.Instance.RunEvent(new EventContainer(ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(177)), EventActionType.NPCSEFFECTCHANGESTATE, true));

        private static void openShip()
        {
            EventHelper.Instance.RunEvent(new EventContainer(
                ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(1)),
                EventActionType.NPCSEFFECTCHANGESTATE, false));
        }

        private static void teleportPlayers(List<ClientSession> sessions)
        {
            foreach (var session in sessions)
                if (ServerManager.Instance.IsAct4Online())
                {
                    switch (session.Character.Faction)
                    {
                        case FactionType.None:
                            ServerManager.Instance.ChangeMap(session.Character.CharacterId, 9, 78, 132); //145 51 41
                            session.SendPacket(
                                UserInterfaceHelper.GenerateInfo("You need to be part of a faction to join Act 4"));
                            return;

                        case FactionType.Angel:
                            session.Character.MapId = 130;
                            session.Character.MapX = 12;
                            session.Character.MapY = 40;
                            break;

                        case FactionType.Demon:
                            session.Character.MapId = 131;
                            session.Character.MapX = 12;
                            session.Character.MapY = 40;
                            break;
                    }

                    session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP,
                        ServerManager.Instance.Configuration.Act4Port, 1);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, 9, 78, 132); //145 51 41
                    session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("ACT4_OFFLINE")));
                }
        }

        #endregion
    }
}