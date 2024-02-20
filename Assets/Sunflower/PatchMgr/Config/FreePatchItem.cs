using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;

namespace Sunflower.PatchManager
{
    [Serializable]
    public class FreePatchItem
    {
        [BoxGroup("PatchItem", centerLabel: true)]
        public string Name;

        [BoxGroup("PatchItem", centerLabel: true)]
        public bool AutoDownload;

        [BoxGroup("PatchItem", centerLabel: true)]
        public bool CopyToStreamingAssets;

        [Sirenix.OdinInspector.FilePath(ParentFolder = "Assets/")]
        [BoxGroup("PatchItem")]
        public string[] Files;

        [FolderPath(ParentFolder = "Assets/")]
        [BoxGroup("PatchItem")]
        public string[] Folders;

        public bool IsEmpty()
        {
            return String.IsNullOrEmpty(Name) || (Files.IsNullOrEmpty() && Folders.IsNullOrEmpty());
        }
    }
}
