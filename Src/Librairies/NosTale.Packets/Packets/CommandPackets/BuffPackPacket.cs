using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$BuffPack", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class BuffPackPacket : PacketDefinition
    {
        // no properties to define
    }
}