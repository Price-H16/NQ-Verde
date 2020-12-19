using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeLockHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeLockHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeLock(ChangeLockPacket changeLockPacket)
        {
            if (changeLockPacket != null)
            {
                if (Session.Character.LockCode == CryptographyBase.Sha512(changeLockPacket.oldlock))
                {
                    if (changeLockPacket.newlock.Length >= 8)
                    {
                        if (changeLockPacket.newlock == "00000000")
                        {
                            #region Unlock Code
                            Session.Character.HeroChatBlocked = false;
                            Session.Character.ExchangeBlocked = false;
                            Session.Character.WhisperBlocked = false;
                            Session.Character.Invisible = false;
                            Session.Character.NoAttack = false;
                            Session.Character.NoMove = false;
                            Session.Character.VerifiedLock = true;
                            Session.Character.LockCode = null;
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.MapInstanceId, Session.Character.PositionX, Session.Character.PositionY, true);
                            #endregion
                            Session.SendPacket(Session.Character.GenerateSay("Done! Your lock is inactive", 10));
                            return;
                        }
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
                        Session.Character.LockCode = CryptographyBase.Sha512(changeLockPacket.newlock);
                        Session.SendPacket(Session.Character.GenerateSay("Done! Your new lock is: " + changeLockPacket.newlock, 10));
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay("The new lock need minimum 8 characters.", 10));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay("You dont put the correct lock. Are you rebember this?", 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeLockPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}