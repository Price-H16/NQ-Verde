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
    public class CaptainAlveus : INpcDialogAsyncHandler
    {
        public long HandledId => 313;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50) // Teleport 
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 261, 179, 209);
                    });

                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class CaptainHapendam : INpcDialogAsyncHandler
    {
        public long HandledId => 314;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50) // Teleport
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, 32, 18);
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
