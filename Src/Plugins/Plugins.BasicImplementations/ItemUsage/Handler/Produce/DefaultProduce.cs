using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.ItemUsage.Handler.Produce
{
    public class DefaultProduce : IUseItemRequestHandlerAsync
    {
        #region Properties

        public long EffectId => default;

        public ItemPluginType Type => ItemPluginType.Produce;

        #endregion

        #region Methods

        public async Task HandleAsync(ClientSession session, InventoryUseItemEvent e)
        {
            if (session.Character.IsVehicled)
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
            {
                return;
            }

            switch (e.Item.Item.Effect)
            {
                case 100:
                    session.Character.LastNRunId = 0;
                    session.Character.LastItemVNum = e.Item.ItemVNum;
                    session.SendPacket("wopen 28 0");
                    var recipeList = ServerManager.Instance.GetRecipesByItemVNum(e.Item.Item.VNum);
                    var list = recipeList.Where(s => s.Amount > 0)
                        .Aggregate("m_list 2", (current, s) => current + $" {s.ItemVNum}");
                    session.SendPacket(list + (e.Item.Item.EffectValue <= 110 && e.Item.Item.EffectValue >= 108 ? " 999" : ""));
                    break;

                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), e.Item.Item.VNum,
                        e.Item.Item.Effect, e.Item.Item.EffectValue));
                    break;
            }
        }

        #endregion
    }
}