using OpenNos.Core;

namespace NosTale.Packets.Packets.FamilyCommandPackets
{
    [PacketHeader("%Familyshout", "%Cridefamille")]
    public class FamilyShoutPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)] public string Data { get; set; }

        #endregion
    }
}