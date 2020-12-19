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
    public class G306 : IGuriHandler
    {
        public long GuriEffectId => 306;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 306)
            {
                if (Session.Character.Inventory.CountItem(e.Argument) < 1)
                {
                    return;
                }

                var item = ServerManager.GetItem((short)e.Argument);

                if (item == null)
                {
                    return;
                }

                if (item.ItemType != ItemType.Title)
                {
                    // Stupid packet hacking → Useless
                    return;
                }

                Session.Character.Title.Add(new CharacterTitleDTO
                {
                    CharacterId = Session.Character.CharacterId,
                    Stat = 1,
                    TitleVnum = e.Argument
                });

                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NEW_TITLE"), 0));

                Session.Character.Inventory.RemoveItemAmount(e.Argument);
                Session.SendPacket(Session.Character.GenerateTitle());
            }
        }
    }
} 