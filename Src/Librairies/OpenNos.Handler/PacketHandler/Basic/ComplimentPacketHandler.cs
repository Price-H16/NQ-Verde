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
            if (complimentPacket == null)
            {
                return;
            }

            long complimentedCharacterId = complimentPacket.CharacterId;
            if (Session.Character.Level >= 30)
            {
                GeneralLogDTO dto = Session.Character.GeneralLogs.LastOrDefault(s => s.LogData == "World" && s.LogType == "Connection");
                GeneralLogDTO lastcompliment = Session.Character.GeneralLogs.LastOrDefault(s => s.LogData == "World" && s.LogType == "Compliment");
                if (dto != null && dto.Timestamp.AddMinutes(60) <= DateTime.Now)
                {
                    if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now.Date)
                    {
                        short? compliment = ServerManager.Instance.GetProperty<short?>(complimentedCharacterId, nameof(Character.Compliment));
                        compliment++;
                        ServerManager.Instance.SetProperty(complimentedCharacterId, nameof(Character.Compliment), compliment);
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"), ServerManager.Instance.GetProperty<string>(complimentedCharacterId, nameof(Character.Name))), 12));
                        Session.Character.GeneralLogs.Add(new GeneralLogDTO
                        {
                            AccountId = Session.Account.AccountId,
                            CharacterId = Session.Character.CharacterId,
                            IpAddress = Session.IpAddress,
                            LogData = "World",
                            LogType = "Compliment",
                            Timestamp = DateTime.Now
                        });

                        Session.CurrentMapInstance?.Broadcast(Session,
                            Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"), Session.Character.Name), 12), ReceiverType.OnlySomeone,
                            characterId: complimentedCharacterId);
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                    }
                }
                else
                {
                    if (dto != null)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"), (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"), 11));
            }
        }

        #endregion
    }
}