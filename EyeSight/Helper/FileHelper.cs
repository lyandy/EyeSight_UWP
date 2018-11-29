//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       FileHelper
//创建时间:     2015/9/21 星期一 15:47:15
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EyeSight.Helper
{
    public class FileHelper : ClassBase<FileHelper>
    {
        public FileHelper() : base() { }

        public async void SaveTextToFile(string relativePath, string fileName, string content)
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(relativePath, CreationCollisionOption.OpenIfExists);
                if (folder != null)
                {
                    var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    if (!string.IsNullOrEmpty(content) && file != null)
                    {
                        await FileIO.WriteTextAsync(file, content);
                    }
                }
            }
            catch { }
        }

        public async Task<string> ReadTextFromFile(string relativePath, string fileName)
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(relativePath, CreationCollisionOption.OpenIfExists);
                if (folder != null)
                {
                    try
                    {
                        var file = await folder.GetFileAsync(fileName);
                        string content = await FileIO.ReadTextAsync(file);
                        return content;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
