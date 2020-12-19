using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.GameObject.CommandPackets
{

    [PacketHeader("$SpawnDb", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.Administrator })]

    public class SpawnpermaPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Time { get; set; }

        [PacketIndex(1)]
        public int Count { get; set; }

        [PacketIndex(2)]
        public short VNum { get; set; }

        [PacketIndex(3)]
        public int MapMonsterId { get; set; }
         
        #endregion

        #region Methods

        public static string ReturnHelp()
        {
            return "$SpawnDb TIME COUNT MONSTERVNUM MAPMONSTERID";
        }

        #endregion
    }
}