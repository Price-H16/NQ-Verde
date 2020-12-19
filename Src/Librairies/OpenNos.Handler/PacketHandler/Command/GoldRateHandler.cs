using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GoldRateHandler : IPacketHandler
    {
        #region Instantiation

        public GoldRateHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GoldRate(GoldRatePacket goldRatePacket)
        {
            if (goldRatePacket != null)
            {
                Session.AddLogsCmd(goldRatePacket);
                if (goldRatePacket.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateGold = goldRatePacket.Value;

                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOLD_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GoldRatePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}