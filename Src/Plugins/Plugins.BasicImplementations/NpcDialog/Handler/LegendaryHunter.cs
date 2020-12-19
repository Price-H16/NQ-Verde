using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class LegendaryHunter : INpcDialogAsyncHandler
    {
        public long HandledId => 691;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 90) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7519);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class LegendaryHunter2 : INpcDialogAsyncHandler
    {
        public long HandledId => 692;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 90) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7520);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class LegendaryHunter3 : INpcDialogAsyncHandler
    {
        public long HandledId => 693;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 90) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7521);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class LegendaryHunter4 : INpcDialogAsyncHandler
    {
        public long HandledId => 694;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 90) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7522);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class LegendaryHunter5 : INpcDialogAsyncHandler
    {
        public long HandledId => 695;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 70) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7523);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class LegendaryHunter6 : INpcDialogAsyncHandler
    {
        public long HandledId => 696;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 70) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7524);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class Lilith : INpcDialogAsyncHandler
    {
        public long HandledId => 719;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 88) //  REQUIRED LEVEL
                {
                    Session.Character.AddQuest(7518);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
}