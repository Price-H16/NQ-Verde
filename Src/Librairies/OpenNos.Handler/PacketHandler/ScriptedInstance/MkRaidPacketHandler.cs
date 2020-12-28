using System;
using System.Collections.Generic;
using System.Linq;
using ChickenAPI.Enums;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.ScriptedInstance
{
    public class MkRaidPacketHandler : IPacketHandler
    {
        #region Instantiation

        public MkRaidPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GenerateRaid(MkRaidPacket mkRaidPacket)
        {
            if (Session.Character.Group?.Raid != null && Session.Character.Group.IsLeader(Session))
            {
                IEnumerable<ClientSession> duplicateIp = ServerManager.Instance.FindSameIpAddresses(Session.Character.Group.Sessions.GetAllItems());

                if (duplicateIp.Any())
                {
                    foreach (var session in duplicateIp)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("SAME_IP"), session.Character.Name), 10));
                    }
                    return;
                }

                if (Session.Character.MapId == Session.Character.Group.Raid.MapId
                    && Map.GetDistance(
                        new MapCell {X = Session.Character.PositionX, Y = Session.Character.PositionY},
                        new MapCell
                        {
                            X = Session.Character.Group.Raid.PositionX, Y = Session.Character.Group.Raid.PositionY
                        }) < 2)
                {
                    if (Session.Character.Group.SessionCount > 0 || Session.Account.Authority > AuthorityType.TGM &&
                        Session.Character.Group.Sessions.All(s => s.CurrentMapInstance == Session.CurrentMapInstance))
                    {
                        //// Act6 raid can only be started on Act6 Map ( like official )
                        //if (Session.Character.Group.Raid.Id == 24 && Session.Character.MapId != 150 || Session.Character.Group.Raid.Id == 23 && Session.Character.MapId != 247)
                        //{
                        //    Session.SendPacket(UserInterfaceHelper.GenerateMsg("WRONG_PORTAL", 0));
                        //    return;
                        //}

                        if (Session.Character.Group.Raid.FirstMap == null)
                            Session.Character.Group.Raid.LoadScript(MapInstanceType.RaidInstance, Session.Character);

                        if (Session.Character.Group.Raid.FirstMap == null) return;

                        Session.Character.Group.Raid.InstanceBag.Lock = true;

                        //Session.Character.Group.Characters.Where(s => s.CurrentMapInstance != Session.CurrentMapInstance).ToList().ForEach(
                        //session =>
                        //{
                        //    Session.Character.Group.LeaveGroup(session);
                        //    session.SendPacket(session.Character.GenerateRaid(1, true));
                        //    session.SendPacket(session.Character.GenerateRaid(2, true));
                        //});

                        Session.Character.Group.Raid.InstanceBag.Lives = (short) Session.Character.Group.SessionCount;

                        foreach (var session in Session.Character.Group.Sessions.GetAllItems())
                            if (session != null)
                            {
                                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                    session.Character.Group.Raid.FirstMap.MapInstanceId,
                                    session.Character.Group.Raid.StartX, session.Character.Group.Raid.StartY);
                                session.SendPacket("raidbf 0 0 25");
                                session.SendPacket(session.Character.Group.GeneraterRaidmbf(session));
                                session.SendPacket(session.Character.GenerateRaid(5));
                                session.SendPacket(session.Character.GenerateRaid(4));
                                session.SendPacket(session.Character.GenerateRaid(3));
                                if (session.Character.Group.Raid.DailyEntries > 0)
                                {
                                    var entries = session.Character.Group.Raid.DailyEntries -
                                                  session.Character.GeneralLogs.CountLinq(s =>
                                                      s.LogType == "InstanceEntry" &&
                                                      short.Parse(s.LogData) == session.Character.Group.Raid.Id &&
                                                      s.Timestamp.Date == DateTime.Today);
                                    session.SendPacket(session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("INSTANCE_ENTRIES"), entries),
                                        10));
                                }
                            }

                        ServerManager.Instance.GroupList.Remove(Session.Character.Group);

                        Logger.LogUserEvent("RAID_START", Session.GenerateIdentity(),
                            $"RaidId: {Session.Character.Group.GroupId}");
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_TEAM_NOT_READY"),
                                0));
                    }
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_PORTAL"), 0));
                }
            }
        }

        #endregion
    }
}