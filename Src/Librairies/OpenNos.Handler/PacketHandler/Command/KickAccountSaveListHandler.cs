using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class KickAccountSaveListHandler : IPacketHandler
    {
        #region Instantiation

        public KickAccountSaveListHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        public void KickAccSave(KickAccountSaveListPacket kickAccountSaveList)
        {
            if (kickAccountSaveList != null && kickAccountSaveList.AccountId > 0)
            {
                ServerManager.Instance.CharacterSynchronizingAtSaveProcess(kickAccountSaveList.AccountId, false);

                Session.SendPacket(Session.Character.GenerateSay("DONE", 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(KickAccountSaveListPacket.ReturnHelp(), 10));
            }
        }
    }
}
