using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D17 : INpcDialogAsyncHandler
    {
        public long HandledId => 17;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
            {
                return;
            }
            if (packet.Type < 0)
            {
                // Packet hacking allowing duplication
                return;
            }
            if (packet.Value == 1)
            {
                Session.SendPacket($"qna #n_run^{packet.Runner}^{packet.Type}^2^{packet.NpcId} {string.Format(Language.Instance.GetMessageFromKey("ASK_ENTER_GOLD"), 5000 * (1 + packet.Type))}");
            }
            else
            {
                var currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                var timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
                if (!(timeSpanSinceLastPortal >= 4) || !Session.HasCurrentMapInstance || ServerManager.Instance.ChannelId == 51 || Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.ArenaInstance.MapInstanceId || Session.CurrentMapInstance.MapInstanceId == ServerManager.Instance.FamilyArenaInstance.MapInstanceId)
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                    return;
                }
                if (packet.Type >= 0 && Session.Character.Gold >= 5000 * (1 + packet.Type))
                {
                    Session.Character.LastPortal = currentRunningSeconds;
                    Session.Character.Gold -= 5000 * (1 + packet.Type);
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(Session.Character.GenerateAscr());
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PVP_ACTIVED_ON_MAP"), 10));
                    var pos = packet.Type == 0 ? ServerManager.Instance.ArenaInstance.Map.GetRandomPosition() : ServerManager.Instance.FamilyArenaInstance.Map.GetRandomPosition();
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, packet.Type == 0 ? ServerManager.Instance.ArenaInstance.MapInstanceId : ServerManager.Instance.FamilyArenaInstance.MapInstanceId, pos.X, pos.Y);
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                }
            }
        }
    }
}