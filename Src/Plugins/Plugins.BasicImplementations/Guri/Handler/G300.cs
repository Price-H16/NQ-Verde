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
    public class G300 : IGuriHandler
    {
        public long GuriEffectId => 300;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 300)
            {
                if (e.Argument == 8023 && short.TryParse(e.User.ToString(), out var slot))
                {
                    var box = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                    if (box != null)
                    {
                        box.Item.Use(Session, ref box, 1, new[] { e.Data.ToString() });
                    }
                }
            }
        }
    }
}