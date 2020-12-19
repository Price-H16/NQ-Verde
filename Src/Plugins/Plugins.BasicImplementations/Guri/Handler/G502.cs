using System;
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
    public class G502 : IGuriHandler
    {
        public long GuriEffectId => 502;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 502)
            {
                long? targetId = e.User;

                if (targetId == null)
                {
                    return;
                }

                var target = ServerManager.Instance.GetSessionByCharacterId(targetId.Value);

                if (target?.Character?.MapInstance == null)
                {
                    return;
                }

                if (target.Character.MapInstance.MapInstanceType == MapInstanceType.IceBreakerInstance || target.Character.MapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
                {
                    if (target.Character.LastPvPKiller == null
                        || target.Character.LastPvPKiller != Session)
                    {
                        IceBreaker.FrozenPlayers.Remove(target);
                        IceBreaker.AlreadyFrozenPlayers.Add(target);
                        target.Character.NoMove = false;
                        target.Character.NoAttack = false;
                        target.SendPacket(target.Character.GenerateCond());
                        target.Character.MapInstance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ICEBREAKER_PLAYER_UNFROZEN"), target.Character.Name), 0));

                        if (!IceBreaker.IceBreakerTeams.FirstOrDefault(s => s.Contains(Session)).Contains(target))
                        {
                            IceBreaker.IceBreakerTeams.Remove(IceBreaker.IceBreakerTeams.FirstOrDefault(s => s.Contains(target)));
                            IceBreaker.IceBreakerTeams.FirstOrDefault(s => s.Contains(Session)).Add(target);
                        }
                    }
                }
                else
                {
                    target.Character.RemoveBuff(569);
                }
            }
        }
    }
} 