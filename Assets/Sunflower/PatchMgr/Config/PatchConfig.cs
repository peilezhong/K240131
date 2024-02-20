using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sunflower.PatchManager
{
    [CreateAssetMenu(menuName = "PackageConfig", fileName = "PackageConfig", order =4)]
    public class PatchConfig : ScriptableObject
    {
        public static PatchConfig instance;
        
        public static string PatchConfigFileName = "PackageConfig";
        public static string PackageConfigFilePath = "Assets/Sunflower/PatchMgr/Resources/" + PatchConfigFileName + ".asset";
        
        public static PatchConfig Instance
        {
            get
            {
                instance = Resources.Load<PatchConfig>(PatchConfigFileName);
                if (instance == null)
                {
#if UNITY_EDITOR
                    instance = ScriptableObject.CreateInstance<PatchConfig>();
                    AssetDatabase.CreateAsset(instance, PackageConfigFilePath);
#else
                    Debug.LogError($"can not found PackageConfigFilePath {PackageConfigFilePath}");
#endif
                }
                return instance;
            }
        }
        
        [Button(ButtonSizes.Gigantic)]
        public void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
        
        [TabGroup("FreePackage")]
        [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        [TableList(ShowIndexLabels = true)]
        [HideLabel]
        public List<FreePatchItem> FreePatchList = new List<FreePatchItem>();
        
        [TabGroup("FolderPackage")]
        [GUIColor(0.3f, 0.8f, 0.8f, 1f)]
        [TableList(ShowIndexLabels = true)]
        [HideLabel]
        public List<FolderPatchItem> FolderPatchList = new List<FolderPatchItem>();
    }
}