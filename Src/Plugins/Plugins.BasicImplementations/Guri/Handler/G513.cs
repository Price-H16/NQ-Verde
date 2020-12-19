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
    public class G513 : IGuriHandler
    {
        public long GuriEffectId => 513;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 513)
            {
                if (Session?.Character?.MapInstance == null)
                {
                    return;
                }

                if (Session.Character.IsLaurenaMorph())
                {
                    Session.Character.MapInstance.Broadcast(Session.Character.GenerateEff(4054));
                    Session.Character.ClearLaurena();
                }
            }
        }
    }
} 