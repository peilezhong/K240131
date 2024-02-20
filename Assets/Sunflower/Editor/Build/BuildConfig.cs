#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sunflower.Helper;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;
using UnityEditor.Build.Player;
using UnityEditorInternal;
using HybridCLR.Editor.Settings;

namespace Sunflower.Editor
{
    [Core.Singleton.FilePath("Assets/Resources/BuildConfig.asset")]
    public class BuildConfig : Core.Singleton.ScriptableSingleton<BuildConfig>
    {
        #region player
        [Required]
        [TabGroup("Build")]
        [Header("构建场景")]
        [Sirenix.OdinInspector.FilePath(ParentFolder = "Client/")]
        public string[] Scenes;

        [Required]
        [TabGroup("Build")]
        [Header("构建平台")]
        public BuildTarget Target;

        [Required]
        [ReadOnly]
        [TabGroup("Build")]
        [Header("构建平台组")]
        public BuildTargetGroup TargetGroup;

        [Required]
        [TabGroup("Build")]
        [ReadOnly]
        [Header("构建输出路径")]
        public string OutputPath = PathHelper.CombineWithClientRoot("BuildOutput");

        [Required]
        [TabGroup("Build")]
        public ScriptCompilationOptions ScriptCompilationOptions;

        [Required]
        [TabGroup("Build")]
        [Header("构建版本")]
        public string AppVersion;

        [Required]
        [TabGroup("Build")]
        public BuildOptions Options;
        #endregion

        #region YooAsset
        [Required]
        [ReadOnly]
        [TabGroup("YooAsset")]
        [Header("YooAsset输出路径")]
        public string YooAssetBuildOutputRoot = PathHelper.CombineWithClientRoot("Bundles");

        [Required]
        [ReadOnly]
        [TabGroup("YooAsset")]
        [Header("YooAsset内建资源输出路径")]
        public string YooAssetBuildinFileRoot = PathHelper.CombineWithApplicationStreamingAssetsPath("Bundles");

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset构建管线")]
        public EBuildPipeline YooAssetBuildPipeline;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset构建模式")]
        public EBuildMode YooAssetBuildMode;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset包名")]
        public string YooAssetPackageName;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset输包版本")]
        public string YooAssetPackageVersion;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset验证构建结果")]
        public bool YooAssetVerifyBuildingResult;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset文件命名风格")]
        public EFileNameStyle YooAssetFileNameStyle;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset内置文件的拷贝选项")]
        public EBuildinFileCopyOption YooAssetBuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset内置文件的拷贝参数")]
        public string YooAssetBuildinFileCopyParams = string.Empty;

        [Required]
        [TabGroup("YooAsset")]
        [Header("YooAsset压缩")]
        public ECompressOption YooAssetCompressOption;
        #endregion


        #region HybridCLR
        [TabGroup("HybridCLR")]
        [Header("开启HybridCLR插件")]
        public bool enable = true;

        [TabGroup("HybridCLR")]
        [Header("使用全局安装的il2cpp")]
        public bool useGlobalIl2cpp;

        [TabGroup("HybridCLR")]
        [Header("hybridclr 仓库 URL")]
        public string hybridclrRepoURL = "https://gitee.com/focus-creative-games/hybridclr";

        [TabGroup("HybridCLR")]
        [Header("il2cpp_plus 仓库 URL")]
        public string il2cppPlusRepoURL = "https://gitee.com/focus-creative-games/il2cpp_plus";

        [TabGroup("HybridCLR")]
        [ReadOnly]
        [Header("hybridclr 内建资源地址")]
        public string hybridclrBuildinFileCopy = PathHelper.CombineWithApplicationDataPath("HotDlls");

        [TabGroup("HybridCLR")]
        [Header("热更新Assembly Definitions")]
        public AssemblyDefinitionAsset[] hotUpdateAssemblyDefinitions;

        [TabGroup("HybridCLR")]
        [Header("热更新dlls")]
        public string[] hotUpdateAssemblies;

        [TabGroup("HybridCLR")]
        [Header("预留的热更新dlls")]
        public string[] preserveHotUpdateAssemblies;

