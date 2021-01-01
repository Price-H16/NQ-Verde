using OpenNos.Core;

namespace NosTale.Packets.Packets.ServerPackets
{
    [PacketHeader("tit_eq")]
    public class TitEqPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(1)] public short ItemVnum { get; set; }

        [PacketIndex(0)] public byte Type { get; set; }

        #endregion
    }
}