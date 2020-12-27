using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Warp", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.User})]
    public class WarpPacket : PacketDefinition
    {
        [PacketIndex(0)] public int MapId { get; set; }

        public static string ReturnHelp() => "$Warp [MapId]";
    }
}