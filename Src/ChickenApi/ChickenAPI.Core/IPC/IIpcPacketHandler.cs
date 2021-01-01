// WingsEmu
//
// Developed by NosWings Team

using ChickenAPI.Core.IPC.Protocol;
using System.Threading.Tasks;

namespace ChickenAPI.Core.IPC
{
    public interface IIpcPacketHandler
    {
        #region Methods

        Task Handle(IAsyncRpcRequest packet);

        #endregion
    }
}