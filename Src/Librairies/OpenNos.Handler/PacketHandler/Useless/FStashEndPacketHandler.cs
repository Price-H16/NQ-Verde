using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Useless
{
    public class FStashEndPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FStashEndPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FStashEnd(FStashEndPacket fStashEndPacket)
        {
            // idk
        }

        #endregion
    }
}