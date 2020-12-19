using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class WigColorHandler : IPacketHandler
    {
        #region Instantiation

        public WigColorHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void WigColor(WigColorPacket wigColorPacket)
        {
            if (wigColorPacket != null)
            {
                Session.AddLogsCmd(wigColorPacket);
                var wig =
                    Session.Character.Inventory.LoadBySlotAndType((byte) EquipmentType.Hat, InventoryType.Wear);
                if (wig != null)
                {
                    wig.Design = wigColorPacket.Color;
                    Session.SendPacket(Session.Character.GenerateEq());
                    Session.SendPacket(Session.Character.GenerateEquipment());
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateIn());
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WIG"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(WigColorPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}