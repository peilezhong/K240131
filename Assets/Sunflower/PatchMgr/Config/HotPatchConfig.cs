using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sunflower.PatchManager
{
    [CreateAssetMenu(menuName = "HotPatchConfig", fileName = "HotPatchConfig", order =4)]
    public class HotPatchConfig: ScriptableObject
    {
        public static HotPatchConfig instance;
        public static string HotPatchConfigFileName = "HotPatchConfig";
        public static string HotPatchConfigPath = "Assets/Sunflower/PatchMgr/Resources/" + HotPatchConfigFileName + ".asset";
        
        public static HotPatchConfig Instance
        {
            get
            {
                instance = Resources.Load<HotPatchConfig>(HotPatchConfigFileName);
                if (instance == null)
                {
#if UNITY_EDITOR
                    instance = ScriptableObject.CreateInstance<HotPatchConfig>();
                    AssetDatabase.CreateAsset(instance, HotPatchConfigPath);
#else
                    Debug.LogError($"can not found HotPatchConfig {HotPatchConfigPath}");
#endif
                }
                return instance;
            }
        }

        public static void Build()
        {
            
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