using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class PacketCallBackHandler : IPacketHandler
    {
        #region Instantiation

        public PacketCallBackHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PacketCallBack(PacketCallbackPacket packetCallbackPacket)
        {
            if (packetCallbackPacket != null)
            {
                Session.AddLogsCmd(packetCallbackPacket);
                Session.SendPacket(packetCallbackPacket.Packet);
                Session.SendPacket(Session.Character.GenerateSay(packetCallbackPacket.Packet, 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(PacketCallbackPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}