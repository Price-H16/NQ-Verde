using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Autofac;
using ChickenAPI.Core.Logging;
using ChickenAPI.Events;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Exceptions;
using ChickenAPI.Plugins.Modules;
using log4net;
using NosQuest.Plugins.Logging;
using NosTale.Configuration;
using NosTale.Configuration.Helper;
using NosTale.Configuration.Utilities;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._BCards;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._gameEvent;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Handler.PacketHandler.Command;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using Plugins.BasicImplementations.Algorithm;
using Plugins.BasicImplementations.BCards;
using Plugins.BasicImplementations.Event;
using Plugins.BasicImplementations.Guri;
using Plugins.BasicImplementations.ItemUsage;
using Plugins.BasicImplementations.NpcDialog;

namespace OpenNos.World
{
    public static class Program
    {
        #region Delegates

        public delegate bool EventHandler(CtrlType sig);

        #endregion

        #region Enums

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        #endregion

        #region Classes

        public static class NativeMethods
        {
            #region Methods

            [DllImport("Kernel32")]
            internal static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

            #endregion
        }

        #endregion

        #region Members

        private static readonly ManualResetEvent _run = new ManualResetEvent(true);

        private static EventHandler _exitHandler;

        private static int _port;

        #endregion

        #region Methods
        private static void PrintHeader()
        {
            Console.Title = "NosQuest - World";
            const string text = @"

███╗░░██╗░█████╗░░██████╗░██████╗░██╗░░░██╗███████╗░██████╗████████╗
████╗░██║██╔══██╗██╔════╝██╔═══██╗██║░░░██║██╔════╝██╔════╝╚══██╔══╝
██╔██╗██║██║░░██║╚█████╗░██║██╗██║██║░░░██║█████╗░░╚█████╗░░░░██║░░░
██║╚████║██║░░██║░╚═══██╗╚██████╔╝██║░░░██║██╔══╝░░░╚═══██╗░░░██║░░░
██║░╚███║╚█████╔╝██████╔╝░╚═██╔═╝░╚██████╔╝███████╗██████╔╝░░░██║░░░
╚═╝░░╚══╝░╚════╝░╚═════╝░░░░╚═╝░░░░╚═════╝░╚══════╝╚═════╝░░░░╚═╝░░░
";
            string separator = new string('=', Console.WindowWidth);
            string logo = text.Split('\n').Select(s => string.Format("{0," + (Console.WindowWidth / 2 + s.Length / 2) + "}\n", s))
                .Aggregate("", (current, i) => current + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(separator + logo + separator);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void InitializeMasterCommunication()
        {
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            var authKey = a.MasterAuthKey;
            if (CommunicationServiceClient.Instance.Authenticate(authKey))
            {
                Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
            }

        }
        private static void InitializeDatabase()
        {
            if (!DataAccessHelper.Initialize())
            {
                Console.ReadKey();
                return;
            }

        }
        private static void InitializeMaps()
        {
            ServerManager.Instance.Initialize();

        }
        private static void InitializeLogger()
        {
            Logger.InitializeLogger(new SerilogLogger());
        }
        public static void Main(string[] args)
        {
            // initialize Logger
            InitializeLogger();
            var pluginBuilder = new ContainerBuilder();
            IContainer container = pluginBuilder.Build();
            using var coreContainer = BuildCoreContainer();
            var gameBuilder = new ContainerBuilder();
            Logger.InitializeLogger(coreContainer.Resolve<ILogger>());
            gameBuilder.RegisterInstance(coreContainer).As<IContainer>();
            gameBuilder.RegisterModule(new CoreContainerModule(coreContainer));
            //gameBuilder.Register(s => new GamePacketHandlersGamePlugin(coreContainer.Resolve<IPacketHandlerContainer<IGamePacketHandler>>(), coreContainer)).As<IGamePlugin>(); TODO
            //gameBuilder.Register(s => new CharScreenPacketHandlerGamePlugin(coreContainer.Resolve<IPacketHandlerContainer<ICharacterScreenPacketHandler>>(), coreContainer)).As<IGamePlugin>(); TODO
            //gameBuilder.Register(s => new CommandHandler(new SerilogLogger(), coreContainer)).As<ICommandContainer>().SingleInstance(); TODO
            //gameBuilder.Register(s => new CommandGlobalExecutorWrapper(s.Resolve<ICommandContainer>())).As<IGlobalCommandExecutor>().SingleInstance(); TODO
            //gameBuilder.Register(s => new EssentialsPlugin(s.Resolve<ICommandContainer>(), ServerManager.Instance)).As<IGamePlugin>().SingleInstance(); TODO
            gameBuilder.Register(s => new ItemUsagePlugin(coreContainer.Resolve<IItemUsageHandlerContainer>(), coreContainer)).As<IGamePlugin>();
            gameBuilder.Register(s => new BasicEventPipelineAsync()).As<IEventPipeline>().SingleInstance();
            gameBuilder.Register(s => new GenericEventPlugin(s.Resolve<IEventPipeline>(), coreContainer)).As<IGamePlugin>().SingleInstance();
            gameBuilder.Register(s => new NpcDialogPlugin(coreContainer.Resolve<INpcDialogHandlerContainer>(), coreContainer)).As<IGamePlugin>();
            gameBuilder.Register(s => new GuriPlugin(coreContainer.Resolve<IGuriHandlerContainer>(), coreContainer)).As<IGamePlugin>();
            gameBuilder.Register(s => new BCardPlugin(coreContainer.Resolve<IBCardEffectHandlerContainer>(), coreContainer)).As<IGamePlugin>();
            var gameContainer = gameBuilder.Build();
            var plugins = gameContainer.Resolve<IEnumerable<IGamePlugin>>();
            if (plugins != null)
            {
                foreach (var gamePlugin in plugins)
                {
                    gamePlugin.OnEnable();
                }
            }
           
            PrintHeader();
            ConfigurationHelper.CustomisationRegistration();

            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            _port = Convert.ToInt32(a.WorldPort);
            var portArgIndex = Array.FindIndex(args, s => s == "--port");
            if (portArgIndex != -1
                && args.Length >= portArgIndex + 1
                && int.TryParse(args[portArgIndex + 1], out _port))
            {
                Console.WriteLine("Port override: " + _port);
            }

            // initialize api
            InitializeMasterCommunication();

            // initialize DB
            InitializeDatabase();

            EventEntity.InitializeEventPipeline(gameContainer.Resolve<IEventPipeline>());

            // initialilize maps
            InitializeMaps();

            // TODO: initialize ClientLinkManager initialize PacketSerialization
            PacketFactory.Initialize<WalkPacket>();
            
            try
            {
                _exitHandler += ExitHandler;
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
                NativeMethods.SetConsoleCtrlHandler(_exitHandler, true);
            }
            catch (Exception ex)
            {
                Logger.Error("General Error", ex);
            }

            NetworkManager<WorldCryptography> networkManager = null;

            var ipAddress = a.IPAddress;

        portloop:
            try
            {
                networkManager = new NetworkManager<WorldCryptography>(ipAddress, _port, typeof(Act4Handler), typeof(LoginCryptography), true);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    _port++;
                    Logger.Info("Port already in use! Incrementing...");
                    goto portloop;
                }

                Logger.Error("General Error", ex);
                Environment.Exit(ex.ErrorCode);
            }
            var authKey = a.MasterAuthKey;
            ServerManager.Instance.ServerGroup = a.ServerGroupS1;
            const int sessionLimit = 150; // Needs workaround
            var newChannelId = CommunicationServiceClient.Instance.RegisterWorldServer(new SerializableWorldServer(ServerManager.Instance.WorldId, ipAddress, _port, sessionLimit, ServerManager.Instance.ServerGroup));
            if (newChannelId.HasValue)
            {
                ServerManager.Instance.ChannelId = newChannelId.Value;
                MailServiceClient.Instance.Authenticate(authKey, ServerManager.Instance.WorldId);
                ConfigurationServiceClient.Instance.Authenticate(authKey, ServerManager.Instance.WorldId);
                ServerManager.Instance.Configuration = ConfigurationServiceClient.Instance.GetConfigurationObject();
                ServerManager.Instance.SynchronizeSheduling();
            }
            else
            {
                Logger.Error("Could not retrieve ChannelId from Web API.");
                Console.ReadKey();
            }
        }

        private static IContainer BuildCoreContainer()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            //pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf(); TODO
            //pluginBuilder.RegisterType<RedisMultilanguagePlugin>().AsImplementedInterfaces().AsSelf(); TODO
            //pluginBuilder.RegisterType<GamePacketHandlersCorePlugin>().AsImplementedInterfaces().AsSelf(); TODO
            //pluginBuilder.RegisterType<CharScreenPacketHandlerCorePlugin>().AsImplementedInterfaces().AsSelf(); TODO
            pluginBuilder.RegisterType<ItemUsagePluginCore>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<GenericEventPluginCore>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<NpcDialogPluginCore>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<GuriPluginCore>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<AlgorithmPluginCore>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<BCardPluginCore>().AsImplementedInterfaces().AsSelf();
            var container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            foreach (var plugin in container.Resolve<IEnumerable<ICorePlugin>>())
            {
                try
                {
                    plugin.OnLoad(coreBuilder);
                }
                catch (PluginException e)
                {
                }
            }
            

            return coreBuilder.Build();
        }
        private static bool ExitHandler(CtrlType sig)
        {
            LogHelper.Instance.InsertAllLogs();
            CommunicationServiceClient.Instance.UnregisterWorldServer(ServerManager.Instance.WorldId);
            ServerManager.Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 5));
            ServerManager.Instance.SaveAll();
            ServerManager.Instance.DisconnectAll();
            Thread.Sleep(5000);
            Console.ReadLine();
            return false;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // test
            LogHelper.Instance.InsertAllLogs();
            ServerManager.Instance.InShutdown = true;
            Logger.Error((Exception)e.ExceptionObject);
            Logger.Debug("Server crashed! Rebooting gracefully...");
            CommunicationServiceClient.Instance.UnregisterWorldServer(ServerManager.Instance.WorldId);
            ServerManager.Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 5));
            ServerManager.Instance.SaveAll();
            ServerManager.Instance.DisconnectAll();
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;
            Process.Start("OpenNos.World.exe", $"--nomsg --port {(ServerManager.Instance.ChannelId == 51 ? $"{a.Act4Port}" : $"{_port}")}");
            Environment.Exit(1);
        }

        #endregion
    }
}