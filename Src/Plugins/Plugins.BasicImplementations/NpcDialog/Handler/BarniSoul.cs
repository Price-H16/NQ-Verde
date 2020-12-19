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
    public class BarniSoul : INpcDialogAsyncHandler
    {
        public long HandledId => 324;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50) // Valhalla quest start?
                {
                    Session.Character.AddQuest(6134, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class BarniSoul1 : INpcDialogAsyncHandler
    {
        public long HandledId => 323;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50) // Teleport to Midgard cuz why not 
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 260, 54, 348);
                    });
                    
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
}
