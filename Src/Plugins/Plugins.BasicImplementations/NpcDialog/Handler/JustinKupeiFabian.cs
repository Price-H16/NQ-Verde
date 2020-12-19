using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class Justin : INpcDialogAsyncHandler
    {
        public long HandledId => 728;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    Session.Character.AddQuest(6393, false);
                }
                else
                {

                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                }
            }
        }
    }

    public class Kupei : INpcDialogAsyncHandler
    {
        public long HandledId => 729;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    Session.Character.AddQuest(6396, false);
                }
                else
                {

                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                }
            }
        }
    }

    public class Fabian : INpcDialogAsyncHandler
    {
        public long HandledId => 730;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    Session.Character.AddQuest(6399, false);
                }
                else
                {

                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                }
            }
        }
    }
}