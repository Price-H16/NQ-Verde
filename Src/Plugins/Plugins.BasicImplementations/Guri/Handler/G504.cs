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

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G504 : IGuriHandler
    {
        public long GuriEffectId => 504;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 504)
            {
                long? targetId = e.User;

                if (targetId == null) return;

                ClientSession target = ServerManager.Instance.GetSessionByCharacterId(targetId.Value);

                if (target == null || target?.Character?.MapInstance == null) return;

                var rbb = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(target));

                if (target.Character.MapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
                {
                    target.Character.PositionX = rbb.TeamEntity == RainbowTeamBattleType.Red ? ServerManager.RandomNumber<short>(30, 32) : ServerManager.RandomNumber<short>(83, 85);
                    target.Character.PositionY = rbb.TeamEntity == RainbowTeamBattleType.Red ? ServerManager.RandomNumber<short>(73, 76) : ServerManager.RandomNumber<short>(2, 4);
                    target.CurrentMapInstance.Broadcast(target.Character.GenerateTp());
                    target.Character.NoAttack = false;
                    target.Character.NoMove = false;
                    target?.SendPacket(target.Character.GenerateCond());
                    target.Character.isFreezed = false;
                }
            }
        }
    }
} 