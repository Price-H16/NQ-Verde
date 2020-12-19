// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace ChickenAPI.Core.IPC.Protocol
{
    public interface ISyncRpcResponse : IAsyncRpcRequest
    {
        Guid RequestId { get; set; }
    }
}