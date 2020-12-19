using System.Threading.Tasks;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._NpcDialog.Event;

namespace OpenNos.GameObject._NpcDialog
{
    public interface INpcDialogHandlerContainer
    {
        Task RegisterAsync(INpcDialogAsyncHandler handler);
        Task UnregisterAsync(INpcDialogAsyncHandler handler);

        void Execute(EventEntity player, NpcDialogEvent e);

        Task ExecuteAsync(EventEntity player, NpcDialogEvent e);
    }
}