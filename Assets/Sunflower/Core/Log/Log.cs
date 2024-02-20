using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using Unity.VisualScripting;

namespace Sunflower.Core
{
    public enum LogColor
    {
        None,
        Blue,
        Cyan,
        Darkblue,
        Green,
        Grey,
        Orange,
        Red,
        Yellow,
        Magenta,
    }
    
    public class Log
    {
        private const int TraceLevel = 1;
        private const int DebugLevel = 2;
        private const int InfoLevel = 3;
        private const int WarningLevel = 4;
        private const int ErrorLevel = 5;
        
        private static LogConfig cfg;
        
        //[Conditional("OPEN_LOG")]
        public static void InitLog(LogConfig config = null)
        {
            if (cfg == null)
            {
                cfg = new LogConfig();
            }
            else
            {
                cfg = config;
            }

            if (cfg.logSave)
            {
                if (!Directory.Exists(cfg.logFileSavePath))
                {
                    Directory.CreateDirectory(cfg.logFileSavePath);
                }
                SingletonMgr.Instance.AddSingletonWithAwake<LogHelper, string, string>(cfg.logFileSavePath, cfg.logFileName);
            }
        }

        public static string GenerateLog(string log,LogColor color = LogColor.None)
        {
            StringBuilder stringBuilder = new StringBuilder(cfg.logHeadFix, 100);
            if (cfg.openTime)
            {
                stringBuilder.AppendFormat(" {0}", DateTime.Now.ToString("hh:mm:ss-fff"));
            }

            if (cfg.showTheadID)
            {
                stringBuilder.AppendFormat(" TheadID: {0}", Thread.CurrentThread.ManagedThreadId);
            }

            if (cfg.showColorName)
            {
                stringBuilder.AppendFormat(" {0}", color.ToString());
            }

            stringBuilder.AppendFormat(" {0}", log);
            return stringBuilder.ToString();
        }

        public static string GetUnityColor(string msg, LogColor color)
        {
            if (color == LogColor.None) return msg;
            switch (color)
            {
                case LogColor.Blue:
                    msg = $"<color=#0000FF>{msg}</color>";
                    break;
                case LogColor.Cyan:
                    msg = $"<color=#00FFFF>{msg}</color>";
                    break;
                case LogColor.Darkblue:
                    msg = $"<color=#8FBC8F>{msg}</color>";
                    break;
                case LogColor.Green:
                    msg = $"<color=#00FF00>{msg}</color>";
                    break;
                case LogColor.Orange:
                    msg = $"<color=#FFA500>{msg}</color>";
                    break;
                case LogColor.Red:
                    msg = $"<color=#FF0000>{msg}</color>";
                    break;
                case LogColor.Yellow:
                    msg = $"<color=#FFFF00>{msg}</color>";
                    break;
                case LogColor.Magenta:
                    msg = $"<color=#FF00FF>{msg}</color>";
                    break;
            }

            return msg;
        }

        //[Conditional("OPEN_LOG")]
        public static void ColorLog(string message, LogColor color)
        {
            if (!cfg.openLog)
            {
                return;
            }
            string log = GenerateLog(message, color);
            log = GetUnityColor(log, color);
            UnityEngine.Debug.Log(log);
        }

        //[Conditional("OPEN_LOG")]
        public static void LogGreen(string message)
        {
            ColorLog(message, LogColor.Green);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogYellow(string message)
        {
            ColorLog(message, LogColor.Yellow);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogOrange(string message)
        {
            ColorLog(message, LogColor.Orange);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogBlue(string message)
        {
            ColorLog(message, LogColor.Blue);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogDarkblue(string message)
        {
            ColorLog(message, LogColor.Darkblue);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogCyan(string message)
        {
            ColorLog(message, LogColor.Cyan);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogGrey(string message)
        {
            ColorLog(message, LogColor.Grey);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogRed(string message)
        {
            ColorLog(message, LogColor.Red);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void LogMagenta(string message)
        {
            ColorLog(message, LogColor.Magenta);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void Trace(string message)
        {
            if (!cfg.openLog || Options.Instance.LogLevel > TraceLevel)
            {
                return;
            }
            StackTrace st = new(1, true);
            string log = GenerateLog($"{message}\n{st}");
            UnityEngine.Debug.Log(log);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void Trace(string message, params object[] args)
        {
            if (!cfg.openLog || Options.Instance.LogLevel > TraceLevel)
            {
                return;
            }
            StackTrace st = new(1, true);
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog($"{message}{content}\n{st}");
            UnityEngine.Debug.Log(log);
        }

        //[Conditional("OPEN_LOG")]
        public static void Warning(string message)
        {
            if (Options.Instance.LogLevel > WarningLevel)
            {
                return;
            }
            string log = GenerateLog($"{message}");
            UnityEngine.Debug.LogWarning(log);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void Warning(string message, params object[] args)
        {
            if (Options.Instance.LogLevel > WarningLevel)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog($"{message}{content}");
            UnityEngine.Debug.Log(log);
        }

        //[Conditional("OPEN_LOG")]
        public static void Info(string message)
        {
            if (Options.Instance.LogLevel > InfoLevel)
            {
                return;
            }
            string log = GenerateLog($"{message}");
            UnityEngine.Debug.Log(log);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void Info(string message, params object[] args)
        {
            if (Options.Instance.LogLevel > InfoLevel)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog($"{message}{content}");
            UnityEngine.Debug.Log(log);
        }

        //[Conditional("OPEN_LOG")]
        public static void Debug(string message)
        {
            if (Options.Instance.LogLevel > DebugLevel)
            {
                return;
            }
            string log = GenerateLog($"{message}");
            UnityEngine.Debug.Log(log);
        }
        
        //[Conditional("OPEN_LOG")]
        public static void Debug(string message, params object[] args)
        {
            if (Options.Instance.LogLevel > DebugLevel)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog($"{message}{content}");
            UnityEngine.Debug.Log(log);
        }

        public static void Error(string message)
        {
            if (Options.Instance.LogLevel > ErrorLevel)
            {
                return;
            }
            string log = GenerateLog($"{message}");
            UnityEngine.Debug.LogError(log);
        }
        
        public static void Error(string message, params object[] args)
        {
            if (Options.Instance.LogLevel > ErrorLevel)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog($"{message}{content}");
            UnityEngine.Debug.LogError(log);
        }

        public static void Error(Exception e)
        {
            if (Options.Instance.LogLevel > ErrorLevel)
            {
                return;
            }
            string log = GenerateLog($"{e.ToString()}");
            UnityEngine.Debug.LogError(log);
        }
    }
}