using OpenNos.Core;

namespace NosTale.Packets.Packets.ServerPackets
{
    [PacketHeader("rl")]
    public class RaidListPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public short MonsterVNum { get; set; }

        #endregion
    }
}