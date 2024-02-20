#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Sunflower.PatchManager.Editor
{
    public class PatchMainWindow : OdinMenuEditorWindow
    {
        
        [MenuItem("Sunflower/Patch")]
        private static void OpenWindow()
        {
            var window = GetWindow<PatchMainWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
            window.ResizableMenuWidth = false;
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false)
            {
                { "PatchConfig", PatchConfig.Instance, EditorIcons.House},
                { "HotPatchConfig", HotPatchConfig.Instance, EditorIcons.House},
                { "Build", PatchSetting.Instance, EditorIcons.SettingsCog}
            };
            return tree;
        }
        
        
    }
}
#endif
