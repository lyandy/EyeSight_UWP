using EyeSight.Api.ApiRoot;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Model.Daily;
using EyeSight.ViewModel.Daily;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using EyeSight.Helper;
using EyeSight.Base;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using EyeSight.Locator;
using Windows.UI.ViewManagement;
using Windows.UI;
using Microsoft.PlayerFramework;
using Windows.UI.Popups;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Extension;
using EyeSight.UIControl.UserControls.TopTapGuestureUICtrl;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EyeSight.View.Daily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DailyPage : Page
    {
        private NavigationHelper navigationHelper;
        private DailyViewModel dailyViewModel;
        public DailyPage()
        {
            this.InitializeComponent();

            this.videoMediaPlayer.AutoPlay = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTOPLAY_TOGGLLESWITCH_ON, true);

            if (dailyViewModel == null)
            {
                if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                {
                    SimpleIoc.Default.Register<DailyViewModel>();
                }

                dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
            }

            this.DataContext = dailyViewModel;

            this.navigationHelper = new NavigationHelper(this);

            this.videoMediaPlayer.ControlPanelTemplate = AppEnvironment.IsPhone ? Application.Current.Resources["ControlPanelControlTemplatePhone"] as ControlTemplate : Application.Current.Resources["ControlPanelControlTemplatePC"] as ControlTemplate;

            this.Loaded += (ss, ee) =>
            {
                if (AppEnvironment.IsPhone)
                {
                    szPhone.IsZoomedInViewActive = true;
                    videoMediaPlayer.IsFullScreenVisible = false;

                    this.videoMediaPlayer.DoubleTapped += videoMediaPlayer_DoubleTapped;

                    szPhone.ViewChangeStarted -= szPhone_ViewChangeStarted;
                    szPhone.ViewChangeCompleted -= szPhone_ViewChangeCompleted;
                    szPhone.ViewChangeStarted += szPhone_ViewChangeStarted;
                    szPhone.ViewChangeCompleted += szPhone_ViewChangeCompleted;

                    TopTapGuestureBox.Instance.ShowTopTapGuestureUIControl();
                }
                else
                {
                    szPhone.IsZoomedInViewActive = false;
                    videoMediaPlayer.IsFullScreenVisible = true;

                    szPC.ViewChangeStarted -= szPC_ViewChangeStarted;
                    szPC.ViewChangeCompleted -= szPC_ViewChangeCompleted;
                    szPC.ViewChangeStarted += szPC_ViewChangeStarted;
                    szPC.ViewChangeCompleted += szPC_ViewChangeCompleted;

                    this.KeyUp += DailyPage_KeyUp;
                    //szPCDailyFlipView.SelectionChanged += szPCDailyFlipView_SelectionChanged;
                }

                if (dailyViewModel != null)
                {
                    bool hasCollection = true;
                    if (AppEnvironment.IsPhone)
                    {
                        if (dailyViewModel.DailyCollection.Count == 0)
                        {
                            hasCollection = false;
                        }
                    }
                    else
                    {
                        if (dailyViewModel.DailyFlipViewCollection.Count == 0)
                        {
                            hasCollection = false;
                        }
                    }

                    if (!hasCollection)
                    {
                        dailyViewModel.GetDaily(dailyViewModel.DailyCollection, dailyViewModel.DailyFlipViewCollection, ApiDailyRoot.Instance.DailyUrl, AppCacheNewsFileNameConst.CACHE_DAILY_FILENAME, false);
                    }
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
                    szPhone.IsZoomedInViewActive = true;
                    videoMediaPlayer.IsFullScreenVisible = false;

                    this.videoMediaPlayer.DoubleTapped -= videoMediaPlayer_DoubleTapped;

                    //TopTapGuestureBox.Instance.HideTopTapGuestureUIControl();
                }
                else
                {
                    szPC.IsZoomedInViewActive = true;
                    videoMediaPlayer.IsFullScreenVisible = true;

                    this.KeyUp -= DailyPage_KeyUp;

                    //szPCDailyFlipView.SelectionChanged -= szPCDailyFlipView_SelectionChanged;
                    //szPCDailyFlipView.SelectedIndex = -1;
                }
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

        private void DailyPage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (this.videoMediaPlayer.IsFullScreen && e.Key == Windows.System.VirtualKey.Escape)
            {
                this.videoMediaPlayer.IsFullScreen = false;
                ViewModelLocator.Instance.VideoFullScreen(this.videoMediaPlayer.IsFullScreen);
            }
        }

        #region 桌面模式下的szPCDailyFlipView_SelectionChanged图片方法处理 -- 已取消
        //private void szPCDailyFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var model = this.szPCDailyFlipView.SelectedItem as Videolist;

        //    if (dailyViewModel != null)
        //    {
        //        dailyViewModel.DailyFlipViewCollection.ForEach(A =>
        //        {
        //            try
        //            {
        //                if (model.id == A.id)
        //                {
        //                    var containter = this.szPCDailyFlipView.ContainerFromItem(A);
        //                    Canvas.SetZIndex(containter as UIElement, 1000);
        //                }
        //                else
        //                {
        //                    var containterOther = this.szPCDailyFlipView.ContainerFromItem(A);
        //                    Canvas.SetZIndex(containterOther as UIElement, 999);
        //                }
        //            }
        //            catch { }
        //        });
        //    }
        //}
        #endregion

        #region szPhone 处理逻辑
        private void szPhone_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                szPC.ViewChangeStarted += szPC_ViewChangeStarted;
                szPC.ViewChangeCompleted += szPC_ViewChangeCompleted;

                if (AppEnvironment.IsPhone)
                {
                    TopTapGuestureBox.Instance.HideTopTapGuestureUIControl();
                }
            }
            else
            {
                if (AppEnvironment.IsPhone)
                {
                    TopTapGuestureBox.Instance.ShowTopTapGuestureUIControl();
                }

                szPC.ViewChangeStarted -= szPC_ViewChangeStarted;
                szPC.ViewChangeCompleted -= szPC_ViewChangeCompleted;
            }
        }
        
        private void szPhone_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                var model = DicStore.GetValueOrDefault<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM
                    , null);
                if (model != null)
                {
                    this.szPCDailyFlipView.SelectedItem = model;
                }
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

        #region szPC 处理逻辑
        private void szPC_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (AppEnvironment.IsPhone)
            {
                if (e.IsSourceZoomedInView)
                {
                    if (AppEnvironment.IsPhone)
                    {
                        Messenger.Default.Send<bool>(true, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW);
                    }
                    DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
                }
                else
                {
                    if (AppEnvironment.IsPhone)
                    {
                        Messenger.Default.Send<bool>(false, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW);
                    }
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
                    Messenger.Default.Send<bool>(true, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE);
                    ViewModelLocator.Instance.VideoFullScreenHandler += VideoFullScreenHandler;
                }

                ViewModelLocator.Instance.VideoVolumeToMuteHandler += VideoVolumeToMuteHandler;

                var model = DicStore.GetValueOrDefault<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM
                    , null);
                if (model != null)
                {
                    this.szPCDailyFlipView.SelectedItem = model;

                    this.videoMediaPlayer.Stop();
                    this.videoMediaPlayer.Source = null;

                    this.videoMediaPlayer.DataContext = model;

                    this.videoMediaPlayer.Volume = SettingsStore.GetValueOrDefault<double>(AppCommonConst.CURRETN_VIDEO_VOLUME_VALUE, 0.5);
                    var playUrl = await CommonHelper.Instance.DecidePlayUrl(model);
                    if (!string.IsNullOrEmpty(playUrl))
                    {
                        this.videoMediaPlayer.Source = new Uri(playUrl, UriKind.RelativeOrAbsolute);
                        this.videoMediaPlayer.Tag = model.title;
                    }
                    else
                    {
                        await new MessageDialog(AppNetworkMessageConst.VIDEO_URL_IS_ERROR, "提示").ShowAsyncQueue();

                        szPC.IsZoomedInViewActive = true;
                    }
                }

                //this.videoMediaPlayer.Play();

                //var ctrl = CoreVisualTreeHelper.Instance.FindVisualChildByName<Control>(this.videoMediaPlayer, "pro");
            }
            else
            {
                if (!AppEnvironment.IsPhone)
                {
                    Messenger.Default.Send<bool>(false, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE);
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

                appBarFullScreenBtn.Icon = isFullScreen  ? new SymbolIcon(Symbol.BackToWindow) : new SymbolIcon(Symbol.FullScreen);

                //appBarFullScreenBtn.Foreground = App.Current.Resources["SystemControlHighlightListAccentMediumBrush"] as Brush;

                //appBarFullScreenBtn.Foreground = new SolidColorBrush(Colors.Red);

                //没办法的办法
                var sp = CoreVisualTreeHelper.Instance.FindVisualChildByName<StackPanel>(appBarFullScreenBtn, "ContentRoot");
                if (sp != null)
                {
                    sp.Opacity = 0.7;
                }
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

        private void videoMediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            //var child = VisualTreeHelper.GetChild(this.videoMediaPlayer, 0);


            ////var pro = (((child as Grid).Children[1] as Grid).Children[0] as Grid).Children[0] as ProgressRing;

            //var pro = ((child as Grid).Children[4] as Grid).Children[0] as Control;

            //pro.Foreground = new SolidColorBrush(Colors.Red);

            //pro.Background = new SolidColorBrush(Colors.Blue);
        }
    }
}
