using Sunflower.PatchManager;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Sunflower.Helper
{
    public class PathHelper
    {
        public static string TempPath
        {
            get
            {
                string path = Combine(Application.persistentDataPath, "Temp");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string DownloadTempPath
        {
            get
            {
                string path = Combine(Application.persistentDataPath, "Temp", "Download");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string LogsPath
        {
            get
            {
                return Combine(Application.persistentDataPath, "Logs");
            }
        }

        public static string ProjectRoot
        {
            get
            {
                return Combine(Application.dataPath, "..", "..");
            }
        }

        public static string ClientRoot
        {
            get
            {
                return Combine(Application.dataPath, "..");
            }
        }

        public static string CombineWithClientRoot(string path)
        {
            return Combine(Application.dataPath, "..", path);
        }

        public static string ResourcesPath
        {
            get
            {
                return CombineWithApplicationDataPath("Resources");
            }
        }

        public static string CombineWithResourcesPath(string path)
        {
            return Combine(ResourcesPath, path);
        }

        public static string GetPatchCompareFilePath(PatchType patchType)
        {
            string patchPathRuntime;
            string patchCompareFilePathRuntime;
            if (patchType.Equals(PatchType.Normal))
            {
                patchPathRuntime = PatchSetting.NormalPatchPathRuntime;
                patchCompareFilePathRuntime = PathHelper.Combine(patchPathRuntime, "Config", PatchCompare.PatchCompareFileName);
                if (!File.Exists(patchCompareFilePathRuntime))
                {
                    patchCompareFilePathRuntime = PathHelper.Combine(PatchSetting.CopyDestPath, "Config", PatchCompare.PatchCompareFileName);
                }
            }
            else
            {
                patchPathRuntime = PatchSetting.HotPatchPathRuntime;
                patchCompareFilePathRuntime = PathHelper.Combine(patchPathRuntime, "Config", PatchCompare.HotPatchCompareFileName);
                //HotPatchConfig���Ḵ�Ƶ�StreamingAsset���Բ����ж�
            }
            if (!File.Exists(patchCompareFilePathRuntime))
            {
                return null;
            }
            return patchCompareFilePathRuntime;
        }

        public static string GetAssetBundlePath(PatchType patchType, string abName)
        {
            string patchPathRuntime;
            if (patchType.Equals(PatchType.Normal))
            {
                patchPathRuntime = PathHelper.Combine(PatchSetting.NormalPatchPathRuntime, abName);
                if (!File.Exists(patchPathRuntime))
                {
                    patchPathRuntime = PathHelper.Combine(PatchSetting.CopyDestPath, abName);
                }
            }
            else
            {
                patchPathRuntime = PathHelper.Combine(PatchSetting.HotPatchPathRuntime, abName);
            }
            if (!File.Exists(patchPathRuntime))
            {
                return null;
            }
            return patchPathRuntime;
        }

        public static bool IsVaildApplicationPersistentDataPath(string path)
        {
            return path.StartsWith(Application.persistentDataPath);
        }

        public static string CombineWithTempPath(string path1)
        {
            return Path.Combine(TempPath, path1);
        }
        public static string Combine(string path1, string path2)
        {
            return ResolvePath(Path.Combine(path1, path2).Replace("\\", "/"));
        }

        public static string Combine(string path1, string path2, string path3)
        {
            return ResolvePath(Path.Combine(path1, path2, path3).Replace("\\", "/"));
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            return ResolvePath(Path.Combine(path1, path2, path3, path4).Replace("\\", "/"));
        }

        public static string Combine(string path1, string path2, string path3, string path4, string path5)
        {
            return ResolvePath(Path.Combine(path1, path2, path3, path4, path5).Replace("\\", "/"));
        }

        public static string CombineWithApplicationDataPath(string path1)
        {
            return ResolvePath(Path.Combine(Application.dataPath, path1).Replace("\\", "/"));
        }

        public static string CombineWithApplicationDataPath(string path1, string path2)
        {
            return ResolvePath(Path.Combine(Application.dataPath, path1, path2).Replace("\\", "/"));
        }

        public static string CombineWithApplicationDataPath(string path1, string path2, string path3)
        {
            return ResolvePath(Path.Combine(Application.dataPath, path1, path2, path3).Replace("\\", "/"));
        }

        public static string CombineWithApplicationDataPath(string path1, string path2, string path3, string path4)
        {
            return ResolvePath(Path.Combine(Application.dataPath, path1, path2, path3, path4).Replace("\\", "/"));
        }

        public static string CombineWithApplicationDataPath(string path1, string path2, string path3, string path4, string path5)
        {
            return ResolvePath(Path.Combine(Application.dataPath, path1, path2, path3, path4, path5).Replace("\\", "/"));
        }

        public static string CombineWithApplicationStreamingAssetsPath(string path1)
        {
            return ResolvePath(Path.Combine(Application.streamingAssetsPath, path1).Replace("\\", "/"));
        }

        public static string CombineWithApplicationStreamingAssetsPath(string path1, string path2)
        {
            return ResolvePath(Path.Combine(Application.streamingAssetsPath, path1, path2).Replace("\\", "/"));
        }

        public static string CombineWithApplicationStreamingAssetsPath(string path1, string path2, string path3)
        {
            return ResolvePath(Path.Combine(Application.streamingAssetsPath, path1, path2, path3).Replace("\\", "/"));
        }

        public static string CombineWithApplicationStreamingAssetsPath(string path1, string path2, string path3, string path4)
        {
            return ResolvePath(Path.Combine(Application.streamingAssetsPath, path1, path2, path3, path4).Replace("\\", "/"));
        }

        public static string CombineWithApplicationStreamingAssetsPath(string path1, string path2, string path3, string path4, string path5)
        {
            return ResolvePath(Path.Combine(Application.streamingAssetsPath, path1, path2, path3, path4, path5).Replace("\\", "/"));
        }

        public static string Format(string path)
        {
            return ResolvePath(path.Replace("\\", "/"));
        }

        public static string CombineWithApplicationPersistentDataPath(string path1)
        {
            return ResolvePath(Path.Combine(Application.persistentDataPath, path1).Replace("\\", "/"));
        }

        public static string CombineWithApplicationPersistentDataPath(string path1, string path2)
        {
            return ResolvePath(Path.Combine(Application.persistentDataPath, path1, path2).Replace("\\", "/"));
        }

        public static string CombineWithApplicationPersistentDataPath(string path1, string path2, string path3)
        {
            return ResolvePath(Path.Combine(Application.persistentDataPath, path1, path2, path3).Replace("\\", "/"));
        }

        public static string CombineWithApplicationPersistentDataPath(string path1, string path2, string path3, string path4)
        {
            return ResolvePath(Path.Combine(Application.persistentDataPath, path1, path2, path3, path4).Replace("\\", "/"));
        }

        public static string CombineWithApplicationPersistentDataPath(string path1, string path2, string path3, string path4, string path5)
        {
            return ResolvePath(Path.Combine(Application.persistentDataPath, path1, path2, path3, path4, path5).Replace("\\", "/"));
        }

        //public static string CombineWithPatchOutPutPath(string path1)
        //{
        //    return Path.Combine(Application.dataPath, PatchSetting.Instance.PatchOutPutPath, path1).Replace("\\", "/");
        //}

        //public static string CombineWithPatchOutPutPath(string path1, string path2)
        //{
        //    return Path.Combine(Application.dataPath, PatchSetting.Instance.PatchOutPutPath, path1, path2).Replace("\\", "/");
        //}

        public static string Last(string path)
        {
            return ResolvePath(path.Replace('\\', '/')).Split('/').Last();
        }

        public static string ToRelativeAssetPath(string s)
        {
            return ResolvePath(StringHelper.SlicingFrom(s, "Assets/"));
        }

        public static string ToRelativeProjectPath(string s)
        {
            return ResolvePath(StringHelper.SlicingFrom(s, "Client/"));
        }

        public static string ResolvePath(string path)
        {
            // ʹ��List���洢·������
            var parts = new List<string>();
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                if (segment == ".")
                {
                    // ��ǰĿ¼������  
                    continue;
                }
                else if (segment == "..")
                {
                    // ��Ŀ¼���Ƴ����һ��Ŀ¼  
                    if (parts.Any())
                    {
                        parts.RemoveAt(parts.Count - 1);
                    }
                }
                else
                {
                    // ����Ŀ¼����ӵ�ĩβ  
                    parts.Add(segment);
                }
            }

            // ��ת�б����ӳ��ַ���  
            return string.Join("/", parts.ToArray());
        }
    }
}