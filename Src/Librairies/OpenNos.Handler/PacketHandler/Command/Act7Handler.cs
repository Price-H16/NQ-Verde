using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class Act7Handler : IPacketHandler
    {
        #region Instantiation

        public Act7Handler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        public void Act7 (Act7Packet act7Packet)
        {
            if (act7Packet != null)
            {

                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2631, 7, 46);

            }

            Session.Character.GenerateSay(Act7Packet.ReturnHelp(), 10);
        }

    }
}