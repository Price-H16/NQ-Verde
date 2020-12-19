using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class GreatLeader : INpcDialogAsyncHandler
    {
        public long HandledId => 680;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4001, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader2 : INpcDialogAsyncHandler
    {
        public long HandledId => 681;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4003, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader3 : INpcDialogAsyncHandler
    {
        public long HandledId => 682;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4005, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader4 : INpcDialogAsyncHandler
    {
        public long HandledId => 683;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4007, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader5 : INpcDialogAsyncHandler
    {
        public long HandledId => 684;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4009, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader6 : INpcDialogAsyncHandler
    {
        public long HandledId => 685;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4011, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader7 : INpcDialogAsyncHandler
    {
        public long HandledId => 686;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4019, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader8 : INpcDialogAsyncHandler
    {
        public long HandledId => 687;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4016, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader9 : INpcDialogAsyncHandler
    {
        public long HandledId => 688;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(336) >= 2)
                {
                    Session.Character.GiftAdd(4013, 1);
                    Session.Character.Inventory.RemoveItemAmount(336, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class GreatLeader10 : INpcDialogAsyncHandler
    {
        public long HandledId => 756;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 80)
                {
                    Session.Character.AddQuest(6415, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
}

