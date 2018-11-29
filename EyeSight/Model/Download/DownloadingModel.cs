//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model.Download
//类名称:       DownloadingModel
//创建时间:     2015/10/27 18:20:15
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Web.Http;

namespace EyeSight.Model.Download
{
    public class DownloadingModel
    {
        public int DownloadId { get; set; }

        private List<ModelPropertyBase> _DownloadModelList = new List<ModelPropertyBase>();
        public List<ModelPropertyBase> DownloadModelList
        {
            get
            {
                return _DownloadModelList;
            }
            set
            {
                _DownloadModelList = value;
            }
        }

        public void StartDownload(string downloadUrl, Action<bool> completed = null)
        {
            if (!string.IsNullOrEmpty(downloadUrl) && Uri.IsWellFormedUriString(downloadUrl, UriKind.RelativeOrAbsolute))
            {
                Uri uri = new Uri(downloadUrl, UriKind.RelativeOrAbsolute);

                using (HttpClient client = new HttpClient())
                {
                    var downloadTask = client.GetAsync(uri);

                    downloadTask.Progress = (result, progress) =>
                    {
                        try
                        {
                            for (int i = 0; i < DownloadModelList.Count; i++)
                            {
                                var model = DownloadModelList[i] as ModelPropertyBase;
                                if (model != null)
                                {
                                    model.downloadProgress = (double)((progress.BytesReceived * 100 / progress.TotalBytesToReceive)) + "%";
                                }
                            }
                        }
                        catch { }
                    };

                    downloadTask.Completed += async (ss, ee) =>
                    {
                        if (ee == AsyncStatus.Completed)
                        {
                            using (HttpResponseMessage response = await ss)
                            {
                                bool isSuccess = false;
                                try
                                {
                                    //var videoName = downloadUrl.Split('/').Last();
                                    var videoName = CommonHelper.Instance.AnalyzeRealVideoName(downloadUrl);
                                    if (videoName != null)
                                    {
                                        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.VideoFileDownloadRelativePath, CreationCollisionOption.OpenIfExists);
                                        var file = await folder.CreateFileAsync(videoName, CreationCollisionOption.OpenIfExists);
                                        using (var content = response.Content)
                                        {
                                            var buffer = await content.ReadAsBufferAsync();
                                            using (var stream = buffer.AsStream())
                                            {
                                                byte[] b = CommonHelper.Instance.StreamToBytes(stream);
                                                await Windows.Storage.FileIO.WriteBytesAsync(file, b);

                                                isSuccess = true;
                                            }
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {

                                    Debug.WriteLine(ex.Message);
                                    isSuccess = false;
                                }

                                if (isSuccess)
                                {
                                    for (int i = 0; i < DownloadModelList.Count; i++)
                                    {
                                        var model = DownloadModelList[i] as ModelPropertyBase;
                                        if (model != null)
                                        {
                                            model.isAleadyDownload = true;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < DownloadModelList.Count; i++)
                                    {
                                        var model = DownloadModelList[i] as ModelPropertyBase;
                                        if (model != null)
                                        {
                                            model.downloadProgress = "存储出错";
                                        }
                                    }
                                }

                                completed(isSuccess);
                            }
                        }
                        else
                        {

                            for (int i = 0; i < DownloadModelList.Count; i++)
                            {
                                var model = DownloadModelList[i] as ModelPropertyBase;
                                if (model != null)
                                {
                                    model.downloadProgress = ee == AsyncStatus.Canceled ? "下载取消" : "下载出错";
                                }
                            }

                            completed(false);
                        }
                    };
                }
            }
        }
    }
}
