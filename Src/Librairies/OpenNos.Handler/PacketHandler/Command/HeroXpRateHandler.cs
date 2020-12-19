using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class HeroXpRateHandler : IPacketHandler
    {
        #region Instantiation

        public HeroXpRateHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void HeroXpRate(HeroXpRatePacket heroXpRatePacket)
        {
            if (heroXpRatePacket != null)
            {
                Session.AddLogsCmd(heroXpRatePacket);
                if (heroXpRatePacket.Value <= 1000)
                {
                    ServerManager.Instance.Configuration.RateHeroicXP = heroXpRatePacket.Value;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HEROXP_RATE_CHANGED"), 0));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HeroXpRatePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}