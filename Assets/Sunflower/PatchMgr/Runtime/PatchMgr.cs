using Newtonsoft.Json;
using Sunflower.Core;
using Sunflower.Helper;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Sunflower.Core;
using Assets.Sunflower.Helper;
using Sunflower.Net;
using System;
using Sunflower.PatchMgr;

namespace Sunflower.PatchManager.Runtime
{
    public class PatchMgr : Singleton<PatchMgr>, ISingletonAwake
    {

        private Dictionary<uint, PatchAsset> allAssetDic = new Dictionary<uint, PatchAsset>();

        private Dictionary<string, PatchAB> allLoadedPatchABDic = new Dictionary<string, PatchAB>();

        private SimpleObjectPool<PatchAB> abPool = new SimpleObjectPool<PatchAB>(100);

        //private SimpleObjectPool<PatchAsset> patchAssetPool = new SimpleObjectPool<PatchAsset>(500);

        public void Awake()
        {
            CompareAB(200, PatchType.Normal);
            LoadAllPatchConfig();
        }

        public void DownloadPatch()
        {

        }

        public void DownAB(string abName)
        {

        }

        public void DownloadPatchConfig()
        {

        }

        public async void CompareAB(int version, PatchType patchType)
        {
            string patchPathRuntime;
            string patchPatchConfigPathRuntime;


            if (patchType.Equals(PatchType.Normal))
            {
                patchPathRuntime = PatchSetting.NormalPatchPathRuntime;
                patchPatchConfigPathRuntime = PathHelper.Combine(patchPathRuntime, "Config", PatchCompare.PatchCompareFileName);
                if (!File.Exists(patchPatchConfigPathRuntime))
                {
                    patchPatchConfigPathRuntime = PathHelper.Combine(PatchSetting.CopyDestPath, "Config", PatchCompare.PatchCompareFileName);
                }
            }
            else
            {
                patchPathRuntime = PatchSetting.HotPatchPathRuntime;
                patchPatchConfigPathRuntime = PathHelper.Combine(patchPathRuntime, "Config", PatchCompare.HotPatchCompareFileName);
                //HotPatchConfig不会复制到StreamingAsset所以不用判断
            }

            void onCompareFileDownLoadSuccess()
            {

            }

            void onCompareFileDownDownLoadFailed()
            {
                Log.Error($"Download compare file failed!");
            }

            void onCompareFileDownProgress(long cur, long total)
            {
                //Log.LogRed("onCompareFileDownProgress");
                Log.LogRed($"cur/total: {cur} / {total}");
            }

            string remotePath = StringHelper.SlicingFrom(PatchSetting.NormalPatchCompareFileOutPutPathBuild, "../");
            string url = PathHelper.Combine(PatchSetting.Instance.PatchDownLoadUrl, remotePath, "Config", PatchCompare.PatchCompareFileName);
            string localPath = PathHelper.CombineWithApplicationPersistentDataPath(remotePath, "Config", PatchCompare.PatchCompareFileName);
            DownloadTask task = new DownloadTask(
                url,
                localPath,
                onCompareFileDownLoadSuccess,
                onCompareFileDownDownLoadFailed,
                onCompareFileDownProgress);

            if (File.Exists(patchPatchConfigPathRuntime))
            {
                string text = await File.ReadAllTextAsync(patchPatchConfigPathRuntime);
                PatchCompare compare = JsonConvert.DeserializeObject<PatchCompare>(text);

                if (compare.vsersion == version)
                {
                    //本地对比文件和远程对比文件一致，不需要重新下载
                    //onCompareFileDownLoadSuccess.Invoke();
                    DownloadTaskGroup taskGroup = CheckABMd5(compare);
                    await DownloaderV2.Instance.DownloadAsync(taskGroup);
                }
                else
                {
                    //下载对比文件
                    await DownloaderV2.Instance.DownloadAsync(task);
                    DownloadTaskGroup taskGroup = CheckABMd5(localPath);
                    if (taskGroup == null)
                    {
                        return;
                    }
                    await DownloaderV2.Instance.DownloadAsync(taskGroup);
                }
            }
            else
            {
                //下载对比文件
                await DownloaderV2.Instance.DownloadAsync(task);
                DownloadTaskGroup taskGroup = CheckABMd5(localPath);
                if (taskGroup == null)
                {
                    return;
                }
                await DownloaderV2.Instance.DownloadAsync(taskGroup);
            }
        }

        private DownloadTaskGroup CheckABMd5(string compareFilePath)
        {
            if (String.IsNullOrEmpty(compareFilePath) || !File.Exists(compareFilePath))
            {
                return null;
            }
            string text = File.ReadAllText(compareFilePath);
            PatchCompare compare = JsonConvert.DeserializeObject<PatchCompare>(text);
            return CheckABMd5(compare);
        }

        private DownloadTaskGroup CheckABMd5(PatchCompare compare)
        {
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

            }

            void downloadFailed()
            {

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

            return downLoadGroup;
        }





        //根据Patch名称加载配置文件
        public void LoadAllPatchConfigByName(PatchName name)
        {
            if (PatchSetting.Instance.LoadMode == PatchLoadMode.Local) return;

            AssetBundle patchInfoAb = null;
            string abPath = PathHelper.Combine(PatchSetting.NormalPatchPathRuntime, PatchSetting.NormalPatchConfigABName);
            if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
            {
                byte[] abBytes = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                patchInfoAb = AssetBundle.LoadFromMemory(abBytes);
            }
            else
            {
                patchInfoAb = AssetBundle.LoadFromFile(abPath);
            }
            byte[] test = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
            TextAsset info = patchInfoAb.LoadAsset<TextAsset>(name.ToString());
            Patch config = JsonConvert.DeserializeObject<Patch>(info.text);

            foreach (PatchAsset asset in config.PatchAsssts)
            {
                if (!allAssetDic.ContainsKey(asset.Crc))
                {
                    allAssetDic.Add(asset.Crc, asset);
                }
            }
            patchInfoAb.Unload(false);
        }

