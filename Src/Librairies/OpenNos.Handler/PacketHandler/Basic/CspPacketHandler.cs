using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class CspPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CspPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SendBubbleMessage(CspPacket cspPacket)
        {
            if (cspPacket.CharacterId == Session.Character.CharacterId && Session.Character.BubbleMessage != null)
                Session.Character.MapInstance.Broadcast(Session.Character.GenerateBubbleMessagePacket());
        }

        #endregion
    }
}