using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class AngelDemonAltar : INpcDialogAsyncHandler
    {
        public long HandledId => 725;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50)
                {
                    Session.Character.AddQuest(6391, false);
                }
                else
                {

                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                }
            }
        }
    }

    public class AngelDemonAltar1 : INpcDialogAsyncHandler
    {
        public long HandledId => 726;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50)
                {
                    Session.Character.AddQuest(6390, false);
                }
                else
                {

                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                }
            }
        }
    }
}