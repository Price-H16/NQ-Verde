using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("fws")]
    public class FwsPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short ItemVNum { get; set; }

        #endregion
    }
}