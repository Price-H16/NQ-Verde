﻿// WingsEmu
//
// Developed by NosWings Team

using System.Threading.Tasks;

namespace ChickenAPI.Core.IPC.Protocol
{
    public interface ISyncRpcRequest : IAsyncRpcRequest
    {
        #region Methods

        Task ReplyAsync<T, TRequest>(T response) where T : ISyncRpcResponse;

        #endregion
    }
}