using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$UserLog", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class UserLogPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$UserLog";

        #endregion
    }
}