using Sunflower.Core;
using System;
using System.Collections.Generic;

namespace Sunflower.Net
{

    public class DownloadTaskGroup: IDObject
    {
        private int _curIndex = 0;

        public List<DownloadTask> tasks = new List<DownloadTask>();

        public Action OnDownloadSuccess;

        public Action OnDownloadFailed;

        public Action<long, long> OnProgress;

        public DownloadState state;

        public long totalSize = 0;

        public long curDownloadSize = 0;

        public int TaskCount
        {
            get { return tasks.Count; }
        }

        public DownloadTaskGroup()
        {

        }

        public DownloadTaskGroup(Action downLoadSuccess, Action downLoadFailed)
        {
            this.OnDownloadSuccess = downLoadSuccess;
            this.OnDownloadFailed = downLoadFailed;
        }
        public DownloadTaskGroup(List<DownloadTask> tasks, Action downLoadSuccess, Action downLoadFailed)
        {
            this.tasks = tasks;
            this.OnDownloadSuccess = downLoadSuccess;
            this.OnDownloadFailed = downLoadFailed;
        }

        public void AddTask(DownloadTask task)
        {
            tasks.Add(task);
        }

        public DownloadTask NextTask()
        {
            if (_curIndex < tasks.Count)
            {
                DownloadTask task = tasks[_curIndex];
                _curIndex++;
                return task;
            }
            return null;
        }

        public void ChangeState(DownloadState state)
        {
            this.state = state;
        }

    }
    public class DownloadTask: IDObject
    {

        public string url;

        public string loacalPath;

        public int MAX_DOWNLOAD_COUNT = 1;

        public DownloadState state;

        public Action OnnDownLoadSuccess;

        public Action OnDownLoadFailed;

        public Action<long, long> OnDownProgress;

        private long _totalSize = 0;

        private long _curDownloadSize = 0;

        private object _locker = new object();

        public long TotalSize
        {
            get { return _totalSize; }
            set
            {
                lock (_locker)
                {
                    _totalSize = value;
                }
            }
        }

        public long CurDownloadSize
        {
            get { return _totalSize; }
            set 
            {
                lock (_locker)
                {
                    _curDownloadSize = value;
                }
            }
        }

        public DownloadTask()
        {
            this.state = DownloadState.Waiting;
        }
        public DownloadTask(string url, string loacalPath)
        {
            this.state = DownloadState.Waiting;
            this.url = url;
            this.loacalPath = loacalPath;
        }

        public DownloadTask(string url, string loacalPath, int maxDownloadCount)
        {
            this.state = DownloadState.Waiting;
            this.url = url;
            this.loacalPath = loacalPath;
        }
        public DownloadTask(string url, string loacalPath, Action onDownLoadSuccess, Action onDownLoadFailed, Action<long, long> onDownProgress)
        {
            this.state = DownloadState.Waiting;
            this.url = url;
            this.loacalPath = loacalPath;
            OnnDownLoadSuccess = onDownLoadSuccess;
            OnDownLoadFailed = onDownLoadFailed;
            OnDownProgress = onDownProgress;
        }

        public void ChangeState(DownloadState state)
        {
            this.state = state;
        }
    }

    public enum DownloadState 
    {
        Waiting,
        Downloading,
        Success,
        Failed
    }

}
