using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ClearInventoryHandler : IPacketHandler
    {
        #region Instantiation

        public ClearInventoryHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ClearInventory(ClearInventoryPacket clearInventoryPacket)
        {
            if (clearInventoryPacket != null && clearInventoryPacket.InventoryType != InventoryType.Wear)
            {
                Session.AddLogsCmd(clearInventoryPacket);
                foreach (var inv in Session.Character.Inventory.Where(s => s.Type == clearInventoryPacket.InventoryType)
                )
                {
                    Session.Character.Inventory.DeleteById(inv.Id);
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(inv.Type, inv.Slot));
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ClearInventoryPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}