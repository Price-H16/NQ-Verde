using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog.Event;

namespace OpenNos.Handler.PacketHandler.Npc
{
    public class NRunPacketHandler : IPacketHandler
    {
        #region Instantiation

        public NRunPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void NpcRunFunction(NRunPacket packet)
        {
            Session.Character.LastNRunId = packet.NpcId;
            Session.Character.LastItemVNum = 0;

            var npc = Session.CurrentMapInstance.Npcs.Find(s => s.MapNpcId == packet.NpcId);

            if (Session.Character.Hp <= 0 || !Session.HasCurrentMapInstance) return;

            Session.Character.Event.EmitEventAsync(new NpcDialogEvent
            {
                Runner = packet.Runner,
                Type = packet.Type,
                Value = packet.Value,
                NpcId = packet.NpcId,
                Npc = npc
            });
        }

        #endregion
    }
}