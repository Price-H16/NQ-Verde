using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1000 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 1000;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null)
            {
                return;
            }

            if (Session.Character.Quests.Any(s => s.Quest.DialogNpcVNum == npc.NpcVNum && s.Quest.QuestObjectives.Any(o => o.SpecialData == packet.Type)))
            {
                if (ServerManager.Instance.TimeSpaces.FirstOrDefault(s => s.QuestTimeSpaceId == packet.Type) is ScriptedInstance timeSpace)
                {
                    Session.Character.EnterInstance(timeSpace);
                }
            }
        }

        #endregion
    }
}