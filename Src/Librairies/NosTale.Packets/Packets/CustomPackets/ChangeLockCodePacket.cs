using OpenNos.Core;

namespace NosTale.Packets.Packets.CustomPackets
{
    [PacketHeader("ChangeLockCode")]
    public class ChangeLockCodePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)]
        public string Lock { get; set; }

        #endregion
    }
}
