//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       WriteableBitmapEx
//创建时间:     2015/9/21 星期一 15:39:48
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace EyeSight.Extension.DependencyObjectEx
{
    public static class WriteableBitmapEx
    {
        /// <summary>
        /// Waits for the given WriteableBitmap to be loaded (non-zero size).
        /// </summary>
        /// <param name="wb">The WriteableBitmap to wait for.</param>
        /// <param name="timeoutInMs">The timeout in ms after which the wait will be cancelled. Use 0 to wait without a timeout.</param>
        /// <returns></returns>
        public async static Task WaitForLoadedAsync(this WriteableBitmap wb, int timeoutInMs = 0)
        {
            int totalWait = 0;

            while (
                wb.PixelWidth <= 1 &&
                wb.PixelHeight <= 1)
            {
                await Task.Delay(10);
                totalWait += 10;

                if (timeoutInMs > 0 &&
                    totalWait > timeoutInMs)
                    return;
            }
        }

        /// <summary>
        /// Loads the WriteableBitmap asynchronously given the storage file and the dimensions.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> LoadAsync(
            this WriteableBitmap writeableBitmap,
            string relativePath)
        {
            var resolvedFile = await ScaledImageFile.Get(relativePath);

            if (resolvedFile == null)
                throw new FileNotFoundException("Could not load image.", relativePath);

            return await writeableBitmap.LoadAsync(resolvedFile);
        }

        /// <summary>
        /// Loads the WriteableBitmap asynchronously given the storage file.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFile">The storage file.</param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> LoadAsync(
            this WriteableBitmap writeableBitmap,
            StorageFile storageFile)
        {
            var wb = writeableBitmap;

            using (var stream = await storageFile.OpenReadAsync())
            {
                await wb.SetSourceAsync(stream);
            }

            //await wb.WaitForLoadedAsync();

            return wb;
        }

        /// <summary>
        /// Loads the WriteableBitmap asynchronously given the storage file and the dimensions.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFile">The storage file.</param>
        /// <param name="decodePixelWidth">Width in pixels of the decoded bitmap.</param>
        /// <param name="decodePixelHeight">Height in pixels of the decoded bitmap.</param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> LoadAsync(
            this WriteableBitmap writeableBitmap,
            StorageFile storageFile,
            uint decodePixelWidth,
            uint decodePixelHeight)
        {
            using (var stream = await storageFile.OpenReadAsync())
            {
                await writeableBitmap.SetSourceAsync(
                    stream,
                    decodePixelWidth,
                    decodePixelHeight);
            }

            return writeableBitmap;
        }

        /// <summary>
        /// Loads the WriteableBitmap asynchronously given the file path relative to install location and the dimensions.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="decodePixelWidth">Width in pixels of the decoded bitmap.</param>
        /// <param name="decodePixelHeight">Height in pixels of the decoded bitmap.</param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> LoadAsync(
            this WriteableBitmap writeableBitmap,
            string relativePath,
            uint decodePixelWidth,
            uint decodePixelHeight)
        {
            var resolvedFile = await ScaledImageFile.Get(relativePath);

            return await writeableBitmap.LoadAsync(
                resolvedFile,
                decodePixelWidth,
                decodePixelHeight);
        }

        /// <summary>
        /// Loads the WriteableBitmap from the source URI of the given BitmapImage
        /// </summary>
        /// <param name="target">The target WriteableBitmap.</param>
        /// <param name="source">The source BitmapImage.</param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> FromBitmapImage(this WriteableBitmap target, BitmapImage source)
        {
            if (source.UriSource == null ||
                source.UriSource.OriginalString == null)
            {
                return target;
            }

            string originalString = source.UriSource.OriginalString;

            if (originalString.StartsWith("ms-appx:/"))
            {
                string installedFolderImageSourceUri = originalString.Replace("ms-appx:/", "");
                await target.LoadAsync(installedFolderImageSourceUri);
            }
            else
            {
                var file = await WebDownFileHelper.Instance.SaveAsyncWithBackgroundDownloader(source.UriSource, ApplicationData.Current.TemporaryFolder);
                await target.LoadAsync(file);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            return target;
        }

        // The Tim Heuer method (see https://twitter.com/timheuer/status/217521386720198656)
        /// <summary>
        /// Sets the WriteableBitmap source asynchronously given a stream and dimensions.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="streamSource">The stream source.</param>
        /// <param name="decodePixelWidth">Width in pixels of the decoded bitmap.</param>
        /// <param name="decodePixelHeight">Height in pixels of the decoded bitmap.</param>
        /// <returns></returns>
        public static async Task SetSourceAsync(
            this WriteableBitmap writeableBitmap,
            IRandomAccessStream streamSource,
            uint decodePixelWidth,
            uint decodePixelHeight)
        {
            var decoder = await BitmapDecoder.CreateAsync(streamSource);

            using (var inMemoryStream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, decoder);
                encoder.BitmapTransform.ScaledWidth = decodePixelWidth;
                encoder.BitmapTransform.ScaledHeight = decodePixelHeight;
                await encoder.FlushAsync();
                inMemoryStream.Seek(0);
                await writeableBitmap.SetSourceAsync(inMemoryStream);
            }
        }

        /// <summary>
        /// Saves the WriteableBitmap to a png file with a unique file name.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <returns>The file the bitmap was saved to.</returns>
        public static async Task<StorageFile> SaveToFile(
            this WriteableBitmap writeableBitmap)
        {
            return await writeableBitmap.SaveToFile(
                KnownFolders.PicturesLibrary,
                string.Format(
                    "{0}_{1}.png",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"),
                    Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Saves the WriteableBitmap to a png file in the given folder with a unique file name.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFolder">The storage folder.</param>
        /// <returns>The file the bitmap was saved to.</returns>
        public static async Task<StorageFile> SaveToFile(
            this WriteableBitmap writeableBitmap,
            StorageFolder storageFolder)
        {
            return await writeableBitmap.SaveToFile(
                storageFolder,
                string.Format(
                    "{0}_{1}.png",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"),
                    Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Saves the WriteableBitmap to a file in the given folder with the given file name.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFolder">The storage folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="options">
        /// The enum value that determines how responds if the fileName is the same
        /// as the name of an existing file in the current folder. Defaults to ReplaceExisting.
        /// </param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveToFile(
            this WriteableBitmap writeableBitmap,
            StorageFolder storageFolder,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            StorageFile outputFile =
                await storageFolder.CreateFileAsync(
                    fileName,
                    options);

            Guid encoderId;

            var ext = Path.GetExtension(fileName);

            if (new[] { ".bmp", ".dib" }.Contains(ext))
            {
                encoderId = BitmapEncoder.BmpEncoderId;
            }
            else if (new[] { ".tiff", ".tif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.TiffEncoderId;
            }
            else if (new[] { ".gif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.GifEncoderId;
            }
            else if (new[] { ".jpg", ".jpeg", ".jpe", ".jfif", ".jif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.JpegEncoderId;
            }
            else if (new[] { ".hdp", ".jxr", ".wdp" }.Contains(ext))
            {
                encoderId = BitmapEncoder.JpegXREncoderId;
            }
            else //if (new [] {".png"}.Contains(ext))
            {
                encoderId = BitmapEncoder.PngEncoderId;
            }

            await writeableBitmap.SaveToFile(outputFile, encoderId);

            return outputFile;
        }

        /// <summary>
        /// Saves the WriteableBitmap to the given file with the specified BitmapEncoder ID.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="encoderId">The encoder id.</param>
        /// <returns></returns>
        public static async Task SaveToFile(
            this WriteableBitmap writeableBitmap,
            StorageFile outputFile,
            Guid encoderId)
        {
            Stream stream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[(uint)stream.Length];
            await stream.ReadAsync(pixels, 0, pixels.Length);

            using (var writeStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(encoderId, writeStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)writeableBitmap.PixelWidth,
                    (uint)writeableBitmap.PixelHeight,
                    96,
                    96,
                    pixels);
                await encoder.FlushAsync();

                using (var outputStream = writeStream.GetOutputStreamAt(0))
                {
                    await outputStream.FlushAsync();
                }
            }
        }
    }

    public static class ScaledImageFile
    {
        /// <summary>
        /// Used to retrieve a StorageFile that uses qualifiers in the naming convention.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static async Task<StorageFile> Get(string relativePath)
        {
            string resourceKey = string.Format("Files/{0}", relativePath);
            var mainResourceMap = ResourceManager.Current.MainResourceMap;

            if (!mainResourceMap.ContainsKey(resourceKey))
                return null;

            return await mainResourceMap[resourceKey].Resolve().GetValueAsFileAsync();
        }
    }
}
