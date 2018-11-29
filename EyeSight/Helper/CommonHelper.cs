//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       CommonHelper
//创建时间:     2015/9/21 星期一 15:43:38
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.View;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace EyeSight.Helper
{
    public class CommonHelper : ClassBase<CommonHelper>
    {
        public CommonHelper() : base() { }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        public Grid GetCurrentAnimationGrid()
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

                if (rootFrame != null)
                {
                    Grid grid =  CoreVisualTreeHelper.Instance.FindVisualChildByName<Grid>(rootFrame.Content as Page, "AnimationGrid");
                    if (grid != null)
                    {
                        return grid;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ListView GetCurrentFavoriteListView()
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

                if (rootFrame != null)
                {
                    ListView lv = CoreVisualTreeHelper.Instance.FindVisualChildByName<ListView>(rootFrame.Content as Page, "szListView");
                    if (lv != null)
                    {
                        return lv;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ListView GetCurrentDownloadListView()
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

                if (rootFrame != null)
                {
                    ListView lv = CoreVisualTreeHelper.Instance.FindVisualChildByName<ListView>(rootFrame.Content as Page, "szListView");
                    if (lv != null)
                    {
                        return lv;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Microsoft.PlayerFramework.MediaPlayer GetCurrentPlayerFramework()
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

                if (rootFrame != null)
                {
                    Microsoft.PlayerFramework.MediaPlayer mp = CoreVisualTreeHelper.Instance.FindVisualChildByName<Microsoft.PlayerFramework.MediaPlayer>(rootFrame.Content as Page, "videoMediaPlayer");

                    //int count = 0;
                    //while(mp == null)
                    //{
                    //    mp = CoreVisualTreeHelper.Instance.FindVisualChildByName<Microsoft.PlayerFramework.MediaPlayer>(rootFrame.Content as Page, "videoMediaPlayer");
                    //    if (mp != null)
                    //    {
                    //        Debug.WriteLine("我找到了，耗时：" + ++count * 1 + "毫秒");
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        await Task.Delay(1);
                    //    }
                    //}
                    if (mp != null)
                    {
                        return mp;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public SemanticZoom GetCurrentSemanticZoom(string name)
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

                if (rootFrame != null)
                {
                    SemanticZoom sz = CoreVisualTreeHelper.Instance.FindVisualChildByName<SemanticZoom>(rootFrame.Content as Page, name);
                    if (sz != null)
                    {
                        return sz;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public TextBlock GetRootPageSubTitle()
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                var tb = (TextBlock)rootPage.FindName("tbSubTitle");

                return tb;
            }
            else
            {
                return null;
            }
        }

        public void NavigateWithOverride(Type t, object parameter = null)
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

                if (rootFrame != null)
                {
                    if (parameter != null)
                    {
                        rootFrame.Navigate(t, parameter);
                    }
                    else
                    {
                        rootFrame.Navigate(t);
                    }
                }
            }
        }

        public void ClearFrameBackStack()
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");
                if (rootFrame != null)
                {
                    rootFrame.BackStack.Clear();
                }
            }
        }

        public async Task<BitmapImage> LoadImageSource(string imagePath)
        {
            //BitmapImage比较奇怪，必须在UI线程进行操作
            BitmapImage img = null;
            //最好不要使用Windows.UI.Core.CoreDispatcherPriority.Low，因为会发生线程的优先级反转，即低优先级的阻塞了高优先级的线程
            await DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                using (var stream = await (await StorageFile.GetFileFromApplicationUriAsync(new Uri(imagePath, UriKind.RelativeOrAbsolute))).OpenAsync(FileAccessMode.Read))
                {
                    img = new BitmapImage();
                    using (stream)
                    {
                        await img.SetSourceAsync(stream);
                    }

                    //设置解析图像的宽和高
                    //try
                    //{
                    //    img.DecodePixelHeight = Convert.ToInt32(Window.Current.Bounds.Height);
                    //    img.DecodePixelWidth = Convert.ToInt32(Window.Current.Bounds.Width);
                    //}
                    //catch (Exception ex)
                    //{
                    //    LogHelper.WriteLog(LogType.Theme, "主题背景图片宽高设置失败。详细：" + ex.Message, true);
                    //}
                }
            });

            int tryCount = 0;
            //这里可能会卡死程序无法继续向下执行，所以要有一个计数。
            while (img == null)
            {
                ++tryCount;
                if (tryCount <= 20)
                {
                    await Task.Delay(1);
                }
                else
                {
                    break;
                }
            }

            Debug.WriteLine("SplashScreenImage加载完毕，耗时：" + tryCount + "毫秒");
            tryCount = 0;

            return img;
        }

        public async Task<string> DecidePlayUrl(ModelPropertyBase model)
        {
            string playUrl = null;

            var mp = GetCurrentPlayerFramework();
            if (mp != null)
            {
                mp.StartupPosition = TimeSpan.Zero;
                mp.StartTime = TimeSpan.Zero;
                
                if (model != null)
                {
                    //选出已经缓存的下载的PlayInfoModel
                    var playInfoModel = await FindHasDownloadVideoPlayInfoModel(model);
                    if (AppEnvironment.IsInternetAccess)
                    {
                        //如果有网络，并且此条目已经被下载，则优先加载下载后的资源，这样速度更快
                        if (model.isAleadyDownload && playInfoModel != null)
                        {
                            if (model.playInfo.Count > 1)
                            {
                                DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, model.playInfo.IndexOf(playInfoModel));
                                mp.IsFastForwardEnabled = true;
                            }
                            else
                            {
                                mp.IsFastForwardEnabled = false;
                            }

                            playUrl = playInfoModel.url;
                        }
                        else
                        {
                            if (model.playInfo.Count > 1)
                            {
                                var p = from m in model.playInfo
                                        where m.name == (AppEnvironment.IsWlanOrInternet && SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_HIGHQUALITY_TOGGLLESWITCH_ON, true) ? "高清" : "标清")
                                        select m;
                                var playInfo = p.FirstOrDefault() as Playinfo;
                                if (playInfo != null)
                                {
                                    DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, model.playInfo.IndexOf(playInfo));
                                    mp.IsFastForwardEnabled = true;
                                    playUrl = playInfo.url;
                                }
                                else
                                {
                                    mp.IsFastForwardEnabled = false;
                                    playUrl = model.playUrl;
                                }
                            }
                            else
                            {
                                mp.IsFastForwardEnabled = false;
                                playUrl = model.playUrl;
                            }
                        }
                    }
                    else
                    {
                        //没有网络则加载本地缓存的。
                        //这里加载本地的缓存的链接。 并且设为不可切换清晰度
                        mp.IsFastForwardEnabled = false;
                        if (playInfoModel != null)
                        {
                            playUrl = playInfoModel.url;
                        }
                    }
                }
                else
                {
                    mp.IsFastForwardEnabled = false;
                    playUrl = null;
                }
            }
            else
            {
                mp.IsFastForwardEnabled = false;
                playUrl = null;
            }

            return await PlayUrlConverte(playUrl);
        }

        public async Task<Playinfo> FindHasDownloadVideoPlayInfoModel(ModelPropertyBase model)
        {
            Playinfo pi = null;
            foreach (var m in model.playInfo)
            {
                try
                {
                    var videoName = CommonHelper.Instance.AnalyzeRealVideoName(m.url);
                    if (videoName != null)
                    {
                        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.VideoFileDownloadRelativePath, CreationCollisionOption.OpenIfExists);
                        var file = await folder.TryGetItemAsync(videoName);
                        if (file != null)
                        {
                            pi = m;
                            break;
                        }
                    }
                }
                catch { }
            }

            return pi;
        }

        public async Task<string> PlayUrlConverte(string downloadUrl)
        {
            try
            {
                var videoName = CommonHelper.Instance.AnalyzeRealVideoName(downloadUrl);
                if (videoName != null)
                {
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.VideoFileDownloadRelativePath, CreationCollisionOption.OpenIfExists);
                    var file = await folder.TryGetItemAsync(videoName);
                    if (file != null)
                    {
                        return file.Path;
                    }
                    else
                    {
                        return downloadUrl;
                    }
                }
                else
                {
                    return downloadUrl;
                }
            }
            catch
            {
                return downloadUrl;
            }
        }

        public string DecideDownloadUrl(ModelPropertyBase model)
        {
            string playUrl = null;

            if (model != null)
            {
                if (model.playInfo.Count > 1)
                {
                    var p = from m in model.playInfo
                            where m.name == (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTO_DOWNLOAD_HIGHT_QUALITY_VIDEO, true) ? "高清" : "标清")
                            select m;
                    var playInfo = p.FirstOrDefault() as Playinfo;
                    if (playInfo != null)
                    {
                        playUrl = playInfo.url;
                    }
                    else
                    {
                        playUrl = model.playUrl;
                    }
                }
                else
                {
                    playUrl = model.playUrl;
                }
            }
            else
            {
                playUrl = null;
            }

            //WebView wv = new WebView();
            //wv.NavigationCompleted += (ss, ee) =>
            //{
            //    Debug.Write(ee.Uri.ToString());
            //};

            //wv.Source = new Uri(playUrl, UriKind.RelativeOrAbsolute);

            //var playArr = playUrl.Split(new string[] { "vid="}, StringSplitOptions.None);
            
            return playUrl;
        }

        public async Task<bool> DeleteVideos(ModelPropertyBase model)
        {
            if (model != null)
            {
                if (model.playInfo.Count > 1)
                {
                    foreach (var p in model.playInfo)
                    {
                        try
                        {
                            var videoName = CommonHelper.Instance.AnalyzeRealVideoName(p.url);
                            if (videoName != null)
                            {
                                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.VideoFileDownloadRelativePath, CreationCollisionOption.OpenIfExists);
                                var file = await folder.TryGetItemAsync(videoName);
                                if (file != null)
                                {
                                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    return true;
                }
                else
                {
                    try
                    {
                        var videoName = CommonHelper.Instance.AnalyzeRealVideoName(model.playUrl);
                        if (videoName != null)
                        {
                            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.VideoFileDownloadRelativePath, CreationCollisionOption.OpenIfExists);
                            var file = await folder.TryGetItemAsync(videoName);
                            if (file != null)
                            {
                                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                            }

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public string AnalyzeRealVideoName(string playUrl)
        {
            if (playUrl != null)
            {
                if (playUrl.Trim().ToLower().EndsWith("mp4"))
                {
                    return playUrl.Split('/').Last();
                }
                else
                {
                    var playArr = playUrl.ToLower().Split(new string[] { "vid=" }, StringSplitOptions.None);
                    if (playArr.Count() >= 2)
                    {
                        var tempStr = playArr[1];
                        tempStr = tempStr.Split('&').FirstOrDefault();
                        if (!string.IsNullOrEmpty(tempStr.Trim()))
                        {
                            return tempStr + ".mp4";
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public async void RateApp()
        {
            
            //Uri uri = new Uri(string.Format("ms-windows-store:{0}?appid={1}", "reviewapp", CurrentApp.AppId));
            Uri uri = new Uri("ms-windows-store://review/?ProductId=9nblggh5xqj9");
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
