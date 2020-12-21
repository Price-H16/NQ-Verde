using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Autofac;
using ChickenAPI.Core.Logging;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Modules;
using NosQuest.Plugins.Logging;
using NosTale.Configuration;
using NosTale.Configuration.Helper;
using NosTale.Configuration.Utilities;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;
using OpenNos.GameObject;
using OpenNos.Handler;
using OpenNos.Handler.PacketHandler.Basic;
using OpenNos.Master.Library.Client;

namespace OpenNos.Login
{
    public static class Program
    {
        #region Members

        private static int _port;

        #endregion

        #region Methods

        private static void InitializeMasterCommunication()
        {
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            if (CommunicationServiceClient.Instance.Authenticate(a.MasterAuthKey))
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
        private static void InitializePacketSerialization()
        {
            PacketFactory.Initialize<WalkPacket>();
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;
            var port = a.LoginPort;
            NetworkManager<LoginCryptography> networkManager = new NetworkManager<LoginCryptography>(a.IPAddress, port, typeof(NoS0575PacketHandler), typeof(LoginCryptography), false);
        }
        private static void InitializeLogger()
        {
            Logger.InitializeLogger(new SerilogLogger());
        }
        private static void InitializePlugins()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
            {
                plugin.OnLoad(coreBuilder);
            }

            using (IContainer coreContainer = coreBuilder.Build())
            {
                var gameBuilder = new ContainerBuilder();
                gameBuilder.RegisterInstance(coreContainer).As<IContainer>();
                gameBuilder.RegisterModule(new CoreContainerModule(coreContainer));
                IContainer gameContainer = gameBuilder.Build();
                IEnumerable<IGamePlugin> plugins = gameContainer.Resolve<IEnumerable<IGamePlugin>>();
                if (plugins != null)
                {
                    foreach (IGamePlugin gamePlugin in plugins)
                    {
                        gamePlugin.OnEnable();
                        gamePlugin.OnDisable();
                    }
                }

                Logger.InitializeLogger(coreContainer.Resolve<ILogger>());
            }
        }
        private static void PrintHeader()
        {
            Console.Title = "NosQuest - Login";
            const string text = @"
███╗   ██╗ ██████╗ ███████╗ ██████╗ ██╗   ██╗███████╗███████╗████████╗    ██╗      ██████╗  ██████╗ ██╗███╗   ██╗
████╗  ██║██╔═══██╗██╔════╝██╔═══██╗██║   ██║██╔════╝██╔════╝╚══██╔══╝    ██║     ██╔═══██╗██╔════╝ ██║████╗  ██║
██╔██╗ ██║██║   ██║███████╗██║   ██║██║   ██║█████╗  ███████╗   ██║       ██║     ██║   ██║██║  ███╗██║██╔██╗ ██║
██║╚██╗██║██║   ██║╚════██║██║▄▄ ██║██║   ██║██╔══╝  ╚════██║   ██║       ██║     ██║   ██║██║   ██║██║██║╚██╗██║
██║ ╚████║╚██████╔╝███████║╚██████╔╝╚██████╔╝███████╗███████║   ██║       ███████╗╚██████╔╝╚██████╔╝██║██║ ╚████║
╚═╝  ╚═══╝ ╚═════╝ ╚══════╝ ╚══▀▀═╝  ╚═════╝ ╚══════╝╚══════╝   ╚═╝       ╚══════╝ ╚═════╝  ╚═════╝ ╚═╝╚═╝  ╚═══╝
";
            string separator = new string('=', Console.WindowWidth);
            string logo = text.Split('\n').Select(s => string.Format("{0," + (Console.WindowWidth / 2 + s.Length / 2) + "}\n", s))
                .Aggregate("", (current, i) => current + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(separator + logo + separator);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Main(string[] args)
        {
            checked
            {
                try
                {
                    PrintHeader();
                    // initialize Logger
                    InitializeLogger();
                    // initialize Plugins
                    InitializePlugins();

                    ConfigurationHelper.CustomisationRegistration();
                    var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

                    var port = a.LoginPort;
                    var portArgIndex = Array.FindIndex(args, s => s == "--port");
                    if (portArgIndex != -1
                        && args.Length >= portArgIndex + 1
                        && int.TryParse(args[portArgIndex + 1], out port))
                    {
                        Console.WriteLine("Port override: " + port);
                    }

                    _port = port;
                    // initialize api
                    InitializeMasterCommunication();

                    // initialize DB
                    InitializeDatabase();

                    Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                    try
                    {
                        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("General Error", ex);
                    }

                    try
                    {
                        InitializePacketSerialization();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogEventError("INITIALIZATION_EXCEPTION", "General Error Server", ex);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogEventError("INITIALIZATION_EXCEPTION", "General Error", ex);
                    Console.ReadKey();
                }
            }
        }
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception) e.ExceptionObject);
            try
            {
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Debug("Login Server crashed! Rebooting gracefully...");
            Process.Start("OpenNos.Login.exe", $"--nomsg --port {_port}");
            Environment.Exit(1);
        }

        #endregion
    }
}