using System;
using System.Runtime.CompilerServices;

namespace ChickenAPI.Core.Logging
{
    public interface ILogger
    {
        void Debug(string msg);
        void DebugFormat(string msg, params object[] objs);

        void DebugHandler(string data, [CallerMemberName] string memberName = "");

        void Info(string msg, Exception ex);
        void InfoFormat(string msg, params object[] objs);

        void Warn(string msg);
        void WarnFormat(string msg, params object[] objs);

        void Error(string msg, Exception ex);
        void ErrorFormat(string msg, Exception ex, params object[] objs);

        void Fatal(string msg, Exception ex);
    }
}