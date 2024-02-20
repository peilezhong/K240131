using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace Client.Starter
{
    public class HotUpdateHandler
    {
        public Action onUpdateSuccess;
        public Action onUpdateFailed;
        public Action<long, long, long, long> onProgress;
    }

    public class HotUpdate : SimpleSingleton<HotUpdate>
    {

        StarterConfig _config;
        ResourceDownloaderOperation _downloader;
        HotUpdateHandler _hotUpdateHandler;

        public StarterConfig Config
        {
            get
            {
                if (!_config)
                {
                    _config = Resources.Load<StarterConfig>("StarterConfig");
                }
                return _config;
            }
        }

        public HotUpdateHandler Initialize()
        {
            
            Entry.Instance.StartCoroutine(YooAssetInitPackage());
            _hotUpdateHandler = new HotUpdateHandler();
            return _hotUpdateHandler;
        }

        private IEnumerator YooAssetInitPackage()
        {
            YooAssets.Initialize();

            var package = YooAssets.TryGetPackage(Config.PackageName);
            if (package == null)
                package = YooAssets.CreatePackage(Config.PackageName);


            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (Config.PlayMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(Config.BuildPipeline.ToString(), Config.PackageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (Config.PlayMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.DecryptionServices = new FileStreamDecryption();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (Config.PlayMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new HostPlayModeParameters();
                createParameters.DecryptionServices = new FileStreamDecryption();
                createParameters.BuildinQueryServices = new GameQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(createParameters);
            }


            // WebGL运行模式
            if (Config.PlayMode == EPlayMode.WebPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new WebPlayModeParameters();
                createParameters.DecryptionServices = new FileStreamDecryption();
                createParameters.BuildinQueryServices = new GameQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;

            // 如果初始化失败弹出提示界面
            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                _hotUpdateHandler.onUpdateFailed?.Invoke();
                Debug.LogWarning($"{initializationOperation.Error}");
            }
            else
            {
                //YooAsset初始化成功，触发事件加载登录页面
                Config.PackageVersion = initializationOperation.PackageVersion;
                var gamePackage = YooAssets.GetPackage(Config.PackageName);
                YooAssets.SetDefaultPackage(gamePackage);
                Entry.Instance.StartCoroutine(YooAssetUpdatePackageVersion());
                Debug.Log($"Init resource package version : {Config.PackageVersion}");
            }
        }

        private IEnumerator YooAssetUpdatePackageVersion()
        {
            var package = YooAssets.GetPackage(Config.PackageName);
            var operation = package.UpdatePackageVersionAsync(false);
            yield return operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                _hotUpdateHandler.onUpdateFailed?.Invoke();
                Debug.LogWarning(operation.Error);
            }
            else
            {
                Config.PackageVersion = operation.PackageVersion;
                Entry.Instance.StartCoroutine(YooAssetUpdateManifest());
            }
        }

        IEnumerator YooAssetUpdateManifest()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            var package = YooAssets.GetPackage(Config.PackageName);
            bool savePackageVersion = true;
            var operation = package.UpdatePackageManifestAsync(Config.PackageVersion, savePackageVersion);
            yield return operation;

            if (operation.Status != EOperationStatus.Succeed)
            {
                _hotUpdateHandler?.onUpdateFailed?.Invoke();
                Debug.LogError(operation.Error);
                yield break;
            }
            else
            {
                Entry.Instance.StartCoroutine(YooAssetCreateDownloader());
            }
        }

        IEnumerator YooAssetCreateDownloader()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            var package = YooAssets.GetPackage(Config.PackageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            _downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            if (_downloader.TotalDownloadCount == 0)
            {
                _hotUpdateHandler.onUpdateSuccess?.Invoke();
                Debug.Log("Not found any download files !");
            }
            else
            {
                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = _downloader.TotalDownloadCount;
                long totalDownloadBytes = _downloader.TotalDownloadBytes;
                Entry.Instance.StartCoroutine(YooAssetBeginDownload());
            }
        }

        private IEnumerator YooAssetBeginDownload()
        {
            _downloader.OnDownloadErrorCallback = (fileName, err) =>
            {
                _hotUpdateHandler.onUpdateFailed?.Invoke();
            };
            _downloader.OnDownloadProgressCallback = (totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes) =>
            {
                _hotUpdateHandler.onProgress?.Invoke(totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes);
            };
            _downloader.BeginDownload();
            yield return _downloader;

            // 检测下载结果
            if (_downloader.Status != EOperationStatus.Succeed)
            {
                Debug.Log("YooAsset download failed!");
                _hotUpdateHandler.onUpdateFailed?.Invoke();
                yield break;
            }

            var package = YooAssets.GetPackage(Config.PackageName);
            var operation = package.ClearUnusedCacheFilesAsync();
            operation.Completed += (YooAsset.AsyncOperationBase obj) =>
            {
                _hotUpdateHandler.onUpdateSuccess?.Invoke();
            };
        }

        /// <summary>
        /// 资源文件流加载解密类
        /// </summary>
        private class FileStreamDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            private static uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }

        /// <summary>
        /// 资源文件解密流
        /// </summary>
        public class BundleStream : FileStream
        {
            public const byte KEY = 64;

            public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
            {
            }
            public BundleStream(string path, FileMode mode) : base(path, mode)
            {
            }

            public override int Read(byte[] array, int offset, int count)
            {
                var index = base.Read(array, offset, count);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] ^= KEY;
                }
                return index;
            }
        }

        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        private string GetHostServerURL()
        {
            string hostServerIP = Config.HostServerIP;
            string packageVersion = Config.PackageVersion;

#if UNITY_EDITOR
            //if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            //    return $"{hostServerIP}/CDN/Android/{packageVersion}";
            //else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            //    return $"{hostServerIP}/CDN/IPhone/{packageVersion}";
            //else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            //    return $"{hostServerIP}/CDN/WebGL/{packageVersion}";
            //else
            //    return $"{hostServerIP}/CDN/PC/{packageVersion}";
            return $"{hostServerIP}{Config.YooAssetRootFolderName}/{UnityEditor.EditorUserBuildSettings.activeBuildTarget}/{Config.PackageName}/{packageVersion}";
#else
        //if (Application.platform == RuntimePlatform.Android)
        //    return $"{hostServerIP}/CDN/Android/{packageVersion}";
        //else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //    return $"{hostServerIP}/CDN/IPhone/{packageVersion}";
        //else if (Application.platform == RuntimePlatform.WebGLPlayer)
        //    return $"{hostServerIP}/CDN/WebGL/{packageVersion}";
        //else
        //    return $"{hostServerIP}/CDN/PC/{packageVersion}";
        return $"{hostServerIP}{Config.YooAssetRootFolderName}/{Config.Target}/{Config.PackageName}/{packageVersion}";
#endif
        }

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }
    }

    public class GameQueryServices : IBuildinQueryServices
    {
        /// <summary>
        /// 查询内置文件的时候，是否比对文件哈希值
        /// </summary>
        public static bool CompareFileCRC = false;

        public bool Query(string packageName, string fileName, string fileCRC)
        {
            // 注意：fileName包含文件格式
            return StreamingAssetsHelper.FileExists(packageName, fileName, fileCRC);
        }
    }

