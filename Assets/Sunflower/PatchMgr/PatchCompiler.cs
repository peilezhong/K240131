#if UNITY_EDITOR
using Assets.Sunflower.Helper;
using Newtonsoft.Json;
using Sirenix.Utilities;
using Sunflower.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sunflower.PatchManager
{
    public class PatchCompiler
    {
        private static string[] excludeExtensions = { ".meta", ".cs" };
        private static string defaultEncryptKey = "Sunflower";
        public static void BuildPatch(PatchType patchType)
        {
            ResetAbName();
            GenCodeForPatch();
            SetABNameForFreePatch(patchType);
            SetABNameForFolderPatch(patchType);
            CreatePatchConfig(patchType);
            BuildAllAssetBundle(patchType);
            EncryptAllPatch(patchType);
            CreatePatchCompareFiles(patchType);
            DeleteManifest(patchType);
            if (patchType == PatchType.Normal)
            {
                CopyPatchDataToStreamingAssets();
            }
            ResetAbName();
        }

        public static void CreatePatchCompareFiles(PatchType patchType)
        {
            string fileName = PatchCompare.PatchCompareFileName;
            string configABName = PatchSetting.NormalPatchConfigABName;
            string patchOutPutPath = PatchSetting.NormalPatchPathBuild;
            Dictionary<string, List<AB>> patchName2Abs = new Dictionary<string, List<AB>>();
            if (patchType == PatchType.Normal)
            {
                foreach (FreePatchItem item in PatchConfig.Instance.FreePatchList)
                {
                    string patchName = item.Name;
                    List<AB> abs = new List<AB>();
                    string abName = String.Concat(patchName, PatchAB.Extension);
                    string md5 = MD5.GetMd5FromFile(PathHelper.Combine(patchOutPutPath, abName));
                    AB ab = new AB(abName, md5);
                    abs.Add(ab);
                    patchName2Abs.Add(patchName, abs);
                }

                foreach (FolderPatchItem item in PatchConfig.Instance.FolderPatchList)
                {
                    string patchName = item.Name;
                    string[] folders = Directory.GetDirectories(PathHelper.CombineWithApplicationDataPath(item.Folder));
                    foreach (string folder in folders)
                    {
                        string dirName = PathHelper.Last(folder);
                        string abName = String.Concat(item.Name, "_", dirName, PatchAB.Extension);
                        if (!patchName2Abs.ContainsKey(patchName))
                        {
                            patchName2Abs.Add(patchName, new List<AB>());
                        }
                        bool contain = false;
                        foreach(AB ab in patchName2Abs[patchName])
                        {
                            if(ab.Name == abName)
                            {
                                contain = true;
                            }
                        }
                        if (!contain)
                        {
                            string md5 = MD5.GetMd5FromFile(PathHelper.Combine(patchOutPutPath, abName));
                            AB ab = new AB(abName, md5);
                            patchName2Abs[patchName].Add(ab);
                        }
                    }
                }
            }
            else if (patchType == PatchType.HotPatch)
            {
                fileName = PatchCompare.HotPatchCompareFileName;
                configABName = PatchSetting.HotPatchConfigABName;
                patchOutPutPath = PatchSetting.HotPatchPathBuild;
                foreach (FreePatchItem item in HotPatchConfig.Instance.FreePatchList)
                {
                    string patchName = item.Name;
                    List<AB> abs = new List<AB>();
                    string abName = String.Concat(patchName, PatchAB.Extension);
                    string md5 = MD5.GetMd5FromFile(PathHelper.Combine(patchOutPutPath, abName));
                    AB ab = new AB(abName, md5);
                    abs.Add(ab);
                    patchName2Abs.Add(patchName, abs);
                }

                foreach (FolderPatchItem item in HotPatchConfig.Instance.FolderPatchList)
                {
                    string patchName = item.Name;
                    string[] folders = Directory.GetDirectories(PathHelper.CombineWithApplicationDataPath(item.Folder));
                    foreach (string folder in folders)
                    {
                        string dirName = PathHelper.Last(folder);
                        string abName = String.Concat(item.Name, "_", dirName, PatchAB.Extension);
                        if (!patchName2Abs.ContainsKey(patchName))
                        {
                            patchName2Abs.Add(patchName, new List<AB>());
                        }
                        bool contain = false;
                        foreach (AB ab in patchName2Abs[patchName])
                        {
                            if (ab.Name == abName)
                            {
                                contain = true;
                            }
                        }
                        if (contain)
                        {
                            string md5 = PathHelper.Combine(patchOutPutPath, abName);
                            AB ab = new AB(abName, md5);
                            patchName2Abs[patchName].Add(ab);
                        }
                    }
                }
            }
            string configPatchPath = PathHelper.Combine(patchOutPutPath, configABName);
            string m = MD5.GetMd5FromFile(configPatchPath);
            AB configPatchAB = new AB(configABName, m);
            PatchCompare compare = new PatchCompare(PatchSetting.Instance.Version, patchType, patchName2Abs, configPatchAB);
            string json = JsonConvert.SerializeObject(compare, Formatting.Indented);
            string dir = PathHelper.Combine(patchOutPutPath, "Config");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileHelper.WriteFile(PathHelper.Combine(dir, fileName), System.Text.Encoding.UTF8.GetBytes(json));
        }

        public static void SetABNameForFreePatch(PatchType packageType)
        {
            List<FreePatchItem> patchList = packageType == PatchType.Normal ? PatchConfig.Instance.FreePatchList : HotPatchConfig.Instance.FreePatchList;
            Dictionary<string, Stack<string>> abName2FilePath = new Dictionary<string, Stack<string>>();
            HashSet<string> excludeExt = new HashSet<string>(excludeExtensions);
            for (int i = 0; i < patchList.Count; i++)
            {
                FreePatchItem item = patchList[i];
                string abName = item.Name;
                if (item.IsEmpty())
                {
                    Debug.LogError("The patch name or (Files or Folder)  is empty, Please check the package name");
                }

                if (!item.Files.IsNullOrEmpty())
                {
                    foreach (string fileName in item.Files)
                    {
                        string extName = Path.GetExtension(fileName);
                        if (excludeExt.Contains(extName))
                        {
                            continue;
                        }
                        if (!abName2FilePath.ContainsKey(abName))
                        {
                            abName2FilePath.Add(abName, new Stack<string>());
                        }
                        abName2FilePath[abName].Push(fileName);
                    }
                }

                if (!item.Folders.IsNullOrEmpty())
                {
                    foreach (string folder in item.Folders)
                    {
                        string[] files = Directory.GetFiles(PathHelper.Combine("Assets", folder), "*.*", SearchOption.AllDirectories);
                        foreach (string fileName in files)
                        {
                            string extName = Path.GetExtension(fileName);
                            if (excludeExt.Contains(extName))
                            {
                                continue;
                            }
                            if (!abName2FilePath.ContainsKey(abName))
                            {
                                abName2FilePath.Add(abName, new Stack<string>());
                            }
                            abName2FilePath[abName].Push(fileName.Replace("Assets/", ""));
                        }
                    }
                }
            }

            foreach (var kv in abName2FilePath)
            {
                foreach (var path in kv.Value)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(PathHelper.Combine("Assets", path));
                    if (importer != null)
                    {
                        importer.assetBundleName = packageType == PatchType.Normal ? kv.Key + PatchAB.Extension : PatchSetting.HotPatchPrefix + kv.Key + PatchAB.Extension;
                    }
                }
            }
        }

        public static void SetABNameForFolderPatch(PatchType patchType)
        {
            List<FolderPatchItem> packageList = patchType == PatchType.Normal ? PatchConfig.Instance.FolderPatchList : HotPatchConfig.Instance.FolderPatchList;
            Dictionary<string, Stack<string>> abName2FilePath = new Dictionary<string, Stack<string>>();
            HashSet<string> excludeExt = new HashSet<string>(excludeExtensions);
            foreach (var item in packageList)
            {
                string folderPath = PathHelper.Combine("Assets", item.Folder);
                if (!String.IsNullOrEmpty(item.Folder) && Directory.Exists(folderPath))
                {
                    string[] childDir = Directory.GetDirectories(folderPath);
                    foreach (string dir in childDir)
                    {
                        string dirName = PathHelper.Last(dir);
                        string abName = String.Concat(item.Name, "_", dirName, PatchAB.Extension);
                        string[] files = Directory.GetFiles(dir);
                        foreach (string fileName in files)
                        {
                            string extName = Path.GetExtension(fileName);
                            if (excludeExt.Contains(extName))
                            {
                                continue;
                            }
                            if (!abName2FilePath.ContainsKey(abName))
                            {
                                abName2FilePath.Add(abName, new Stack<string>());
                            }
                            abName2FilePath[abName].Push(fileName);
                        }
                    }
                }
                else
                {
                    Debug.LogError($"FolderPackage Folder is Empty or can not found path: {folderPath}");
                }
            }

            foreach (var kv in abName2FilePath)
            {
                foreach (var path in kv.Value)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = patchType == PatchType.Normal ? kv.Key : PatchSetting.HotPatchPrefix + kv.Key;
                    }
                }
            }
        }

        public static void ResetAbName()
        {
            string[] allBundleArr = AssetDatabase.GetAllAssetBundleNames();
            Dictionary<string, string> allBundleFilePathDic = new Dictionary<string, string>();
            foreach (string bundleName in allBundleArr)
            {
                string[] bundleFileArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                foreach (string filePath in bundleFileArr)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(filePath);
                    if (importer != null)
                    {
                        importer.assetBundleName = "";
                    }
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        public static void BuildAllAssetBundle(PatchType patchType)
        {
            string patchOutputPath = patchType == PatchType.Normal ? PatchSetting.NormalPatchPathBuild : PatchSetting.HotPatchPathBuild;
            if (Directory.Exists(patchOutputPath))
            {
                Directory.Delete(patchOutputPath, true);
            }
            Directory.CreateDirectory(patchOutputPath);
            AssetDatabase.Refresh();
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
                patchOutputPath,
                PatchSetting.Instance.BuildPatchOptions,
                PatchSetting.Instance.BuildTarget);

            //AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
            //    patchOutputPath,
            //    BuildAssetBundleOptions.UncompressedAssetBundle,
            //    BuildTarget.StandaloneWindows64);
            if (manifest == null)
            {
                EditorUtility.DisplayProgressBar("BuildAssetBundle!", "BuildAssetBundle failed!", 1);
                Debug.LogError("AssetBundle Build failed!");
            }
            else
            {
                Debug.Log("AssetBundle Build Successs!:");
            }
            EditorUtility.ClearProgressBar();
        }

        public static void EncryptAllPatch(PatchType patchType)
        {
            if (!PatchSetting.Instance.PatchEncrypt.IsEncrypt)
            {
                return;
            }

            string patchOutputPath = patchType == PatchType.Normal ? PatchSetting.NormalPatchPathBuild : PatchSetting.HotPatchPathBuild;
            string encryptKey = String.IsNullOrEmpty(PatchSetting.Instance.PatchEncrypt.EncryptKey) ? defaultEncryptKey : PatchSetting.Instance.PatchEncrypt.EncryptKey;
            
            DirectoryInfo directoryInfo = new DirectoryInfo(patchOutputPath);
            FileInfo[] fileInfoArr = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < fileInfoArr.Length; i++)
            {
                string ext = Path.GetExtension(fileInfoArr[i].FullName);
                if (!ext.Equals(PatchAB.Extension))
                {
                    continue;
                }
                EditorUtility.DisplayProgressBar("加密文件", "Name:" + fileInfoArr[i].Name, i * 1.0f / fileInfoArr.Length);
                AES.AESFileEncrypt(fileInfoArr[i].FullName, encryptKey);
                Debug.Log(String.Format($"AssetBundle {fileInfoArr[i].Name} encrypt success"));
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("All patch encrypt finish!");
        }

        public static void CreatePatchConfig(PatchType patchType)
        {
            HashSet<string> excludeExt = new HashSet<string>(excludeExtensions);
            string[] allBundleArr = AssetDatabase.GetAllAssetBundleNames();
            Dictionary<string, string> abAssetPath2AbName = new Dictionary<string, string>();
            foreach (string bundleName in allBundleArr)
            {
                string[] bundleFileArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                foreach (string filePath in bundleFileArr)
                {
                    if (excludeExt.Contains(Path.GetExtension(filePath)))
                    {
                        continue;
                    }
                    abAssetPath2AbName.Add(filePath, bundleName);
                }
            }

            Dictionary<string, List<PatchAsset>> abName2PatchAssets = new Dictionary<string, List<PatchAsset>>();
            foreach (string bundleName in allBundleArr)
            {
                string[] bundleFileArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

                List<PatchAsset> assets = new List<PatchAsset>();
                foreach (string filePath in bundleFileArr)
                {

                    List<string> abs = new List<string>();
                    string[] dependences = AssetDatabase.GetDependencies(filePath);
                    foreach (var dependencePath in dependences)
                    {
                        if (excludeExt.Contains(filePath) || dependencePath.Equals(filePath))
                        {
                            continue;
                        }
                        string name = "";
                        if (abAssetPath2AbName.TryGetValue(filePath, out name))
                        {
                            if (!name.Equals(bundleName) && !String.IsNullOrEmpty(name) && !abs.Contains(name))
                            {
                                abs.Add(name);
                            }
                        }
                    }
                    PatchAsset asset = new PatchAsset(
                        filePath,
                        Crc32.GetCrc32(filePath),
                        Path.GetFileName(filePath),
                        bundleName,
                        abs,
                        patchType == PatchType.Normal ? false : true);
                    assets.Add(asset);
                }
                abName2PatchAssets.Add(bundleName, assets);
            }

            Dictionary<string, List<string>> patchName2Abs = new Dictionary<string, List<string>>();
            string configABName = PatchSetting.NormalPatchConfigABName;
            if (patchType == PatchType.Normal)
            {
                foreach (FreePatchItem item in PatchConfig.Instance.FreePatchList)
                {
                    string patchName = item.Name;
                    List<string> abs = new List<string>();
                    abs.Add(String.Concat(patchName, PatchAB.Extension));
                    patchName2Abs.Add(patchName, abs);
                }

                foreach (FolderPatchItem item in PatchConfig.Instance.FolderPatchList)
                {
                    string patchName = item.Name;
                    string[] folders = Directory.GetDirectories(PathHelper.CombineWithApplicationDataPath(item.Folder));
                    foreach (string folder in folders)
                    {
                        string dirName = PathHelper.Last(folder);
                        string abName = String.Concat(item.Name, "_", dirName, PatchAB.Extension);
                        if (!patchName2Abs.ContainsKey(patchName))
                        {
                            patchName2Abs.Add(patchName, new List<string>());
                        }
                        if (!patchName2Abs[patchName].Contains(abName))
                        {
                            patchName2Abs[patchName].Add(abName);
                        }
                    }
                }
            }
            else if (patchType == PatchType.HotPatch)
            {
                configABName = PatchSetting.HotPatchConfigABName;
                foreach (FreePatchItem item in HotPatchConfig.Instance.FreePatchList)
                {
                    string patchName = item.Name;
                    List<string> abs = new List<string>();
                    abs.Add(patchName);
                    patchName2Abs.Add(patchName, abs);
                }

                foreach (FolderPatchItem item in HotPatchConfig.Instance.FolderPatchList)
                {
                    string patchName = item.Name;
                    string[] folders = Directory.GetDirectories(PathHelper.CombineWithApplicationDataPath(item.Folder));
                    foreach (string folder in folders)
                    {
                        string dirName = PathHelper.Last(folder);
                        string abName = String.Concat(item.Name, "_", dirName, PatchAB.Extension);
                        if (!patchName2Abs.ContainsKey(patchName))
                        {
                            patchName2Abs.Add(patchName, new List<string>());
                        }
                        if (!patchName2Abs[patchName].Contains(abName))
                        {
                            patchName2Abs[patchName].Add(abName);
                        }
                    }
                }
            }

            int cur = 0;
            foreach (var kv in patchName2Abs)
            {
                cur++;
                string patchName = kv.Key;
                List<string> abNames = kv.Value;

                EditorUtility.DisplayProgressBar("生成配置文件: ", String.Concat("PatchName: "), cur * 1.0f / patchName2Abs.Count);

                List<PatchAsset> assets = new List<PatchAsset>();
                List<string> abs = new List<string>();
                foreach (string ab in abNames)
                {
                    List<PatchAsset> abAssets = abName2PatchAssets[ab];
                    foreach (PatchAsset asset in abAssets)
                    {
                        assets.Add(asset);
                    }
                    abs.Add(ab);
                }
                Patch patch = new Patch(patchName, abs, assets);
                string json = JsonConvert.SerializeObject(patch, Formatting.Indented);
                string patchConfigOutPutPath = PatchSetting.PatchConfigOutPutPath;
                if (!Directory.Exists(patchConfigOutPutPath))
                {
                    Directory.CreateDirectory(patchConfigOutPutPath);
                }
                string patchConfigPath = PathHelper.Combine(patchConfigOutPutPath, String.Concat(patchName, PatchSetting.PatchCinfigSuffix));
                FileHelper.WriteFile(patchConfigPath, System.Text.Encoding.UTF8.GetBytes(json));
                AssetDatabase.Refresh();
                string p = patchConfigPath.Replace(Application.dataPath, "Assets");
                AssetImporter importer = AssetImporter.GetAtPath(patchConfigPath.Replace(Application.dataPath, "Assets"));
                if (importer != null)
                {
                    importer.assetBundleName = configABName;
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void DeleteManifest(PatchType patchType)
        {
            string patchOutputPath = patchType == PatchType.Normal ? PatchSetting.NormalPatchPathBuild : PatchSetting.HotPatchPathBuild;
            string[] files = Directory.GetFiles(patchOutputPath);
            foreach (var item in files)
            {
                if(File.Exists(item) && Path.GetExtension(item).Equals(".manifest"))
                {
                    string targetString = StringHelper.SlicingFromWithParam(item, "Assets/");
                    AssetDatabase.DeleteAsset(targetString);
                }
            }
        }

        public static void CopyPatchDataToStreamingAssets()
        {
            string path = PatchSetting.CopyDestPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else if(PatchSetting.Instance.IsClearStreamingAssets)
            {
                DirectoryHelper.ClearDirectory(path);
            }

            List<string> copyList = new List<string>();
            foreach(FreePatchItem item in PatchConfig.Instance.FreePatchList)
            {
                if(item.CopyToStreamingAssets)
                {
                    copyList.Add(item.Name + PatchAB.Extension);
                }
            }
            foreach (FolderPatchItem item in PatchConfig.Instance.FolderPatchList)
            {
                if (item.CopyToStreamingAssets)
                {
                    string[] folders = Directory.GetDirectories(PathHelper.CombineWithApplicationDataPath(item.Folder));
                    foreach (string folder in folders)
                    {
                        string dirName = PathHelper.Last(folder);
                        string abName = String.Concat(item.Name, "_", dirName, PatchAB.Extension);
                        copyList.Add(abName);                        
                    }
                }
            }

            foreach (string abName in copyList)
            {
                string srcPath = PathHelper.Combine(PatchSetting.NormalPatchPathBuild, abName);
                string destName = PathHelper.Combine(PatchSetting.CopyDestPath, abName);
                File.Copy(srcPath, destName, true);
            }

            string src = PathHelper.Combine(PatchSetting.NormalPatchPathBuild, PatchSetting.NormalPatchConfigABName);
            string dest = PathHelper.Combine(PatchSetting.CopyDestPath, PatchSetting.NormalPatchConfigABName);
            File.Copy(src, dest, true);

            string dir = PathHelper.Combine(PatchSetting.NormalPatchPathBuild, "Config");
            string s = PathHelper.Combine(dir, PatchCompare.PatchCompareFileName);
            string streamingConfigPath = PathHelper.Combine(PatchSetting.CopyDestPath, "Config");
            if(!Directory.Exists(streamingConfigPath))
            {
                Directory.CreateDirectory(streamingConfigPath);
            }
            string d = PathHelper.Combine(streamingConfigPath, PatchCompare.PatchCompareFileName);
            File.Copy(s, d, true);
        }

        public static void GenCodeForPatch()
        {
            string namespaceName = "Sunflower.PatchMgr";
            string className = "PatchName";
            string path = String.Concat(PathHelper.Combine("Assets", PatchSetting.PatchNameEnumOutPutPath, className), ".cs");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var writer = File.CreateText(path);
            List<string> patchList = new List<string>();
            foreach(FreePatchItem patch in PatchConfig.Instance.FreePatchList)
            {
                patchList.Add(patch.Name);
            }
            foreach (FolderPatchItem patch in PatchConfig.Instance.FolderPatchList)
            {
                patchList.Add(patch.Name);
            }

            writer.WriteLine($"namespace {namespaceName}");
            writer.WriteLine("{");
            writer.WriteLine("\t" + $"public enum {className}");
            writer.WriteLine("\t" + "{");
            writer.WriteLine("\t\tNone,");

            for (int i = 0; i < patchList.Count; i++)
            {
                writer.WriteLine("\t\t" + patchList[i] + ",");
            }
            writer.WriteLine("\t" + "}");
            writer.WriteLine("}");
            writer.Close();
            Debug.Log("GenCode for Patch success!");
            AssetDatabase.Refresh();
        }
    }
}
#endif

