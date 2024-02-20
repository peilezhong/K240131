#if UNITY_EDITOR
using HybridCLR.Editor.Settings;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using YooAsset.Editor;

namespace Sunflower.Editor
{
    public class BuildWindow : OdinMenuEditorWindow
    {
        public static void OpenWindow()
        {
            var window = GetWindow<BuildWindow>("Builder", true, WindowsDefine.DockedWindowTypes);
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 1000);
            window.ResizableMenuWidth = false;
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false)
            {
                { "Build Setting", BuildConfig.Instance, EditorIcons.SettingsCog},
                {"YooAsset Collector", AssetBundleCollectorSettingData.Setting, EditorIcons.SettingsCog }
            };
            return tree;
        }
    }
}
#endif