        //加载所有Patch配置文件
        public void LoadAllPatchConfig()
        {
            if (PatchSetting.Instance.LoadMode == PatchLoadMode.Local) return;

            AssetBundle patchInfoAb = null;
            string abPath = PathHelper.Combine(PatchSetting.CopyDestPath, PatchSetting.NormalPatchConfigABName);
            if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
            {
                byte[] abBytes = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                patchInfoAb = AssetBundle.LoadFromMemory(abBytes);
            }
            else
            {
                patchInfoAb = AssetBundle.LoadFromFile(abPath);
            }
            TextAsset[] infos = patchInfoAb.LoadAllAssets<TextAsset>();
            foreach (TextAsset info in infos)
            {
                Patch config = JsonConvert.DeserializeObject<Patch>(info.text);
                foreach (PatchAsset asset in config.PatchAsssts)
                {
                    if (!allAssetDic.ContainsKey(asset.Crc))
                    {
                        allAssetDic.Add(asset.Crc, asset);
                    }
                }
            }
            patchInfoAb.Unload(false);
        }

        //解压Patch
        public void DecompressionPatch(PatchName patchName, PatchType patchType)
        {

        }

        //解压所有Patch
        public void DecompressionAllPatch(PatchType patchType)
        {

        }

        //加载资源所在的AB包
        public PatchAB LoadPatchABByPatchAssetCrc(uint crc)
        {
            PatchAsset asset = null;
            allAssetDic.TryGetValue(crc, out asset);
            if (asset != null)
            {
                List<string> dependences = asset.DependenceABs;
                foreach (string dependenceAb in dependences)
                {
                    LoadPatchAB(dependenceAb);
                }
                string abName = asset.ABName;
                PatchAB patchAB = LoadPatchAB(abName);
                asset.SetPatchAB(patchAB);
                return patchAB;
            }
            else
            {
                Log.Error($"Can not find crc: {crc} in config, or patch config not load!");
                return null;
            }
        }

        public PatchAB LoadPatchAB(string abName, PatchType patchType = PatchType.Normal)
        {

            if (allLoadedPatchABDic.ContainsKey(abName))
            {
                PatchAB ab = allLoadedPatchABDic[abName];
                ab.RefCount++;
                return ab;
            }
            PatchAB patchAB = abPool.Spawn();
            string abPath = string.Concat(patchType == PatchType.Normal ? PatchSetting.HotPatchPathRuntime : PatchSetting.NormalPatchPathRuntime, abName);
            if (PatchSetting.Instance.PatchEncrypt.IsEncrypt)
            {
                byte[] abBytes = AES.AESFileByteDecrypt(abPath, PatchSetting.Instance.PatchEncrypt.EncryptKey);
                AssetBundle ab = AssetBundle.LoadFromMemory(abBytes);
                patchAB.AssetBundle = ab;
            }
            else
            {
                patchAB.AssetBundle = AssetBundle.LoadFromFile(abPath);
            }
            if (patchAB == null)
            {
                Log.Error($"AssetBundle {abName} load failed!");
                return null;
            }
            patchAB.Name = abName;
            patchAB.RefCount++;
            allLoadedPatchABDic.Add(abName, patchAB);
            return patchAB;
        }

        //释放PatchAB
        public void ReleasePatchAB(PatchAB patchAB, bool unLoad = false)
        {
            if (patchAB == null)
            {
                Log.Error("PatchAB is null!");
            }
            string abName = patchAB.Name;
            PatchAB cacheAb = null;
            if (allLoadedPatchABDic.TryGetValue(abName, out cacheAb) && !string.IsNullOrEmpty(abName))
            {
                if (cacheAb != null)
                {
                    cacheAb.RefCount--;
                    if (cacheAb.RefCount <= 0)
                    {
                        cacheAb.AssetBundle.Unload(unLoad);
                        allLoadedPatchABDic.Remove(abName);
                        cacheAb.Release();
                        abPool.Recycl(cacheAb);
                    }
                }
            }
        }

        public PatchAsset LoadPatchAsset(string path)
        {
            uint crc = Crc32.GetCrc32(path);
            return LoadPatchAsset(crc);
        }

        public PatchAsset LoadPatchAsset(uint crc)
        {
            if (!allAssetDic.ContainsKey(crc))
            {
                Log.Error($"PatchAsset load failed, crc: {crc}, Because patchasset not in config!");
                return null;
            }
            PatchAsset patchAsset = allAssetDic[crc];
            return patchAsset;
        }

        public void ReleasePatchAsset(PatchAsset patchAsset)
        {
            ReleasePatchAsset(patchAsset.Crc);
        }

        public void ReleasePatchAsset(string path)
        {
            ReleasePatchAsset(Crc32.GetCrc32(path));
        }

        public void ReleasePatchAsset(uint crc)
        {
            if (!allAssetDic.ContainsKey(crc))
            {
                Log.Error($"Release patchasset failed, crc: {crc}, Because patchasset not in config!");
            }
            else
            {
                PatchAsset patchAsset = allAssetDic[crc];
                PatchAB patchAB = patchAsset.PatchAB;
                ReleasePatchAB(patchAB);
                patchAsset.Release();
                allAssetDic.Remove(crc);
            }
        }
    }
}
