using Sirenix.OdinInspector;
using System;

namespace Sunflower.PatchManager
{
    [Serializable]
    public class FolderPatchItem
    {
        [BoxGroup("PatchItem", centerLabel: true)]
        public string Name;

        [BoxGroup("PatchItem", centerLabel: true)]
        public bool AutoDownload;

        [BoxGroup("PatchItem", centerLabel: true)]
        public bool CopyToStreamingAssets;

        [FolderPath(ParentFolder = "Assets/")]
        [BoxGroup("PatchItem")]
        public string Folder;

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Folder);
        }
    }
}
