using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Sunflower.Core
{
    public class LogData
    {
        public string log;
        public string trace;
        public LogType type;
    }
    
    public class LogHelper: Singleton<LogHelper>, ISingletonAwake<string, string>
    {
        //写入文件流
        private StreamWriter streamWriter;
        //日志数据队列
        private readonly ConcurrentQueue<LogData> conCurrentQueue = new ConcurrentQueue<LogData>();
        //工作信号事件
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        private bool theadRunning = false;
        
        private string nowTime
        {
            get { return DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"); }
        }

        public void InitLogFileModule(string savePath, string logfineName)
        {
            string logFilePath = Path.Combine(savePath, logfineName);
            Log.Debug("logFilePath: " + logFilePath);
            streamWriter = new StreamWriter(logFilePath);
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
            Thread fileThead = new Thread(FileLogThread);
            theadRunning = true;
            fileThead.Start();
        }

        public void FileLogThread()
        {
            while (theadRunning)
            {
                manualResetEvent.WaitOne();
                if (streamWriter == null)
                {
                    break;
                }

                LogData data;
                while (conCurrentQueue.Count>0 && conCurrentQueue.TryDequeue(out data))
                {
                    if (data.type == LogType.Log)
                    {
                        streamWriter.Write("Log >>> ");
                        streamWriter.WriteLine(data.log);
                        streamWriter.WriteLine(data.trace);
                    }else if (data.type == LogType.Warning)
                    {
                        streamWriter.Write("LogWrning >>> ");
                        streamWriter.WriteLine(data.log);
                        streamWriter.WriteLine(data.trace);
                    }else if (data.type == LogType.Error)
                    {
                        streamWriter.Write("LogWrning >>> ");
                        streamWriter.WriteLine(data.log);
                        streamWriter.Write("\n");
                        streamWriter.WriteLine(data.trace);
                    }
                    streamWriter.Write("\r\n");
                }
                streamWriter.Flush();
                manualResetEvent.Reset();
                Thread.Sleep(1);
            }
        }
        
        public void Awake(string savePath, string fileName)
        {
            InitLogFileModule(savePath, fileName);
        }

        public override void Dispose()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
            theadRunning = false;
            manualResetEvent.Set();
            streamWriter.Close();
            streamWriter = null;
        }

        private void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            conCurrentQueue.Enqueue(new LogData{log=nowTime + " " + condition, trace=stackTrace, type=type});
            manualResetEvent.Set();
        }
    }
}