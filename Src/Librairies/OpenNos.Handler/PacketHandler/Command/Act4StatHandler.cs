using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class Act4StatHandler : IPacketHandler
    {
        #region Instantiation

        public Act4StatHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Act4Stat(Act4StatPacket packet)
        {
            if (packet != null && ServerManager.Instance.ChannelId == 51)
            {
                Session.AddLogsCmd(packet);

                switch (packet.Faction)
                {
                    case 1:
                        ServerManager.Instance.Act4AngelStat.Percentage = packet.Value;
                        break;

                    case 2:
                        ServerManager.Instance.Act4DemonStat.Percentage = packet.Value;
                        break;
                }

                foreach (var sess in ServerManager.Instance.Sessions) sess.SendPacket(sess.Character.GenerateFc());
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Act4StatPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}