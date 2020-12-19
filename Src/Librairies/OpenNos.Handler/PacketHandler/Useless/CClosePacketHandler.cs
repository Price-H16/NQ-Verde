using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Useless
{
    public class CClosePacketHandler : IPacketHandler
    {
        #region Instantiation

        public CClosePacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void CClose(CClosePacket cClosePacket)
        {
            // idk
        }

        #endregion
    }
}