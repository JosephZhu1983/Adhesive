
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Adhesive.Common
{
    public class LocalLoggingService
    {
        private static readonly object locker = new object();
        private static StreamWriter sw;
        private static Timer changePathTimer;
        private static readonly int CHANGEPATHINTERVAL = 60 * 1000;
        private static readonly string LOGFILENAMEFORMAT = "yyyyMMdd_HH_mm";
        private static readonly int MAXLOGFILESIZE = 10 * 1024 * 1024;
        private static string logFileName;
        private static readonly string LOGLINEFORMAT = "HH:mm:ss_ffff";
        private static LogLevel logLevel;

        static LocalLoggingService()
        {
            if (CommonConfiguration.GetConfig().ClearLocalLogWhenStart)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (Directory.Exists(path)) Directory.Delete(path);
            }

            changePathTimer = new Timer(state =>
            {
                if (logFileName != null)
                {
                    FileInfo logFileInfo = new FileInfo(logFileName);
                    if (logFileInfo.Length >= MAXLOGFILESIZE)
                    {
                        lock (locker)
                        {
                            Close();
                            InitStreamWriter();
                        }
                    }
                }
            }, null, CHANGEPATHINTERVAL, CHANGEPATHINTERVAL);
            InitStreamWriter();
        }

        #region internal
        internal static void Close()
        {
            try
            {
                if (sw != null)
                    sw.Close();
            }
            catch
            {
            }
        }
        #endregion

        #region private

        private static void InitStreamWriter()
        {
            logLevel = CommonConfiguration.GetConfig().LocalLoggingServiceLevel;
            try
            {
                logFileName = GetLogFileName();
                sw = new StreamWriter(logFileName, true, Encoding.UTF8, 1024);
                sw.AutoFlush = true;
            }
            catch
            {
            }
        }

        private static string GetLogFileName()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string file = DateTime.Now.ToString(LOGFILENAMEFORMAT) + ".txt";
            return Path.Combine(path, file);
        }

        private static string WrapWithContext(LogLevel logLevel, string s)
        {
            return string.Format("[{0}] @{1} #{2} - {3}",
                logLevel,
                DateTime.Now.ToString(LOGLINEFORMAT),
                Thread.CurrentThread.ManagedThreadId, s);
        }

        #endregion

        #region public

        public static void Debug(string s)
        {
            Log(LogLevel.Debug, s);
        }

        public static void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public static void Info(string s)
        {
            Log(LogLevel.Info, s);
        }

        public static void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public static void Warning(string s)
        {
            Log(LogLevel.Warning, s);
        }

        public static void Warning(string format, params object[] args)
        {
            Log(LogLevel.Warning, format, args);
        }

        public static void Error(string s)
        {
            Log(LogLevel.Error, s);
        }

        public static void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public static void Log(LogLevel level, string format, params object[] args)
        {
            if ((int)logLevel <= (int)level)
                InternalLog(level, string.Format(format, args));
        }

        public static void Log(LogLevel level, string s)
        {
            if ((int)logLevel <= (int)level)
                InternalLog(level, s);
        }

        private static void InternalLog(LogLevel logLevel, string s)
        {
            try
            {

                lock (locker)
                {
                    var message = WrapWithContext(logLevel, s);
#if DEBUG
                    switch (logLevel)
                    {
                        case LogLevel.Debug:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case LogLevel.Info:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case LogLevel.Warning:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case LogLevel.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                    }
                    Console.WriteLine(message);
                    Console.ResetColor();
#endif
                    sw.WriteLine(message);
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}
