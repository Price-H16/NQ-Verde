using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$SearchBuff", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.Administrator })]
    public class SearchBuff : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Contents { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp()
        {
            return "$SearchBuff NAME(*)";
        }

        #endregion
    }
}