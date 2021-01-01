using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.ItemUsage.Handler.Title
{
    public class DefaultTitle : IUseItemRequestHandlerAsync
    {
        #region Properties

        public long EffectId => default;

        public ItemPluginType Type => ItemPluginType.Title;

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

            if (session.Character.Inventory.CountItem(e.Item.ItemVNum) < 1)
            {
                return;
            }

            if (session.Character.Title.Any(s => s.TitleVnum == e.Item.ItemVNum))
            {
                return;
            }

            session.SendPacket($"qna #guri^306^{e.Item.ItemVNum}^{e.Item.Slot} {Language.Instance.GetMessageFromKey("ASK_TITLE")}");
        }

        #endregion
    }
}