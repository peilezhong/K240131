using Assets.Sunflower.Core;
using Sunflower.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Sunflower.PatchManager
{
    [System.Serializable]
    public class PatchCompare
    {
        public PatchCompare(int version, PatchType patchType, Dictionary<string, List<AB>> patchName2AB, AB configAB) 
        {
            this.patchType = patchType;
            this.vsersion = version;
            this.patchName2AB = patchName2AB;
            this.configAB = configAB;
        }

        public static string PatchCompareFileName = "PatchCompare.json";
        public static string HotPatchCompareFileName = "HotPatchCompare.json";

        public int vsersion;
        public AB configAB;
        public PatchType patchType;
        public Dictionary<string, List<AB>> patchName2AB;
    }

    [System.Serializable]
    public class Patch
    {
        public Patch(string name, List<string> abs, List<PatchAsset> assets, bool isHot = false) 
        {
            this.Name = name;
            this.PatchABs = abs;
            this.PatchAsssts = assets;
            this.IsHot = isHot;
        }

        public string Name;
        public List<string> PatchABs;
        public List<PatchAsset> PatchAsssts;
        public bool IsHot;
    }

    public class AB
    {
        public string Name;
        public string Md5;

        public AB(string name, string md5)
        {
            this.Name = name;
            this.Md5 = md5;
        }
    }

    [System.Serializable]
    public class PatchAB: IRelease
    {
        public static string Extension = ".patchab";

        public string Name;
        public AssetBundle AssetBundle;
        public int RefCount;

        public void Release()
        {
            Name = null;
            AssetBundle = null;
            RefCount = 0;
        }
    }

    [System.Serializable]
    public class PatchAsset
    {

        public PatchAsset()
        {

        }
        public PatchAsset(string path, uint crc, string assetsName, string abName, List<string> abs, bool isHot)
        {
            this.Path = path;
            this.Crc = crc;
            this.AssetName = assetsName;
            this.ABName = abName;
            this.DependenceABs = abs;
            this.IsHot = isHot;
        }

        public string Path;
        public uint Crc;
        public string AssetName;
        public string ABName;
        public List<string> DependenceABs;
        public PatchAB PatchAB;
        public bool IsHot;

        public void Release()
        {
            Path = null;
            Crc = 0;
            AssetName = null;
            ABName = null;
            DependenceABs = null;
            PatchAB = null;
        }

        public void SetPatchAB(PatchAB patchAB)
        {
            this.PatchAB = patchAB;
        }
    }
}
