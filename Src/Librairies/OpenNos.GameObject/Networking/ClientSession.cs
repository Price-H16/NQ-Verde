using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.GameObject
{
    public class ClientSession
    {
        #region Instantiation

        public ClientSession(INetworkClient client)
        {
            // set the time of last received packet
            _lastPacketReceive = DateTime.Now.Ticks;

            // lag mode
            _random = new Random((int)client.ClientId);

            // initialize network client
            _client = client;

            // absolutely new instantiated Client has no SessionId
            SessionId = 0;

            // register for NetworkClient events
            _client.MessageReceived += OnNetworkClientMessageReceived;

            // start observer for receiving packets
            _receiveQueue = new ConcurrentQueue<byte[]>();
            _receiveQueueObservable = Observable.Interval(new TimeSpan(0, 0, 0, 0, 10)).Subscribe(x => HandlePackets());
        }

        #endregion

        #region Members

        public bool HealthStop;

        private static CryptographyBase _encryptor;

        private readonly INetworkClient _client;

        private readonly Random _random;

        private readonly ConcurrentQueue<byte[]> _receiveQueue;

        private readonly object _receiveQueueObservable;

        private readonly IList<string> _waitForPacketList = new List<string>();

        private Character _character;

        private IDictionary<string[], HandlerMethodReference> _handlerMethods;

        private int _lastPacketId;

        // private byte countPacketReceived;

        private long _lastPacketReceive;

        private int? _waitForPacketsAmount;

        #endregion

        #region Properties

        public static ThreadSafeGenericLockedList<string> UserLog { get; set; } = new ThreadSafeGenericLockedList<string>();


        public Account Account { get; private set; }

        public string ParsedAddress
        {
            get
            {
                string[] split = IpAddress.Split(':');
                return split[1].Substring(2);
            }
        }

        public Character Character
        {
            get
            {
                if (_character == null || !HasSelectedCharacter)
                    // cant access an
                    Logger.Warn("An uninitialized character should not be accessed.");

                return _character;
            }
            private set => _character = value;
        }

        public string CleanIpAddress
        {
            get
            {
                var cleanIp = _client.IpAddress.Replace("tcp://", "");
                return cleanIp.Substring(0, cleanIp.LastIndexOf(":") > 0 ? cleanIp.LastIndexOf(":") : cleanIp.Length);
            }
            set { }
        }

        public long ClientId => _client.ClientId;

        public MapInstance CurrentMapInstance { get; set; }

        public IDictionary<string[], HandlerMethodReference> HandlerMethods
        {
            get => _handlerMethods ?? (_handlerMethods = new Dictionary<string[], HandlerMethodReference>());
            private set => _handlerMethods = value;
        }

        public bool HasCurrentMapInstance => CurrentMapInstance != null;

        public bool HasSelectedCharacter { get; private set; }

        public bool HasSession => _client != null;

        public string IpAddress => _client.IpAddress;

        public bool IsAuthenticated { get; private set; }

        public bool IsConnected => _client.IsConnected;

        public bool IsDisposing
        {
            get => _client.IsDisposing;
            internal set => _client.IsDisposing = value;
        }

        public bool IsLocalhost => IpAddress.Contains("127.0.0.1");

        public bool IsOnMap => CurrentMapInstance != null;

        public DateTime RegisterTime { get; internal set; }

        public int SessionId { get; private set; }

        #endregion

        #region Methods

        public void ClearLowPriorityQueue()
        {
            _client.ClearLowPriorityQueueAsync();
        }

        public void Destroy()
        {
            // unregister from WCF events
            CommunicationServiceClient.Instance.CharacterConnectedEvent -= OnOtherCharacterConnected;
            CommunicationServiceClient.Instance.CharacterDisconnectedEvent -= OnOtherCharacterDisconnected;

            // do everything necessary before removing client, DB save, Whatever
            if (HasSelectedCharacter)
            {
                if (Character?.MapInstance?.MapInstanceType == MapInstanceType.RainbowBattleInstance)
                {
                    CharacterDTO characterToMute = DAOFactory.CharacterDAO.LoadByName(Character?.Name);
                    if (Character?.IsMuted() == false)
                    {
                        Character?.Session?.SendPacket(UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_PLURAL"), "RBB DISCONNECT", "120")));
                    }

                    PenaltyLogDTO log = new PenaltyLogDTO
                    {
                        AccountId = characterToMute.AccountId,
                        Reason = "RBB DISCONNECT",
                        Penalty = PenaltyType.Muted,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(120),
                        AdminName = "SYSTEM"
                    };
                    Character.InsertOrUpdatePenalty(log);
                    Character?.Session?.SendPacket(Character?.Session?.Character?.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                Logger.LogUserEvent("CHARACTER_LOGOUT", GenerateIdentity(), "");
                Character.Dispose();

                if (Character.MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance || Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    Character.MapInstance.InstanceBag.DeadList.Add(Character.CharacterId);
                    if (Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        Character?.Group?.Sessions.ToList().ForEach(s =>
                        {
                            if (s != null)
                            {
                                s.SendPacket(s.Character.Group.GeneraterRaidmbf(s));
                                s.SendPacket(s.Character.Group.GenerateRdlst());
                            }
                        });
                    }
                }

                if (Character?.Miniland != null)
                {
                    ServerManager.RemoveMapInstance(Character.Miniland.MapInstanceId);
                }

                // disconnect client
                CommunicationServiceClient.Instance.DisconnectCharacter(ServerManager.Instance.WorldId, Character.CharacterId);

                // unregister from map if registered
                if (CurrentMapInstance != null)
                {
                    CurrentMapInstance.UnregisterSession(Character.CharacterId);
                    CurrentMapInstance = null;
                    ServerManager.Instance.UnregisterSession(Character.CharacterId);
                }
            }

            if (Account != null)
            {
                CommunicationServiceClient.Instance.DisconnectAccount(Account.AccountId);
            }

            ClearReceiveQueue();
        }

        public void Disconnect()
        {
            if (Character?.MapInstance?.MapInstanceType == MapInstanceType.RainbowBattleInstance)
            {
                CharacterDTO characterToMute = DAOFactory.CharacterDAO.LoadByName(Character?.Name);
                if (Character?.IsMuted() == false)
                {
                    Character?.Session?.SendPacket(UserInterfaceHelper.GenerateInfo(
                        string.Format(Language.Instance.GetMessageFromKey("MUTED_PLURAL"), "RBB DISCONNECT", "120")));
                }

                PenaltyLogDTO log = new PenaltyLogDTO
                {
                    AccountId = characterToMute.AccountId,
                    Reason = "RBB DISCONNECT",
                    Penalty = PenaltyType.Muted,
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now.AddMinutes(120),
                    AdminName = "SYSTEM"
                };
                Character.InsertOrUpdatePenalty(log);
                Character?.Session?.SendPacket(Character?.Session?.Character?.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            _client.Disconnect();
        }

        public string GenerateIdentity()
        {
            if (Character != null) return $"Character: {Character.Name}";
            return $"Account: {Account.Name}";
        }

        public void Initialize(CryptographyBase encryptor, Type packetHandler, bool isWorldServer)
        {
            _encryptor = encryptor;
            _client.Initialize(encryptor);

            // dynamically create packethandler references
            GenerateHandlerReferences(packetHandler, isWorldServer);
        }

        public void InitializeAccount(Account account, bool crossServer = false)
        {
            Account = account;
            if (crossServer)
            {
                CommunicationServiceClient.Instance.ConnectAccountCrossServer(ServerManager.Instance.WorldId, account.AccountId, SessionId);
            }
            else
            {
                CommunicationServiceClient.Instance.ConnectAccount(ServerManager.Instance.WorldId, account.AccountId, SessionId);
            }
            IsAuthenticated = true;
        }

        public void ReceivePacket(string packet, bool ignoreAuthority = false)
        {
            var header = packet.Split(' ')[0];
            TriggerHandler(header, $"{_lastPacketId} {packet}", false, ignoreAuthority);
            _lastPacketId++;
        }

        public void SendPacket(string packet, byte priority = 10)
        {
            if (!IsDisposing)
            {
                _client.SendPacket(packet, priority);
            }
        }

        public void SendPacket(PacketDefinition packet, byte priority = 10)
        {
            if (!IsDisposing) _client.SendPacket(PacketFactory.Serialize(packet), priority);
        }

        public void SendPacketAfter(string packet, int milliseconds)
        {
            if (!IsDisposing)
                Observable.Timer(TimeSpan.FromMilliseconds(milliseconds)).Subscribe(o => SendPacket(packet));
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            if (!IsDisposing) _client.SendPacketFormat(packet, param);
        }

        public void SendPackets(IEnumerable<string> packets, byte priority = 10)
        {
            if (!IsDisposing)
            {
                _client.SendPackets(packets, priority);
            }
        }

        public void SendPackets(IEnumerable<PacketDefinition> packets, byte priority = 10)
        {
            if (!IsDisposing) packets.ToList().ForEach(s => _client.SendPacket(PacketFactory.Serialize(s), priority));
        }

        public void SetCharacter(Character character)
        {
            Character = character;
            HasSelectedCharacter = true;

            Logger.LogUserEvent("CHARACTER_LOGIN", GenerateIdentity(), "");

            // register CSC events
            CommunicationServiceClient.Instance.CharacterConnectedEvent += OnOtherCharacterConnected;
            CommunicationServiceClient.Instance.CharacterDisconnectedEvent += OnOtherCharacterDisconnected;

            // register for servermanager
            ServerManager.Instance.RegisterSession(this);
            ServerManager.Instance.CharacterScreenSessions.Remove(character.AccountId);
            Character.SetSession(this);
        }

        private void ClearReceiveQueue()
        {
            while (_receiveQueue.TryDequeue(out var outPacket))
            {
                // do nothing
            }
        }

        private void GenerateHandlerReferences(Type type, bool isWorldServer)
        {
            IEnumerable<Type> handlerTypes = !isWorldServer ? type.Assembly.GetTypes().Where(t => t.Name.Equals("NoS0575PacketHandler")) // shitty but it works == hardocde
                                                            : type.Assembly.GetTypes().Where(p =>
                                                            {
                                                                Type interfaceType = type.GetInterfaces().FirstOrDefault();
                                                                return interfaceType != null && !p.IsInterface && interfaceType.IsAssignableFrom(p);
                                                            });

            // iterate thru each type in the given assembly
            foreach (var handlerType in handlerTypes)
            {
                var handler = (IPacketHandler)Activator.CreateInstance(handlerType, this);

                // include PacketDefinition
                foreach (var methodInfo in handlerType.GetMethods().Where(x =>
                    x.GetCustomAttributes(false).OfType<PacketAttribute>().Any() ||
                    x.GetParameters().FirstOrDefault()?.ParameterType.BaseType == typeof(PacketDefinition)))
                {
                    var packetAttributes = methodInfo.GetCustomAttributes(false).OfType<PacketAttribute>().ToList();

                    // assume PacketDefinition based handler method
                    if (packetAttributes.Count == 0)
                    {
                        var methodReference = new HandlerMethodReference(
                            DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler,
                            methodInfo.GetParameters().FirstOrDefault()?.ParameterType);
                        HandlerMethods.Add(methodReference.Identification, methodReference);
                    }
                    else
                    {
                        // assume string based handler method
                        foreach (var packetAttribute in packetAttributes)
                        {
                            var methodReference = new HandlerMethodReference(
                                DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler,
                                packetAttribute);
                            HandlerMethods.Add(methodReference.Identification, methodReference);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handle the packet received by the Client.
        /// </summary>
        private void HandlePackets()
        {
            try
            {
                while (_receiveQueue.TryDequeue(out var packetData))
                {
                    // determine first packet
                    if (_encryptor.HasCustomParameter && SessionId == 0)
                    {
                        var sessionPacket = _encryptor.DecryptCustomParameter(packetData);

                        var sessionParts = sessionPacket.Split(' ');

                        if (sessionParts.Length == 0) return;

                        if (!int.TryParse(sessionParts[0], out var packetId)) Disconnect();

                        _lastPacketId = packetId;

                        // set the SessionId if Session Packet arrives
                        if (sessionParts.Length < 2) return;

                        if (int.TryParse(sessionParts[1].Split('\\').FirstOrDefault(), out var sessid))
                        {
                            SessionId = sessid;
                            Logger.Debug(
                                string.Format(Language.Instance.GetMessageFromKey("CLIENT_ARRIVED"), SessionId));

                            if (!_waitForPacketsAmount.HasValue)
                                TriggerHandler("OpenNos.EntryPoint", string.Empty, false);
                        }

                        return;
                    }

                    // Decrypts the packet at the beginning
                    var packetConcatenated = _encryptor.Decrypt(packetData, SessionId);

                    foreach (var packet in packetConcatenated.Split(new[] { (char)0xFF },
                        StringSplitOptions.RemoveEmptyEntries))
                    {
                        // FIxes the packet string
                        var packetstring = packet.Replace('^', ' ');
                        var packetsplit = packetstring.Split(' ');

                        if (_encryptor.HasCustomParameter)
                        {
                            var nextRawPacketId = packetsplit[0];

                            if (!int.TryParse(nextRawPacketId, out var nextPacketId) &&
                                nextPacketId != _lastPacketId + 1)
                            {
                                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("CORRUPTED_KEEPALIVE"),
                                    _client.ClientId));
                                _client.Disconnect();
                                return;
                            }

                            if (nextPacketId == 0)
                            {
                                if (_lastPacketId == ushort.MaxValue) _lastPacketId = nextPacketId;
                            }
                            else
                            {
                                _lastPacketId = nextPacketId;
                            }

                            if (_waitForPacketsAmount.HasValue)
                            {
                                _waitForPacketList.Add(packetstring);

                                var packetssplit = packetstring.Split(' ');

                                if (packetssplit.Length > 3 && packetsplit[1] == "DAC")
                                    _waitForPacketList.Add("0 CrossServerAuthenticate");

                                if (_waitForPacketList.Count == _waitForPacketsAmount)
                                {
                                    _waitForPacketsAmount = null;
                                    var queuedPackets = string.Join(" ", _waitForPacketList.ToArray());
                                    var header = queuedPackets.Split(' ', '^')[1];
                                    TriggerHandler(header, queuedPackets, true);
                                    _waitForPacketList.Clear();
                                    return;
                                }
                            }
                            else if (packetsplit.Length > 1)
                            {
                                if (packetsplit[1].Length >= 1 &&
                                    (packetsplit[1][0] == '/' || packetsplit[1][0] == ':' || packetsplit[1][0] == ';'))
                                {
                                    packetsplit[1] = packetsplit[1][0].ToString();
                                    packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                                }

                                if (packetsplit[1] != "0")
                                    TriggerHandler(packetsplit[1].Replace("#", ""), packetstring, false);
                            }
                        }
                        else
                        {
                            var packetHeader = packetstring.Split(' ')[0];

                            // simple messaging
                            if (packetHeader[0] == '/' || packetHeader[0] == ':' || packetHeader[0] == ';')
                            {
                                packetHeader = packetHeader[0].ToString();
                                packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                            }

                            TriggerHandler(packetHeader.Replace("#", ""), packetstring, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Invalid packet (Crash Exploit)", ex);
                Disconnect();
            }
        }

        /// <summary>
        ///     This will be triggered when the underlying NetworkClient receives a packet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNetworkClientMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message as ScsRawDataMessage;
            if (message == null) return;
            if (message.MessageData.Length > 0 && message.MessageData.Length > 2)
                _receiveQueue.Enqueue(message.MessageData);
            _lastPacketReceive = e.ReceivedTimestamp.Ticks;
        }

        private void OnOtherCharacterConnected(object sender, EventArgs e)
        {
            if (Character?.IsDisposed != false) return;

            var loggedInCharacter = (Tuple<long, string>)sender;

            if (Character.IsFriendOfCharacter(loggedInCharacter.Item1) && Character != null &&
                Character.CharacterId != loggedInCharacter.Item1)
            {
                _client.SendPacket(Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_IN"), loggedInCharacter.Item2),
                    10));
                _client.SendPacket(Character.GenerateFinfo(loggedInCharacter.Item1, true));
            }

            var chara = Character.Family?.FamilyCharacters.Find(s => s.CharacterId == loggedInCharacter.Item1);

            if (chara != null && loggedInCharacter.Item1 != Character?.CharacterId)
                _client.SendPacket(Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("CHARACTER_FAMILY_LOGGED_IN"),
                        loggedInCharacter.Item2,
                        Language.Instance.GetMessageFromKey(chara.Authority.ToString().ToUpper())), 10));
        }

        private void OnOtherCharacterDisconnected(object sender, EventArgs e)
        {
            if (Character?.IsDisposed != false) return;

            var loggedOutCharacter = (Tuple<long, string>)sender;

            if (Character.IsFriendOfCharacter(loggedOutCharacter.Item1) && Character != null &&
                Character.CharacterId != loggedOutCharacter.Item1)
            {
                _client.SendPacket(Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_OUT"),
                        loggedOutCharacter.Item2), 10));
                _client.SendPacket(Character.GenerateFinfo(loggedOutCharacter.Item1, false));
            }
        }

        private void TriggerHandler(string packetHeader, string packet, bool force, bool ignoreAuthority = false)
        {
            if (ServerManager.Instance.InShutdown || string.IsNullOrWhiteSpace(packetHeader))
            {
                return;
            }

            if (!IsDisposing)
            {
                //if (Account?.Name != null && UserLog.Contains(Account.Name))
                //    try
                //    {
                //        File.AppendAllText($"C:\\Accounts\\{Account.Name.Replace(" ", "")}.txt", packet + "\n");
                //    }
                //    catch (Exception ex)
                //    {
                //        Logger.Error(ex);
                //    }


                // Fast made code. Will improve it in a custom logger soon.
                //try
                //{
                //    File.AppendAllText($"C:\\Packetlogging\\Ch{ServerManager.Instance.ChannelId}Packets.txt", $"[{DateTime.Now.ToLongDateString()}] [{Account?.Name ?? "Unspecified"}] - {packet} \n");
                //}
                //catch (Exception ex)
                //{
                //    Logger.Error(ex);
                //}

                var key = HandlerMethods.Keys.FirstOrDefault(s =>
                    s.Any(m => string.Equals(m, packetHeader, StringComparison.CurrentCultureIgnoreCase)));
                var methodReference = key != null ? HandlerMethods[key] : null;

                if (methodReference != null)
                {
                    if (!force && methodReference.Amount > 1 && !_waitForPacketsAmount.HasValue)
                    {
                        // we need to wait for more
                        _waitForPacketsAmount = methodReference.Amount;
                        _waitForPacketList.Add(packet != string.Empty ? packet : $"1 {packetHeader} ");
                        return;
                    }
                    try
                    {
                        if (HasSelectedCharacter || methodReference.IsCharScreen)
                        {
                            // call actual handler method
                            if (methodReference.PacketDefinitionParameterType != null)
                            {
                                //check for the correct authority
                                if (!IsAuthenticated
                                    || Account.Authority.Equals(AuthorityType.Administrator)
                                    || methodReference.Authorities.Any(a => a.Equals(Account.Authority))
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.User)) &&
                                    Account.Authority >= AuthorityType.User
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.DSGM)) &&
                                    Account.Authority >= AuthorityType.DSGM && Account.Authority <= AuthorityType.BA
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.MOD)) &&
                                    Account.Authority >= AuthorityType.MOD && Account.Authority <= AuthorityType.BA
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.SMOD)) &&
                                    Account.Authority >= AuthorityType.SMOD && Account.Authority <= AuthorityType.BA
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.TGM)) &&
                                    Account.Authority >= AuthorityType.TGM
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.GM)) &&
                                    Account.Authority >= AuthorityType.GM
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.SGM)) &&
                                    Account.Authority >= AuthorityType.SGM
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.GA)) &&
                                    Account.Authority >= AuthorityType.GA
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.TM)) &&
                                    Account.Authority >= AuthorityType.TM
                                    || methodReference.Authorities.Any(a => a.Equals(AuthorityType.CM)) &&
                                    Account.Authority >= AuthorityType.CM
                                    || ignoreAuthority)
                                {
                                    var deserializedPacket = PacketFactory.Deserialize(packet,
                                        methodReference.PacketDefinitionParameterType, IsAuthenticated);
                                    if (deserializedPacket != null || methodReference.PassNonParseablePacket)
                                    {
                                        methodReference.HandlerMethod(methodReference.ParentHandler, deserializedPacket);
                                    }
                                    else
                                    {
                                        Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("CORRUPT_PACKET"), packetHeader, packet));
                                    }
                                }
                            }
                            else
                            {
                                methodReference.HandlerMethod(methodReference.ParentHandler, packet);
                            }
                        }
                    }
                    catch (DivideByZeroException ex)
                    {
                        // disconnect if something unexpected happens
                        Logger.Error("Handler Error SessionId: " + SessionId, ex);
                        //Disconnect();
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Handler Error SessionId: " + SessionId, e);
                        //   Disconnect();
                    }
                }
                else
                {
                    // Bot
                    if (packetHeader.ToLower() == "$commander")
                    {
                        Disconnect();
                    }

                    Logger.Warn(
                        $"{string.Format(Language.Instance.GetMessageFromKey("HANDLER_NOT_FOUND"), packetHeader)} From IP: {_client.IpAddress} and content: {packet}");
                }
            }
            else
            {
                Logger.Warn(string.Format(
                    Language.Instance.GetMessageFromKey("CLIENTSESSION_DISPOSING"),
                    packetHeader));
            }
        }

        // wtf is that ???
        //public int GetPacketloggingNumber(int number, int channel)
        //{
        //    string[] lines = File.ReadAllLines($"C:\\Packetlogging\\Packets{number}.txt");

        //    if (lines.Count() < 10000)
        //        return number;
        //    else
        //    {
        //        return GetPacketloggingNumber(number++, channel);
        //    }
        //}

        #endregion
    }
}