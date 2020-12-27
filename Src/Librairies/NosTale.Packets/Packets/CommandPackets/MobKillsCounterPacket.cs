using OpenNos.Core;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChickenAPI.Enums;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Counter", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.User })]
    public class MobKillsCounterPacket : PacketDefinition
    {
    }
}
