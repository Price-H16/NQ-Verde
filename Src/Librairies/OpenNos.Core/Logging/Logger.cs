using System;
using System.Runtime.CompilerServices;
using ChickenAPI.Core.Logging;
using log4net;

namespace OpenNos.Core
{
    public static class Logger
    {
        #region Properties
        public static ILogger Log { get; set; }
        //public static ILog Log { get; set; }

        #endregion

        #region Methods
        /// <summary>
        ///     Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="innerException"></param>
        public static void Error(Exception innerException = null, [CallerMemberName] string memberName = "")
        {
            if (innerException != null)
            {
                Log?.Error($"{memberName}: {innerException.Message}", innerException);
            }
        }
        /// <summary>
        ///     Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="memberName"></param>
        public static void Debug(string data, [CallerMemberName] string memberName = "")
        {
            Log?.Debug($"[{memberName}]: {data}");
        }

        /// <summary>
        ///     Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public static void Error(string data, Exception ex = null, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
                Log?.Error($"[{memberName}]: {data} {ex.InnerException}", ex);
        }

        /// <summary>
        ///     Wraps up the fatal message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public static void Fatal(string data, Exception ex = null, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
                Log?.Fatal($"[{memberName}]: {data} {ex.InnerException}", ex);
        }

        /// <summary>
        ///     Wraps up the info message with the CallerMemberName
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public static void Info(string message, Exception ex = null, [CallerMemberName] string memberName = "")
        {
            if (ex != null)
                Log?.Info($"[{memberName}]: {message}", ex);
            else
                Log?.InfoFormat($"[{memberName}]: {message}");
        }
        public static void InitializeLogger(ILogger log)
        {
            Log = log;
        }
        //public static void InitializeLogger(ILog log)
        //{
        //    Log = log;
        //}

        /// <summary>
        ///     Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public static void LogEvent(string logEvent, string data, Exception ex = null,
            [CallerMemberName] string memberName = "")
        {
            if (ex != null)
                Log?.InfoFormat($"[{memberName}]: [{logEvent}]{data}");
            else
                Log?.Info($"[{memberName}]: [{logEvent}]{data}", ex);
        }

        /// <summary>
        ///     Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        /// <param name="memberName"></param>
        public static void LogEventError(string logEvent, string data, Exception ex = null,
            [CallerMemberName] string memberName = "")
        {
            if (ex != null)
                Log?.Error($"[{memberName}]: [{logEvent}]{data}", ex);
        }

        /// <summary>
        ///     Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        public static void LogUserEvent(string logEvent, string caller, string data)
        {
            Log?.InfoFormat($"[{logEvent}][{caller}]{data}");
        }

        /// <summary>
        ///     Wraps up the message with the CallerMemberName
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        public static void LogUserEventDebug(string logEvent, string caller, string data)
        {
            Log?.Debug($"[{logEvent}][{caller}]{data}");
        }

        /// <summary>
        ///     Wraps up the error message with the Logging Event
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="caller"></param>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        public static void LogUserEventError(string logEvent, string caller, string data, Exception ex)
        {
            Log?.Error($"[{logEvent}][{caller}]{data}", ex);
        }

        /// <summary>
        ///     Wraps up the warn message with the CallerMemberName
        /// </summary>
        /// <param name="data"></param>
        /// <param name="innerException"></param>
        /// <param name="memberName"></param>
        public static void Warn(string data, Exception innerException = null, [CallerMemberName] string memberName = "")
        {
            if (innerException != null)
                Log?.WarnFormat($"[{memberName}]: {data} {innerException.InnerException}", innerException);
            else
                Log?.Warn($"[{memberName}]: {data}");
        }

        #endregion
    }
}