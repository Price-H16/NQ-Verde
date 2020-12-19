using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChannelInfoHandler : IPacketHandler
    {
        #region Instantiation

        public ChannelInfoHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChannelInfo(ChannelInfoPacket channelInfoPacket)
        {
            Session.AddLogsCmd(channelInfoPacket);
            Session.SendPacket(Session.Character.GenerateSay(
                $"-----------Channel Info-----------\n-------------Channel:{ServerManager.Instance.ChannelId}-------------",
                11));
            foreach (var session in ServerManager.Instance.Sessions)
                Session.SendPacket(
                    Session.Character.GenerateSay(
                        $"CharacterName: {session.Character.Name} | CharacterId: {session.Character.CharacterId} | SessionId: {session.SessionId}",
                        12));

            Session.SendPacket(Session.Character.GenerateSay("----------------------------------------", 11));
        }

        #endregion
    }
}