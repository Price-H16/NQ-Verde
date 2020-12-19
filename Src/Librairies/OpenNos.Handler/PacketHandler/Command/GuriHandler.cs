using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GuriHandler : IPacketHandler
    {
        #region Instantiation

        public GuriHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Guri(GuriCommandPacket guriCommandPacket)
        {
            if (guriCommandPacket != null)
            {
                Session.AddLogsCmd(guriCommandPacket);
                Session.SendPacket(UserInterfaceHelper.GenerateGuri(guriCommandPacket.Type, guriCommandPacket.Argument,
                    Session.Character.CharacterId, guriCommandPacket.Value));
            }

            Session.Character.GenerateSay(GuriCommandPacket.ReturnHelp(), 10);
        }

        #endregion
    }
}