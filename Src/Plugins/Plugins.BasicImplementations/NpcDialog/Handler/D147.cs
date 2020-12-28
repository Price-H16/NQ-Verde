using System.Linq;
using System.Threading.Tasks;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D147 : INpcDialogAsyncHandler
    {
        public long HandledId => 147;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc == null)
           {
               return;
           }
           if (packet.Type == 0)
           {
               Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
           }
           else
           {
               if (Session.Character.Inventory.CountItem(2523) < 25)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                   return;
               }
               switch (Session.Character.Class)
               {
                   case CharacterClassType.Swordsman:
                       Session.Character.GiftAdd(4497, 1);
                       break;

                   case CharacterClassType.Archer:
                       Session.Character.GiftAdd(4498, 1);
                       break;

                   case CharacterClassType.Magician:
                       Session.Character.GiftAdd(4499, 1);
                       break;
               }
               Session.Character.Inventory.RemoveItemAmount(2523, 25);
           }
        }
    }
}