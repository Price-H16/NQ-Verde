using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Unstuck", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.User})]
    public class UnstuckPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Unstuck";

        #endregion
    }
}