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
    public class G750 : IGuriHandler
    {
        public long GuriEffectId => 750;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 750)
            {
                const short baseVnum = 1623;
                if (ServerManager.Instance.ChannelId == 51)
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED_ACT4"),
                            0));
                    return;
                }
                if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4ShipAngel
                    || Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4ShipDemon)
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED_ACT4SHIP"),
                            0));
                    return;
                }
                if (Enum.TryParse(e.Argument.ToString(), out FactionType faction)
                    && Session.Character.Inventory.CountItem(baseVnum + (byte)faction) > 0)
                {
                    if ((byte)faction < 3) // Single faction change
                    {
                        if (Session.Character.LastFactionChange > DateTime.Now.AddDays(1).Ticks)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED"), 0));
                            return;
                        }
                        if (Session.Character.Faction == (FactionType)faction)
                        {
                            return;
                        }
                        if (Session.Character.Family != null)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"),0));
                            return;
                        }
                        Session.Character.Inventory.RemoveItemAmount(baseVnum + (byte)faction);
                        Session.Character.ChangeFaction((FactionType)faction);
                    }
                    else // Family faction change
                    {
                        faction -= 2;
                        if ((FactionType)Session.Character.Family.FamilyFaction == faction)
                        {
                            return;
                        }
                        if (Session.Character.Family == null)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY"),
                                    0));
                            return;
                        }
                        if (Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"),
                                    0));
                            return;
                        }
                        if (Session.Character.Family.LastFactionChange > DateTime.Now.AddDays(-1).Ticks)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED"), 0));
                            return;
                        }

                        Session.Character.Inventory.RemoveItemAmount(baseVnum + (byte)faction + 2);
                        Session.Character.Family.ChangeFaction((byte)faction, Session);
                    }
                }
            }
        }
    }
} 