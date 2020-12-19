using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class GroupSayPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GroupSayPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GroupTalk(GroupSayPacket groupSayPacket)
        {
            if (!string.IsNullOrEmpty(groupSayPacket.Message))
            {
                ServerManager.Instance.Broadcast(Session, Session.Character.GenerateSpk(groupSayPacket.Message, 3),
                    ReceiverType.Group);

                //ServerManager.Instance.ChatLogs.Add(new ChatLogDTO
                //{
                //    AccountId = (int) Session.Account.AccountId,
                //    CharacterId = (int) Session.Character.CharacterId,
                //    CharacterName = Session.Character.Name,
                //    DateTime = DateTime.Now,
                //    Message = groupSayPacket.Message,
                //    Type = DialogType.Group
                //});
            }
        }

        #endregion
    }
}