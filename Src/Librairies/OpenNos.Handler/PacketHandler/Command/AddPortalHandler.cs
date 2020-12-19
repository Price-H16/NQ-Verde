using NosTale.Extension.Extension.Command;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddPortalHandler : IPacketHandler
    {
        #region Instantiation

        public AddPortalHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddPortal(AddPortalPacket addPortalPacket)
        {
            if (addPortalPacket != null)
            {
                Session.AddLogsCmd(addPortalPacket);

                Session.AddPortal(addPortalPacket.DestinationMapId, addPortalPacket.DestinationX,
                    addPortalPacket.DestinationY,
                    addPortalPacket.PortalType == null ? (short) -1 : (short) addPortalPacket.PortalType, true);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddPortalPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}