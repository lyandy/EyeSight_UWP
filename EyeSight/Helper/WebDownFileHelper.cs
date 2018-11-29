//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       WebDownFileHelper
//创建时间:     2015/9/21 星期一 15:50:10
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Extension.IOEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace EyeSight.Helper
{
    public class WebDownFileHelper : ClassBase<WebDownFileHelper>
    {
        public WebDownFileHelper() : base() { }

        public const long MAX_RESPONSE_CONTENT_BUFFER_SIZE = 10 * 10 * 10 * 1024 * 1024;
        private static Semaphore _semaphoreForNoneReturn = new Semaphore(200, 200, "HttpSaveAsyncWidthTimeOut");
        private static Semaphore _semaphoreForReturn = new Semaphore(200, 200, "HttpSaveAsync");
        //下载标示队列
        private static List<string> downloadList = new List<string>();

        public async void SaveAsyncWidthHttpAndTimeOut(
        string url,
        string fileName,
        StorageFolder folder = null,
        NameCollisionOption option = NameCollisionOption.ReplaceExisting, int _timeOut = 10)
        {

            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            var httpClient = new HttpClient();
            try
            {
                Debug.WriteLine(string.Format("HttpSaveAsyncWidthTimeOut task {0} 等待一个许可证", Task.CurrentId));
                if (_semaphoreForNoneReturn == null) _semaphoreForNoneReturn = new Semaphore(20, 20, "HttpSaveAsyncWidthTimeOut");
                _semaphoreForNoneReturn.WaitOne();
                Debug.WriteLine(string.Format("HttpSaveAsyncWidthTimeOut task {0} 申请到一个许可证", Task.CurrentId));
                httpClient.MaxResponseContentBufferSize = WebDownFileHelper.MAX_RESPONSE_CONTENT_BUFFER_SIZE;

                httpClient.Timeout = TimeSpan.FromSeconds(_timeOut);
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    //这里注释掉是为了尽可能减少崩溃，具体可以看EnsureSuccessStatusCode得微软解释。 修改：李扬  时间：2014.7.18   10：25
                    //response.EnsureSuccessStatusCode();
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        try
                        {


                            if (folder == null)
                            {
                                folder = ApplicationData.Current.LocalFolder;
                            }
                            byte[] b = CommonHelper.Instance.StreamToBytes(stream);
                            await Windows.Storage.FileIO.WriteBytesAsync(file, b);
                        }
                        catch
                        {
                            stream.Dispose();
                        }
                    }
                    response.Dispose();
                }
            }
            catch
            {
                file.DeleteAsync();
                Debug.WriteLine("HttpClient下载出错 下载地址:" + url);
            }
            finally
            {
                httpClient.Dispose();
                httpClient = null;
                var releaseCount = _semaphoreForNoneReturn.Release();
                Debug.WriteLine(string.Format("HttpSaveAsyncWidthTimeOut task {0} 归还了一个许可证", Task.CurrentId + " releaseCount:" + releaseCount + " 个"));
            }

        }

        public async Task<StorageFile> SaveAsyncWithHttp(
            string url,
            string fileName,
            StorageFolder folder = null,
            NameCollisionOption option = NameCollisionOption.GenerateUniqueName)
        {
            return await Task.Run(async () =>
            {
                Debug.WriteLine(string.Format("HttpSaveAsync task {0} 等待一个许可证", Task.CurrentId));
                _semaphoreForReturn.WaitOne();
                Debug.WriteLine(string.Format("HttpSaveAsync {0} 申请到一个许可证", Task.CurrentId));
                var httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = WebDownFileHelper.MAX_RESPONSE_CONTENT_BUFFER_SIZE;
                try
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(100);
                    using (HttpResponseMessage response = await httpClient.GetAsync(url))
                    {
                        //response.EnsureSuccessStatusCode();
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            if (folder == null)
                            {
                                folder = ApplicationData.Current.LocalFolder;
                            }
                            byte[] b = CommonHelper.Instance.StreamToBytes(stream);
                            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                            await Windows.Storage.FileIO.WriteBytesAsync(file, b);

                            return file;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                    Debug.WriteLine("HttpClient下载出错 下载地址:" + url);
                    throw ex;
                }
                finally
                {
                    if (null != httpClient)
                        httpClient.Dispose();
                    var releaseCount = _semaphoreForReturn.Release();
                    Debug.WriteLine(string.Format("HttpSaveAsync task {0} 归还了一个许可证", Task.CurrentId + " releaseCount:" + releaseCount + " 个"));
                }

            });
        }

        /// <summary>
        /// Downloads a file from the specified address and returns the file.
        /// </summary>
        /// <param name="fileUri">The URI of the file.</param>
        /// <param name="folder">The folder to save the file to.</param>
        /// <param name="fileName">The file name to save the file as.</param>
        /// <param name="option">
        /// A value that indicates what to do
        /// if the filename already exists in the current folder.
        /// </param>
        /// <remarks>
        /// If no file name is given - the method will try to find
        /// the suggested file name in the HTTP response
        /// based on the Content-Disposition HTTP header.
        /// </remarks>
        /// <returns></returns>
        public async Task<StorageFile> SaveAsyncWithBackgroundDownloader(
            Uri fileUri,
            StorageFolder folder = null,
            string fileName = null,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            //将要下载的文件标记记录到下载队列标识集合里面
            var fileUniqueMark = folder + fileName;
            if (!downloadList.Contains(fileUniqueMark))
            {
                downloadList.Add(fileUniqueMark);
            }
            else
            {
                return null;
            }

            if (folder == null)
            {
                folder = ApplicationData.Current.LocalFolder;
            }

            //首先以reationCollisionOption.GenerateUniqueName的方式创建一个临时文件
            var file = await folder.CreateTempFileAsync();
            //创建BackgroundDownloader任务
            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(
                fileUri,
                file);

            #region 之前的代码
            //var res = await download.StartAsync();

            //var ss = download.GetResponseInformation();

            //if (string.IsNullOrEmpty(fileName))
            //{
            //    // Use temp file name by default
            //    fileName = file.Name;

            //    // Try to find a suggested file name in the http response headers
            //    // and rename the temp file before returning if the name is found.
            //    var info = res.GetResponseInformation();

            //    if (info.Headers.ContainsKey("Content-Disposition"))
            //    {
            //        var cd = info.Headers["Content-Disposition"];
            //        var regEx = new Regex("filename=\"(?<fileNameGroup>.+?)\"");
            //        var match = regEx.Match(cd);

            //        if (match.Success)
            //        {
            //            fileName = match.Groups["fileNameGroup"].Value;
            //            await file.RenameAsync(fileName, option);
            //            return file;
            //        }
            //    }
            //}
            #endregion

            try
            {
                //开始下载
                await download.StartAsync();
                //下载成功则重命名之前已经创建的文件为目标文件名
                await file.RenameAsync(fileName, option);
            }
            catch (Exception ex)
            {
                //如果下载或者重命名失败则立刻删除之前的临时文件
                file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                //及时从下载队列标识中移除标记
                if (downloadList.Contains(fileUniqueMark))
                {
                    downloadList.Remove(fileUniqueMark);
                }

                //抛出异常以便外部调用的方法能够处理
                throw ex;
            }

            //var ri = download.GetResponseInformation();

            ////如果返回的是404，说明网络上没有此资源，则应立刻删除.tmp临时文件
            //if (ri.StatusCode == 404)
            //{
            //    await 

            //    throw new FileNotFoundException();
            //}
            //else
            //{
            //    await file.RenameAsync(fileName, option);
            //}

            //及时从下载队列标识中移除标记
            if (downloadList.Contains(fileUniqueMark))
            {
                downloadList.Remove(fileUniqueMark);
            }

            return file;
        }

        /// <summary>
        /// Downloads a file from the specified address and returns the file.
        /// </summary>
        /// <param name="fileUri">The URI of the file.</param>
        /// <param name="folder">The folder to save the file to.</param>
        /// <param name="fileName">The file name to save the file as.</param>
        /// <param name="option">
        /// A value that indicates what to do
        /// if the filename already exists in the current folder.
        /// </param>
        /// <remarks>
        /// If no file name is given - the method will try to find
        /// the suggested file name in the HTTP response
        /// based on the Content-Disposition HTTP header.
        /// </remarks>
        /// <returns></returns>
        public async Task<StorageFile> SaveAsyncWithBackgroundDownloaderAndProgress(
            Uri fileUri,
            IProgress<DownloadOperation> ProgressCallBack,
            StorageFolder folder = null,
            string fileName = null,
            NameCollisionOption option = NameCollisionOption.GenerateUniqueName)
        {

            Uri uri = new Uri("...");
            try
            {
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var downloadTask = client.GetAsync(uri);

                downloadTask.Progress = (result, progress) =>
                {
                    Debug.WriteLine("===start===");
                    Debug.WriteLine(progress.BytesReceived);
                    Debug.WriteLine(progress.TotalBytesToReceive);
                    Debug.WriteLine("===end===");
                };

                var Downloadresult = await downloadTask;
                Debug.WriteLine("Done: " + Downloadresult.StatusCode.ToString());
            }
            catch (Exception ex)
            {
            }

            if (folder == null)
            {
                folder = ApplicationData.Current.LocalFolder;
            }

            var file = await folder.CreateTempFileAsync();
            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(
                fileUri,
                file);

            var res = await download.StartAsync().AsTask(ProgressCallBack);

            if (string.IsNullOrEmpty(fileName))
            {
                // Use temp file name by default
                fileName = file.Name;

                // Try to find a suggested file name in the http response headers
                // and rename the temp file before returning if the name is found.
                var info = res.GetResponseInformation();

                if (info.Headers.ContainsKey("Content-Disposition"))
                {
                    var cd = info.Headers["Content-Disposition"];
                    var regEx = new Regex("filename=\"(?<fileNameGroup>.+?)\"");
                    var match = regEx.Match(cd);

                    if (match.Success)
                    {
                        fileName = match.Groups["fileNameGroup"].Value;
                        await file.RenameAsync(fileName, option);
                        return file;
                    }
                }
            }

            await file.RenameAsync(fileName, option);
            return file;
        }
    }
}
