#if UNITY_EDITOR
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Installer;
using Sunflower.Editor.YooAsset;
using Sunflower.Helper;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using YooAsset.Editor;

namespace Sunflower.Editor
{
    public class BuildHandler : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IActiveBuildTargetChanged
    {
        private static Dictionary<BuildTarget, string> _buildTarget2Name = new() {
            { BuildTarget.Android, "Test.apk" },
            { BuildTarget.StandaloneWindows64, "Test.exe" }
        };

        public int callbackOrder { get { return 0; } }

        [MenuItem("Sunflower/Setting/BuildSetting")]
        private static void OpenBuildSettingWindow()
        {
            BuildWindow.OpenWindow();
        }

        [MenuItem("Sunflower/Build/Android")]
        public static void BuildAndroid()
        {
            BuildByTarget(BuildTarget.Android);
        }

        [MenuItem("Sunflower/BuildHotUpdate/Android")]
        public static void BuildHotUpdateAndroid()
        {
            BuildByTarget(BuildTarget.Android);
        }

        [MenuItem("Sunflower/Build/Win-64")]
        public static void BuildWin64()
        {
            BuildByTarget(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Sunflower/BuildHotUpdate/Win-64")]
        public static void BuildHotUpdateWin64()
        {
            BuildByTarget(BuildTarget.StandaloneWindows64);
        }

        public static void BuildHotUpdate(BuildTarget target)
        {
            BuildConfig config = BuildConfig.Instance;
            config.Target = target;
            config.TargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            config.SaveConfig();
            //hybridClr处理
            PrebuildCommand.GenerateAll();
            CopyAOTAssembliesToHybridclrBuildinFileCopy();
            CopyHotUpdateAssembliesToHybridclrBuildinFileCopy();
            //把HybridClr生成的dll加到YooAsset的收集器中
            AssetBundleCollectorPackage package = AssetBundleCollectorSettingData.Setting.GetPackage(config.YooAssetPackageName);
            for (int i = 0; i < package.Groups.Count; i++)
            {
                var g = package.Groups[i];
                if (g.GroupName == config.HotDllGroupName)
                {
                    AssetBundleCollectorSettingData.RemoveGroup(package, g);
                }
            }
            AssetBundleCollectorGroup group = AssetBundleCollectorSettingData.CreateGroup(AssetBundleCollectorSettingData.Setting.GetPackage(config.YooAssetPackageName), config.HotDllGroupName);
            group.GroupDesc = "热更Dll";
            AssetBundleCollector collector = new AssetBundleCollector();
            collector.CollectPath = PathHelper.ToRelativeProjectPath(config.hybridclrBuildinFileCopy);
            collector.PackRuleName = nameof(PackDirectory);
            AssetBundleCollectorSettingData.CreateCollector(group, collector);
            AssetBundleCollectorSettingData.SaveFile();
            YooAssetBuildder.BuildInternal();
        }


        //直接构建，时间比较久，发现一个很矛盾的逻辑，hybridclr推荐的优化打包流程与yooasset冲突，hybridclr必须导出工程才能生成必须的dll，yooasset生成包必须要在导包之前，而yooasset流程需要先导出dll，循环依赖，无解
        public static void BuildByTarget(BuildTarget target)
        {
            BuildHotUpdate(target);
            BuildClient();
        }

        public static void CopyAOTAssembliesToHybridclrBuildinFileCopy()
        {
            BuildConfig config = BuildConfig.Instance;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(config.Target);
            string aotAssembliesDstDir = config.hybridclrBuildinFileCopy;
            if (!Directory.Exists(aotAssembliesDstDir))
            {
                Directory.CreateDirectory(aotAssembliesDstDir);
            }
            foreach (var dll in SettingsUtil.AOTAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToHybridclrBuildinFileCopy] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        public static void CopyHotUpdateAssembliesToHybridclrBuildinFileCopy()
        {
            BuildConfig config = BuildConfig.Instance;
            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(config.Target);
            string hotfixAssembliesDstDir = config.hybridclrBuildinFileCopy;
            if(!Directory.Exists(hotfixAssembliesDstDir))
            {
                Directory.CreateDirectory(hotfixAssembliesDstDir);
            }
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToHybridclrBuildinFileCopy] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }

        public static void ExportClientProject()
        {
            //TODO
        }

        private static void BuildClient()
        {
            BuildConfig config = BuildConfig.Instance;
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = config.Scenes;
            options.target = config.Target;
            options.options = config.Options;
            string ouput = PathHelper.Combine(config.OutputPath, config.Target.ToString(), config.AppVersion);
            if (!Directory.Exists(ouput))
            {
                Directory.CreateDirectory(ouput);
            }
            options.locationPathName = PathHelper.Combine(ouput, _buildTarget2Name[config.Target]);
            BuildPipeline.BuildPlayer(options);
        }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            //前置处理
            BuildConfig config = BuildConfig.Instance;
            InstallerController controller = new InstallerController();
            //判断HybridClr是否升级
            if (!controller.HasInstalledHybridCLR())
            {
                controller.InstallDefaultHybridCLR();
            }
            if(EditorUserBuildSettings.activeBuildTarget != config.Target)
            {
                //切换对应的平台
                EditorUserBuildSettings.SwitchActiveBuildTarget(config.TargetGroup, config.Target);
            }
        }

        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            //后置处理
            BuildConfig config = BuildConfig.Instance;
            //设置自动上传构建报告到服务器
            //TODO
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"打包成功! {config.Target} 包已经输出到：{report.summary.outputPath}.");
            }
            else
            {
                Debug.Log(report.summary.result);
                Debug.LogError($"打包失败!");
            }
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            BuildConfig config = BuildConfig.Instance;
            config.OnActiveBuildTargetChanged(newTarget);
        }
    }
}
#endif
