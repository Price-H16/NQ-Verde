using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Extension.Inventory;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UpgradeHandler : IPacketHandler
    {
        #region Instantiation

        public UpgradeHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Upgrade(UpgradeCommandPacket upgradePacket)
        {
            if (upgradePacket != null)
            {
                Session.AddLogsCmd(upgradePacket);
                if (upgradePacket.Slot >= 0)
                {
                    var wearableInstance =
                        Session.Character.Inventory.LoadBySlotAndType(upgradePacket.Slot, 0);
                    wearableInstance?.UpgradeItem(Session, upgradePacket.Mode, upgradePacket.Protection, true);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UpgradeCommandPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}