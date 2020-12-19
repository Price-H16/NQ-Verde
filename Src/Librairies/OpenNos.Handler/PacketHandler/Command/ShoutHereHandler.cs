using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ShoutHereHandler : IPacketHandler
    {
        #region Instantiation

        public ShoutHereHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ShoutHere(ShoutHerePacket shoutHerePacket)
        {
            if (shoutHerePacket != null)
            {
                Session.AddLogsCmd(shoutHerePacket);
                ServerManager.Shout(shoutHerePacket.Message);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ShoutHerePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}