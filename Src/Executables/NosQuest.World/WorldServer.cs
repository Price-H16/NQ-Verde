// NosQuest - Feature
// 
// Developed by NosWings Team
// Reworked by Price for NosQuest

using System;
using System.Net;
using System.Net.Sockets;
using NetCoreServer;
using NosQuest.Network.Cryptography;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace NosQuest.World
{
    public class WorldServer : TcpServer
    {
        private readonly SessionManager _sessionManager;


        public WorldServer(IPAddress address, int port) : base(address, port)
        {
        }

        protected override TcpSession CreateSession()
        {
            var infos = new NetworkInformations();
            var tmp = new WorldServerSession(this, new WorldEncrypter(infos), new WorldDecrypter(infos), infos);
            //_sessionManager.AddSession(tmp); Finish
            return tmp;
        }

        protected override void OnConnected(TcpSession session)
        {

            Logger.Info($"Connected : {(session.Socket.RemoteEndPoint as IPEndPoint).Address}");
        }

        protected override void OnStarted()
        {
            Logger.Info("[TCP-SERVER] Started");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"TCP server caught an error with code {error}");
            ServerManager.Instance.Shutdown();
            Stop();
        }
    }
}