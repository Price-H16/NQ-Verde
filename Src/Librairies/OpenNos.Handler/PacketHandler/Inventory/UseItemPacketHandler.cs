using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class UseItemPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UseItemPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void UseItem(UseItemPacket useItemPacket)
        {
            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                return;
            }

            if (useItemPacket == null || (byte)useItemPacket.Type >= 9)
            {
                return;
            }

            var packet = useItemPacket.OriginalContent.Split(' ', '^');
            
            if (packet.Length < 2 || packet[1].Length <= 0)
            {
                return;
            }

            var inv = Session.Character.Inventory.LoadBySlotAndType(useItemPacket.Slot, useItemPacket.Type);

            inv?.Item.Use(Session, ref inv, packet[1].ElementAt(0) == '#' ? (byte) 255 : (byte) 0, packet);
            return;
            
            Session.Character.Event.EmitEvent(new InventoryUseItemEvent
            {
                Item = inv,
                Option = packet[1].ElementAt(0) == '#' ? (byte)255 : (byte)0,
                PacketSplit = packet
            });
        }

        #endregion
    }
}