using System.Linq;
using System.Threading.Tasks;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D200 : INpcDialogAsyncHandler
    {
        public long HandledId => 200;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Quests.Any(s => s.Quest.QuestType == (int)QuestType.Dialog2 && s.Quest.QuestObjectives.Any(b => b.Data == npc.NpcVNum)))
                {
                    Session.Character.AddQuest(packet.Type);
                    Session.Character.IncrementQuests(QuestType.Dialog2, npc.NpcVNum);
                }
            }
        }
    }
}