using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D3 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 3;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            var heldMonster = ServerManager.GetNpcMonster((short)packet.Type);
            if (heldMonster != null && !Session.Character.Mates.Any(m => m.NpcMonsterVNum == heldMonster.NpcMonsterVNum && !m.IsTemporalMate) && Session.Character.Mates.FirstOrDefault(s => s.NpcMonsterVNum == heldMonster.NpcMonsterVNum && s.IsTemporalMate && s.IsTsReward) is Mate partnerToReceive)
            {
                Session.Character.RemoveTemporalMates();
                var partner = new Mate(Session.Character, heldMonster, heldMonster.Level, MateType.Partner);
                partner.Experience = partnerToReceive.Experience;
                if (!Session.Character.Mates.Any(s => s.MateType == MateType.Partner && s.IsTeamMember))
                {
                    partner.IsTeamMember = true;
                }
                Session.Character.AddPet(partner);
            }
        }

        #endregion
    }
}