using NosTale.Packets.Packets.ServerPackets;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.ScriptedInstance
{
    public class TreqPacketHandler : IPacketHandler
    {
        #region Instantiation

        public TreqPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GetTreq(TreqPacket treqPacket)
        {
            var timespace = Session.CurrentMapInstance.ScriptedInstances
                .Find(s => treqPacket.X == s.PositionX && treqPacket.Y == s.PositionY).Copy();

            if (timespace != null)
            {
                if (treqPacket.StartPress == 1 || treqPacket.RecordPress == 1)
                    Session.Character.EnterInstance(timespace);
                else
                    Session.SendPacket(timespace.GenerateRbr());
            }
        }

        #endregion
    }
}