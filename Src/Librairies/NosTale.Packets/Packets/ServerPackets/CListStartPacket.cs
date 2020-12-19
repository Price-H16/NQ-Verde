using OpenNos.Core;

namespace NosTale.Packets.Packets.ServerPackets
{
    [PacketHeader("clist_start")]
    public class CListStartPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        #endregion
    }
}
