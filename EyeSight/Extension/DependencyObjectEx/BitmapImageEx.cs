//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       BitmapImageEx
//创建时间:     2015/9/21 星期一 15:32:53
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Extension.IOEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace EyeSight.Extension.DependencyObjectEx
{
    public static class BitmapImageEx
    {
        /// <summary>
        /// Waits for the BitmapImage to load.
        /// </summary>
        /// <param name="bitmapImage">The bitmap image.</param>
        /// <param name="timeoutInMs">The timeout in ms.</param>
        /// <returns></returns>
        public async static Task<ExceptionRoutedEventArgs> WaitForLoadedAsync(this BitmapImage bitmapImage, int timeoutInMs = 0)
        {
            var tcs = new TaskCompletionSource<ExceptionRoutedEventArgs>();

            // TODO: NOTE: This returns immediately if the image is already loaded,
            // but if the image already failed to load - the task will never complete and the app might hang.
            if (bitmapImage.PixelWidth > 0 ||
                bitmapImage.PixelHeight > 0)
            {
                tcs.SetResult(null);
                return await tcs.Task;
            }

            //var tc = new TimeoutCheck(bitmapImage);

            // Need to set it to null so that the compiler does not
            // complain about use of unassigned local variable.
            RoutedEventHandler reh = null;
            ExceptionRoutedEventHandler ereh = null;
            EventHandler<object> progressCheckTimerTickHandler = null;
            var progressCheckTimer = new DispatcherTimer();
            Action dismissWatchmen = () =>
            {
                bitmapImage.ImageOpened -= reh;
                bitmapImage.ImageFailed -= ereh;
                progressCheckTimer.Tick -= progressCheckTimerTickHandler;
                progressCheckTimer.Stop();
                //tc.Stop();
            };

            int totalWait = 0;
            progressCheckTimerTickHandler = (sender, o) =>
            {
                totalWait += 10;

                if (bitmapImage.PixelWidth > 0)
                {
                    dismissWatchmen.Invoke();
                    tcs.SetResult(null);
                }
                else if (timeoutInMs > 0 && totalWait >= timeoutInMs)
                {
                    dismissWatchmen.Invoke();
                    tcs.SetResult(null);
                    //ErrorMessage = string.Format("BitmapImage loading timed out after {0}ms for {1}.", totalWait, bitmapImage.UriSource)
                }
            };

            progressCheckTimer.Interval = TimeSpan.FromMilliseconds(10);
            progressCheckTimer.Tick += progressCheckTimerTickHandler;
            progressCheckTimer.Start();

            reh = (s, e) =>
            {
                dismissWatchmen.Invoke();
                tcs.SetResult(null);
            };

            ereh = (s, e) =>
            {
                dismissWatchmen.Invoke();
                tcs.SetResult(e);
            };

            bitmapImage.ImageOpened += reh;
            bitmapImage.ImageFailed += ereh;

            return await tcs.Task;
        }

        /// <summary>
        /// Loads a BitmapImage asynchronously from a given uri.
        /// </summary>
        /// <param name="uri">The uri</param>
        /// <returns></returns>
        public static async Task<BitmapImage> LoadAsyncFromUrl(this BitmapImage bitmap, string uri)
        {
            return await bitmap.SetSourceAsync(uri);
        }

        /// <summary>
        /// Loads a BitmapImage asynchronously given a specific file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> LoadAsync(this BitmapImage bitmap, StorageFile file)
        {
            return await bitmap.SetSourceAsync(file);
        }

        /// <summary>
        /// Loads a BitmapImage asynchronously given a specific folder and file name.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> LoadAsync(this BitmapImage bitmap, StorageFolder folder, string fileName)
        {
            try
            {
                if (await folder.CheckFileExistedAsync(fileName))
                {
                    var file = await folder.GetFileByPathAsync(fileName);
                    return await bitmap.SetSourceAsync(file);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Sets the source image for a BitmapImage by opening
        /// a given file and processing the result asynchronously.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> SetSourceAsync(this BitmapImage bitmap, StorageFile file)
        {
            try
            {
                if (file == null) return null;
                using (var stream = await file.OpenReadAsync())
                {
                    await bitmap.SetSourceAsync(stream);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Sets the source image for a BitmapImage from
        /// a given uri and processing the result asynchronously.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="uri">The uri.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> SetSourceAsync(this BitmapImage bitmap, string uri)
        {
            try
            {
                var rass = RandomAccessStreamReference.CreateFromUri(new Uri(uri));
                if (rass == null) return null;
                using (var stream = await rass.OpenReadAsync())
                {
                    stream.Seek(0);
                    await bitmap.SetSourceAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return null;
            }
            return bitmap;
        }

        /// <summary>
        /// Loads a BitmapImage from a Base64 encoded string.
        /// </summary>
        /// <param name="bitmap">The bitmap into which the image will be loaded.</param>
        /// <param name="img">The Base64-encoded image string.</param>
        /// <returns></returns>
        public static async Task<BitmapImage> LoadFromBase64String(this BitmapImage bitmap, string img)
        {
            //img = @"/9j/4AAQSkZJRgABAQAAAQABAAD//gA7Q1JFQ ... "; // Full Base64 image as string here
            var imgBytes = Convert.FromBase64String(img);

            using (var ms = new InMemoryRandomAccessStream())
            {
                using (var dw = new DataWriter(ms))
                {
                    dw.WriteBytes(imgBytes);
                    await dw.StoreAsync();
                    ms.Seek(0);
                    await bitmap.SetSourceAsync(ms);
                }
            }

            return bitmap;
        }
    }
}
