// WingsEmu
//
// Developed by NosWings Team

using System;

namespace ChickenAPI.Core.IPC.Protocol
{
    public interface IAsyncRpcRequest
    {
        #region Properties

        Guid Id { get; set; }

        #endregion
    }
}