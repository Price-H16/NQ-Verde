using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D301 : INpcDialogAsyncHandler
    {
        public long HandledId => 301;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            //var tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
            if (npc != null && Session.Character.Level >= 85)
            {
                //ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 228, 78, 14);
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 228, (short)(68 + ServerManager.RandomNumber(-3, 3)), (short)(102 + ServerManager.RandomNumber(-3, 3)));
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
            }
        }
    }
}