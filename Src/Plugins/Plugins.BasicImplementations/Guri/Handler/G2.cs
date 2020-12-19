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
    public class G2 : IGuriHandler
    {
        public long GuriEffectId => 2;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 2)
            {
                Session.CurrentMapInstance?.Broadcast(
                    UserInterfaceHelper.GenerateGuri(2, 1, Session.Character.CharacterId),
                    Session.Character.PositionX, Session.Character.PositionY);
            }
        }
    }
} 