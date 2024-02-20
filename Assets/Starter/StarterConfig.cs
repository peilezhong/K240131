
using UnityEngine;
using YooAsset;

namespace Client.Starter
{
    [CreateAssetMenu(menuName = "StarterConfig", fileName = "StarterConfig", order = 4)]
    public class StarterConfig : ScriptableObject
    {
        public string Target;
        public string PackageName = "DefaultPackage";
        public string BuildPipeline = "BuiltinBuildPipeline";
        public EPlayMode PlayMode;
        public string PackageVersion;
        public string HostServerIP;
        public string YooAssetRootFolderName;
    }
}

