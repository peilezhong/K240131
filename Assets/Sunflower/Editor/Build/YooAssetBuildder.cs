#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace Sunflower.Editor.YooAsset
{
    internal class YooAssetBuildder
    {
        //使用内建构建管线构建
        public static void BuildInternal()
        {
            BuildConfig config = BuildConfig.Instance;
            Debug.Log($"YooAsset开始构建 : {config.Target}");

            // 构建参数
            BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
            buildParameters.BuildOutputRoot = config.YooAssetBuildOutputRoot ;
            buildParameters.BuildinFileRoot = config.YooAssetBuildinFileRoot;
            buildParameters.BuildPipeline = config.YooAssetBuildPipeline.ToString();
            buildParameters.BuildTarget = config.Target;
            buildParameters.BuildMode = config.YooAssetBuildMode;
            buildParameters.PackageName = config.YooAssetPackageName;
            buildParameters.PackageVersion = config.YooAssetPackageVersion;
            buildParameters.VerifyBuildingResult = config.YooAssetVerifyBuildingResult;
            buildParameters.FileNameStyle = config.YooAssetFileNameStyle;
            buildParameters.BuildinFileCopyOption = config.YooAssetBuildinFileCopyOption;
            buildParameters.BuildinFileCopyParams = config.YooAssetBuildinFileCopyParams;
            //buildParameters.EncryptionServices = CreateEncryptionInstance();
            buildParameters.CompressOption = ECompressOption.LZ4;

            // 执行构建
            BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                Debug.Log($"YooAsset构建成功 : {buildResult.OutputPackageDirectory}");
            }
            else
            {
                Debug.LogError($"YooAsset构建失败 : {buildResult.ErrorInfo}");
            }
        }
    }
}
#endif
