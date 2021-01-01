using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$ChangeLock", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class ChangeLockPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(1)]
        public string newlock { get; set; }

        [PacketIndex(0)]
        public string oldlock { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp()
        {
            return "$ChangeLock ACTUALCODE NEWCODE";
        }

        #endregion
    }
}