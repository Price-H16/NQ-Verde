using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class DropRateHandler : IPacketHandler
    {
        #region Instantiation

        public DropRateHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void DropRate(DropRatePacket dropRatePacket)
        {
            if (dropRatePacket != null)
            {
                Session.AddLogsCmd(dropRatePacket);
                if (dropRatePacket.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateDrop = dropRatePacket.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("DROP_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(DropRatePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}