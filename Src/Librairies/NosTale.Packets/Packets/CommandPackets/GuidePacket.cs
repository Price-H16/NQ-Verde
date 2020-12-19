using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Guide", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.User})]
    public class GuidePacket : PacketDefinition
    {
    }
}