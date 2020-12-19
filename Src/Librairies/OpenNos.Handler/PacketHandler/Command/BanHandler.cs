using System;
using NosTale.Extension.Extension.Command;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class BanHandler : IPacketHandler
    {
        #region Instantiation

        public BanHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Ban(BanPacket banPacket)
        {
            if (banPacket != null)
            {
                Session.AddLogsCmd(banPacket);
                Session.BanMethod(banPacket.CharacterName, banPacket.Duration, banPacket.Reason, banPacket.IsBanIp);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BanPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}