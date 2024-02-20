
using System;
using UnityEngine;

namespace Sunflower.Core
{
    public class LogConfig
    {
        //是否打开日志系统
        public bool openLog = true;
        //日志前缀
        public string logHeadFix = "###";
        //是否显示时间
        public bool openTime = true;
        //显示线程id
        public bool showTheadID = true;
        //日志文件存储开关
        public bool logSave = true;
        //显示颜色
        public bool showColorName = true;
        //日志储存路劲

        public string logFileSavePath
        {
            get
            {
                return System.IO.Path.Combine(Application.persistentDataPath, "Logs").Replace("\\", "/");
            }
        }
        //日志文件的名称
        public string logFileName
        {
            get { return Application.productName + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".log"; }
        }
    }
}