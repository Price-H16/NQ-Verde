using OpenNos.Core;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1503 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 1503;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (!ServerManager.Instance.Configuration.HalloweenEvent)
            {
                return;
            }

            if (Session.Character.Level < 20)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                return;
            }

            if (!Session.Character.GeneralLogs.Any(s => s.LogType == "DailyReward" && short.Parse(s.LogData) == 1917 && s.Timestamp.Date == DateTime.Today))
            {
                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = Session.Account.AccountId,
                    CharacterId = Session.Character.CharacterId,
                    IpAddress = Session.IpAddress,
                    LogData = "1917",
                    LogType = "DailyReward",
                    Timestamp = DateTime.Now
                });
                short amount = 1;
                if (Session.Character.IsMorphed)
                {
                    amount *= 2;
                }
                Session.Character.GiftAdd(1917, amount);
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE"), 0));
            }
        }

        #endregion
    }
}