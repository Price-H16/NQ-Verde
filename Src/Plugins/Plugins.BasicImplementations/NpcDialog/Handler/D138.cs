using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D138 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 138;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            var rand = new Random();
            var at = ServerManager.Instance.ArenaTeams.ToList().Where(s => s.Any(c => c.Session?.CurrentMapInstance != null)).OrderBy(s => rand.Next()).FirstOrDefault();
            if (at != null)
            {
                ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, at.FirstOrDefault().Session.CurrentMapInstance.MapInstanceId, 69, 100);

                var zenas = at.OrderBy(s => s.Order).FirstOrDefault(s => s.Session != null && !s.Dead && s.ArenaTeamType == ArenaTeamType.ZENAS);
                var erenia = at.OrderBy(s => s.Order).FirstOrDefault(s => s.Session != null && !s.Dead && s.ArenaTeamType == ArenaTeamType.ERENIA);
                Session.SendPacket(erenia?.Session?.Character?.GenerateTaM(0));
                Session.SendPacket(erenia?.Session?.Character?.GenerateTaM(3));
                Session.SendPacket("taw_sv 0");
                Session.SendPacket(zenas?.Session?.Character?.GenerateTaP(0, true));
                Session.SendPacket(erenia?.Session?.Character?.GenerateTaP(2, true));
                Session.SendPacket(zenas?.Session?.Character?.GenerateTaFc(0));
                Session.SendPacket(erenia?.Session?.Character?.GenerateTaFc(1));
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_TEAM_ARENA")));
            }
        }

        #endregion
    }
}