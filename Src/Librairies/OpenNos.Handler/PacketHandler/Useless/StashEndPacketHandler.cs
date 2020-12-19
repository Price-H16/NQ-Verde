using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Useless
{
    public class StashEndPacketHandler : IPacketHandler
    {
        #region Instantiation

        public StashEndPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FStashEnd(StashEndPacket stashEndPacket)
        {
            // idk
        }

        #endregion
    }
}