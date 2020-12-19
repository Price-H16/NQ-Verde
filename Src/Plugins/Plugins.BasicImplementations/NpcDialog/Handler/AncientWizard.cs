using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class AncientWizard : INpcDialogAsyncHandler
    {
        public long HandledId => 712;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    Session.Character.AddQuest(6384, false);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                    return;
                }

            }
        }
    }

    public class AncientWizard2 : INpcDialogAsyncHandler // first TP
    {
        public long HandledId => 713;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 95, 2, 14);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                    return;
                }

            }
        }
    }

    public class AncientWizard3 : INpcDialogAsyncHandler // second TP
    {
        public long HandledId => 714;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55 && Session.Character.Inventory.CountItem(11118) >= 1)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 96, 2, 14);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }

            }
        }
    }

    public class AncientWizard4 : INpcDialogAsyncHandler // third TP
    {
        public long HandledId => 715;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55 && Session.Character.Inventory.CountItem(11119) >= 1)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 94, 2, 14);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }

            }
        }
    }
}