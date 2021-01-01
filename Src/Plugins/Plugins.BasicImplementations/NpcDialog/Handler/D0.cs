using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D0 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 0;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null)
            {
                return;
            }

            var goldless = 2500000;
            if (Session?.Character?.Gold < goldless)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_HAVE_GOLD"), 10));
                return;
            }
            if (packet?.Type == 0)
            {
                Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("TRY_YOUR_LUCK")}");
            }
            else
            {
                Session.GoldLess(goldless);
                Session.SendPacket(UserInterfaceHelper.GenerateInfo("Testing your luck..."));
                Session.Character.DisposeShopAndExchange();
                Session.SendPacket(Session.Character.GenerateCond());
                Observable.Timer(TimeSpan.FromSeconds(4)).Subscribe(o =>
                {
                    var rnd = ServerManager.RandomNumber(0, 100);
                    if (rnd <= 100)
                    {
                        short[] vnums = { 2282, 1030, 1428, 1244, 1013, 5018, 1286, 1296, 1012, 1363, 1364, 1218, 5369, 2037, 2041, 2049, 2045, 4129, 4130, 4131, 4132, 4262, 1, 33, 46, 1366, 1219 };
                        byte[] counts = { 99, 99, 50, 50, 99, 1, 3, 3, 99, 5, 5, 10, 5, 99, 99, 99, 99, 1, 1, 1, 1, 1, 1, 1, 1, 1, 5 };
                        var item = ServerManager.RandomNumber(0, 27);
                        Session.Character.GiftAdd(vnums[item], counts[item]);
                        Session.Character.GiftAdd(11001, 1);
                    }

                    //Session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("RECEIVED_ITEM"))));
                });
            }
        }

        #endregion
    }
}