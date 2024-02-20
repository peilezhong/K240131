using System;
using System.Reflection;
using CommandLine;
using Sunflower.UI;
using Sunflower.Core;
using Sunflower.Net;
using Sunflower.EventSys;
using Sunflower.Common;
using Sunflower.Res;
using Sunflower.Core.Singleton;

namespace Sunflower.Loader
{
    public class SumFlower: MonoSingletonDontDestroy<SumFlower>
    {
        public void Start()
        {
            string[] args = "".Split(" ");
            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(error => throw new Exception($"命令行格式错误！{error}"))
                .WithParsed(o => SingletonMgr.Instance.AddSingleton(o));
            
            Log.InitLog(new LogConfig
            {
                openLog = true,
                openTime = true,
                showTheadID = true,
                showColorName = true,
                logSave = true,
            });

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            SingletonMgr.Instance.AddSingletonWithAwake<Timer>();
            SingletonMgr.Instance.AddSingletonWithAwake<EventSystem>();
            SingletonMgr.Instance.AddSingletonWithAwake<NetMgr>();
            //SingletonMgr.Instance.AddSingletonWithAwake<ResourceMgr>();
            SingletonMgr.Instance.AddSingletonWithAwake<ResMgr>();
            SingletonMgr.Instance.AddSingletonWithAwake<CodeTpyes, Assembly[]>(assemblies);
            SingletonMgr.Instance.AddSingleton<AddonMgr>();
            
            Type type = CodeTpyes.Instance.GetTypeByAttribute(typeof(AddonAttribute));
            if (type == null)
            {
                Log.Error("Please confirm attribute [EntryAttribute] is in Entry class!!!");
            }
            AAddon addon = Activator.CreateInstance(type) as AAddon;

            SingletonMgr.Instance.AddSingletonWithAwake<UIMgr>();
            addon.Start();
        }

        public void OnApplicationQuit()
        {
            SingletonMgr.Instance.Dispose();
        }
    }
}