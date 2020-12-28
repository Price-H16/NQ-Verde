using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Log.Shared;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OpenNos.Log.Server
{
    internal class LogManager
    {
        #region Members

        private static LogManager _instance;

        private readonly LogFileReader _reader;

        #endregion

        #region Instantiation

        public LogManager()
        {
            _reader = new LogFileReader();
            AuthentificatedClients = new List<long>();
            ChatLogs = new ThreadSafeGenericList<LogEntry>();
            AllChatLogs = new ThreadSafeGenericList<LogEntry>();
            PacketLogs = new ThreadSafeGenericList<PacketLogEntry>();
            AllPacketLogs = new ThreadSafeGenericList<PacketLogEntry>();
            recursiveChatFileOpen("chatlogs");
            recursivePacketFileOpen("packetlogs");
            var a = ContainerIoc.GetInstance<JsonGameConfiguration>().Server;
            AuthentificationServiceClient.Instance.Authenticate(a.AuthentificationServiceAuthKey);
            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(observer => SaveChatLogs());
            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(observer => SavePacketLogs());
        }

        #endregion

        #region Properties

        public static LogManager Instance => _instance;

        public ThreadSafeGenericList<LogEntry> AllChatLogs { get; set; }

        public ThreadSafeGenericList<PacketLogEntry> AllPacketLogs { get; set; }

        public List<long> AuthentificatedClients { get; set; }

        public ThreadSafeGenericList<LogEntry> ChatLogs { get; set; }

        public ThreadSafeGenericList<PacketLogEntry> PacketLogs { get; set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            _instance = new LogManager();
        }

        public void SaveChatLogs()
        {
            try
            {
                LogFileWriter writer = new LogFileWriter();
                Logger.Info(Language.Instance.GetMessageFromKey("SAVE_CHATLOGS"));
                List<LogEntry> tmp = ChatLogs.GetAllItems();
                ChatLogs.Clear();
                DateTime current = DateTime.Now;

                string path = "chatlogs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Year.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Month.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Day.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                writer.WriteChatLogFile(Path.Combine(path, $"{(current.Hour < 10 ? $"0{current.Hour}" : $"{current.Hour}")}.{(current.Minute < 10 ? $"0{current.Minute}" : $"{current.Minute}")}.onc"), tmp);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void SavePacketLogs()
        {
            try
            {
                LogFileWriter writer = new LogFileWriter();
                Logger.Info(Language.Instance.GetMessageFromKey("SAVE_PACKETLOGS"));
                List<PacketLogEntry> tmp = PacketLogs.GetAllItems();
                PacketLogs.Clear();
                DateTime current = DateTime.Now;

                string path = "packetlogs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Year.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Month.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Day.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                writer.WritePacketLogFile(Path.Combine(path, $"{(current.Hour < 10 ? $"0{current.Hour}" : $"{current.Hour}")}.{(current.Minute < 10 ? $"0{current.Minute}" : $"{current.Minute}")}.onc"), tmp);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void recursiveChatFileOpen(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (var s in Directory.GetFiles(d).Where(s => s.EndsWith(".onc")))
                    {
                        try
                        {
                            AllChatLogs.AddRange(_reader.ReadChatLogFile(s));
                        }
                        catch (Exception e)
                        {
                            Logger.LogEventError("LogFileRead", $"Something went wrong while opening Chat Log File {s}\n{e}");
                        }
                    }
                    recursiveChatFileOpen(d);
                }
            }
            catch (Exception e)
            {
                Logger.LogEventError("LogFileRead", $"Something went wrong while opening Chat Log Files. Exiting...\n{e}");
                Environment.Exit(-1);
            }
        }

        private void recursivePacketFileOpen(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (var s in Directory.GetFiles(d).Where(s => s.EndsWith(".onc")))
                    {
                        try
                        {
                            AllPacketLogs.AddRange(_reader.ReadPacketLogFile(s));
                        }
                        catch (Exception e)
                        {
                            Logger.LogEventError("LogFileRead", $"Something went wrong while opening Packet Log File {s}\n{e}");
                        }
                    }
                    recursivePacketFileOpen(d);
                }
            }
            catch (Exception e)
            {
                Logger.LogEventError("LogFileRead", $"Something went wrong while opening Packet Log Files. Exiting...\n{e}");
                Environment.Exit(-1);
            }
        }

        #endregion
    }
}