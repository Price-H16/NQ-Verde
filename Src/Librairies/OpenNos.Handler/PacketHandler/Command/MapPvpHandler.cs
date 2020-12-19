using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MapPvpHandler : IPacketHandler
    {
        #region Instantiation

        public MapPvpHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void MapPvp(MapPVPPacket mapPvpPacket)
        {
            Session.AddLogsCmd(mapPvpPacket);
            Session.CurrentMapInstance.IsPVP = !Session.CurrentMapInstance.IsPVP;
            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}