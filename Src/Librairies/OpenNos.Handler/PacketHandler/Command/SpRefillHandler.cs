using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class SpRefillHandler : IPacketHandler
    {
        #region Instantiation

        public SpRefillHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SpRefill(SPRefillPacket spRefillPacket)
        {
            Session.AddLogsCmd(spRefillPacket);
            Session.Character.SpPoint = 10000;
            Session.Character.SpAdditionPoint = 1000000;
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_REFILL"), 0));
            Session.SendPacket(Session.Character.GenerateSpPoint());
        }

        #endregion
    }
}