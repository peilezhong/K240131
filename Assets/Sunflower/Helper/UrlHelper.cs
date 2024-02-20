//using Sunflower.PatchManager;
using Sunflower.PatchManager;
using System;
using UnityEngine;

namespace Sunflower.Helper
{
    public class UrlHelper
    {
        public static bool IsValidUri(string uriString)
        {
            Uri result; 
            return Uri.TryCreate(uriString, UriKind.Absolute, out result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        //public static string GetAssetBundleUrl(PatchType patchType, string abName)
        //{

        //}

        public static string GetCompareFileUrl(PatchType patchType)
        {
            string url = PatchSetting.Instance.PatchDownLoadUrl;
            string path;
            if (patchType == PatchType.Normal)
            {
                path = PathHelper.Combine(PatchSetting.NormalPatchPathRuntime, "Config", PatchCompare.PatchCompareFileName);
                path = path.Split(string.Concat(Application.persistentDataPath, "/"))[1];
            }
            else
            {
                path = PathHelper.Combine(PatchSetting.HotPatchPathRuntime, "Config", PatchCompare.PatchCompareFileName);
                path = path.Split(string.Concat(Application.persistentDataPath, "/"))[1];
            }
            return PathHelper.Combine(url, path);
        }
    }
}
