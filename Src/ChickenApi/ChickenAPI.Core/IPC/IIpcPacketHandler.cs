// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using ChickenAPI.Core.IPC.Protocol;

namespace ChickenAPI.Core.IPC
{
    public interface IIpcPacketHandler
    {
        Task Handle(IAsyncRpcRequest packet);
    }
}