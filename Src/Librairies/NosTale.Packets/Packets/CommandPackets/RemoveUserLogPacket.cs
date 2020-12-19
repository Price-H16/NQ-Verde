using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$RemoveUserLog", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class RemoveUserLogPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public string Username { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$RemoveUserLog <Username>";

        #endregion
    }
}