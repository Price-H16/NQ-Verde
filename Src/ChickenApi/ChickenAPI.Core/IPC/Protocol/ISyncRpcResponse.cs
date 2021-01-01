// WingsEmu
//
// Developed by NosWings Team

using System;

namespace ChickenAPI.Core.IPC.Protocol
{
    public interface ISyncRpcResponse : IAsyncRpcRequest
    {
        #region Properties

        Guid RequestId { get; set; }

        #endregion
    }
}