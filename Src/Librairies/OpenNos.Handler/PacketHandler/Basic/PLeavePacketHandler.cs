using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class PLeavePacketHandler : IPacketHandler
    {
        #region Instantiation

        public PLeavePacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GroupLeave(PLeavePacket pleavePacket)
        {
            ServerManager.Instance.GroupLeave(Session);
        }

        #endregion
    }
}