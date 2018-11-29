//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.IOEx
//类名称:       StorageFolderEx
//创建时间:     2015/9/21 星期一 15:41:38
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace EyeSight.Extension.IOEx
{
    public static class StorageFolderEx
    {
        /// <summary>
        /// 递归删除文件夹及其子文件夹和文件。注意：如果是删除KnownFolders下的，需要添加文件类型关联，即文件扩展名。
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteFolderAsync(this StorageFolder folder)
        {
            try
            {
                var folders = await folder.GetFoldersAsync();

                //首先删除自身的文件
                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }

                //遍历其子文件夹删除文件
                foreach (StorageFolder f in folders)
                {
                    await DeleteFolderAsync(f);
                }

                //删除文件夹
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 递归获取文件夹及其子文件夹下的。
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static async Task<List<StorageFile>> ArraryAllFilesAsync(this StorageFolder folder, List<StorageFile> allFiles)
        {
            try
            {
                var folders = await folder.GetFoldersAsync();

                //首先添加自身的文件
                var files = await folder.GetFilesAsync();
                allFiles.AddRange(files);

                //遍历其子文件夹获取文件
                foreach (StorageFolder f in folders)
                {
                    await f.ArraryAllFilesAsync(allFiles);
                }

                return allFiles;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 递归获取文件夹及其子文件夹下的。
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static async Task<List<StorageFile>> GetAllFilesAsync(this StorageFolder folder)
        {
            List<StorageFile> allFiles = new List<StorageFile>();

            var list = await folder.ArraryAllFilesAsync(allFiles);

            return list;
        }
        public static async Task<bool> CheckFileExistedAsync(this StorageFolder folder, string fileName)
        {
            if (folder == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                var obj = await folder.TryGetItemAsync(fileName);
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Returns the StorageFile object for a file at a given path relative to the given folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetFileByPathAsync(this StorageFolder folder, string relativePath)
        {
            if (folder == Package.Current.InstalledLocation)
            {
                string resourceKey = string.Format("Files/{0}", relativePath);
                var mainResourceMap = ResourceManager.Current.MainResourceMap;

                if (mainResourceMap.ContainsKey(resourceKey))
                {
                    return await mainResourceMap[resourceKey].Resolve().GetValueAsFileAsync();
                }
            }

            var parts = relativePath.Split('\\', '/');

            for (int i = 0; i < parts.Length - 1; i++)
            {
                folder = await folder.GetFolderAsync(parts[i]);
            }

            string fileName = parts[parts.Length - 1];

            return await folder.GetFileAsync(fileName);
        }

        public static async Task<string> CreateTempFileNameAsync(
            this StorageFolder folder,
            string extension = ".tmp",
            string prefix = "",
            string suffix = "")
        {
            string fileName;

            if (folder == null)
            {
                folder = ApplicationData.Current.TemporaryFolder;
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = ".tmp";
            }
            else if (extension[0] != '.')
            {
                extension = string.Format(".{0}", extension);
            }

            // Try no random numbers
            if (!string.IsNullOrEmpty(prefix) &&
                !string.IsNullOrEmpty(prefix))
            {
                fileName = string.Format(
                    "{0}{1}{2}",
                    prefix,
                    suffix,
                    extension);
                if (!await folder.CheckFileExistedAsync(fileName))
                {
                    return fileName;
                }
            }

            do
            {
                fileName = string.Format(
                    "{0}{1}{2}{3}",
                    prefix,
                    Guid.NewGuid(),
                    suffix,
                    extension);
            } while (await folder.CheckFileExistedAsync(fileName));

            return fileName;
        }

        public static async Task<StorageFile> CreateTempFileAsync(this StorageFolder folder, string extension = ".tmp")
        {
            if (folder == null)
            {
                folder = ApplicationData.Current.TemporaryFolder;
            }

            var fileName = await folder.CreateTempFileNameAsync(
                extension,
                DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.ffffff"));

            var file = await folder.CreateFileAsync(
                fileName,
                CreationCollisionOption.GenerateUniqueName);

            return file;
        }
    }
}
