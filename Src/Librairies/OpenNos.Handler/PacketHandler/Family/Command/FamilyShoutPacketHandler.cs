using NosTale.Packets.Packets.FamilyCommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Family.Command
{
    public class FamilyShoutPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyShoutPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyCall(FamilyShoutPacket packet)
        {
            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
                if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familydeputy
                    || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familykeeper
                    && Session.Character.Family.ManagerCanShout
                    || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                {
                    var msg = packet.Data?.Split(' ');
                    var output = string.Empty;
                    if (msg == null) return;

                    for (var i = 0; i < msg.Length; i++) output += msg[i] + " ";

                    CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = Session.Character.Family.FamilyId,
                        SourceCharacterId = Session.Character.CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = UserInterfaceHelper.GenerateMsg(
                            $"<{Language.Instance.GetMessageFromKey("FAMILYCALL")}> {output}", 0),
                        Type = MessageType.Family
                    });
                }
        }

        #endregion
    }
}