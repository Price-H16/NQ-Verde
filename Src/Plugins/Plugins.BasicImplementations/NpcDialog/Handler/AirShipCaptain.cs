using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class AirShipCaptain : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 307;

        #endregion

        // "Destination"

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 228, 165, 45);
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

    public class AirShipCaptain2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 305;

        #endregion

        // "Destination"

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2527, 35, 35);
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

    public class AirShipCaptainQuest : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 308;

        #endregion

        // "Destination"

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 88)
                {
                    Session.Character.AddQuest(7515, false);
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
}