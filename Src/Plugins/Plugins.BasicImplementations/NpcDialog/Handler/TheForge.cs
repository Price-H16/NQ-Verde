using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class TheForge : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 664;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(199, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_1"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge10 : INpcDialogAsyncHandler //martial
    {
        #region Properties

        public long HandledId => 674;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(4728, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_1"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge11 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 675;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(4764, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_2"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge12 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 676;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(4746, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_3"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge13 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 690;

        #endregion

        //rep quest

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 50)
                {
                    Session.Character.AddQuest(6379, false);
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 665;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(947, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_2"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge3 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 667;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(4014, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_3"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge4 : INpcDialogAsyncHandler //archer
    {
        #region Properties

        public long HandledId => 668;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(940, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_1"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion
    }

    public class TheForge5 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 669;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                {
                    Session.Character.GiftAdd(948, 1);
                    Session.Character.Reputation -= 100000;
                    Session.Character.Act4Kill -= 25;
                    Session.SendPacket(Session.Character.GenerateFd());
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_2"), 10));
                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                    return;
                }
            }
        }

        #endregion

        #region Classes

        public class TheForge6 : INpcDialogAsyncHandler
        {
            #region Properties

            public long HandledId => 670;

            #endregion

            #region Methods

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                    {
                        Session.Character.GiftAdd(4017, 1);
                        Session.Character.Reputation -= 100000;
                        Session.Character.Act4Kill -= 25;
                        Session.SendPacket(Session.Character.GenerateFd());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_3"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                        return;
                    }
                }
            }

            #endregion
        }

        public class TheForge7 : INpcDialogAsyncHandler //magicians
        {
            #region Properties

            public long HandledId => 671;

            #endregion

            #region Methods

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                    {
                        Session.Character.GiftAdd(946, 1);
                        Session.Character.Reputation -= 100000;
                        Session.Character.Act4Kill -= 25;
                        Session.SendPacket(Session.Character.GenerateFd());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_1"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                        return;
                    }
                }
            }

            #endregion
        }

        public class TheForge8 : INpcDialogAsyncHandler
        {
            #region Properties

            public long HandledId => 672;

            #endregion

            #region Methods

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                    {
                        Session.Character.GiftAdd(949, 1);
                        Session.Character.Reputation -= 100000;
                        Session.Character.Act4Kill -= 25;
                        Session.SendPacket(Session.Character.GenerateFd());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_2"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                        return;
                    }
                }
            }

            #endregion
        }

        public class TheForge9 : INpcDialogAsyncHandler
        {
            #region Properties

            public long HandledId => 673;

            #endregion

            #region Methods

            public async Task Execute(ClientSession Session, NpcDialogEvent packet)
            {
                var npc = packet.Npc;
                if (npc != null)
                {
                    if (Session.Character.Reputation >= 100000 && Session.Character.Act4Kill >= 25)
                    {
                        Session.Character.GiftAdd(4020, 1);
                        Session.Character.Reputation -= 100000;
                        Session.Character.Act4Kill -= 25;
                        Session.SendPacket(Session.Character.GenerateFd());
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("FORGE_BUY_3"), 10));
                    }
                    else
                    {
                        Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MEET_REQUIREMENTS"), 10));
                        return;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}