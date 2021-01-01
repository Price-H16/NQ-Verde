// WingsEmu
//
// Developed by NosWings Team

using ChickenAPI.Core.IPC.Protocol;
using System;
using System.Threading.Tasks;

namespace ChickenAPI.Core.IPC
{
    public interface IIpcPacketHandlersContainer
    {
        #region Events

        event EventHandler<Type> Registered;

        event EventHandler<Type> Unregistered;

        #endregion

        #region Methods

        Task HandleAsync(IAsyncRpcRequest request, Type type);

        Task RegisterAsync(IIpcPacketHandler handler, Type type);

        Task UnregisterAsync(Type type);

        #endregion
    }
}