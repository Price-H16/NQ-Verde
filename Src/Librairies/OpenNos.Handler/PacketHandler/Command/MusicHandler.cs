using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MusicHandler : IPacketHandler
    {
        #region Instantiation

        public MusicHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Music(MusicPacket musicPacket)
        {
            if (musicPacket != null)
            {
                if (musicPacket.Music < 0)
                {
                    return;
                }
                Session.CurrentMapInstance?.Broadcast($"bgm {musicPacket.Music}");
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MusicPacket.ReturnHelp(), 10));
            }
        }
        #endregion
    }
}