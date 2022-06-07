using System;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Common
{
    public class Logger
    {
        public static Logger Current;
        public readonly List<LogData> logs = new List<LogData>();
        public readonly int MaxSentences = 100;
        public event EventHandler<LogData> Added;

        public Logger() { }
        public Logger(int maxSentences) { MaxSentences = maxSentences; }

        public static void Initialize()
        {
            Current = new Logger();
            WriteLine("Started!");
        }

        public static void WriteLine(string message)
        {
            Current?.WriteInternal(LogContext.Info, message);
        }

        public static void Warn(string message)
        {
            Current?.WriteInternal(LogContext.Warning, message);
        }

        public static void Error(string message)
        {
            Current?.WriteInternal(LogContext.Error, message);
        }

        public void Append(string message)
        {
            WriteInternal(LogContext.Info, message);
        }

        private void WriteInternal(LogContext context, string message)
        {
            LogData d = new LogData(context, message);
            logs.Add(d);
            if (logs.Count > MaxSentences)
                logs.RemoveRange(0, logs.Count - MaxSentences);
            Added?.Invoke(this, d);
        }

        public void Clear()
        {
            logs.Clear();
        }
    }

    public enum LogContext
    {
        Info,
        Warning,
        Error
    }

    public class LogData : EventArgs
    {
        public readonly DateTime timestamp;
        public readonly LogContext context;
        public readonly string message;

        public LogData(LogContext context, string message)
        {
            timestamp = DateTime.Now;
            this.context = context;
            this.message = message;
        }
    }
}
