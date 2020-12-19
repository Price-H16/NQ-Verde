using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class UpgradeTattooExtension
    {
        #region Methods

        public static void UpgradeTattoo(this CharacterSkill e, ClientSession s, bool isProtected)
        {
            if (isProtected && s.Character.Inventory.CountItem(5815) < 1)
            {
                s.SendShopEnd();
                return;
            }

            if (e.TattooLevel == 9)
            {
                s.SendShopEnd();
                return;
            }

            if (!e.IsTattoo)
            {
                s.SendShopEnd();
                return;
            }

            var skill = ServerManager.GetSkill(e.SkillVNum);
            var value = e.TattooLevel;

            if (skill.Class != 27)
            {
                s.SendShopEnd();
                return;
            }

            var get = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().TUpgrade;

            if (s.Character.Gold < get.GoldPrice[value])
            {
                s.SendShopEnd();
                return;
            }

            foreach (var item in get.Item[value])
                if (s.Character.Inventory.CountItem(item.Id) < item.Quantity)
                {
                    // Not Enough Item
                    s.SendShopEnd();
                    return;
                }

            var rnd = ServerManager.RandomNumber();
            string msg;
            int effectId;
            if (rnd < get.PercentDestroyed[value]) // fail + level --
            {
                if (!isProtected)
                {
                    e.TattooLevel--;
                    effectId = 3003;
                    msg = $"The {skill.Name} tattoo improvement FAILED ! and Decreased ! -{e.TattooLevel}";
                }
                else
                {
                    effectId = 3004;
                    msg = $"The {skill.Name} tattoo improvement FAILED ! But the level was saved with the scroll !";
                }
            }
            else if (rnd < get.PercentFail[value]) // fail
            {
                effectId = 3004;
                msg = $"The {skill.Name} tattoo improvement FAILED !";
            }
            else // success
            {
                e.TattooLevel++;
                effectId = 3005;
                msg = $"The {skill.Name} tattoo has been improved ! +{e.TattooLevel}";
            }

            foreach (var item in get.Item[value]) s.Character.Inventory.RemoveItemAmount(item.Id, item.Quantity);

            if (isProtected) s.Character.Inventory.RemoveItemAmount(5815);

            s.GoldLess(get.GoldPrice[value]);
            s.SendPacket(s.Character.GenerateSki());
            s.SendPackets(s.Character.GenerateQuicklist());
            s.CurrentMapInstance.Broadcast(
                StaticPacketHelper.GenerateEff(UserType.Player, s.Character.CharacterId, effectId),
                s.Character.PositionX, s.Character.PositionY);
            s.SendPacket(UserInterfaceHelper.GenerateMsg(msg, 0));
            s.SendPacket(UserInterfaceHelper.GenerateSay(msg, 11));
            s.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, s.Character.CharacterId, 2388));
            s.SendShopEnd();
        }

        #endregion
    }
}