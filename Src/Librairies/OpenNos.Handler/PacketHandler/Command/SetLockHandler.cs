using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class SetLockHandler : IPacketHandler
    {
        #region Instantiation

        public SetLockHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods
        public void SetLock(SetLockPacket setLockPacket)
        {
            if (setLockPacket != null)
            {
                if (setLockPacket.Psw2.Length >= 8)
                {
                    if (Session.Character.VerifiedLock)
                    {
                        if (Session.Character.LockCode == null)
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
                            Session.Character.LockCode = CryptographyBase.Sha512(setLockPacket.Psw2);
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("YOUR_LOCK_CODE_IS") + setLockPacket.Psw2 + Language.Instance.GetMessageFromKey("TAKE_SCREENSHOT_FOR_RECOVERY"), 10));

                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CHANGE_LOCK_CODE") + ChangeLockPacket.ReturnHelp(), 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UNLOCK_BEFORE_CHANGE_LOCK_CODE") + ChangeLockPacket.ReturnHelp(), 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOCK_CODE_MUST_CONTAINS_8_CHARACTERS"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(SetLockPacket.ReturnHelp(), 10));
            }
        }
    }
}
#endregion