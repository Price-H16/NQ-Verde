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
    public class G711 : IGuriHandler
    {
        public long GuriEffectId => 711;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 711)
            {
                var tp = Session.CurrentMapInstance.Npcs.FirstOrDefault(n => n.Teleporters.Any(t => t?.TeleporterId == e.Argument))?.Teleporters.FirstOrDefault(t => t?.Type == TeleporterType.TeleportOnOtherMap);
                if (tp == null)
                {
                    return;
                }
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
            }
        }
    }
} 