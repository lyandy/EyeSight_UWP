//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Setting
//类名称:       SettingViewModelExecuteCommand
//创建时间:     2015/11/11 10:51:45
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using Brain.Animate;
using EyeSight.Config;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace EyeSight.ViewModelExecuteCommand.Setting
{
    public class SettingViewModelExecuteCommand
    {
        private RelayCommand<Button> _SettingBtnClickCommand;
        public RelayCommand<Button> SettingBtnClickCommand
        {
            get
            {
                return _SettingBtnClickCommand
                    ?? (_SettingBtnClickCommand = new RelayCommand<Button>(async o =>
                    {
                        //不为null，说明点击了 清理缓存 按钮
                        if (o != null)
                        {
                            var tb = CoreVisualTreeHelper.Instance.FindVisualChildByName<TextBlock>(o, "tbClear");
                            if (tb != null)
                            {
                                if (tb.Text == "清理缓存")
                                {
                                    var messageDialog = new MessageDialog("清理缓存后，视频数据将会重新加载，特别是图片将会被重新下载。确定要清理缓存吗？", "提醒");

                                    messageDialog.Commands.Add(new UICommand("确定", async s =>
                                    {
                                        tb.Text = "正在扫描缓存文件...";

                                        tb.AnimateAsync(new FlashAnimation() { Forever = true });

                                        List<StorageFile> ls = new List<StorageFile>();

                                        //扫描视频列表Txt缓存
                                        var videoListTxtFileCacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ListFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                                        var videoListTxtFileCacheFiles = await videoListTxtFileCacheFolder.GetFilesAsync();
                                        ls.AddRange(videoListTxtFileCacheFiles);

                                        //扫描视频列表图片缓存
                                        var videoListImageFileCacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ImageFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                                        var videoListImageCacheFiles = await videoListImageFileCacheFolder.GetFilesAsync();
                                        ls.AddRange(videoListImageCacheFiles);

                                        await tb.AnimateAsync(new ResetAnimation());

                                        if (ls.Count != 0)
                                        {
                                            tb.Text = "正在清理，请稍后...";

                                            int value = 0;

                                            foreach (var file in ls)
                                            {
                                                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                                value++;

                                                double percent = value * 100 / ls.Count;

                                                tb.Text = "正在清理，请稍后... " + percent + "%";
                                            }

                                            tb.Text = "清理即将完成，请稍后...";
                                        }
                                        else
                                        {
                                            tb.Text = "未扫描到缓存数据";
                                        }

                                        await Task.Delay(800);

                                        await tb.AnimateAsync(new BounceOutLeftAnimation());
                                        tb.Text = "清理缓存";
                                        await tb.AnimateAsync(new BounceInLeftAnimation());
                                    }));

                                    messageDialog.Commands.Add(new UICommand("取消"));

                                    //默认按钮聚焦在第二个位置上，即“取消”按钮
                                    messageDialog.DefaultCommandIndex = 1;

                                    await messageDialog.ShowAsyncQueue();
                                }
                            }
                        }
                    }
                ));
            }
        }
    }
}
