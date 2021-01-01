using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$SetLock", PassNonParseablePacket = true, Authorities = new AuthorityType[] { AuthorityType.User })]
    public class SetLockPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Psw2 { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp()
        {
            return "$SetLock CODE";
        }

        #endregion
    }
}