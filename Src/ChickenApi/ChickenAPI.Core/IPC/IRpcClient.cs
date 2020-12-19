// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using ChickenAPI.Core.IPC.Protocol;

namespace ChickenAPI.Core.IPC
{
    public interface IRpcClient
    {
        Task<T> RequestAsync<T>(ISyncRpcRequest request) where T : class, ISyncRpcResponse;
        Task BroadcastAsync<T>(T packet) where T : IAsyncRpcRequest;
    }
}