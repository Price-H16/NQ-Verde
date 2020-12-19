using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class BlPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BlPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlBlacklistAdd(BlPacket blPacket)
        {
            if (blPacket.CharacterName != null &&
                ServerManager.Instance.GetSessionByCharacterName(blPacket.CharacterName) is ClientSession
                    receiverSession)
                new BlInsPacketHandler(Session).BlacklistAdd(new BlInsPacket
                    {CharacterId = receiverSession.Character.CharacterId});
        }

        #endregion
    }
}