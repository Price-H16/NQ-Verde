using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D5002 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 5002;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            var tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
            if (tp != null)
            {
                //Session.SendPacket("it 3");
                if (ServerManager.Instance.ChannelId == 51)
                {
                    var connection = CommunicationServiceClient.Instance.RetrieveOriginWorld(Session.Account.AccountId);
                    if (string.IsNullOrWhiteSpace(connection))
                    {
                        return;
                    }
                    Session.Character.MapId = tp.MapId;
                    Session.Character.MapX = tp.MapX;
                    Session.Character.MapY = tp.MapY;
                    var port = Convert.ToInt32(connection.Split(':')[1]);
                    Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
                }
            }
        }

        #endregion
    }
}