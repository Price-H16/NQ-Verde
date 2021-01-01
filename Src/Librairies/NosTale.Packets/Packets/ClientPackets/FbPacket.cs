using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("fb")]
    public class FbPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        [PacketIndex(2)]
        public short? Parameter { get; set; }

        [PacketIndex(0)]
        public short Type { get; set; }

        #endregion
    }
}