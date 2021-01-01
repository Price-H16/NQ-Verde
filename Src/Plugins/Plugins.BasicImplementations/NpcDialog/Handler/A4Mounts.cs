using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class A4Mounts2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 658;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(5911) >= 25 && Session.Character.Inventory.CountItem(2307) >= 50 && Session.Character.Inventory.CountItem(2308) >= 50)
                {
                    Session.Character.GiftAdd(5319, 1);
                    Session.Character.Inventory.RemoveItemAmount(5911, 25);
                    Session.Character.Inventory.RemoveItemAmount(2307, 50);
                    Session.Character.Inventory.RemoveItemAmount(2308, 50);
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("A4_MOUNT_BUY"), 10));
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

    public class A4Mounts3 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 659;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(5911) >= 25 && Session.Character.Inventory.CountItem(2307) >= 50 && Session.Character.Inventory.CountItem(2308) >= 50)
                {
                    Session.Character.GiftAdd(5321, 1);
                    Session.Character.Inventory.RemoveItemAmount(5911, 25);
                    Session.Character.Inventory.RemoveItemAmount(2307, 50);
                    Session.Character.Inventory.RemoveItemAmount(2308, 50);
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("A4_MOUNT_BUY"), 10));
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

    public class A4Mounts4 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 660;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(5911) >= 25 && Session.Character.Inventory.CountItem(2307) >= 50 && Session.Character.Inventory.CountItem(2308) >= 50 && Session.Character.Inventory.CountItem(1158) >= 1)
                {
                    Session.Character.GiftAdd(5330, 1);
                    Session.Character.Inventory.RemoveItemAmount(5911, 25);
                    Session.Character.Inventory.RemoveItemAmount(2307, 50);
                    Session.Character.Inventory.RemoveItemAmount(2308, 50);
                    Session.Character.Inventory.RemoveItemAmount(1158, 1);
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("A4_MOUNT_BUY"), 10));
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

    public class A4MountsQuest : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 656;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 30)
                {
                    Session.Character.AddQuest(3353, false);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }

        #endregion

        #region Classes

        public class A4Mounts : INpcDialogAsyncHandler
        {
            #region Properties

            public long HandledId => 657;

            #endregion

            #region Methods

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Inventory.CountItem(5911) >= 25 && Session.Character.Inventory.CountItem(2307) >= 50 && Session.Character.Inventory.CountItem(2308) >= 50)
                    {
                        Session.Character.GiftAdd(5323, 1);
                        Session.Character.Inventory.RemoveItemAmount(5911, 25);
                        Session.Character.Inventory.RemoveItemAmount(2307, 50);
                        Session.Character.Inventory.RemoveItemAmount(2308, 50);
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("A4_MOUNT_BUY"), 10));
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

        #endregion
    }
}