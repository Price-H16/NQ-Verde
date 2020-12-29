using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class ComplimentPacketHandler : IPacketHandler
    {
        #region Instantiation

        public ComplimentPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Compliment(ComplimentPacket complimentPacket)
        {
            if (complimentPacket != null)
            {
                if (Session.Character.CharacterId == complimentPacket.CharacterId) return;

                var sess = ServerManager.Instance.GetSessionByCharacterId(complimentPacket.CharacterId);
                if (sess != null)
                {
                    if (Session.Character.Level >= 30)
                    {
                        var dto =
                            Session.Character.GeneralLogs.LastOrDefault(s =>
                                s.LogData == "World" && s.LogType == "Connection");
                        var lastcompliment =
                            Session.Character.GeneralLogs.LastOrDefault(s =>
                                s.LogData == "World" && s.LogType == nameof(Compliment));
                        if (dto?.Timestamp.AddMinutes(60) <= DateTime.Now)
                        {
                            if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now)
                            {
                                sess.Character.Compliment++;
                                Session.SendPacket(Session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"),
                                        sess.Character.Name), 12));
                                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                                {
                                    AccountId = Session.Account.AccountId,
                                    CharacterId = Session.Character.CharacterId,
                                    IpAddress = Session.IpAddress,
                                    LogData = "World",
                                    LogType = nameof(Compliment),
                                    Timestamp = DateTime.Now
                                });

                                Session.CurrentMapInstance?.Broadcast(Session,
                                    Session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"),
                                            Session.Character.Name), 12), ReceiverType.OnlySomeone,
                                    characterId: complimentPacket.CharacterId);
                            }
                            else
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                            }
                        }
                        else if (dto != null)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"),
                                    (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"),
                                11));
                    }
                }
            }
        }

        #endregion
    }
}