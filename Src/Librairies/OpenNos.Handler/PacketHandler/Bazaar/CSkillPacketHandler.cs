using System;
using System.Threading;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CSkillPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CSkillPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void OpenBazaar(CSkillPacket cSkillPacket)
        {
            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);

            var medal = Session.Character.StaticBonusList.Find(s =>
                s.StaticBonusType == StaticBonusType.BazaarMedalGold ||
                s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

            if (medal != null)
            {
                var medalType = medal.StaticBonusType == StaticBonusType.BazaarMedalGold
                    ? MedalType.Gold
                    : MedalType.Silver;

                var time = (int) (medal.DateEnd - DateTime.Now).TotalHours;

                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOTICE_BAZAAR"),
                    0));
                Session.SendPacket($"wopen 32 {(byte) medalType} {time}");
            }
            else
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
            }
        }

        #endregion
    }
}