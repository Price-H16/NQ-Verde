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
        #region Properties

        public long HandledId => 680;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader10 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 756;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 681;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader3 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 682;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader4 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 683;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader5 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 684;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader6 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 685;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader7 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 686;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader8 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 687;

        #endregion

        #region Methods

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

        #endregion
    }

    public class GreatLeader9 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 688;

        #endregion

        #region Methods

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

        #endregion
    }
}