using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class ICTrade4 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 702;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 2)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(1286, 3);
                    Session.Character.Inventory.RemoveItemAmount(2236, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 642;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 4)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(4240, 1);
                    Session.Character.Inventory.RemoveItemAmount(2236, 4);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 700;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 2)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(1363, 5); //todo
                    Session.Character.Inventory.RemoveItemAmount(2236, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader3 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 701;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 2)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(1363, 5); //todo
                    Session.Character.Inventory.RemoveItemAmount(2236, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader5 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 703;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 10)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(5372, 1);
                    Session.Character.Inventory.RemoveItemAmount(2236, 10);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader6 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 704;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 3)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(2282, 75);
                    Session.Character.Inventory.RemoveItemAmount(2236, 3);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader7 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 705;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 2)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(1030, 75);
                    Session.Character.Inventory.RemoveItemAmount(2236, 2);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader8 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 706;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 3)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(387, 1, 0, 5);
                    Session.Character.Inventory.RemoveItemAmount(2236, 3);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }

    public class ICTrader9 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 707;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (packet.Type == 0)
                {
                    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
                }
                else if (Session.Character.Inventory.CountItem(2236) >= 3)
                {
                    Session.Character.DisposeShopAndExchange();
                    Session.SendPacket(Session.Character.GenerateCond());
                    Session.Character.GiftAdd(379, 1, 0, 5);
                    Session.Character.Inventory.RemoveItemAmount(2236, 3);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                }
            }
        }

        #endregion
    }
}