using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.CommandPackets;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UnlockHandler : IPacketHandler
    {
        #region Instantiation

        public UnlockHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Unlock(UnlockPacket unlockPacket)
        {
            if (unlockPacket != null || unlockPacket.lockcode != null)
            {
                if (!Session.Character.VerifiedLock)
                {
                    #region Unlock Code
                    Session.Character.HeroChatBlocked = false;
                    Session.Character.ExchangeBlocked = false;
                    Session.Character.WhisperBlocked = false;
                    Session.Character.Invisible = false;
                    Session.Character.NoAttack = false;
                    Session.Character.NoMove = false;
                    Session.Character.VerifiedLock = true;
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.MapInstanceId, Session.Character.PositionX, Session.Character.PositionY, true);
                    #endregion
                    Session.SendPacket(Session.Character.GenerateSay($"Your account now is unlocked.", 10));
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay("Your account is unlocked.", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UnlockPacket.ReturnHelp(), 10));
            }
            return;
        }

        #endregion
    }
}
