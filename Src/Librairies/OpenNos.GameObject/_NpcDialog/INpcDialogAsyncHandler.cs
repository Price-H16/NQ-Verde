using System.Threading.Tasks;
using OpenNos.GameObject._NpcDialog.Event;

namespace OpenNos.GameObject._NpcDialog
{
    public interface INpcDialogAsyncHandler
    {
        long HandledId { get; }

        Task Execute(ClientSession player, NpcDialogEvent e);
    }
}