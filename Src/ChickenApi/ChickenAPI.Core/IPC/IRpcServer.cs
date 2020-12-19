// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Threading.Tasks;
using ChickenAPI.Core.IPC.Protocol;

namespace ChickenAPI.Core.IPC
{
    public interface IRpcServer
    {
        Task ResponseAsync<T>(T response, Type requestType) where T : ISyncRpcResponse;
    }
}