#if UNITY_EDITOR
    public sealed class StreamingAssetsHelper
    {
        public static void Init() { }
        public static bool FileExists(string packageName, string fileName, string fileCRC)
        {
            Debug.Log(HotUpdate.Instance.Config.YooAssetRootFolderName);
            string filePath = Path.Combine(Application.streamingAssetsPath, HotUpdate.Instance.Config.YooAssetRootFolderName, packageName, fileName);
            if (File.Exists(filePath))
            {
                if (GameQueryServices.CompareFileCRC)
                {
                    string crc32 = YooAsset.Editor.EditorTools.GetFileCRC32(filePath);
                    return crc32 == fileCRC;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
#else
    public sealed class StreamingAssetsHelper
{
    private class PackageQuery
    {
        public readonly Dictionary<string, BuildinFileManifest.Element> Elements = new Dictionary<string, BuildinFileManifest.Element>(1000);
    }

    private static bool _isInit = false;
    private static readonly Dictionary<string, PackageQuery> _packages = new Dictionary<string, PackageQuery>(10);

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        if (_isInit == false)
        {
            _isInit = true;

            var manifest = Resources.Load<BuildinFileManifest>("BuildinFileManifest");
            if (manifest != null)
            {
                foreach (var element in manifest.BuildinFiles)
                {
                    if (_packages.TryGetValue(element.PackageName, out PackageQuery package) == false)
                    {
                        package = new PackageQuery();
                        _packages.Add(element.PackageName, package);
                    }
                    package.Elements.Add(element.FileName, element);
                }
            }
        }
    }

    /// <summary>
    /// 内置文件查询方法
    /// </summary>
    public static bool FileExists(string packageName, string fileName, string fileCRC32)
    {
        if (_isInit == false)
            Init();

        if (_packages.TryGetValue(packageName, out PackageQuery package) == false)
            return false;

        if (package.Elements.TryGetValue(fileName, out var element) == false)
            return false;

        if (GameQueryServices.CompareFileCRC)
        {
            return element.FileCRC32 == fileCRC32;
        }
        else
        {
            return true;
        }
    }
}
#endif
}
