using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("fws")]
    public class FwsPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public short ItemVNum { get; set; }
    }
}