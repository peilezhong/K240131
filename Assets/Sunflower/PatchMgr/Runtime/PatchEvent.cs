using Sunflower.EventSys;
using Sunflower.PatchManager;

namespace Sunflower.PatchMgr.Runtime
{
    public class PatchEventCompareFileDownloadSuccess : SimpleEvent<PatchEventCompareFileDownloadSuccess>
    {
        public PatchEventCompareFileDownloadSuccess(PatchType patchType)
        { 
            this.patchType = patchType;
        }

        public PatchType patchType;
    }

    public class PatchEventCompareFileDownloadFailed : SimpleEvent<PatchEventCompareFileDownloadSuccess>
    {
        public PatchEventCompareFileDownloadFailed(PatchType patchType)
        {
            this.patchType = patchType;
        }

        public PatchType patchType;
    }

    public class PatchEventPatchDownloadSuccess : SimpleEvent<PatchEventCompareFileDownloadSuccess>
    {
        public PatchEventPatchDownloadSuccess(PatchType patchType)
        {
            this.patchType = patchType;
        }

        public PatchType patchType;
    }

    public class PatchEventPatchDownloadFailed : SimpleEvent<PatchEventCompareFileDownloadSuccess>
    {
        public PatchEventPatchDownloadFailed(PatchType patchType)
        {
            this.patchType = patchType;
        }

        public PatchType patchType;
    }
}