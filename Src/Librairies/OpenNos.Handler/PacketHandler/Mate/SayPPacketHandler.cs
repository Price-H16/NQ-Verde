﻿using System;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Mate
{
    public class SayPPacketHandler : IPacketHandler
    {
        #region Instantiation

        public SayPPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PetTalk(SayPPacket sayPPacket)
        {
            if (string.IsNullOrEmpty(sayPPacket.Message)) return;

            var mate = Session.Character.Mates.Find(s => s.MateTransportId == sayPPacket.PetId);
            if (mate != null)
            {
                var penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
                var message = sayPPacket.Message;
                if (Session.Character.IsMuted() && penalty != null)
                {
                    if (Session.Character.Gender == GenderType.Female)
                    {
                        var member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                        if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                        {
                            var member2 = member.FirstOrDefault(o => o.Session == Session);
                            member.Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2).Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o => o.Session.SendPacket( Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1)));
                        }
                        else
                        {
                            Session.CurrentMapInstance?.Broadcast(Session,Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                        }

                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                    }
                    else
                    {
                        var member = ServerManager.Instance.ArenaTeams.ToList().FirstOrDefault(s => s.Any(e => e.Session == Session));
                        if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                        {
                            var member2 = member.FirstOrDefault(o => o.Session == Session);
                            member .Replace(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2) .Replace(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType) .ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"),1)));
                        }
                        else
                        {
                            Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                        }

                        Session.SendPacket(Session.Character.GenerateSay( string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                        Session.SendPacket(Session.Character.GenerateSay( string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                    }
                }
                else
                {
                    //ServerManager.Instance.ChatLogs.Add(new ChatLogDTO
                    //{
                    //    AccountId = (int) Session.Account.AccountId,
                    //    CharacterId = (int) Session.Character.CharacterId,
                    //    CharacterName = Session.Character.Name,
                    //    DateTime = DateTime.Now,
                    //    Message = message,
                    //    Type = DialogType.Normal
                    //});

                    Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Say((byte) UserType.Npc, mate.MateTransportId, 2, message));
                }
            }
        }

        #endregion
    }
}