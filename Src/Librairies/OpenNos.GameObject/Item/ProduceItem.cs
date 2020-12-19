using System.Linq;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class ProduceItem : Item
    {
        #region Instantiation

        public ProduceItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0, string[] packetsplit = null)
        {
            if (session.Character.IsVehicled)
            {
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
            {
                return;
            }

            switch (Effect)
            {
                case 100:
                    session.Character.LastNRunId = 0;
                    session.Character.LastItemVNum = inv.ItemVNum;
                    session.SendPacket("shop_end 1");
                    session.SendPacket("wopen 28 0");
                    var recipeList = ServerManager.Instance.GetRecipesByItemVNum(VNum);
                    var list = recipeList.Where(s => s.Amount > 0).Aggregate("m_list 2", (current, s) => current + $" {s.ItemVNum}");
                    session.SendPacket(list + (EffectValue <= 110 && EffectValue >= 108 ? " 999" : ""));
                    break;
                
                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), VNum, Effect, EffectValue));
                    break;
            }
        }

        #endregion
    }
}