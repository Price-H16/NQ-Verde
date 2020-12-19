using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    #region Tudotito
    public class Tudotito : INpcDialogAsyncHandler
    {
        public long HandledId => 734;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) //  Howling Monsters
                {
                    Session.Character.AddQuest(7610, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class Tudotito2 : INpcDialogAsyncHandler
    {
        public long HandledId => 735;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Creaky Hinges
                {
                    Session.Character.AddQuest(6719, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Pikidir
    public class Pikidir : INpcDialogAsyncHandler
    {
        public long HandledId => 736;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Belial's Return
                {
                    Session.Character.AddQuest(7627, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Churisgo
    public class Churisgo : INpcDialogAsyncHandler
    {
        public long HandledId => 737;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Needle and Ink (Side)
                {
                    Session.Character.AddQuest(6723, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class ChurisgoDaily1 : INpcDialogAsyncHandler
    {
        public long HandledId => 738;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // No Potion, No Motion
                {
                    Session.Character.AddQuest(7603, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class ChurisgoDaily2 : INpcDialogAsyncHandler
    {
        public long HandledId => 739;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Materials for a new spell
                {
                    Session.Character.AddQuest(7613, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class ChurisgoDaily3 : INpcDialogAsyncHandler
    {
        public long HandledId => 740;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // The Magic Crystal's Creation
                {
                    Session.Character.AddQuest(7630, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Logotor
    public class Logotor : INpcDialogAsyncHandler
    {
        public long HandledId => 741;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Recognition (Side)
                {
                    Session.Character.AddQuest(6709, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Martas
    public class Martas : INpcDialogAsyncHandler
    {
        public long HandledId => 742;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Advanced Runic Carvings (Side)
                {
                    Session.Character.AddQuest(6728, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class Martas2 : INpcDialogAsyncHandler
    {
        public long HandledId => 743;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // The Magic Divine Crystal (Daily)
                {
                    Session.Character.AddQuest(7633, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Ituros
    public class Ituros : INpcDialogAsyncHandler
    {
        public long HandledId => 744;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Regional Delicacies (Daily)
                {
                    Session.Character.AddQuest(7607, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region ChiefTator
    public class ChiefTator : INpcDialogAsyncHandler
    {
        public long HandledId => 745;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Demon Hunt (Daily)
                {
                    Session.Character.AddQuest(7620, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class ChiefTator1 : INpcDialogAsyncHandler
    {
        public long HandledId => 746;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Gathering Informations (Side)
                {
                    Session.Character.AddQuest(7648, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Ligtas
    public class Ligtas : INpcDialogAsyncHandler
    {
        public long HandledId => 747;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Scorpion Poison (Daily)
                {
                    Session.Character.AddQuest(7609, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Cottari
    public class Cottari : INpcDialogAsyncHandler
    {
        public long HandledId => 748;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Roun 2 Against Beast King (Daily)
                {
                    Session.Character.AddQuest(7621, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class Cottari1 : INpcDialogAsyncHandler
    {
        public long HandledId => 749;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Beast Bloat (Daily)
                {
                    Session.Character.AddQuest(7636, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Datitus
    public class Datitus : INpcDialogAsyncHandler
    {
        public long HandledId => 750;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Round 2 against spirit king (Daily)
                {
                    Session.Character.AddQuest(7624, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }

    public class Datitus1 : INpcDialogAsyncHandler
    {
        public long HandledId => 751;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Warath of the Spirits (Daily)
                {
                    Session.Character.AddQuest(7639, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Asgothi
    public class Asgothi : INpcDialogAsyncHandler
    {
        public long HandledId => 752;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Keep the Forest Clean (Daily)
                {
                    Session.Character.AddQuest(7642, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion

    #region Propga
    public class Propga : INpcDialogAsyncHandler
    {
        public long HandledId => 753;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) // Keep the Forest Clean (Daily)
                {
                    Session.Character.AddQuest(7614, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
    #endregion
}
