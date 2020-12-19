using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ZoomHandler : IPacketHandler
    {
        #region Instantiation

        public ZoomHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Zoom(ZoomPacket zoomPacket)
        {
            if (zoomPacket != null)
            {
                Session.AddLogsCmd(zoomPacket);
                Session.SendPacket(
                    UserInterfaceHelper.GenerateGuri(15, zoomPacket.Value, Session.Character.CharacterId));
            }

            Session.Character.GenerateSay(ZoomPacket.ReturnHelp(), 10);
        }

        #endregion
    }
}