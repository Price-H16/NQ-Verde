using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class HellOrHeaven : INpcDialogAsyncHandler
    {
        public long HandledId => 708; // "Accept"

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 88 && Session.Character.Inventory.CountItem(1122) >= 1 && Session.Character.HasDoneQuestId(6133))
                {
                    Session.Character.AddQuest(6088, false);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }
    }

    public class HellOrHeaven2 : INpcDialogAsyncHandler
    {
        public long HandledId => 709; // "Refuse"

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 88 && Session.Character.HasDoneQuestId(6133))
                {
                    Session.Character.AddQuest(6111, false);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                    return;
                }
            }
        }
    }
}
