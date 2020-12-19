using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class XpRateHandler : IPacketHandler
    {
        #region Instantiation

        public XpRateHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void XpRate(XpRatePacket xpRatePacket)
        {
            if (xpRatePacket != null)
            {
                Session.AddLogsCmd(xpRatePacket);
                if (xpRatePacket.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateXP = xpRatePacket.Value;

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("XP_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(XpRatePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}