
using Sunflower.Core;
using Sunflower.Helper;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;
using UnityEngine;
using Task = System.Threading.Tasks.Task;
using ASunflower.Helper;

namespace Sunflower.Net
{
    public class DownloaderV2: SimpleSingleton<DownloaderV2>
    {
        private static string _tempPath = PathHelper.DownloadTempPath;
        private static Dictionary<Guid, DownloadTask> _allTasks = new();
        private static List<Guid> _curDownloadingTasks = new();
        private static List<Guid> _curWaitingTasks = new();
        private static Dictionary<Guid, DownloadTaskGroup> _allTaskGroups = new();
        private static List<Guid> _curDownloadingTaskGroups = new();
        private static List<Guid> _curWaitingTaskGroups = new();
        private int _maxThreadCount = 10;
        private int _curThreadCount = 0;
        private object _locker = new object();
        private string applicationPersistentDataPath = Application.persistentDataPath;

        //异步方法，在主线程中执行
        public async Task DownloadAsync(string url, string localPath, Action successCallback = null, Action failedCallback = null, Action<long, long> progressCallback = null)
        {
            if (!UrlHelper.IsValidUri(url))
            {
                Log.Error("ValidUri failed, please check the url!");
                return;
            }

            if(!localPath.StartsWith(applicationPersistentDataPath))
            {
                Log.Error("Valid local path failed, please check the localPath!");
                return;
            }

            if(String.IsNullOrEmpty(url) || String.IsNullOrEmpty(localPath))
            {
                Log.Error("Url or local path is null or enpty!");
                return;
            }

            bool isSuccess = false;
            string tempFilePath = localPath.Split(string.Concat(applicationPersistentDataPath, "/"))[1];
            string tempPath = PathHelper.Combine(_tempPath, tempFilePath);

            string tempDir = Path.GetDirectoryName(tempPath);
            if (!File.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            string localDir = Path.GetDirectoryName(localPath);
            if (!File.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            long tempFileSize = -1;
            bool tempFileExist = false;
            FileInfo tempFileInfo = new FileInfo(tempPath);
            if (tempFileInfo.Exists)
            {
                tempFileSize = tempFileInfo.Length;
                tempFileExist = true;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage r = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    r.EnsureSuccessStatusCode();
                    long remoteFileSize = r.Content.Headers.ContentLength.Value;
                    if (tempFileExist)
                    {
                        if(tempFileSize > 0 && tempFileSize < remoteFileSize)
                        {
                            client.DefaultRequestHeaders.Range = new RangeHeaderValue(tempFileSize, null);
                        }
                        else
                        {
                            File.Delete(tempPath);
                            tempFileExist = false;
                            tempFileSize = 0;
                        }
                    }

                    HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                    response.EnsureSuccessStatusCode();

                    FileMode mode;
                    if (tempFileExist)
                    {
                        mode = FileMode.Append;
                    }
                    else
                    {
                        mode = FileMode.Create;
                    }

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            fileStream = new FileStream(tempPath, mode, FileAccess.Write, FileShare.None))
                    {
                        long allDownloadSize = remoteFileSize - tempFileSize;
                        byte[] buffer = new byte[512];
                        int alreadyRead = 0;
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            alreadyRead += bytesRead;
                            //Log.Debug($"{alreadyRead}/{allDownloadSize}");
                            progressCallback?.Invoke(alreadyRead, allDownloadSize);
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }

                    using (FileStream originalFileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None),
                        downloadedFileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await downloadedFileStream.CopyToAsync(originalFileStream);
                        successCallback?.Invoke();
                        Log.Debug($"{url} already download to {localPath}.");
                        isSuccess = true;
                    }
                }
                catch (HttpRequestException ex)
                {
                    failedCallback?.Invoke();
                    Log.Error(ex);
                }
                finally
                {
                    if (isSuccess && File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
            }
        }

        //开起线程下载
        public async Task DownloadAsync(DownloadTask task)
        {
            if (_curDownloadingTasks.Contains(task.Id))
            {
                Log.Error($"Task {task.Id} is downloading!");
                return;
            }

            if (_curWaitingTasks.Contains(task.Id))
            {
                Log.Error($"Task {task.Id} is waiting!");
                return;
            }

            //添加到等到队列
            if (_curThreadCount >= _maxThreadCount)
            {
                lock (_locker)
                {
                    task.ChangeState(DownloadState.Failed);
                    _curWaitingTasks.Add(task.Id);
                    _allTasks.Add(task.Id, task);
                }
                return;
            }

            lock (_locker)
            {
                _curDownloadingTasks.Add(task.Id);
                _allTasks.Add(task.Id, task);
            }

            Action successWrapper = ActionHelper.ActionWrapper(task.OnnDownLoadSuccess, before: () =>
            {
                task.ChangeState(DownloadState.Success);
            });

            Action failedWrapper = ActionHelper.ActionWrapper(task.OnDownLoadFailed, before: () =>
            {
                task.ChangeState(DownloadState.Failed);
            });

            await Task.Run(async () =>
            {
                lock (_locker)
                {
                    _curThreadCount++;
                    task.ChangeState(DownloadState.Downloading);
                }
                await DownloadAsync(task.url, task.loacalPath, successWrapper, failedWrapper, task.OnDownProgress);
            });

            lock (_locker)
            {
                _curThreadCount--;
                _curDownloadingTasks.Remove(task.Id);
                _allTasks.Remove(task.Id);
            }
            if (_curWaitingTasks.Count > 0)
            {
                Guid id = _curWaitingTasks[0];
                await DownloadAsync(_allTasks[id]);
            }
        }

        //开启线程下载
        public async Task DownloadAsync(DownloadTaskGroup taskGroup)
        {
            if (_curDownloadingTaskGroups.Contains(taskGroup.Id))
            {
                Log.Error($"TaskGroup {taskGroup.Id} is downloading!");
                return;
            }

            if (_curWaitingTaskGroups.Contains(taskGroup.Id))
            {
                Log.Error($"TaskGroup {taskGroup.Id} is waiting!");
                return;
            }

            //添加到等到队列
            if(_curThreadCount >= _maxThreadCount)
            {
                lock (_locker)
                {
                    taskGroup.ChangeState(DownloadState.Waiting);
                    _curWaitingTaskGroups.Add(taskGroup.Id);
                    _allTaskGroups.Add(taskGroup.Id, taskGroup);
                }
                return;
            }

            lock (_locker)
            {
                taskGroup.ChangeState(DownloadState.Downloading);
                _curDownloadingTaskGroups.Add(taskGroup.Id);
                _allTaskGroups.Add(taskGroup.Id, taskGroup);
            }

            foreach (DownloadTask task in taskGroup.tasks)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage r = await client.GetAsync(task.url, HttpCompletionOption.ResponseHeadersRead);
                    r.EnsureSuccessStatusCode();
                    long remoteFileSize = r.Content.Headers.ContentLength.Value;
                    taskGroup.totalSize += remoteFileSize;
                }
            }

