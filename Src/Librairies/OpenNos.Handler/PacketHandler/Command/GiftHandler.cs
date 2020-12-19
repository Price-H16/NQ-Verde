using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GiftHandler : IPacketHandler
    {
        #region Instantiation

        public GiftHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Gift(GiftPacket giftPacket)
        {
            if (giftPacket != null)
            {
                var Amount = giftPacket.Amount;

                if (Amount <= 0) Amount = 1;

                Session.AddLogsCmd(giftPacket);
                if (giftPacket.CharacterName == "*")
                {
                    if (Session.HasCurrentMapInstance)
                    {
                        foreach (var session in Session.CurrentMapInstance.Sessions)
                            session.Character.SendGift(session.Character.CharacterId, giftPacket.VNum,
                                Amount, giftPacket.Rare, giftPacket.Upgrade, giftPacket.Design, false);
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GIFT_SENT"), 10));
                    }
                }
                else if (giftPacket.CharacterName == "ALL")
                {
                    int levelMin = giftPacket.ReceiverLevelMin;
                    var levelMax = giftPacket.ReceiverLevelMax == 0 ? 99 : giftPacket.ReceiverLevelMax;

                    DAOFactory.CharacterDAO.LoadAll().ToList().ForEach(chara =>
                    {
                        if (chara.Level >= levelMin && chara.Level <= levelMax)
                            Session.Character.SendGift(chara.CharacterId, giftPacket.VNum, Amount,
                                giftPacket.Rare, giftPacket.Upgrade, giftPacket.Design, false);
                    });
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GIFT_SENT"), 10));
                }
                else
                {
                    var chara = DAOFactory.CharacterDAO.LoadByName(giftPacket.CharacterName);
                    if (chara != null)
                    {
                        Session.Character.SendGift(chara.CharacterId, giftPacket.VNum, Amount,
                            giftPacket.Rare, giftPacket.Upgrade, giftPacket.Design, false);
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GIFT_SENT"), 10));
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"),
                                0));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GiftPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}