        [TabGroup("HybridCLR")]
        [Header("热更新dll编译输出根目录")]
        public string hotUpdateDllCompileOutputRootDir = "HybridCLRData/HotUpdateDlls";

        [TabGroup("HybridCLR")]
        [Header("外部热更新dll搜索路径")]
        public string[] externalHotUpdateAssembliyDirs;

        [TabGroup("HybridCLR")]
        [Header("裁减后AOT dll输出根目录")]
        public string strippedAOTDllOutputRootDir = "HybridCLRData/AssembliesPostIl2CppStrip";

        [TabGroup("HybridCLR")]
        [Header("补充元数据AOT dlls")]
        public string[] patchAOTAssemblies;

        [TabGroup("HybridCLR")]
        [Header("生成的link.xml路径")]
        public string outputLinkFile = "HybridCLRGenerate/link.xml";

        [TabGroup("HybridCLR")]
        [Header("自动扫描生成的AOTGenericReferences.cs路径")]
        public string outputAOTGenericReferenceFile = "HybridCLRGenerate/AOTGenericReferences.cs";

        [TabGroup("HybridCLR")]
        [Header("AOT泛型实例化搜索迭代次数")]
        public int maxGenericReferenceIteration = 10;

        [TabGroup("HybridCLR")]
        [Header("MethodBridge泛型搜索迭代次数")]
        public int maxMethodBridgeGenericIteration = 10;
        #endregion

        [Button(ButtonSizes.Gigantic)]
        public void SaveConfig()
        {
            HybridCLRSettings hSetting = HybridCLRSettings.Instance;
            hSetting.enable = enable;
            hSetting.useGlobalIl2cpp = useGlobalIl2cpp;
            hSetting.hybridclrRepoURL = hybridclrRepoURL;
            hSetting.il2cppPlusRepoURL = il2cppPlusRepoURL;
            hSetting.hotUpdateAssemblyDefinitions = hotUpdateAssemblyDefinitions;
            hSetting.hotUpdateAssemblies = hotUpdateAssemblies;
            hSetting.preserveHotUpdateAssemblies = preserveHotUpdateAssemblies;
            hSetting.hotUpdateDllCompileOutputRootDir = hotUpdateDllCompileOutputRootDir;
            hSetting.externalHotUpdateAssembliyDirs = externalHotUpdateAssembliyDirs;
            hSetting.strippedAOTDllOutputRootDir = strippedAOTDllOutputRootDir;
            hSetting.patchAOTAssemblies = patchAOTAssemblies;
            hSetting.outputLinkFile = outputLinkFile;
            hSetting.outputAOTGenericReferenceFile = outputAOTGenericReferenceFile;
            hSetting.maxGenericReferenceIteration = maxGenericReferenceIteration;
            hSetting.maxMethodBridgeGenericIteration = maxMethodBridgeGenericIteration;

            AssetBundleBuilderSetting.SetPackageBuildPipeline(YooAssetPackageName, YooAssetBuildPipeline);
            AssetBundleBuilderSetting.SetPackageBuildMode(YooAssetPackageName, YooAssetBuildPipeline, YooAssetBuildMode);
            AssetBundleBuilderSetting.SetPackageCompressOption(YooAssetPackageName, YooAssetBuildPipeline, YooAssetCompressOption);
            AssetBundleBuilderSetting.SetPackageFileNameStyle(YooAssetPackageName, YooAssetBuildPipeline, YooAssetFileNameStyle);
            AssetBundleBuilderSetting.SetPackageBuildinFileCopyOption(YooAssetPackageName, YooAssetBuildPipeline, YooAssetBuildinFileCopyOption);
            AssetBundleBuilderSetting.SetPackageBuildinFileCopyParams(YooAssetPackageName, YooAssetBuildPipeline, YooAssetBuildinFileCopyParams);
            //AssetBundleBuilderSetting.SetPackageEncyptionClassName(PackageName, BuildPipeline, );
        }

        public string HotDllGroupName
        {
            get
            {
                return "HotDll";
            }
        }

        //手动更改了平台，这里自动更改配置
        public void OnActiveBuildTargetChanged(BuildTarget newTarget)
        {
            if(newTarget != Target)
            {
                Target = newTarget;
                SaveConfig();
            }
        }
    }
}
#endif
