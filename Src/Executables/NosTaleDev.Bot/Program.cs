//using System;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using DotNetEnv;
//using DSharpPlus;
//using DSharpPlus.CommandsNext;
//using DSharpPlus.CommandsNext.Exceptions;
//using DSharpPlus.Entities;
//using DSharpPlus.EventArgs;
//using log4net;
//using NosTale.Configuration;
//using NosTale.Configuration.Helper;
//using NosTale.Configuration.Utilities;
//using OpenNos.Core;
//using OpenNos.DAL.EF.Helpers;
//using OpenNos.Master.Library.Client;
//using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
//using OpenNos.SCS.Communication.ScsServices.Service;

//namespace NosTale.Bot
//{
//    public class Program
//    {
//        private static bool _isDebug;

//        public static string DefaultPrefix = "++";

//        public static DiscordClient Client { get; set; }

//        public CommandsNextExtension Commands { get; set; }

//        public static void Main(string[] args)
//        {
//            StartApi();
//            var prog = new Program();
//            var write = "DISCORD_TOKEN=\n" +
//                        $"PREFIX={DefaultPrefix}";

//           if (!File.Exists(".env"))
//            {
//                File.WriteAllText(".env", write, new UTF8Encoding(false));
//                Console.WriteLine(
//                    "Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
//                Console.ReadKey();

//                return;
//            }

//            prog.RunBotAsync().GetAwaiter().GetResult();
//        }

//        public async Task RunBotAsync()
//        {
//            Env.Load();

//            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
//           var prefix = Environment.GetEnvironmentVariable("PREFIX");

//            var cfg = new DiscordConfiguration
//            {
//                Token = token,
//                TokenType = TokenType.Bot,
//                AutoReconnect = true,
//                LogLevel = LogLevel.Info,
//                UseInternalLogHandler = true,
//                MessageCacheSize = 65536,
//                DateTimeFormat = "dd/MM/yyyy HH:mm:ss"
//            };

//            Client = new DiscordClient(cfg);

//            Client.Ready += Client_Ready;
//            Client.GuildAvailable += Client_GuildAvailable;
//            Client.ClientErrored += Client_ClientError;

//            var ccfg = new CommandsNextConfiguration
//            {
//                StringPrefixes = new[] {DefaultPrefix},
//                EnableDms = true,
//                EnableMentionPrefix = true,
//                EnableDefaultHelp = false
//            };

//            Commands = Client.UseCommandsNext(ccfg);
            
//            //Commands.CommandExecuted += Commands_CommandExecuted;
//            //Commands.CommandErrored += Commands_CommandErrored;

//            Commands.RegisterCommands(typeof(DiscordBotCommand).GetTypeInfo().Assembly);

//            await Client.ConnectAsync();
//            await Task.Delay(Timeout.Infinite);
//        }

//        private static Task Client_ClientError(ClientErrorEventArgs e)
//        {
//            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotDev",
//                $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
//            return Task.CompletedTask;
//        }

//        private static Task Client_GuildAvailable(GuildCreateEventArgs e)
//        {
//            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotDev",
//                $"Guild available: {e.Guild.MemberCount} Member in {e.Guild.Name}", DateTime.Now);
//            return Task.CompletedTask;
//        }

//        private static Task Client_Ready(ReadyEventArgs e)
//        {
//            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotDev", "Client is ready to process events.",
//                DateTime.Now);
//            e.Client.UpdateStatusAsync(new DiscordActivity("++help [BOT by Price]", ActivityType.Playing));
//            return Task.CompletedTask;
//        }

//        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
//        {
//            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "BotDev",
//                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}",
//                DateTime.Now);
//            if (e.Exception is ChecksFailedException ex)
//            {
//                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

//                var embed = new DiscordEmbedBuilder
//                {
//                    Title = "Access denied",
//                    Description = $"{emoji} You do not have the permissions required to execute this command.",
//                    Color = new DiscordColor(0xFF0000) // red
//                };
//                await e.Context.RespondAsync("", embed: embed);
//            }
//        }

//        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
//        {
//            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "BotDev",
//                $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
//            return Task.CompletedTask;
//        }

//        public static void StartApi()
//        {
//            try
//            {
//#if DEBUG
//                _isDebug = true;
//#endif
//                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
//                Console.Title = $"Discord Bot Server{(_isDebug ? " Development Environment" : string.Empty)}";

//                var ignoreStartupMessages = false;

//                //initialize Logger
//                Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

//                ConfigurationHelper.CustomisationRegistration();
//                var port = Convert.ToInt32("6969");

//                if (!ignoreStartupMessages)
//                {
//                    var assembly = Assembly.GetExecutingAssembly();
//                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
//                    var text = $"DISCORD BOT v{fileVersionInfo.ProductVersion}dev by ZANOU";
//                    var offset = Console.WindowWidth / 2 + text.Length / 2;
//                    var separator = new string('=', Console.WindowWidth);
//                    Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
//                }

//                Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

//                var authKey = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server.MasterAuthKey;
//                if (CommunicationServiceClient.Instance.Authenticate(authKey))
//                {
//                    Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
//                }

//                if (!DataAccessHelper.Initialize())
//                {
//                    Console.ReadKey();
//                    return;
//                }

//                try
//                {
//                    var ipAddress = "89.203.249.171";
//                    var _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(ipAddress, port));
//                    _server.ClientConnected += OnClientConnected;
//                    _server.ClientDisconnected += OnClientDisconnected;

//                    _server.Start();
//                    Logger.Info(Language.Instance.GetMessageFromKey("STARTED"));
//                }
//                catch (Exception ex)
//                {
//                    Logger.Error("General Error Server", ex);
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.Error("General Error", ex);
//                Console.ReadKey();
//            }
//        }

//        private static void OnClientConnected(object sender, ServiceClientEventArgs e)
//        {
//            Logger.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + e.Client.ClientId);
//        }

//        private static void OnClientDisconnected(object sender, ServiceClientEventArgs e)
//        {
//            Logger.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + e.Client.ClientId);
//        }
//    }
//}
