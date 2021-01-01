// WingsEmu
//
// Developed by NosWings Team

using ChickenAPI.Core.IPC.Protocol;
using System;
using System.Threading.Tasks;

namespace ChickenAPI.Core.IPC
{
    public interface IRpcServer
    {
        #region Methods

        Task ResponseAsync<T>(T response, Type requestType) where T : ISyncRpcResponse;

        #endregion
    }
}