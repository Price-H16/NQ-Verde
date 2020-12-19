using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GodModeHandler : IPacketHandler
    {
        #region Instantiation

        public GodModeHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GodMode(GodModePacket godModePacket)
        {
            Session.AddLogsCmd(godModePacket);
            Session.Character.HasGodMode = !Session.Character.HasGodMode;
            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}