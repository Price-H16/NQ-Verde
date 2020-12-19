using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class PinvPacketHandler : IPacketHandler
    {
        #region Instantiation

        public PinvPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PinvGroupJoin(PinvPacket pinvPacket)
        {
            if (pinvPacket.CharacterName != null &&
                ServerManager.Instance.GetSessionByCharacterName(pinvPacket.CharacterName) is ClientSession
                    receiverSession)
                new PJoinPacketHandler(Session).GroupJoin(new PJoinPacket
                    {RequestType = GroupRequestType.Requested, CharacterId = receiverSession.Character.CharacterId});
        }

        #endregion
    }
}