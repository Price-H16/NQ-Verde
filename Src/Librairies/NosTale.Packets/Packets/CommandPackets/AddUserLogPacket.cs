using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$AddUserLog", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class AddUserLogPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public string Username { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$AddUserLog <Username>";

        #endregion
    }
}