// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace ChickenAPI.Core.IPC.Protocol
{
    public interface IAsyncRpcRequest
    {
        Guid Id { get; set; }
    }
}