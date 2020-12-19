using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("u_ps")]
    public class UsePartnerSkillPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long TransportId { get; set; }

        [PacketIndex(1)]
        public UserType TargetType { get; set; }

        [PacketIndex(2)]
        public long TargetId { get; set; }

        [PacketIndex(3)]
        public byte CastId { get; set; }

        #endregion
    }
}