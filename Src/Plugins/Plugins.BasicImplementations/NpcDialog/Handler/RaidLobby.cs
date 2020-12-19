using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class RaidLobby : INpcDialogAsyncHandler
    {
        public long HandledId => 644;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));

                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                {
                    
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 5415, 18, 18);
                });
            }
        }
    }
}
