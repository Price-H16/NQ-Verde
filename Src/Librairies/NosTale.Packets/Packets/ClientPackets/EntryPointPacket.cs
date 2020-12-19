using OpenNos.Core;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("OpenNos.EntryPoint", IsCharScreen = true, Amount = 3)]
    public class OpenNosEntryPointPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string PacketData { get; set; }

        #endregion

        //TODO: Find defined values
    }
}