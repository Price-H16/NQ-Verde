using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeHeroLevelHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeHeroLevelHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeHeroLevel(ChangeHeroLevelPacket changeHeroLevelPacket)
        {
            if (changeHeroLevelPacket != null)
            {
                Session.AddLogsCmd(changeHeroLevelPacket);
                if (changeHeroLevelPacket.HeroLevel <= 255)
                {
                    Session.Character.HeroLevel = changeHeroLevelPacket.HeroLevel;
                    Session.Character.HeroXp = 0;
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HEROLEVEL_CHANGED"), 0));
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.SendPackets(Session.Character.GenerateStatChar());
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 6),
                        Session.Character.PositionX, Session.Character.PositionY);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 198),
                        Session.Character.PositionX, Session.Character.PositionY);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeHeroLevelPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}