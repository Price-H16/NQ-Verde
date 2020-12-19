using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ReloadSiHandler : IPacketHandler
    {
        #region Instantiation

        public ReloadSiHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ReloadSI(ReloadSIPacket reloadSIPacket)
        {
            if (reloadSIPacket != null)
            {
                Session.AddLogsCmd(reloadSIPacket);
                ServerManager.Instance.LoadScriptedInstances();
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ReloadSIPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}