using OpenNos.Core;

namespace NosTale.Packets.Packets.FamilyCommandPackets
{
    [PacketHeader("%Título", "%Title", "%Titre")]
    public class FamilyTitleChangePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)] public string Data { get; set; }

        #endregion
    }
}