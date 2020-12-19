using OpenNos.GameObject._Event;

namespace OpenNos.GameObject._ItemUsage.Event
{
    public class InventoryUseItemEvent : PlayerEvent
    {
        public string[] PacketSplit { get; set; }
        
        public ItemInstance Item { get; set; }

        public byte Option { get; set; }
    }
}