﻿using EyeSight.Api.ApiRoot;
using EyeSight.Base;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.UIControl.UserControls.TopTapGuestureUICtrl;
using EyeSight.ViewModel.Rank;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.PlayerFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EyeSight.View.Rank
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RankPage : Page
    {
        private NavigationHelper navigationHelper;
        private RankViewModel rankViewModel;
        public RankPage()
        {
            this.InitializeComponent();

            this.videoMediaPlayer.AutoPlay = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTOPLAY_TOGGLLESWITCH_ON, true);

            if (rankViewModel == null)
            {
                if (!SimpleIoc.Default.IsRegistered<RankViewModel>())
                {
                    SimpleIoc.Default.Register<RankViewModel>();
                }

                rankViewModel = SimpleIoc.Default.GetInstance<RankViewModel>();
            }

            this.DataContext = rankViewModel;

            this.navigationHelper = new NavigationHelper(this);

            this.videoMediaPlayer.ControlPanelTemplate = AppEnvironment.IsPhone ? Application.Current.Resources["ControlPanelControlTemplatePhone"] as ControlTemplate : Application.Current.Resources["ControlPanelControlTemplatePC"] as ControlTemplate;

            this.szListView.ItemContainerStyle = AppEnvironment.IsPhone ? Application.Current.Resources["PhoneListViewItemStyle"] as Style : Application.Current.Resources["PCPastCategoryListViewItemStyle"] as Style;
            this.szListView.Padding = AppEnvironment.IsPhone ? new Thickness(0, 0, 0, 0) : new Thickness(0, 0, 0, 3);

            this.Loaded += (ss, ee) =>
            {
                if (AppEnvironment.IsPhone)
                {
                    videoMediaPlayer.IsFullScreenVisible = false;
                    this.videoMediaPlayer.DoubleTapped += videoMediaPlayer_DoubleTapped;

                    TopTapGuestureBox.Instance.ShowTopTapGuestureUIControl();
                }
                else
                {
                    videoMediaPlayer.IsFullScreenVisible = true;

                    this.KeyUp += RankPage_KeyUp;
                    //szPCDailyFlipView.SelectionChanged += szPCDailyFlipView_SelectionChanged;
                }

                szListPC.IsZoomedInViewActive = true;

                szListPC.ViewChangeStarted -= szListPC_ViewChangeStarted;
                szListPC.ViewChangeCompleted -= szListPC_ViewChangeCompleted;
                szListPC.ViewChangeStarted += szListPC_ViewChangeStarted;
                szListPC.ViewChangeCompleted += szListPC_ViewChangeCompleted;

                if (rankViewModel != null && rankViewModel.RankPerformanceCollection.Count == 0)
                {
                    rankViewModel.GetRank(rankViewModel.RankPerformanceCollection, rankViewModel.WeekCollection, ApiRankRoot.Instance.RankUrl, AppCacheNewsFileNameConst.CACHE_RANK_WEEK_FILENAME);
                }
            };

            this.Unloaded += (ss, ee) =>
            {
                //这里是用来即可激发ZoomSemanticButtonMessenger，使zoomSemanticButton马上隐藏
                if (!AppEnvironment.IsPhone)
                {
                    Messenger.Default.Send<bool>(false, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE);
                }

                if (AppEnvironment.IsPhone)
                {
                    videoMediaPlayer.IsFullScreenVisible = false;

                    this.videoMediaPlayer.DoubleTapped -= videoMediaPlayer_DoubleTapped;

                    //TopTapGuestureBox.Instance.HideTopTapGuestureUIControl();
                }
                else
                {
                    videoMediaPlayer.IsFullScreenVisible = true;

                    this.KeyUp -= RankPage_KeyUp;

                    //szPCDailyFlipView.SelectionChanged -= szPCDailyFlipView_SelectionChanged;
                    //szPCDailyFlipView.SelectedIndex = -1;
                }

                szListPC.IsZoomedInViewActive = true;
            };
        }

        private void VideoMediaPlayer_PlayerStateChanged(object sender, RoutedPropertyChangedEventArgs<Microsoft.PlayerFramework.PlayerState> e)
        {
            if (e.NewValue == PlayerState.Ending)
            {
                if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTOBACK_TOGGLLESWITCH_ON, true))
                {
                    if (this.videoMediaPlayer.IsFullScreen)
                    {
                        this.videoMediaPlayer.IsFullScreen = false;
                        ViewModelLocator.Instance.VideoFullScreen(this.videoMediaPlayer.IsFullScreen);
                    }
                    videoMediaPlayer_DoubleTapped(null, null);
                }
            }
        }

        private void RankPage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (this.videoMediaPlayer.IsFullScreen && e.Key == Windows.System.VirtualKey.Escape)
            {
                this.videoMediaPlayer.IsFullScreen = false;
                ViewModelLocator.Instance.VideoFullScreen(this.videoMediaPlayer.IsFullScreen);
            }
        }

        #region szListPC 处理逻辑
        private void szListPC_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                if (AppEnvironment.IsPhone)
                {
                    TopTapGuestureBox.Instance.HideTopTapGuestureUIControl();
                }
                else
                {
                    Messenger.Default.Send<bool>(true, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE);
                }

                szPC.ViewChangeStarted += szPC_ViewChangeStarted;
                szPC.ViewChangeCompleted += szPC_ViewChangeCompleted;
            }
            else
            {
                if (AppEnvironment.IsPhone)
                {
                    TopTapGuestureBox.Instance.ShowTopTapGuestureUIControl();
                }
                else
                {
                    Messenger.Default.Send<bool>(false, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE);
                }

                szPC.ViewChangeStarted -= szPC_ViewChangeStarted;
                szPC.ViewChangeCompleted -= szPC_ViewChangeCompleted;
            }
        }

        private void szListPC_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                var model = DicStore.GetValueOrDefault<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM
                    , null);
                if (model != null)
                {
                    this.szCategoryDetailFlipView.SelectedItem = model;
                }
            }
        }
        #endregion

        #region szPC 处理逻辑
        private void szPC_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            //这里主要是用来激发 排行榜 页面 当播放视频展现时隐藏排行榜选择按钮的Messenger
            if (!AppEnvironment.IsPhone)
            {
                Messenger.Default.Send<bool>(true, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE);
            }

            if (AppEnvironment.IsPhone)
            {
                if (e.IsSourceZoomedInView)
                {
                    Messenger.Default.Send<bool>(true, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW);
                    DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
                }
                else
                {
                    Messenger.Default.Send<bool>(false, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW);
                    DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                }
            }
        }

        private async void szPC_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                if (!AppEnvironment.IsPhone)
                {
                    ViewModelLocator.Instance.VideoFullScreenHandler += VideoFullScreenHandler;
                }

                ViewModelLocator.Instance.VideoVolumeToMuteHandler += VideoVolumeToMuteHandler;

                var model = DicStore.GetValueOrDefault<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM
                    , null);
                if (model != null)
                {
                    this.szCategoryDetailFlipView.SelectedItem = model;

                    this.videoMediaPlayer.Stop();
                    this.videoMediaPlayer.Source = null;

                    this.videoMediaPlayer.DataContext = model;
                    this.videoMediaPlayer.Volume = SettingsStore.GetValueOrDefault<double>(AppCommonConst.CURRETN_VIDEO_VOLUME_VALUE, 0.5);
                    var playUrl = await CommonHelper.Instance.DecidePlayUrl(model);
                    if (!string.IsNullOrEmpty(playUrl))
                    {
                        this.videoMediaPlayer.Source = new Uri(playUrl, UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        await new MessageDialog(AppNetworkMessageConst.VIDEO_URL_IS_ERROR, "提示").ShowAsyncQueue();

                        szPC.IsZoomedInViewActive = true;
                    }
                }
            }
            else
            {
                if (!AppEnvironment.IsPhone)
                {
                    appBarFullScreenBtn = null;
                    ViewModelLocator.Instance.VideoFullScreenHandler -= VideoFullScreenHandler;
                }

                appBarVolumeBtn = null;
                ViewModelLocator.Instance.VideoVolumeToMuteHandler -= VideoVolumeToMuteHandler;

                this.videoMediaPlayer.Stop();
                this.videoMediaPlayer.Source = null;
            }
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #region 视频全屏图标控制
        AppBarButton appBarFullScreenBtn = null;

        private void VideoFullScreenHandler(bool isFullScreen)
        {
            if (appBarFullScreenBtn == null)
            {
                appBarFullScreenBtn = CoreVisualTreeHelper.Instance.FindVisualChildByName<AppBarButton>(this.videoMediaPlayer, "FullScreenButton");
            }

            if (appBarFullScreenBtn != null)
            {
                if (isFullScreen)
                {
                    Messenger.Default.Send<bool>(true, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW);

                    this.videoMediaPlayer.ControlPanelTemplate = Application.Current.Resources["ControlPanelControlTemplatePhone"] as ControlTemplate;
                }
                else
                {
                    Messenger.Default.Send<bool>(false, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW);

                    this.videoMediaPlayer.ControlPanelTemplate = Application.Current.Resources["ControlPanelControlTemplatePC"] as ControlTemplate;
                }

                appBarFullScreenBtn.Icon = isFullScreen ? new SymbolIcon(Symbol.BackToWindow) : new SymbolIcon(Symbol.FullScreen);
            }
        }
        #endregion

        #region 视频声音图标控制
        AppBarButton appBarVolumeBtn = null;

        private void VideoVolumeToMuteHandler(double volume)
        {
            if (appBarVolumeBtn == null)
            {
                appBarVolumeBtn = CoreVisualTreeHelper.Instance.FindVisualChildByName<AppBarButton>(this.videoMediaPlayer, "VolumeButton");
            }

            if (appBarVolumeBtn != null)
            {
                appBarVolumeBtn.Icon = volume == 0 ? new SymbolIcon(Symbol.Mute) : new SymbolIcon(Symbol.Volume);
            }
        }
        #endregion

        #region DoubleTapped 双击 videoMediaPlayer 处理逻辑

        private void videoMediaPlayer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            szPC.IsZoomedInViewActive = true;
        }
        #endregion
    }
}
