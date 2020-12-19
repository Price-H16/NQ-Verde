using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class RestartAllHandler : IPacketHandler
    {
        #region Instantiation

        public RestartAllHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void RestartAll(RestartAllPacket restartAllPacket)
        {
            if (restartAllPacket != null)
            {
                Session.AddLogsCmd(restartAllPacket);
                var worldGroup = !string.IsNullOrEmpty(restartAllPacket.WorldGroup)
                    ? restartAllPacket.WorldGroup
                    : ServerManager.Instance.ServerGroup;

                var time = restartAllPacket.Time;

                if (time < 1) time = 5;

                CommunicationServiceClient.Instance.Restart(worldGroup, time);

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(RestartAllPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}