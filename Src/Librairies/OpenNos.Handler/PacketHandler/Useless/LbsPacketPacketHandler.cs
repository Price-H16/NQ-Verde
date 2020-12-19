using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Useless
{
    public class LbsPacketPacketHandler : IPacketHandler
    {
        #region Instantiation

        public LbsPacketPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Lbs(LbsPacket lbsPacket)
        {
            // idk
        }

        #endregion
    }
}