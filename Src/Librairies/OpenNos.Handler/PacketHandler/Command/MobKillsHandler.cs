using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MobKillsHandler : IPacketHandler
    {
        #region Instantiation

        public MobKillsHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        public void MobKillsPacket(MobKillsCounterPacket packet)
        {
            if(Session != null && packet != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(
                    "-------------------------------------------------------------------------------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"Mob Kill Counter: {Session.Character.MobKillCounter.ToString("###,##0")}", 10));
                Session.SendPacket(Session.Character.GenerateSay(
                    "-------------------------------------------------------------------------------------------", 10));
            }
        }
    }
}
