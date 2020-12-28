using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Act7", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.DSGM, AuthorityType.Administrator })]

    public class Act7Packet : PacketDefinition
    {
        #region Properties

        public static string ReturnHelp() => "$Act7";

        #endregion
    }
}