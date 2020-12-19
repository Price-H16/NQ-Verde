using OpenNos.GameObject._Event;

namespace OpenNos.GameObject._NpcDialog.Event
{
    public class NpcDialogEvent : PlayerEvent
    {
        public MapNpc Npc { get; set; }

        public short Runner { get; set; }

        public short Type { get; set; }

        public short Value { get; set; }

        public int NpcId { get; set; }
    }
}