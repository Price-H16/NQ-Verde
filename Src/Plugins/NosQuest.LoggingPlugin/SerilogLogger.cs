using System;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;
using ILogger = ChickenAPI.Core.Logging.ILogger;

namespace NosQuest.Plugins.Logging
{
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogLogger() => _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"logs/{DateTime.Now:yyyyMMddHHmmss}.log", LogEventLevel.Information, flushToDiskInterval: TimeSpan.FromMinutes(5))
                .CreateLogger();

        public void Debug(string msg)
        {
            _logger.Debug(msg);
        }
        public void DebugHandler(string data, [CallerMemberName] string memberName = "")
        {
            _logger.Debug($"[{memberName}]: {data}");
        }
        public void DebugFormat(string msg, params object[] objs)
        {
            _logger.Debug(msg, objs);
        }

        public void Info(string msg)
        {
            _logger.Information(msg);
        }

        public void InfoFormat(string msg, params object[] objs)
        {
            _logger.Information(msg, objs);
        }

        public void Warn(string msg)
        {
            _logger.Warning(msg);
        }

        public void WarnFormat(string msg, params object[] objs)
        {
            _logger.Warning(msg, objs);
        }

        public void Error(string msg, Exception ex)
        {
            _logger.Error(ex, msg);
        }

        public void ErrorFormat(string msg, Exception ex, params object[] objs)
        {
            _logger.Error(ex, msg, objs);
        }

        public void Fatal(string msg, Exception ex)
        {
            _logger.Fatal(ex, msg);
        }

        public void Info(string msg, Exception ex)
        {
            _logger.Information(ex, msg);
        }
    }
}