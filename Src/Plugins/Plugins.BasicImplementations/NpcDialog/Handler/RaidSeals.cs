﻿using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class RaidSeals : INpcDialogAsyncHandler
    {
        public long HandledId => 645;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 100000)
                {
                    Session.Character.GiftAdd(1127, 1);
                    Session.Character.Inventory.RemoveItemAmount(1012, 10);
                    Session.Character.Gold -= 100000;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class RaidSeals2 : INpcDialogAsyncHandler
    {
        public long HandledId => 646;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 150000)
                {
                    Session.Character.GiftAdd(1128, 1);
                    Session.Character.Inventory.RemoveItemAmount(1012, 10);
                    Session.Character.Gold -= 150000;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class RaidSeals3 : INpcDialogAsyncHandler
    {
        public long HandledId => 647;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 150000)
                {
                    Session.Character.GiftAdd(1129, 1);
                    Session.Character.Inventory.RemoveItemAmount(1012, 10);
                    Session.Character.Gold -= 150000;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class RaidSeals4 : INpcDialogAsyncHandler
    {
        public long HandledId => 648;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 200000)
                {
                    Session.Character.GiftAdd(1131, 1);
                    Session.Character.Inventory.RemoveItemAmount(1012, 10);
                    Session.Character.Gold -= 200000;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }
    }

    public class RaidSeals5 : INpcDialogAsyncHandler
    {
        public long HandledId => 649;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 300000)
                {
                    Session.Character.GiftAdd(1130, 1);
                    Session.Character.Inventory.RemoveItemAmount(1012, 10);
                    Session.Character.Gold -= 300000;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                    return;
                }
            }
        }

        public class RaidSeals6 : INpcDialogAsyncHandler
        {
            public long HandledId => 650;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 500000)
                    {
                        Session.Character.GiftAdd(1892, 1);
                        Session.Character.Inventory.RemoveItemAmount(1012, 10);
                        Session.Character.Gold -= 500000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                        return;
                    }
                }
            }
        }

        //public class RaidSeals7 : INpcDialogAsyncHandler
        //{
        //    public long HandledId => 651;

        //    public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        //    {
        //        var npc = packet.Npc;
        //        if (npc != null)
        //        {
        //            if (Session.Character.Inventory.CountItem(1012) >= 50 && Session.Character.Gold >= 1000000)
        //            {
        //                Session.Character.GiftAdd(5500, 1);
        //                Session.Character.Inventory.RemoveItemAmount(1012, 50);
        //                Session.Character.Gold -= 1000000;
        //                Session.SendPacket(Session.Character.GenerateGold());
        //                Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
        //            }
        //            else
        //            {
        //                Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
        //                return;
        //            }
        //        }
        //    }
        //}

        //public class RaidSeals8 : INpcDialogAsyncHandler
        //{
        //    public long HandledId => 652;

        //    public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        //    {
        //        var npc = packet.Npc;
        //        if (npc != null)
        //        {
        //            if (Session.Character.Inventory.CountItem(1012) < 5 || Session.Character.Inventory.CountItem(2307) < 5 || Session.Character.Inventory.CountItem(5911) < 5)
        //            {
        //                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
        //                return;
        //            }
        //            Session.Character.GiftAdd(5512, 1);
        //            Session.Character.Inventory.RemoveItemAmount(1012, 5);
        //            Session.Character.Inventory.RemoveItemAmount(1013, 5);
        //            Session.Character.Inventory.RemoveItemAmount(1027, 5);
        //        }
        //    }
        //}

        public class RaidSeals9 : INpcDialogAsyncHandler
        {
            public long HandledId => 653;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 500000)
                    {
                        Session.Character.GiftAdd(5920, 1);
                        Session.Character.Inventory.RemoveItemAmount(1012, 10);
                        Session.Character.Gold -= 500000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                        return;
                    }
                }
            }
        }

        public class RaidSeals10 : INpcDialogAsyncHandler
        {
            public long HandledId => 654;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 500000)
                    {
                        Session.Character.GiftAdd(5921, 1);
                        Session.Character.Inventory.RemoveItemAmount(1012, 10);
                        Session.Character.Gold -= 500000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                        return;
                    }
                }
            }
        }

        public class RaidSeals11 : INpcDialogAsyncHandler
        {
            public long HandledId => 655;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Inventory.CountItem(1012) >= 10 && Session.Character.Gold >= 500000)
                    {
                        Session.Character.GiftAdd(5922, 1);
                        Session.Character.Inventory.RemoveItemAmount(1012, 10);
                        Session.Character.Gold -= 500000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("THANK_YOU"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 10));
                        return;
                    }
                }
            }
        }

        public class NoeliaDailyQuests : INpcDialogAsyncHandler
        {
            public long HandledId => 720;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Level >= 30) //  Daily Cuby
                    {
                        Session.Character.AddQuest(6160, false);
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                        return;
                    }
                }
            }
        }

        public class NoeliaDailyQuests1 : INpcDialogAsyncHandler
        {
            public long HandledId => 721;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Level >= 30) //  Daily Ginseng
                    {
                        Session.Character.AddQuest(6161, false);
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                        return;
                    }
                }
            }
        }

        public class NoeliaDailyQuests2 : INpcDialogAsyncHandler
        {
            public long HandledId => 722;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Level >= 30) //  Daily Castra
                    {
                        Session.Character.AddQuest(6162, false);
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                        return;
                    }
                }
            }
        }

        public class NoeliaDailyQuests3 : INpcDialogAsyncHandler
        {
            public long HandledId => 723;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Level >= 40) //  Daily Slade
                    {
                        Session.Character.AddQuest(6388, false);
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                        return;
                    }
                }
            }
        }

        public class NoeliaDailyQuests4 : INpcDialogAsyncHandler
        {
            public long HandledId => 724;

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Level >= 40) //  Daily Slade
                    {
                        Session.Character.AddQuest(6389, false);
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
}
