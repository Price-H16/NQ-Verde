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
    public class G503 : IGuriHandler
    {
        public long GuriEffectId => 503;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 503)
            {
                if (ServerManager.Instance.EventInWaiting == true && Session.Character.IsWaitingForEvent == false)
                {
                    Session.SendPacket("bsinfo 0 7 30 0");
                    Session.SendPacket("esf 2");
                    Session.Character.IsWaitingForEvent = true;
                }
            }
        }
    }
} 