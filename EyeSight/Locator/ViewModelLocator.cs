//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Locator
//类名称:       ViewModelLocator
//创建时间:     2015/9/21 星期一 15:52:25
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.ViewModel.Daily;
using EyeSight.ViewModelExecuteCommand;
using EyeSight.ViewModelExecuteCommand.Author;
using EyeSight.ViewModelExecuteCommand.Collection;
using EyeSight.ViewModelExecuteCommand.Daily;
using EyeSight.ViewModelExecuteCommand.Download;
using EyeSight.ViewModelExecuteCommand.Past;
using EyeSight.ViewModelExecuteCommand.Rank;
using EyeSight.ViewModelExecuteCommand.Setting;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Locator
{
    public class ViewModelLocator : ClassBase<ViewModelLocator>
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator() : base()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            //SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<DailyViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<VideoControlExecuteCommand>(false);
            SimpleIoc.Default.Register<PastCategoryViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<PastDetailViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<AppBarButtonCommonExcuteCommand>(false);
            SimpleIoc.Default.Register<CollectionViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<DownloadViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<RankViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<SettingViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<AuthorViewModelExecuteCommand>(false);
            SimpleIoc.Default.Register<AuthorDetailViewModelExecuteCommand>(false);

            SimpleIoc.Default.Register<ViewModelAttributeBase>(false);
            //SimpleIoc.Default.Register<VideoCategoryViewModelExecuteCommand>(false);
        }

        public ViewModelAttributeBase BaseVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewModelAttributeBase>();
            }
        }

        public DailyViewModelExecuteCommand ExeCommandDailyVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DailyViewModelExecuteCommand>();
            }
        }

        public VideoControlExecuteCommand ExeCommandVideoCtrl
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VideoControlExecuteCommand>();
            }
        }

        public AuthorViewModelExecuteCommand ExeCommandAuthorVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AuthorViewModelExecuteCommand>();
            }
        }

        public AuthorDetailViewModelExecuteCommand ExeCommandAuthorDetailVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AuthorDetailViewModelExecuteCommand>();
            }
        }

        public PastCategoryViewModelExecuteCommand ExeCommandPastCategoryVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PastCategoryViewModelExecuteCommand>();
            }
        }

        public PastDetailViewModelExecuteCommand ExeCommandPastDetailVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PastDetailViewModelExecuteCommand>();
            }
        }
        public CollectionViewModelExecuteCommand ExeCommandCollectionVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CollectionViewModelExecuteCommand>();
            }
        }

        public DownloadViewModelExecuteCommand ExeCommandDownloadVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DownloadViewModelExecuteCommand>();
            }
        }

        public RankViewModelExecuteCommand ExeCommandRankVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RankViewModelExecuteCommand>();
            }
        }


        public AppBarButtonCommonExcuteCommand ExeCommandAppBarButtonCommonVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AppBarButtonCommonExcuteCommand>();
            }
        }

        public SettingViewModelExecuteCommand ExeCommandSettingVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingViewModelExecuteCommand>();
            }
        }

        //public VideoCategoryViewModelExecuteCommand ExeCommandVideoCategoryVM
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<VideoCategoryViewModelExecuteCommand>();
        //    }
        //}

        #region  平板模式页面导航事件委托
        public delegate void NavigateBackEventHandler(ref bool handled);
        public NavigateBackEventHandler NavigateBackHandler;
        public void NavigateBack(ref bool handled)
        {
            NavigateBackEventHandler handler = NavigateBackHandler;
            if (handler != null)
            {
                handler(ref handled);
            }
        }
        #endregion

        #region 视频 声音Mute和Volume图标控制事件委托
        public delegate void VideoVolumeToMuteEventHandler(double volume);
        public VideoVolumeToMuteEventHandler VideoVolumeToMuteHandler;
        public void VideoVolumeToMute(double volume)
        {
            VideoVolumeToMuteEventHandler handler = VideoVolumeToMuteHandler;
            if (handler != null)
            {
                handler(volume);
            }
        }
        #endregion

        #region 视频 全屏图标控制事件委托
        public delegate void VideoFullScreenEventHandler(bool isFullScreen);
        public VideoFullScreenEventHandler VideoFullScreenHandler;
        public void VideoFullScreen(bool isFullScreen)
        {
            VideoFullScreenEventHandler handler = VideoFullScreenHandler;
            if (handler != null)
            {
                handler(isFullScreen);
            }
        }
        #endregion

        #region 视频 清晰度控制事件委托
        public delegate void VideoSolutionEventHandler(string solutionName);
        public VideoSolutionEventHandler VideoSolutionHandler;
        public void VideoSolution(string solutionName)
        {
            VideoSolutionEventHandler handler = VideoSolutionHandler;
            if (handler != null)
            {
                handler(solutionName);
            }
        }
        #endregion

        #region 我的收藏 和 我的下载 ListView 滚动控制事件委托
        public delegate void FavoriteOrDownloadListViewScrollEventHandler();
        public FavoriteOrDownloadListViewScrollEventHandler FavoriteOrDownloadListViewScrollHandler;
        public void FavoriteOrDownloadListViewScroll()
        {
            FavoriteOrDownloadListViewScrollEventHandler handler = FavoriteOrDownloadListViewScrollHandler;
            if (handler != null)
            {
                handler();
            }
        }
        #endregion

        #region ListView 滚动控制事件委托
        public delegate void ListViewScrollEventHandler();
        public ListViewScrollEventHandler ListViewScrollHandler;
        public void ListViewScroll()
        {
            ListViewScrollEventHandler handler = ListViewScrollHandler;
            if (handler != null)
            {
                handler();
            }
        }
        #endregion

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
