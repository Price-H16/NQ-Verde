using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GoldHandler : IPacketHandler
    {
        #region Instantiation

        public GoldHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Gold(GoldPacket goldPacket)
        {
            if (goldPacket != null)
            {
                Session.AddLogsCmd(goldPacket);
                var gold = goldPacket.Amount;
                var maxGold = ServerManager.Instance.Configuration.MaxGold;
                gold = gold > maxGold ? maxGold : gold;
                if (gold >= 0)
                {
                    Session.Character.Gold = gold;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("GOLD_SET"),
                        0));
                    Session.SendPacket(Session.Character.GenerateGold());
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GoldPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}