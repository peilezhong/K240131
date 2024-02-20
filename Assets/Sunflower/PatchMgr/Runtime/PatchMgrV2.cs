using Assets.Sunflower.Core;
using Assets.Sunflower.Helper;
using Newtonsoft.Json;
using Sunflower.Core;
using Sunflower.EventSys;
using Sunflower.Helper;
using Sunflower.Net;
using Sunflower.PatchManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Sunflower.PatchMgr.Runtime
{
    public class PatchMgrV2: Singleton<PatchMgrV2>, ISingletonAwake
    {
        private int _curPatchVersion = 0;
        private Dictionary<uint, PatchAsset> _crc2NormalPatchAsset = new();
        private Dictionary<uint, PatchAsset> _crc2HotPatchAsset = new();
        private SimpleObjectPool<PatchAB> _abPool = new SimpleObjectPool<PatchAB>(200);
        private Dictionary<uint, PatchAB> _crc2PatchAB = new();

        public async void Awake()
        {
            EventSystem.Instance.RegisterEvent<PatchEventCompareFileDownloadSuccess>(OnEventPatchEventCompareFileDownloadSuccessAsync);
            EventSystem.Instance.RegisterEvent<PatchEventCompareFileDownloadFailed>(OnPatchEventCompareFileDownloadFailed);
            EventSystem.Instance.RegisterEvent<PatchEventPatchDownloadSuccess>(OnEventPatchEventPatchDownloadSuccessAsync);
            EventSystem.Instance.RegisterEvent<PatchEventPatchDownloadFailed>(OnPatchEventPatchDownloadFailed);

            await DownloadCompareFile(PatchType.Normal);
            Coroutine co = UnityMono.Instance.StartCoroutine(LoadPatchConfig(LoadPatchConfigSuccess));
            
        }

        public override void Dispose()
        {
            base.Dispose();
            EventSystem.Instance.UnRegisterEvent<PatchEventCompareFileDownloadSuccess>(OnEventPatchEventCompareFileDownloadSuccessAsync);
            EventSystem.Instance.UnRegisterEvent<PatchEventCompareFileDownloadFailed>(OnPatchEventCompareFileDownloadFailed);
            EventSystem.Instance.UnRegisterEvent<PatchEventPatchDownloadSuccess>(OnEventPatchEventPatchDownloadSuccessAsync);
            EventSystem.Instance.UnRegisterEvent<PatchEventPatchDownloadFailed>(OnPatchEventPatchDownloadFailed);
        }

        public void PatchVersionChange(int vsersion)
        {
            _curPatchVersion = vsersion;
        }

        public void LoadPatchConfigSuccess()
        {
            Log.LogGreen("加载Patch配置成功！");
            //整个Patch模块成功初始化，分发事件

        }

        public async void OnEventPatchEventCompareFileDownloadSuccessAsync(PatchEventCompareFileDownloadSuccess e)
        {
            string compareFilePath = PathHelper.GetPatchCompareFilePath(e.patchType);
            if(compareFilePath == null)
            {
                Log.Error("下载资源失败，请重试！");
                return;
            }
            await DownloadAssetBundleAsync(compareFilePath, e.patchType);
        }

        //patch下载失败
        private void OnPatchEventPatchDownloadFailed(PatchEventPatchDownloadFailed failed)
        {
            Log.Error("Patch download failed!");
        }

        //patch下载成功
        private void OnEventPatchEventPatchDownloadSuccessAsync(PatchEventPatchDownloadSuccess success)
        {
            Log.LogGreen("AssetBundle资源下载成功！");
        }

        //对比文件下载失败
        public void OnPatchEventCompareFileDownloadFailed(PatchEventCompareFileDownloadFailed e)
        {
            Log.Error("下载资源失败，请重试！");
        }

        //对比文件下载成功
        public async Task DownloadCompareFile(PatchType patchType)
        {
            string compareFilePath = PathHelper.GetPatchCompareFilePath(patchType);
            string compareFileUrl = UrlHelper.GetCompareFileUrl(patchType);
            if (compareFilePath == null) //本地没有对比文件
            {
                await DownloaderV2.Instance.DownloadAsync(compareFileUrl, compareFilePath, () =>
                {
                    //分发下载完成
                    EventSystem.Instance.Trigger<PatchEventCompareFileDownloadSuccess>(new PatchEventCompareFileDownloadSuccess(patchType));
                },
                () =>
                {
                    //分发下载失败
                    EventSystem.Instance.Trigger<PatchEventCompareFileDownloadFailed>(new PatchEventCompareFileDownloadFailed(patchType));
                });
            }
            else //本地存在对比文件
            {
                await DownloadAssetBundleAsync(compareFilePath, patchType);
            }
        }

        public async Task DownloadAssetBundleAsync(string compareFilePath, PatchType patchType)
        {
            string text = await File.ReadAllTextAsync(compareFilePath);
            PatchCompare compare = JsonConvert.DeserializeObject<PatchCompare>(text);

            List<string> downloadList = new List<string>();
            string patchPathRuntime = PatchSetting.NormalPatchPathRuntime;
            if (compare.patchType == PatchType.HotPatch)
            {
                patchPathRuntime = PatchSetting.HotPatchPathRuntime;
            }
            AB configAB = compare.configAB;
            string configABPath = PathHelper.Combine(patchPathRuntime, configAB.Name);

            void downloadSuccess()
            {
                EventSystem.Instance.Trigger<PatchEventPatchDownloadSuccess>(new PatchEventPatchDownloadSuccess(patchType));
            }

            void downloadFailed()
            {
                EventSystem.Instance.Trigger<PatchEventPatchDownloadFailed>(new PatchEventPatchDownloadFailed(patchType));
            }

            DownloadTaskGroup downLoadGroup = new DownloadTaskGroup(downloadSuccess, downloadFailed);

            string remotePath = StringHelper.SlicingFrom(PatchSetting.NormalPatchCompareFileOutPutPathBuild, "../");
            string url = PathHelper.Combine(PatchSetting.Instance.PatchDownLoadUrl, remotePath);
            if (!File.Exists(configABPath))
            {
                DownloadTask t = new DownloadTask(PathHelper.Combine(url, configAB.Name), PathHelper.Combine(patchPathRuntime, configAB.Name));
                downLoadGroup.AddTask(t);
            }
            else
            {
                string md5 = MD5.GetMd5FromFile(configABPath);
                if (md5 != configAB.Md5)
                {
                    DownloadTask t = new DownloadTask(PathHelper.Combine(url, configAB.Name), PathHelper.Combine(patchPathRuntime, configAB.Name));
                    downLoadGroup.AddTask(t);
                }
            }

            foreach (List<AB> abList in compare.patchName2AB.Values)
            {
                foreach (AB ab in abList)
                {
                    string abPath = PathHelper.Combine(patchPathRuntime, ab.Name);
                    if (!File.Exists(abPath))
                    {
                        DownloadTask t = new DownloadTask(PathHelper.Combine(url, ab.Name), PathHelper.Combine(patchPathRuntime, ab.Name));
                        downLoadGroup.AddTask(t);
                    }
                    else
                    {
                        string m = MD5.GetMd5FromFile(abPath);
                        if (m != ab.Md5)
                        {
                            DownloadTask t = new DownloadTask(PathHelper.Combine(url, ab.Name), PathHelper.Combine(patchPathRuntime, ab.Name));
                            downLoadGroup.AddTask(t);
                        }
                    }
                }
            }

            await DownloaderV2.Instance.DownloadAsync(downLoadGroup);
        }

        public PatchAsset LoadPatchAsset(string path)
        {
            uint crc = Crc32.GetCrc32(path);
            return LoadPatchAsset(crc);
        }

        public PatchAsset LoadPatchAsset(uint crc)
        {
            PatchAsset patchAsset;
            if (_crc2HotPatchAsset.ContainsKey(crc))
            {
                patchAsset = _crc2HotPatchAsset[crc];
            }
            else
            {
                if (!_crc2NormalPatchAsset.ContainsKey(crc))
                {
                    Log.Error($"PatchAsset load failed, crc: {crc}, Because patchasset not in config!");
                    return null;
                }
                patchAsset = _crc2NormalPatchAsset[crc];
            }
            return patchAsset;
        }

        IEnumerator LoadPatchConfig(Action callback = null)
        {
            string[] configAbNames = { PatchSetting.NormalPatchConfigABName, PatchSetting.HotPatchConfigABName };
            foreach(string configAbName in configAbNames)
            {
                AssetBundle ab = null;
                string abPath = PathHelper.Combine(PatchSetting.CopyDestPath, configAbName);
                if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
                {
                    byte[] abBytes = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                    AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(abBytes);
                    yield return request;
                    ab = request.assetBundle;
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(abPath);
                    yield return request;
                    ab = request.assetBundle;
                }
                TextAsset[] configs = ab.LoadAllAssets<TextAsset>();
                foreach (TextAsset info in configs)
                {
                    Patch config = JsonConvert.DeserializeObject<Patch>(info.text);
                    foreach (PatchAsset asset in config.PatchAsssts)
                    {
                        if (asset.IsHot)
                        {
                            if (!_crc2HotPatchAsset.ContainsKey(asset.Crc))
                            {
                                _crc2HotPatchAsset.Add(asset.Crc, asset);
                            }
                            if (!_crc2NormalPatchAsset.ContainsKey(asset.Crc))
                            {
                                _crc2NormalPatchAsset.Remove(asset.Crc);
                            }
                        }
                        else
                        {
                            if (!_crc2NormalPatchAsset.ContainsKey(asset.Crc))
                            {
                                _crc2NormalPatchAsset.Add(asset.Crc, asset);
                            }
                        }

                    }
                }
                ab.Unload(false);
            }
            callback?.Invoke();
        }

        public AssetBundle LoadAssetBundleByPatchAsset(PatchAsset patchAsset)
        {
            if (_crc2PatchAB.ContainsKey(patchAsset.Crc))
            {
                return _crc2PatchAB[patchAsset.Crc].AssetBundle;
            }

            foreach(string abName in patchAsset.DependenceABs)
            {
                string dAbPath = PathHelper.GetAssetBundlePath(patchAsset.IsHot ? PatchType.HotPatch : PatchType.Normal, abName);
                uint abCrc = Crc32.GetCrc32(dAbPath.Split(string.Concat(Application.persistentDataPath, "/"))[1]);

                AssetBundle dAb;
                if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
                {
                    byte[] abBytes = AES.AESFileByteDecrypt(dAbPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                    dAb = AssetBundle.LoadFromMemory(abBytes);
                }
                else
                {
                    dAb = AssetBundle.LoadFromFile(dAbPath);
                }
                PatchAB patchAB = _abPool.Spawn();
                patchAB.Name = abName;
                patchAB.AssetBundle = dAb;
                patchAB.RefCount++;
                _crc2PatchAB.Add(abCrc, patchAB);
            }

            AssetBundle ab;
            string abPath = PathHelper.GetAssetBundlePath(patchAsset.IsHot ? PatchType.HotPatch : PatchType.Normal, patchAsset.ABName);
            uint crc = Crc32.GetCrc32(abPath.Split(string.Concat(Application.persistentDataPath, "/"))[1]);
            if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
            {
                byte[] abBytes = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                ab = AssetBundle.LoadFromMemory(abBytes);
            }
            else
            {
                ab = AssetBundle.LoadFromFile(abPath);
            }
            PatchAB pAB = _abPool.Spawn();
            pAB.Name = patchAsset.ABName;
            pAB.AssetBundle = ab;
            pAB.RefCount++;
            _crc2PatchAB.Add(crc, pAB);
            return ab;
        }

        public void ReleasePatchAsset(uint crc)
        {
            if(_crc2PatchAB.ContainsKey(crc))
            {
                PatchAB pAB = _crc2PatchAB[crc];
                pAB.RefCount--;
                if(pAB.RefCount == 0)
                {
                    pAB.AssetBundle.Unload(false);
                    _abPool.Recycl(pAB);
                    _crc2PatchAB.Remove(crc);
                }
            }
        }

            public IEnumerator LoadAssetBundleByPatchAssetAsync(PatchAsset patchAsset, Action<AssetBundle> callback)
        {
            foreach (string abName in patchAsset.DependenceABs)
            {
                string dAbPath = PathHelper.GetAssetBundlePath(patchAsset.IsHot ? PatchType.HotPatch : PatchType.Normal, abName);
                AssetBundle dAb = null;
                if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
                {
                    byte[] abBytes = AES.AESFileByteDecrypt(dAbPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                    AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(abBytes);
                    yield return request;
                    dAb = request.assetBundle;
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(dAbPath);
                    yield return request;
                    dAb = request.assetBundle;
                }

                PatchAB patchAB = _abPool.Spawn();
                patchAB.Name = abName;
                patchAB.AssetBundle = dAb;
                patchAB.RefCount++;
                uint abCrc = Crc32.GetCrc32(dAbPath.Split(string.Concat(Application.persistentDataPath, "/"))[1]);
                _crc2PatchAB.Add(abCrc, patchAB);
            }

            AssetBundle ab;
            string abPath = PathHelper.GetAssetBundlePath(patchAsset.IsHot ? PatchType.HotPatch : PatchType.Normal, patchAsset.ABName);
            if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
            {
                byte[] abBytes = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(abBytes);
                yield return request;
                ab = request.assetBundle;
            }
            else
            {
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(abPath);
                yield return request;
                ab = request.assetBundle;
            }
            callback?.Invoke(ab);
        }
    }
}
