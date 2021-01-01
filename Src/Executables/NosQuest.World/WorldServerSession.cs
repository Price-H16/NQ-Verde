using NetCoreServer;
using NosQuest.Network.Cryptography;
using OpenNos.Core.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NosQuest.World
{
    public class NetworkInformations : INetworkClientInformation
    {
        #region Properties

        public Encoding Encoding => Encoding.Default;

        public int SessionId { get; set; }

        #endregion
    }

    public class WorldServerSession : TcpSession, INetworkSession
    {
        #region Members

        private readonly IDecrypter _decrypter;
        private readonly IEncrypter _encrypter;
        private readonly NetworkInformations _networkClient;
        private IPEndPoint _ip;

        #endregion

        #region Instantiation

        public WorldServerSession(TcpServer server, IEncrypter encrypter, IDecrypter decrypter, NetworkInformations networkClient) : base(server)
        {
            _encrypter = encrypter;
            _decrypter = decrypter;
            _networkClient = networkClient;
        }

        #endregion

        #region Events

        public event EventHandler<string> PacketReceived;

        #endregion

        #region Properties

        public long ClientId { get; }

        public Encoding Encoding => Encoding.Default;

        public IPAddress IpAddress => _ip.Address;

        public bool IsDisposing { get; set; }

        public int SessionId
        {
            get => _networkClient.SessionId;
            set => _networkClient.SessionId = value;
        }

        #endregion

        #region Methods

        public void DisconnectClient()
        {
            Disconnect();
        }

        public void SendPacket(string packet) => Send(_encrypter.Encode(packet).ToArray());

        public async Task SendPacketAsync(string packet)
        {
            await Task.Run(() => SendAsync(_encrypter.Encode(packet).ToArray()));
        }

        public async Task SendPacketAsyncFormat(string packet, params object[] param)
        {
            await SendPacketAsync(string.Format(packet, param));
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            SendPacket(string.Format(packet, param));
        }

        public void SendPackets(IEnumerable<string> packets)
        {
            foreach (var packet in packets)
            {
                SendPacket(packet);
            }
        }

        public async Task SendPacketsAsync(IEnumerable<string> packets)
        {
            foreach (var packet in packets)
            {
                await SendPacketAsync(packet);
            }
        }

        protected override void OnConnected()
        {
            _ip = Socket.RemoteEndPoint as IPEndPoint;
        }

        protected override void OnDisconnected()
        {
        }

        protected override void OnError(SocketError error)
        {
            Disconnect();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string buff = _decrypter.Decode(buffer.AsSpan((int)offset, (int)size));
            PacketReceived?.Invoke(this, buff);
        }

        #endregion
    }
}