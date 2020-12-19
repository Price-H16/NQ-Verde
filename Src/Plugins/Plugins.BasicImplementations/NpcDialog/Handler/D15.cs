using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D15 : INpcDialogAsyncHandler
    {
        public long HandledId => 15;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc != null)
           {
                if (packet.Value == 2)
                {
                    Session.SendPacket($"qna #n_run^15^1^1^{npc.MapNpcId} {Language.Instance.GetMessageFromKey("ASK_CHANGE_SPAWNLOCATION")}");
                }
                else
                {
                    switch (npc.MapId)
                    {
                        case 1:
                            Session.Character.SetRespawnPoint(1, 58, 84);
                            break;

                        case 20:
                            Session.Character.SetRespawnPoint(20, 9, 92);
                            break;

                        case 145:
                            Session.Character.SetRespawnPoint(145, 36, 108);
                            break;

                        case 170:
                            Session.Character.SetRespawnPoint(170, 79, 47);
                            break;

                        case 177:
                            Session.Character.SetRespawnPoint(177, 149, 74);
                            break;

                        case 189:
                            Session.Character.SetRespawnPoint(189, 58, 166);
                            break;
                    }
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RESPAWNLOCATION_CHANGED"), 0));
                }      
           }
        }
    }
}