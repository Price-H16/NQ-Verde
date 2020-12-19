﻿using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G199 : IGuriHandler
    {
        public long GuriEffectId => 199;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 199 && e.Argument == 2)
            {
                if (Session.Character.IsSeal)
                {
                    return;
                }

                short[] listWingOfFriendship = {2160, 2312, 10048};
                short vnumToUse = -1;
                foreach (var vnum in listWingOfFriendship)
                {
                    if (Session.Character.Inventory.CountItem(vnum) > 0)
                    {
                        vnumToUse = vnum;
                        break;
                    }
                }

                var isCouple = Session.Character.IsCoupleOfCharacter(e.User);
                if (vnumToUse != -1 || isCouple)
                {
                    var target = ServerManager.Instance.GetSessionByCharacterId(e.User);
                    if (target != null && !target.Character.IsChangingMapInstance)
                    {
                        if (Session.Character.IsFriendOfCharacter(e.User))
                        {
                            if (target.CurrentMapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance)
                            {
                                var isTargetInA7 = target.Character.MapId.CheckIfInRange(2628, 2644);
                                if (!isTargetInA7 || (isTargetInA7 && !Session.Character.MapId.CheckIfInRange(2628, 2644)))
                                {
                                    if (Session.Character.MapInstance.MapInstanceType
                                        != MapInstanceType.BaseMapInstance
                                        || ServerManager.Instance.ChannelId == 51
                                        && Session.Character.Faction != target.Character.Faction)
                                    {
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                                        return;
                                    }

                                    var mapy = target.Character.PositionY;
                                    var mapx = target.Character.PositionX;
                                    var mapId = target.Character.MapInstance.Map.MapId;

                                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapId, mapx, mapy);
                                    if (!isCouple)
                                    {
                                        Session.Character.Inventory.RemoveItemAmount(vnumToUse);
                                    }
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg("The target character is in Act 7, you must be there too.", 0));
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                            }
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_WINGS"), 10));
                }
            }
            else if (e.Type == 199 && e.Argument == 1)
            {
                if (Session.Character.IsSeal)
                {
                    return;
                }

                short[] listWingOfFriendship = {2160, 2312, 10048};
                short vnumToUse = -1;
                foreach (var vnum in listWingOfFriendship)
                {
                    if (Session.Character.Inventory.CountItem(vnum) > 0)
                    {
                        vnumToUse = vnum;
                    }
                }

                var isCouple = Session.Character.IsCoupleOfCharacter(e.User);
                if (vnumToUse != -1 || isCouple)
                {
                    var session = ServerManager.Instance.GetSessionByCharacterId(e.User);
                    if (session != null)
                    {
                        if (Session.Character.IsFriendOfCharacter(e.User))
                        {
                            if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance)
                            {
                                if (Session.Character.MapInstance.MapInstanceType
                                    != MapInstanceType.BaseMapInstance
                                    || ServerManager.Instance.ChannelId == 51
                                    && Session.Character.Faction != session.Character.Faction)
                                {
                                    Session.SendPacket(
                                        Session.Character.GenerateSay(
                                            Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                                    return;
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("USER_ON_INSTANCEMAP"), 0));
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                        return;
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_WINGS"), 10));
                    return;
                }

                if (!Session.Character.IsFriendOfCharacter(e.User))
                {
                    Session.SendPacket(Language.Instance.GetMessageFromKey("CHARACTER_NOT_IN_FRIENDLIST"));
                    return;
                }

                Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 7, $"#guri^199^2^{e.User}"));
            }
        }
    }
}