            List<Task> tasks = new();

            foreach (DownloadTask task in taskGroup.tasks)
            {
                Action failedWrapper = ActionHelper.ActionWrapper(task.OnDownLoadFailed, before: () =>
                {
                    Guid taskId = task.Id;
                    Guid groupId = taskGroup.Id;
                    if(_allTaskGroups.ContainsKey(groupId))
                    {
                        DownloadTaskGroup tg = _allTaskGroups[groupId];
                        foreach (DownloadTask t in tg.tasks)
                        {
                            if(t.Id == taskId)
                            {
                                t.ChangeState(DownloadState.Failed);
                            }
                        }
                        tg.ChangeState(DownloadState.Failed);
                    }
                });

                Action successWrapper = ActionHelper.ActionWrapper(task.OnnDownLoadSuccess, before: () =>
                {
                    Guid taskId = task.Id;
                    Guid groupId = taskGroup.Id;
                    if (_allTaskGroups.ContainsKey(groupId))
                    {
                        DownloadTaskGroup tg = _allTaskGroups[groupId];
                        foreach (DownloadTask t in tg.tasks)
                        {
                            if (t.Id == taskId)
                            {
                                t.ChangeState(DownloadState.Success);
                            }
                        }
                        tg.ChangeState(DownloadState.Success);
                    }
                });

                Action<long, long> progressWrapper = ActionHelper.ActionWrapper<long, long>(task.OnDownProgress, before: (curSize, totalSize) =>
                {
                    taskGroup.curDownloadSize += curSize;
                    taskGroup.OnProgress?.Invoke(curSize, taskGroup.totalSize);
                    //Log.LogRed($"{curSize} / {taskGroup.totalSize}");
                });

                Task t = Task.Run(async () =>
                {
                    lock (_locker)
                    {
                        _curThreadCount++;
                        task.ChangeState(DownloadState.Downloading);
                    }
                    await DownloadAsync(task.url, task.loacalPath, successWrapper, failedWrapper, progressWrapper);
                });
                tasks.Add(t);
            }

            await Task.WhenAll(tasks);
            //如果全部成功则成功回调，如果不是全部成功就是失败了
            if (taskGroup.state == DownloadState.Failed)
            {
                taskGroup.OnDownloadFailed?.Invoke();
            }
            else if(taskGroup.state == DownloadState.Success)
            {
                taskGroup.OnDownloadSuccess?.Invoke();
            }
            lock (_locker)
            {
                _curThreadCount--;
                _curDownloadingTaskGroups.Remove(taskGroup.Id);
                _allTaskGroups.Remove(taskGroup.Id);
            }
            if(_curWaitingTaskGroups.Count > 0) 
            {
                Guid id = _curWaitingTaskGroups[0];
                await DownloadAsync(_allTaskGroups[id]);
            }
        }
    }
}
