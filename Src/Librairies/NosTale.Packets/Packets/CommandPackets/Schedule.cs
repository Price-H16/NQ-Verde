using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Schedule", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.User})]
    public class SchedulePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public static string Contents { get; set; }

        #endregion
    }
}