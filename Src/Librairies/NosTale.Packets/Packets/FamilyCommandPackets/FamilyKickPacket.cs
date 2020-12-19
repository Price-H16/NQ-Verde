using OpenNos.Core;

namespace NosTale.Packets.Packets.FamilyCommandPackets
{
    [PacketHeader("%Familydismiss", "%FamilyKick", "%Rejetdefamille")]
    public class FamilyKickPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public string Name { get; set; }

        #endregion
    }
}