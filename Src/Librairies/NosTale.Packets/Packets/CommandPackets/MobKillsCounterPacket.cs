using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Counter", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.User })]
    public class MobKillsCounterPacket : PacketDefinition
    {
    }
}