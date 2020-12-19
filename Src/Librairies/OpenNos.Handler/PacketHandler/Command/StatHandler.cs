using System;
using System.Diagnostics;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class StatHandler : IPacketHandler
    {
        #region Instantiation

        public StatHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Stat(StatCommandPacket statCommandPacket)
        {
            var blablablaDuplicatedCode = statCommandPacket.isStart == 1 == true;
            if (!blablablaDuplicatedCode)
            {
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("XP_RATE_NOW")}: {ServerManager.Instance.Configuration.RateXP} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("DROP_RATE_NOW")}: {ServerManager.Instance.Configuration.RateDrop} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("GOLD_RATE_NOW")}: {ServerManager.Instance.Configuration.RateGold} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("GOLD_DROPRATE_NOW")}: {ServerManager.Instance.Configuration.RateGoldDrop} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("HERO_XPRATE_NOW")}: {ServerManager.Instance.Configuration.RateHeroicXP} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("FAIRYXP_RATE_NOW")}: {ServerManager.Instance.Configuration.RateFairyXP} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("REPUTATION_RATE_NOW")}: {ServerManager.Instance.Configuration.RateReputation} ",
                    13));
                Session.SendPacket(Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("SERVER_WORKING_TIME")}: {(Process.GetCurrentProcess().StartTime - DateTime.Now).ToString(@"d\ hh\:mm\:ss")} ",
                    13));
            }

            foreach (var message in CommunicationServiceClient.Instance.RetrieveServerStatistics(
                blablablaDuplicatedCode))
            {
                Session.SendPacket(Session.Character.GenerateSay(message, 13));
            }
        }

        #endregion
    }
}