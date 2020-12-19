using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class HomeHandler : IPacketHandler
    {
        #region Instantiation

        public HomeHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Home(HomePacket homePacket)
        {
            if (homePacket != null)
            {
                Session.AddLogsCmd(homePacket);
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 0, 24, 24);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HomePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}