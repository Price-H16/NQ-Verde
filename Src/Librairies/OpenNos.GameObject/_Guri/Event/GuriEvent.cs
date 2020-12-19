using OpenNos.GameObject._Event;

namespace OpenNos.GameObject._Guri.Event
{
    public class GuriEvent : PlayerEvent
    {
        public long Type { get; set; }

        public int Argument { get; set; }

        public int Data { get; set; }

        public long User { get; set; }

        public string Value { get; set; }
    }
}