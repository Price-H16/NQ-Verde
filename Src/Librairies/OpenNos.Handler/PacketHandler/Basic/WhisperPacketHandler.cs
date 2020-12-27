using System;
using System.Linq;
using ChickenAPI.Enums;
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
    public class WhisperPacketHandler : IPacketHandler
    {
        #region Instantiation

        public WhisperPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Whisper(WhisperPacket whisperPacket)
        {
            try
            {
                // TODO: Implement WhisperSupport
                if (string.IsNullOrEmpty(whisperPacket.Message)) return;

                var characterName =
                    whisperPacket.Message.Split(' ')[
                            whisperPacket.Message.StartsWith("GM ", StringComparison.CurrentCulture) ? 1 : 0]
                        .Replace("[Angel]", "").Replace("[Demon]", "");

                Enum.GetNames(typeof(AuthorityType)).ToList()
                    .ForEach(at => characterName = characterName.Replace($"[{at}]", ""));

                var message = "";
                var packetsplit = whisperPacket.Message.Split(' ');
                for (var i = packetsplit[0] == "GM" ? 2 : 1; i < packetsplit.Length; i++)
                    message += packetsplit[i] + " ";

                if (message.Length > 60) message = message.Substring(0, 60);

                message = message.Trim();
                Session.SendPacket(Session.Character.GenerateSpk(message, 5));
                var receiver = DAOFactory.CharacterDAO.LoadByName(characterName);
                int? sentChannelId = null;
                if (receiver != null)
                {
                    if (receiver.CharacterId == Session.Character.CharacterId) return;

                    if (Session.Character.IsBlockedByCharacter(receiver.CharacterId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                        return;
                    }

                    var receiverSession =
                        ServerManager.Instance.GetSessionByCharacterId(receiver.CharacterId);
                    if (receiverSession?.CurrentMapInstance?.Map.MapId == Session.CurrentMapInstance?.Map.MapId
                        && Session.Account.Authority >= AuthorityType.DSGM)
                        receiverSession?.SendPacket(Session.Character.GenerateSay(message, 2));

                    //ServerManager.Instance.ChatLogs.Add(new ChatLogDTO
                    //{
                    //    AccountId = (int) Session.Account.AccountId,
                    //    CharacterId = (int) Session.Character.CharacterId,
                    //    CharacterName = Session.Character.Name,
                    //    TargetCharacterId = (int?) receiver?.CharacterId,
                    //    TargetCharacterName = receiver?.Name,
                    //    DateTime = DateTime.Now,
                    //    Message = message,
                    //    Type = DialogType.Whisper
                    //});

                    sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = receiver.CharacterId,
                        SourceCharacterId = Session.Character.CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = Session.Character.Authority >= AuthorityType.Supporter
                            ? Session.Character.GenerateSay(
                                $"(whisper)(From {Session.Character.Authority} {Session.Character.Name}):{message}", 11)
                            : Session.Character.GenerateSpk(message,
                                Session.Account.Authority >= AuthorityType.DSGM ? 15 : 5),
                        Type = Enum.GetNames(typeof(AuthorityType)).Any(a =>
                               {
                                   if (a.Equals(packetsplit[0]))
                                   {
                                       Enum.TryParse(a, out AuthorityType auth);
                                       if (auth >= AuthorityType.DSGM) return true;
                                   }

                                   return false;
                               })
                               || Session.Account.Authority >= AuthorityType.DSGM
                            ? MessageType.WhisperGM
                            : MessageType.Whisper
                    });
                }

                if (sentChannelId == null)
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED")));
            }
            catch (Exception e)
            {
                Logger.Error("Whisper failed.", e);
            }
        }

        #endregion
    }
}