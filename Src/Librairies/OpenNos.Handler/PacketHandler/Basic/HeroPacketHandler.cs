using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class HeroPacketHandler : IPacketHandler
    {
        #region Instantiation

        public HeroPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Hero(HeroPacket heroPacket)
        {
            if (!string.IsNullOrEmpty(heroPacket.Message))
            {
                if (Session.Character.IsReputationHero() >= 3 && Session.Character.Reputation > 5000000)
                {
                    heroPacket.Message = heroPacket.Message.Trim();
                    ServerManager.Instance.Broadcast(Session, $"msg 5 [{Session.Character.Name}]:{heroPacket.Message}",
                        ReceiverType.AllNoHeroBlocked);
                    //ServerManager.Instance.ChatLogs.Add(new ChatLogDTO
                    //{
                    //    AccountId = (int) Session.Account.AccountId,
                    //    CharacterId = (int) Session.Character.CharacterId,
                    //    CharacterName = Session.Character.Name,
                    //    DateTime = DateTime.Now,
                    //    Message = heroPacket.Message,
                    //    Type = DialogType.Hero
                    //});
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_HERO"), 11));
                }
            }
        }

        #endregion
    }
}