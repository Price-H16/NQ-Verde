// WingsEmu
//
// Developed by NosWings Team

using ChickenAPI.Core.IPC.Protocol;
using System.Threading.Tasks;

namespace ChickenAPI.Core.IPC
{
    public interface IRpcClient
    {
        #region Methods

        Task BroadcastAsync<T>(T packet) where T : IAsyncRpcRequest;

        Task<T> RequestAsync<T>(ISyncRpcRequest request) where T : class, ISyncRpcResponse;

        #endregion
    }
}