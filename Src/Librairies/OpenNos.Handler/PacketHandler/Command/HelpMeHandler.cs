using System.Linq;
using ChickenAPI.Enums;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class HelpMeHandler : IPacketHandler
    {
        #region Instantiation

        public HelpMeHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void HelpMe(HelpMePacket packet)
        {
            if (packet != null && !string.IsNullOrWhiteSpace(packet.Message))
            {
                Session.AddLogsCmd(packet);
                var count = 0;
                foreach (var team in ServerManager.Instance.Sessions.Where(s =>
                    s.Account.Authority >= AuthorityType.GM))
                    if (team.HasSelectedCharacter)
                    {
                        count++;

                        // TODO: move that to resx soo we follow i18n
                        team.SendPacket(team.Character.GenerateSay($"User {Session.Character.Name} needs your help!",
                            12));
                        team.SendPacket(team.Character.GenerateSay($"Reason: {packet.Message}", 12));
                        team.SendPacket(
                            team.Character.GenerateSay("Please inform the family chat when you take care of!", 12));
                        team.SendPacket(Session.Character.GenerateSpk("Click this message to start chatting.", 5));
                        team.SendPacket(
                            UserInterfaceHelper.GenerateMsg($"User {Session.Character.Name} needs your help!", 0));
                    }

                if (count != 0)
                {
                    Session.SendPacket(Session.Character.GenerateSay( $"{count} Team members were informed! You should get a message shortly.", 10));
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay( "Sadly, there are no online team member right now. Please ask for help on our Discord Server at:",10));
                    Session.SendPacket(Session.Character.GenerateSay("https://discord.gg/qAKvr5Cb", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HelpMePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}