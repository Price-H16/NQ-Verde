using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Extension.Inventory;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D666 : INpcDialogAsyncHandler
    {
        public long HandledId => 666;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           // 4949 ~ 4966 = c25/c28 4978 ~ 4986 = c45/c48

           const long price = 10000000;

           var itemInstance = Session?.Character?.Inventory?.LoadBySlotAndType(0, InventoryType.Equipment);

           if (itemInstance?.Item != null && (itemInstance.ItemVNum >= 4949 && itemInstance.ItemVNum <= 4966 || itemInstance.ItemVNum >= 4978 && itemInstance.ItemVNum <= 4986) && itemInstance.Rare == 8)
           {
               if (Session.Character.Gold < price)
               {
                   Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                   return;
               }

               Session.Character.Gold -= price;
               Session.SendPacket(Session.Character.GenerateGold());

               itemInstance.RarifyItem(Session, RarifyMode.HeroEquipmentDowngrade, RarifyProtection.None);

               Session.SendPacket(itemInstance.GenerateInventoryAdd());
           }
        }
    }
}