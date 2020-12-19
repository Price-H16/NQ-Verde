﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.RainbowBattle;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G720 : IGuriHandler
    {
        public long GuriEffectId => 720;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 720)
            {
                MapNpc npc = Session.CurrentMapInstance.Npcs.Find(s => s.MapNpcId == e.User);

                if (Session == null || Session?.Character?.MapInstance == null) return;

                if (Session.Character.isFreezed == true) return;

                //Packet Hacking
                if (npc == null) return;


                int dist = Map.GetDistance(
                    new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                    new MapCell { X = npc.MapX, Y = npc.MapY });
                if (dist > 5)
                {
                    return;
                }

                var RainbowTeam = ServerManager.Instance.RainbowBattleMembers.First(s => s.Session.Contains(Session));

                if (RainbowTeam == null || RainbowBattleManager.AlreadyHaveFlag(RainbowTeam, (RainbowNpcType)e.Argument, (int)e.User)) return;

                RainbowBattleManager.AddFlag(Session, RainbowTeam, (RainbowNpcType)e.Argument, (int)e.User);
            }
        }
    }
} 