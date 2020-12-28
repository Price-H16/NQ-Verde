using System;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FamilyChatPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyChatPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyChat(FamilyChatPacket familyChatPacket)
        {
            if (string.IsNullOrEmpty(familyChatPacket.Message)) return;

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                var msg = familyChatPacket.Message;
                var ccmsg = $"[{Session.Character.Name}]:{msg}";
                if (Session.Account.Authority >= AuthorityType.DSGM)
                    ccmsg = $"[{Session.Account.Authority.ToString()} {Session.Character.Name}]:{msg}";

                //ServerManager.Instance.ChatLogs.Add(new ChatLogDTO
                //{
                //    AccountId = (int) Session.Account.AccountId,
                //    CharacterId = (int) Session.Character.CharacterId,
                //    CharacterName = Session.Character.Name,
                //    DateTime = DateTime.Now,
                //    Message = msg.TrimEnd(),
                //    Type = DialogType.Family
                //});

                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = Session.Character.Family.FamilyId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = ccmsg,
                    Type = MessageType.FamilyChat
                });
                foreach (var session in ServerManager.Instance.Sessions.ToList())
                    if (session.HasSelectedCharacter && session.Character.Family != null
                                                     && Session.Character.Family != null
                                                     && session.Character.Family?.FamilyId ==
                                                     Session.Character.Family?.FamilyId)
                    {
                        if (Session.HasCurrentMapInstance && session.HasCurrentMapInstance
                                                          && Session.CurrentMapInstance == session.CurrentMapInstance)
                        {
                            if (Session.Account.Authority != AuthorityType.Supporter && !Session.Character.InvisibleGm)
                                session.SendPacket(Session.Character.GenerateSay(msg, 6));
                            else
                                session.SendPacket(Session.Character.GenerateSay(ccmsg, 6, true));
                        }
                        else
                        {
                            session.SendPacket(Session.Character.GenerateSay(ccmsg, 6));
                        }

                        if (!Session.Character.InvisibleGm) session.SendPacket(Session.Character.GenerateSpk(msg, 1));
                    }
            }
        }

        #endregion
    }
}