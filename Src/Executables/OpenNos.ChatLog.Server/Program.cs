using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using NosTale.Configuration;
using NosTale.Configuration.Helper;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Log.Networking;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace OpenNos.Log.Server
{
    internal static class Program
    {
        #region Members

        private static readonly ManualResetEvent _run = new ManualResetEvent(true);

        private static bool _isDebug;

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

        #region Methods

        public static void Main(string[] args)
        {
            try
            {
#if DEBUG
                _isDebug = true;
#endif
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                Console.Title = $"OpenNos Log Server{(_isDebug ? " Development Environment" : "")}";

                bool ignoreStartupMessages = false;
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "--nomsg":
                            ignoreStartupMessages = true;
                            break;
                    }
                }

                //initialize Logger
                Logger.InitializeLogger(log4net.LogManager.GetLogger(typeof(Program)));

                ConfigurationHelper.CustomisationRegistration();
                var a = ContainerIoc.GetInstance<JsonGameConfiguration>().Server;

                int port = Convert.ToInt32(a.LogerPort);
                if (!ignoreStartupMessages)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    string text = $"CHAT LOG SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port} by OpenNos Team";
                    int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                    string separator = new string('=', Console.WindowWidth);
                    Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
                }

                Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                try
                {
                    //configure Services and Service Host
                    string ipAddress = a.IPAddress;
                    IScsServiceApplication _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(ipAddress, port));

                    _server.AddService<ILogService, LogService>(new LogService());
                    _server.ClientConnected += OnClientConnected;
                    _server.ClientDisconnected += OnClientDisconnected;

                    _server.Start();
                    LogManager.Initialize();
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

        private static bool ExitHandler(CtrlType sig)
        {
            LogManager.Instance.SaveChatLogs();
            LogManager.Instance.SavePacketLogs();
            Thread.Sleep(5000);
            return false;
        }

        private static void OnClientConnected(object sender, ServiceClientEventArgs e) => Logger.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + e.Client.ClientId);

        private static void OnClientDisconnected(object sender, ServiceClientEventArgs e) => Logger.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + e.Client.ClientId);

        #endregion
    }
}