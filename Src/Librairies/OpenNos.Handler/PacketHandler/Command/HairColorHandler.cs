using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class HairColorHandler : IPacketHandler
    {
        #region Instantiation

        public HairColorHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Haircolor(HairColorPacket hairColorPacket)
        {
            if (hairColorPacket != null)
            {
                Session.AddLogsCmd(hairColorPacket);
                Session.Character.HairColor = hairColorPacket.HairColor;
                Session.SendPacket(Session.Character.GenerateEq());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateIn());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HairColorPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}