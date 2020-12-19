using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeClassHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeClassHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeClass(ChangeClassPacket changeClassPacket)
        {
            if (changeClassPacket != null)
            {
                Session.AddLogsCmd(changeClassPacket);
                Session.Character.ChangeClass(changeClassPacket.ClassType, true);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeClassPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}