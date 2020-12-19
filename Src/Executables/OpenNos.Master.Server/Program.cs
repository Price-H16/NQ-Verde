using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using ChickenAPI.Core.Logging;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Modules;
using NosQuest.Plugins.Logging;
using NosTale.Configuration;
using NosTale.Configuration.Helper;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Service;


namespace OpenNos.Master.Server
{
    internal static class Program
    {
        #region Members

        private static readonly ManualResetEvent _run = new ManualResetEvent(true);

        private static bool _isDebug;

        #endregion

        #region Methods
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
            Console.Title = "NosQuest - Master";
            const string text = @"                                                                                                                               
 __    __                    ______                               __          
|  \  |  \                  /      \                             |  \         
| ▓▓\ | ▓▓ ______   _______|  ▓▓▓▓▓▓\__    __  ______   _______ _| ▓▓_        
| ▓▓▓\| ▓▓/      \ /       \ ▓▓  | ▓▓  \  |  \/      \ /       \   ▓▓ \       
| ▓▓▓▓\ ▓▓  ▓▓▓▓▓▓\  ▓▓▓▓▓▓▓ ▓▓  | ▓▓ ▓▓  | ▓▓  ▓▓▓▓▓▓\  ▓▓▓▓▓▓▓\▓▓▓▓▓▓       
| ▓▓\▓▓ ▓▓ ▓▓  | ▓▓\▓▓    \| ▓▓ _| ▓▓ ▓▓  | ▓▓ ▓▓    ▓▓\▓▓    \  | ▓▓ __      
| ▓▓ \▓▓▓▓ ▓▓__/ ▓▓_\▓▓▓▓▓▓\ ▓▓/ \ ▓▓ ▓▓__/ ▓▓ ▓▓▓▓▓▓▓▓_\▓▓▓▓▓▓\ | ▓▓|  \     
| ▓▓  \▓▓▓\▓▓    ▓▓       ▓▓\▓▓ ▓▓ ▓▓\▓▓    ▓▓\▓▓     \       ▓▓  \▓▓  ▓▓     
 \▓▓   \▓▓ \▓▓▓▓▓▓ \▓▓▓▓▓▓▓  \▓▓▓▓▓▓\ \▓▓▓▓▓▓  \▓▓▓▓▓▓▓\▓▓▓▓▓▓▓    \▓▓▓▓      
                                 \▓▓▓                                         
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
            try
            {
                PrintHeader();
                // initialize Plugins
                InitializePlugins();
                // initialize Logger
                InitializeLogger();


                ConfigurationHelper.CustomisationRegistration();
                var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();

                var port = a.Server.MasterPort;

                // initialize DB
                if (!DataAccessHelper.Initialize())
                {
                    Console.ReadLine();
                    return;
                }

                Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                try
                {
                    // configure Services and Service Host
                    var ipAddress = a.Server.MasterIP;
                    var _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(ipAddress, port));

                    _server.AddService<ICommunicationService, CommunicationService>(new CommunicationService());
                    _server.AddService<IConfigurationService, ConfigurationService>(new ConfigurationService());
                    _server.AddService<IMailService, MailService>(new MailService());
                    _server.AddService<IMallService, MallService>(new MallService());
                    _server.AddService<IAuthentificationService, AuthentificationService>(new AuthentificationService());
                    _server.ClientConnected += OnClientConnected;
                    _server.ClientDisconnected += OnClientDisconnected;

                    _server.Start();
                    Logger.Info(Language.Instance.GetMessageFromKey("STARTED"));

                }
                catch (Exception ex)
                {
                    Logger.Error("General Error Server", ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("General Error", ex);
                Console.ReadKey();
            }
        }
        private static void OnClientConnected(object sender, ServiceClientEventArgs e)
        {
            Logger.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + e.Client.ClientId);
        }

        private static void OnClientDisconnected(object sender, ServiceClientEventArgs e)
        {
            Logger.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + e.Client.ClientId);
        }

        #endregion
    }
}