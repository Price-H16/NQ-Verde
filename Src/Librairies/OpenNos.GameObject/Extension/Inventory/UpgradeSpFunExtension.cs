using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class UpgradeSpFunExtension
    {
        #region Methods

        public static void UpgradeSpFun(this ItemInstance e, ClientSession CharacterSession, int value)
        {
            var conf = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().UpgradeSpFun;

            if (CharacterSession == null) return;

            if (!CharacterSession.HasCurrentMapInstance) return;

            if (e.Upgrade >= 15) return;

            if (CharacterSession.Character.Inventory.CountItem(conf.CostItemVnum[value]) < 1)
            {
                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(
                    Language.Instance.GetMessageFromKey(string.Format(
                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                        ServerManager.GetItem(conf.CostItemVnum[value]).Name, 1)), 10));
                return;
            }

            CharacterSession.Character.Inventory.RemoveItemAmount(conf.CostItemVnum[value]);
            CharacterSession.SendPacket(CharacterSession.Character.Inventory.CountItem(conf.CostItemVnum[value]) < 1
                ? "shop_end 2"
                : "shop_end 1");

            var wearable = CharacterSession.Character.Inventory.GetItemInstanceById(e.Id);
            var rnd = ServerManager.RandomNumber();
            if (rnd < conf.UpFail[e.Upgrade]
            ) // - DependencyContainer.Instance.Get<JsonGameConfiguration>().DefaultEvent.PerfectionSp)
            {
                CharacterSession.CurrentMapInstance.Broadcast(CharacterSession.Character.GenerateEff(3004),
                    CharacterSession.Character.MapX, CharacterSession.Character.MapY);
                CharacterSession.SendPacket(
                    CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"),
                        11));
                CharacterSession.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"), 0));
            }
            else
            {
                CharacterSession.CurrentMapInstance.Broadcast(CharacterSession.Character.GenerateEff(3004), CharacterSession.Character.MapX, CharacterSession.Character.MapY);
                CharacterSession.CurrentMapInstance.Broadcast(CharacterSession.Character.GenerateEff(3005), CharacterSession.Character.MapX, CharacterSession.Character.MapY);
                CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 12));
                CharacterSession.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 0));

                wearable.Upgrade++;
                if (wearable.Upgrade > 8)
                {
                    CharacterSession.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, CharacterSession.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                }                
                CharacterSession.SendPacket(wearable.GenerateInventoryAdd());
            }

            CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
            CharacterSession.SendPacket(CharacterSession.Character.GenerateEq());
            CharacterSession.SendPacket("shop_end 1");
        }

        #endregion
    }
}