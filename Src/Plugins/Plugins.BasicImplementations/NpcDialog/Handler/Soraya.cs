using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;
using ChickenAPI.Enums.Game.Character;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class Soraya : INpcDialogAsyncHandler
    {
        public long HandledId => 340;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 80 && Session.Character.Class == CharacterClassType.MartialArtist) //  Bizarre Energy MA quest
                {
                    Session.Character.AddQuest(6332, false);
                }
                else
                {

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }
    }
}
