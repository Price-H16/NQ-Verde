using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D195 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 195;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;

            // 100 Nastri VS x*X
            if (npc == null)
            {
                return;
            }
            const short Nastro = 1188;
            const short XXX = 1; //TODO
            switch (packet.Type)
            {
                case 0:
                    Session.SendPacket($"qna #n_run^{packet.Runner}^61^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("EXCHANGE_MATERIAL")}");
                    break;

                case 61:
                    if (Session.Character.Inventory.CountItem(Nastro) <= 100)
                    {
                        // Non hai Nastri
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENT"), 11));
                        return;
                    }
                    Session.Character.GiftAdd(XXX, 2);
                    Session.Character.Inventory.RemoveItemAmount(Nastro, 5);
                    break;
            }
        }

        #endregion
    }
}