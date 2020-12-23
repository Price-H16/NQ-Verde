using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    internal class CMDHandler : IPacketHandler
    {
        #region Instantiation

        public CMDHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void CMD(CMDPacket commandsPacket)
        {
            // Useless just use $Help
            Session.AddLogsCmd(commandsPacket);
            var time = Session.Character.LastCMD.AddSeconds(10);

            if (DateTime.Now <= time) // Anti spam
                return;
            Session.Character.LastCMD = DateTime.Now;

            Session.SendPacket(Session.Character.GenerateSay("---------Player Commands---------", 10));
            Session.SendPacket(Session.Character.GenerateSay("$Guide", 12));
            Session.SendPacket(Session.Character.GenerateSay("$HelpMe", 12));
            Session.SendPacket(Session.Character.GenerateSay("$Warp", 12));
            Session.SendPacket(Session.Character.GenerateSay("$Unstuck", 12));
            Session.SendPacket(Session.Character.GenerateSay("$Schedule", 12));
            Session.SendPacket(Session.Character.GenerateSay("---------------------------------", 10));
        }

        #endregion
    }
}