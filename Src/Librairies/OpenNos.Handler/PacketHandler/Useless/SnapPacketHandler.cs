using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Useless
{
    public class SnapPacketHandler : IPacketHandler
    {
        #region Instantiation

        public SnapPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Snap(SnapPacket snapPacket)
        {
            // Not needed for now. (pictures)
        }

        #endregion
    }
}