using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class FairyXpRateHandler : IPacketHandler
    {
        #region Instantiation

        public FairyXpRateHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FairyXpRate(FairyXpRatePacket fairyXpRatePacket)
        {
            if (fairyXpRatePacket != null)
            {
                Session.AddLogsCmd(fairyXpRatePacket);
                if (fairyXpRatePacket.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateFairyXP = fairyXpRatePacket.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("FAIRYXP_RATE_CHANGED"),
                            0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(FairyXpRatePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}