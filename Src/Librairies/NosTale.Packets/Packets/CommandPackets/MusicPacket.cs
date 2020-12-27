using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Music", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.Administrator })]
    public class MusicPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Music { get; set; }

        public static string ReturnHelp() => "$Music BGM";

        #endregion
    }
}