using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class BtkPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BtkPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FriendTalk(BtkPacket btkPacket)
        {
            if (string.IsNullOrEmpty(btkPacket.Message)) return;

            var message = btkPacket.Message;
            if (message.Length > 60) message = message.Substring(0, 60);

            message = message.Trim();

            var character = DAOFactory.CharacterDAO.LoadById(btkPacket.CharacterId);
            if (character != null)
            {
                var sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = character.CharacterId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = PacketFactory.Serialize(Session.Character.GenerateTalk(message)),
                    Type = MessageType.PrivateChat
                });

                //ServerManager.Instance.ChatLogs.Add(new ChatLogDTO
                //{
                //    AccountId = (int) Session.Account.AccountId,
                //    CharacterId = (int) Session.Character.CharacterId,
                //    CharacterName = Session.Character.Name,
                //    DateTime = DateTime.Now,
                //    Message = message,
                //    Type = DialogType.Friend
                //});

                if (!sentChannelId.HasValue) //character is even offline on different world
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_OFFLINE")));
            }
        }

        #endregion
    }
}