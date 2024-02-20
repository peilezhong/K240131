using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Sunflower.Helper;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sunflower.PatchManager
{
    [Serializable]
    [CreateAssetMenu(menuName = "PatchSetting", fileName = "PatchSetting", order =4)]
    public class PatchSetting : ScriptableObject
    {
        public static PatchSetting instance;

        public static string buildSettingFileName = "PatchSetting";
        public static string buildSettingFilePath = "Assets/Sunflower/PatchMgr/Resources/" + buildSettingFileName + ".asset";
        public static string HotPatchPrefix = "HotPatch_";
        public static string PatchCinfigSuffix = ".patch.json";
        public static string PatchNameEnumOutPutPath = PathHelper.CombineWithApplicationDataPath("Sunflower", "PatchMgr", "Runtime");
        public static string PatchConfigOutPutPath = PathHelper.CombineWithApplicationDataPath("PatchConfig");

        public static string NormalPatchConfigABName
        {
            get
            {
                return String.Concat("normal_patch_config", PatchAB.Extension);
            }
        }

        public static string HotPatchConfigABName
        {
            get
            {
                return String.Concat("hot_patch_config", PatchAB.Extension);
            }
        }

        public static string HotPatchPathBuild
        {
            get
            {
                return PathHelper.CombineWithApplicationDataPath(Instance.PatchOutPutPath, "Platform", "Hot");
            }
        }

        public static string NormalPatchPathBuild
        {
            get
            {
                return PathHelper.CombineWithApplicationDataPath(Instance.PatchOutPutPath, "Platform", "Normal");
            }
        }

        public static string HotPatchPathRuntime
        {
            get
            {
                return PathHelper.CombineWithApplicationPersistentDataPath(Instance.PatchOutPutPath, "Platform", "Hot");
            }
        }

        public static string NormalPatchPathRuntime
        {
            get
            {
                return PathHelper.CombineWithApplicationPersistentDataPath(PathHelper.Last(Instance.PatchOutPutPath), "Platform", "Normal");
            }
        }

        public static string CopyDestPath
        {
            get
            {
                return PathHelper.CombineWithApplicationStreamingAssetsPath(PathHelper.Last(Instance.PatchOutPutPath), "Platform", "Normal");
            }
        }

        public static string NormalPatchCompareFileOutPutPathBuild
        {
            get
            {
                return PathHelper.CombineWithApplicationStreamingAssetsPath(Instance.PatchOutPutPath, "Platform", "Normal");
            }
        }

        public static PatchSetting Instance
        {
            get
            {
                instance = Resources.Load<PatchSetting>(buildSettingFileName);
                if (instance == null)
                {
#if UNITY_EDITOR
                    instance = ScriptableObject.CreateInstance<PatchSetting>();
                    AssetDatabase.CreateAsset(instance, buildSettingFilePath);
#else
                    Debug.LogError($"can not found buildSettingFilePath {buildSettingFilePath}");
#endif
                }
                return instance;
            }
        }

        [TitleGroup("Patch打包设置"), LabelText("清空StreamingAssets Patch存路径")]
        public bool IsClearStreamingAssets;

        [TitleGroup("Patch打包设置")]
        [LabelText("是否加密Patch")]
        public BundleEncryptToggle PatchEncrypt = new BundleEncryptToggle();

#if UNITY_EDITOR
        [TitleGroup("Patch打包设置")]
        [LabelText("压缩格式")]
        public BuildAssetBundleOptions BuildPatchOptions;
        
        [TitleGroup("Patch打包设置")]
        [LabelText("打包平台")]
        public BuildTarget BuildTarget;
#endif

        [TitleGroup("Patch打包设置")]
        [LabelText("Patch输出路径")]
        [FolderPath(ParentFolder = "Assets/")]
        public string PatchOutPutPath;

        [TitleGroup("Patch打包设置")]
        [LabelText("Patch运行时保存路径")]
        [FolderPath(ParentFolder = "Assets/")]
        public string PatchPathRuntime;

        [TitleGroup("Patch打包设置")]
        [LabelText("Patch运行时解压路径")]
        [FolderPath(ParentFolder = "Assets/")]
        public string PatchDecompressionPath;

        [TitleGroup("资源加载热更设置"), LabelText("Patch下载地址")]
        public string PatchDownLoadUrl;

        [TitleGroup("资源加载热更设置")] 
        [LabelText("资源加载模式")]
        public PatchLoadMode LoadMode;
        
        [TitleGroup("资源加载热更设置")]
        [LabelText("最大下载线程数量")]
        [ShowInInspector, PropertyRange(1, 6)]
        public int MAX_THREAD_COUNT = 1;
        
        [TitleGroup("更多设置")]
        [LabelText("Patch资源版本")]
        public int Version = 1;
        
        [TitleGroup("更多设置")]
        [LabelText("是否开启热更")]
        public bool IsHot;

        [Button(ButtonSizes.Large)]
        [ButtonGroup("Build Button")]
        private void BuildPatch()
        {
#if UNITY_EDITOR
            PatchCompiler.BuildPatch(PatchType.Normal);
#endif
        }

        [ButtonGroup("Build Button")]
        private void BuildHotPatch()
        {
#if UNITY_EDITOR
            PatchCompiler.BuildPatch(PatchType.Normal);
#endif
        }

        [GUIColor(0, 1, 0)]
        [Button(ButtonSizes.Large)]
        private void BuildAll()
        {
#if UNITY_EDITOR
            PatchCompiler.BuildPatch(PatchType.Normal);
            PatchCompiler.BuildPatch(PatchType.HotPatch);
#endif
        }

    }

    public enum PatchLoadMode
    {
        Local,
        Normal
    }
    
    [Serializable,Toggle("IsEncrypt")]
    public class BundleEncryptToggle
    {
        //是否加密
        public bool IsEncrypt;
        [LabelText("加密密钥")]
        public string EncryptKey;
    }
}