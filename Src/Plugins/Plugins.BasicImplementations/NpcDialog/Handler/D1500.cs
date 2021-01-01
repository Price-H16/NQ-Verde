using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

public class D500 : INpcDialogAsyncHandler
{
    #region Properties

    public long HandledId => 1500;

    #endregion

    //Fortune-teller

    #region Methods

    public async Task Execute(ClientSession Session, NpcDialogEvent packet)
    {
        var npc = packet.Npc;
        if (npc != null)
        {
            Session.Character.AddQuest(2252, false);
        }
    }

    #endregion
}