using Brain.Animate;
using EyeSight.Api.ApiRoot;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.Model;
using EyeSight.UIControl.UserControls.LeftSliderGuestureUICtrl;
using EyeSight.UIControl.UserControls.RankListUICtrl;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.View.About;
using EyeSight.View.Rank;
using EyeSight.View.Setting;
using EyeSight.ViewModel;
using EyeSight.ViewModel.Author;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Daily;
using EyeSight.ViewModel.Download;
using EyeSight.ViewModel.Past;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EyeSight.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NavigationRootPage : Page
    {
        private NavigationRootViewModel navigationRootViewModel;

        
        public NavigationRootPage()
        {
            this.InitializeComponent();

            if (navigationRootViewModel == null)
            {
                if (!SimpleIoc.Default.IsRegistered<NavigationRootViewModel>())
                {
                    SimpleIoc.Default.Register<NavigationRootViewModel>();
                }

                navigationRootViewModel = SimpleIoc.Default.GetInstance<NavigationRootViewModel>();
            }

            this.DataContext = navigationRootViewModel;

            this.Loaded += async (ss, ee) =>
            {
                if (!AppEnvironment.IsPhone)
                {
                    this.rootFrame.Margin = new Thickness(-1, -1, -0.5, -0.5);
                    this.appBarRefreshBtn.Visibility = Visibility.Visible;
                    InitZoomSemanticButtonMessenger();
                }
                else
                {
                    this.rootFrame.Margin = new Thickness(0, 0, 0, 0);
                    this.appBarRefreshBtn.Visibility = Visibility.Collapsed;
                }

                InitFavoriteMessenger();

                InitDownloadMessenger();

                InitVideoMessenger();

                InitRankSelectedNameMessenger();

                if (navigationRootViewModel != null)
                {
                    await navigationRootViewModel.InitCategoryPngs();
                    //获取类别展示数据
                    navigationRootViewModel.GetEyeSightCommonCollection();

                    //获取底部展示数据
                    navigationRootViewModel.GetEyeSightBottomCollection();

                    //默认选中第一条
                    lvCommonCategory.SelectedIndex = 0;

                }
            };

            this.Unloaded += (ss, ee) =>
            {
                Messenger.Default.Unregister(this);
                
                if (AppEnvironment.IsPhone)
                {
                    LeftSliderGuestureBox.Instance.HideLeftSliderGuestureUIControl();
                }
            };
        }

        #region back页面后退的处理

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            bool ignored = false;
            ViewModelLocator.Instance.NavigateBack(ref ignored);
        }
        #endregion

        #region 页面堆栈维护
        private List<Page> listPageStack = new List<Page>();
        #endregion

        #region 页面导航的处理
        //页面导航完成后
        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            //页面跳转传递参数的处理
            var parameter = e.Parameter as string;
            if (parameter != null)
            {
                this.tbTitle.Text = parameter;
            }

            //如果点击了左侧的类别，则隐藏返回按钮，同时清空导航堆栈
            if (isItemClicked)
            {
                this.backButton.Visibility = Visibility.Collapsed;
                this.rootFrame.BackStack.Clear();
                listPageStack.Clear();
            }

            //一定要放在清理的下面，顺序不可调换
            if (e.NavigationMode == NavigationMode.New)
            {
                listPageStack.Add(this.rootFrame.Content as Page);
            }

            //如果是手机，则直接隐藏后退键
            if (AppEnvironment.IsPhone)
            {
                this.backButton.Visibility = Visibility.Collapsed;
                this.zoomSemanticButton.Visibility = Visibility.Collapsed;
            }
            //如果不是手机则处理后退键的显隐性
            else
            {
                this.backButton.Visibility = this.rootFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            }

            isItemClicked = false;
        }

        private void rootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //页面一旦发生导航跳转都要隐藏重试提示
            RetryBox.Instance.HideRetry();

            if (isItemClicked)
            {
                foreach (var lps in listPageStack)
                {
                    lps.NavigationCacheMode = NavigationCacheMode.Disabled;
                }

                (this.rootFrame.Content as Page).NavigationCacheMode = NavigationCacheMode.Disabled;
            }
        }

        //是否点击了ListView
        bool isItemClicked = false;
        private async void lv_ItemClick(object sender, ItemClickEventArgs e)
        {
            DicStore.AddOrUpdateValue<string>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_NAME, "");

            var model = e.ClickedItem as NavigationRootModel;
            if (model != null)
            {
                isItemClicked = true;
                //取消上下栏ListView的选择
                var listview = sender as ListView;
                if (listview != null)
                {
                    switch (listview.Tag.ToString())
                    {
                        case "lvCommon":
                            lvBottomCategory.SelectedIndex = -1;
                            break;
                        case "lvBottom":
                            lvCommonCategory.SelectedIndex = -1;
                            break;
                    }
                }

                //及时关闭SpliteView
                if (rootSplitView.IsPaneOpen)
                {
                    rootSplitView.IsPaneOpen = false;
                }

                if (rootFrame.BackStack.Count == 0)
                {
                    if (rootFrame.SourcePageType == model.ClassType)
                    {
                        isItemClicked = false;
                        Debug.WriteLine("0我不导");
                        return;
                    }
                }
                else if (rootFrame.BackStack.Count > 0)
                {
                    if (rootFrame.BackStack[0].SourcePageType == model.ClassType)
                    {
                        isItemClicked = false;
                        Debug.WriteLine("非0我也不导");
                        return;
                    }
                }

                //页面一旦发生导航跳转都要隐藏重试提示
                RetryBox.Instance.HideRetry();

                var animationGrid = CommonHelper.Instance.GetCurrentAnimationGrid();
                if (animationGrid != null)
                {
                    var page = rootFrame.Content as Page;
                    if (page != null)
                    {
                        if (page.GetType() == typeof(SettingPage) || page.GetType() == typeof(AboutPage))
                        {
                            await animationGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.13 });
                        }
                        else
                        {
                            if (AppEnvironment.IsPhone)
                            {
                                await animationGrid.AnimateAsync(new FadeOutRightAnimation() { Duration = 0.25, Distance = 400 });
                            }
                            else
                            {
                                await animationGrid.AnimateAsync(new FadeOutRightAnimation() { Duration = 0.13, Distance = 600 });
                            }
                        }
                    }
                }

                //这里要清空非一级顶级页面的数据。例如：修复 精彩发现 当PastDetailPage 以new的方式离开时，其所包含的集合数据为清空导致的数据不正确的问题
                if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                {
                    SimpleIoc.Default.Register<PastDetailViewModel>();
                }
                var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();
                if (pastDetailViewModel != null)
                {
                    pastDetailViewModel.Cleanup();
                }

                //作者这里处理

                if (!SimpleIoc.Default.IsRegistered<AuthorDetailViewModel>())
                {
                    SimpleIoc.Default.Register<AuthorDetailViewModel>();
                }
                var authorDetailViewModel = SimpleIoc.Default.GetInstance<AuthorDetailViewModel>();
                if (authorDetailViewModel != null)
                {
                    authorDetailViewModel.Cleanup();
                }

                    rootFrame.Navigate(model.ClassType, model.Title);

                isItemClicked = false;

                if (this.appBarFavoriteBtn.IsChecked.Value)
                {
                    this.appBarFavoriteBtn.IsChecked = false;
                }

                if (this.appBarDownloadBtn.IsChecked.Value)
                {
                    this.appBarDownloadBtn.IsChecked = false;
                }

                if (animationGrid != null)
                {
                    await animationGrid.AnimateAsync(new ResetAnimation());
                }
            }
        }

        private async void lvCommonCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.appBarRefreshBtn.Visibility = !AppEnvironment.IsPhone && this.lvCommonCategory.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;

            this.appBarRankBtn.Visibility = this.lvCommonCategory.SelectedIndex == 3 ? Visibility.Visible : Visibility.Collapsed;

            this.appBarFavoriteBtn.Visibility = this.lvCommonCategory.SelectedIndex == 4 && !AppEnvironment.IsPhone ? Visibility.Visible : Visibility.Collapsed;

            this.appBarDownloadBtn.Visibility = this.lvCommonCategory.SelectedIndex == 5 && !AppEnvironment.IsPhone ? Visibility.Visible : Visibility.Collapsed;

            if (this.lvCommonCategory.SelectedIndex == 3)
            {
                this.appBarFavoriteBtn.IsChecked = false;
            }

            if (this.lvCommonCategory.SelectedIndex == 4)
            {
                this.appBarDownloadBtn.IsChecked = false;
            }

            var lvCommonCategoryIndex = this.lvCommonCategory.SelectedIndex;
            if (lvCommonCategoryIndex != -1)
            {
                for (int i = 0; i < navigationRootViewModel.EyeSightCommonCollection.Count; i++)
                {
                    if (i == lvCommonCategoryIndex)
                    {
                        navigationRootViewModel.EyeSightCommonCollection[i].IsSelected = true;
                    }
                    else
                    {
                        navigationRootViewModel.EyeSightCommonCollection[i].IsSelected = false;
                    }
                }

                var dtCtrl = this.lvCommonCategory.ContainerFromIndex(lvCommonCategoryIndex);
                if (dtCtrl != null)
                {
                    var img = CoreVisualTreeHelper.Instance.FindVisualChildByName<Image>(dtCtrl, "img");
                    if (img != null)
                    {
                        switch (lvCommonCategoryIndex)
                        {
                            case 0:
                                await img.RotateAsync(0.25, 360);
                                break;
                            case 1:
                                await img.AnimateAsync(new TurnstileRightOutAnimation());
                                await img.AnimateAsync(new TurnstileLeftInAnimation());
                                break;
                            case 2:
                                img.AnimateAsync(new FlashAnimation());
                                await img.AnimateAsync(new PulseAnimation() { MaxScale = 1.3});
                                break;
                            case 3:
                                await img.AnimateAsync(new FlipAnimation());
                                break;
                            case 4:
                                await img.AnimateAsync(new JumpAnimation() { Distance = -10});
                                break;
                            case 5:
                                await img.AnimateAsync(new FadeOutDownAnimation());
                                await img.AnimateAsync(new FadeInDownAnimation());
                                break;
                            case 6:
                                await img.AnimateAsync(new SwingAnimation() { Distance = 50 });
                                break;
                            case 7:
                                //await img.ScaleToAsync(0.3, new Point(1.5, 1.5));
                                await img.AnimateAsync(new SwingAnimation() { Distance = 50 });
                                break;
                            default:
                                await img.AnimateAsync(new FlipAnimation());
                                break;
                        }

                        img.AnimateAsync(new ResetAnimation());
                    }
                }
            }
            else
            {
                foreach (var item in navigationRootViewModel.EyeSightCommonCollection)
                {
                    item.IsSelected = false;
                }
            }
        }

        #endregion

        #region 汉堡菜单的打开和关闭
        public void splitViewToggleButton_Click(object sender, RoutedEventArgs e)
        {
            this.rootSplitView.IsPaneOpen = !this.rootSplitView.IsPaneOpen;

            this.splitViewToggleButtonIcon.RotateAsync(0.2, 90);

            if (this.rootSplitView.IsPaneOpen && AppEnvironment.IsPhone)
            {
                LeftSliderGuestureBox.Instance.HideLeftSliderGuestureUIControl();
            }
        }

        private void rootSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            this.splitViewToggleButtonIcon.RotateAsync(0.2, 0);

            if (AppEnvironment.IsPhone)
            {
                LeftSliderGuestureBox.Instance.ShowLeftSliderGuestureUIControl();
            }
        }
        #endregion

        #region 处理状态改变
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //处理CommandBar
            //if (AppEnvironment.IsPhone)
            //{
            //    if (AppEnvironment.IsPortrait)
            //    {
            //        //this.topbar.Margin = new Thickness(0, 0, 0, 0);
            //        this.headerRoot.Margin = new Thickness(48, 0, 0, 0);
            //    }
            //    else
            //    {
            //        //this.topbar.Margin = new Thickness(48, 0, 0, 0);
            //        this.headerRoot.Margin = new Thickness(0, 0, 0, 0);
            //    }
            //}
            //else
            //{
            //    //this.topbar.Margin = new Thickness(48, 0, 0, 0);
            //    this.headerRoot.Margin = new Thickness(0, 0, 0, 0);
            //}

            //处理SplitView的状态
            if (AppEnvironment.IsPortrait)
            {
                if (AppEnvironment.IsPhone)
                {
                    this.rootSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                }
            }
            else
            {
                //this.rootSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
        }
        #endregion

        #region 首页 每日精选刷新
        private async void appBarRefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
            {
                SimpleIoc.Default.Register<DailyViewModel>();
            }

            var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();

            if (dailyViewModel != null && !dailyViewModel.IsBusy && DicStore.GetValueOrDefault<bool>(AppCommonConst.DAILY_HAS_NEXT_PAGE, false))
            {
                if (AppEnvironment.IsInternetAccess)
                {
                    //及时将当前的nextPageUrl置为null
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.DAILY_NEXT_PAGE_URL, null);
                    dailyViewModel.GetDaily(dailyViewModel.DailyCollection, dailyViewModel.DailyFlipViewCollection, ApiDailyRoot.Instance.DailyUrl, AppCacheNewsFileNameConst.CACHE_DAILY_FILENAME, true);
                }
                else
                {
                    await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
                }
            }


            //try
            //{
            //    var rp = DicStore.GetValueOrDefault<RetryParameter>(AppCommonConst.RETRY_PARAMETER, null);
            //    if (rp != null)
            //    {
            //        Type fromType = rp.fromType;

            //        string method = rp.method;

            //        object[] parameters = rp.parameters;

            //        parameters[parameters.Count() - 1] = true;

            //        object o = Activator.CreateInstance(fromType);

            //        object obj2 = fromType.GetMethod(method).Invoke(o, parameters);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string s = ex.Message;
            //}
        }
        #endregion

        #region zoomSemanticButton 处理逻辑
        private void zoomSemanticButton_Click(object sender, RoutedEventArgs e)
        {
            zoomSemanticPCInViewActive();
        }

        public void InitZoomSemanticButtonMessenger()
        {
            //对后台能否能够获取关键字返回数据是否为空的通知
            Messenger.Default.Register<bool>(this, AppMessengerTokenConst.IS_ZOOMSEMANTIC_BUTTON_VISIBLE, o =>
           {
               this.zoomSemanticButton.Visibility = !AppEnvironment.IsPhone && o ? Visibility.Visible : Visibility.Collapsed;
               this.appBarRefreshBtn.Visibility = !AppEnvironment.IsPhone && this.lvCommonCategory.SelectedIndex == 0 && !o ? Visibility.Visible : Visibility.Collapsed;

               //这里是处理 排行榜 页面 当播放视频展现时隐藏排行榜选择按钮
               var szPC = CommonHelper.Instance.GetCurrentSemanticZoom("szPC");
               var content = this.rootFrame.Content;
               if (szPC != null &&content != null && content.GetType() == typeof(RankPage) && !AppEnvironment.IsPhone)
               {
                   if (szPC.IsZoomedInViewActive == true)
                   {
                       this.appBarRankBtn.Visibility = Visibility.Visible;
                   }
                   else
                   {
                       this.appBarRankBtn.Visibility = Visibility.Collapsed;
                   }
               }
           });
        }

        private void zoomSemanticPCInViewActive()
        {
            var szPC = CommonHelper.Instance.GetCurrentSemanticZoom("szPC");
            if (szPC != null && szPC.IsZoomedInViewActive == false)
            {
                szPC.IsZoomedInViewActive = true;

                return;
            }

            var szListPC = CommonHelper.Instance.GetCurrentSemanticZoom("szListPC");
            if (szListPC != null && szListPC.IsZoomedInViewActive == false)
            {
                szListPC.IsZoomedInViewActive = true;
            }
        }

        #endregion

        #region 针对 我的收藏 的处理
        public void InitFavoriteMessenger()
        {
            //对后台能否能够获取关键字返回数据是否为空的通知
            Messenger.Default.Register<bool>(this, AppMessengerTokenConst.IS_FAVORITE_FLIPVIEW_SHOW, o =>
            {
                if (!AppEnvironment.IsPhone)
                {
                    if (o)
                    {
                        this.appBarFavoriteBtn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.appBarFavoriteBtn.Visibility = Visibility.Visible;
                    }

                    if (this.lvCommonCategory.SelectedIndex != 4)
                    {
                        this.appBarFavoriteBtn.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }
        #endregion

        #region 针对 我的下载 的处理
        public void InitDownloadMessenger()
        {
            //对后台能否能够获取关键字返回数据是否为空的通知
            Messenger.Default.Register<bool>(this, AppMessengerTokenConst.IS_DOWNLOAD_FLIPVIEW_SHOW, o =>
            {
                if (!AppEnvironment.IsPhone)
                {
                    if (o)
                    {
                        this.appBarDownloadBtn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.appBarDownloadBtn.Visibility = Visibility.Visible;
                    }

                    if (this.lvCommonCategory.SelectedIndex != 5)
                    {
                        this.appBarDownloadBtn.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }
        #endregion

        #region 针对 视频播放旋转和全屏处理
        public void InitVideoMessenger()
        {
            //对后台能否能够获取关键字返回数据是否为空的通知
            Messenger.Default.Register<bool>(this, AppMessengerTokenConst.IS_PHONE_VIDEO_SHOW, o =>
            {
                if (o)
                {
                    this.rootGrid.RowDefinitions[0].Height = new GridLength(0);
                    this.rootSplitView.DisplayMode = SplitViewDisplayMode.Overlay;

                    if (AppEnvironment.IsPhone)
                    {
                        //隐藏侧滑手势响应者控件
                        LeftSliderGuestureBox.Instance.HideLeftSliderGuestureUIControl();
                    }
                    else
                    {
                        //如果是平板的话，则使其进入全屏模式
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    }
                }
                else
                {
                    this.rootGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                    this.rootSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;

                    if (AppEnvironment.IsPhone)
                    {
                        //显示侧滑手势响应者控件
                        LeftSliderGuestureBox.Instance.ShowLeftSliderGuestureUIControl();
                    }
                    else
                    {
                        //如果是平板的话，则使其退出全屏模式
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    }
                }
            });
        }
        #endregion

        #region 排行榜处理
        private void appBarRankBtn_Click(object sender, RoutedEventArgs e)
        {
            RankListBox.Instance.ShowRankList();
        }

        public void InitRankSelectedNameMessenger()
        {
            //对后台能否能够获取关键字返回数据是否为空的通知
            Messenger.Default.Register<int>(this, AppMessengerTokenConst.IS_RANK_LIST_SELECTED, o =>
            {
                string selectedName = "周排行";
                switch (o)
                {
                    case 0:
                        selectedName = "周排行";
                        break;
                    case 1:
                        selectedName = "月排行";
                        break;
                    case 2:
                        selectedName = "总排行";
                        break;
                }
                this.tbSubTitle.Text = " / " + selectedName;
            });
        }
        #endregion

        #region 我的收藏操作
        private void appBarFavoriteBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
            {
                SimpleIoc.Default.Register<CollectionViewModel>(false);
            }
            var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

            if (cvm != null)
            {
                foreach (var model in cvm.FavoriteCollection)
                {
                    model.isEditing = true;
                }
            }
        }

        private void appBarFavoriteBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
            {
                SimpleIoc.Default.Register<CollectionViewModel>(false);
            }
            var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

            if (cvm != null)
            {
                foreach (var model in cvm.FavoriteCollection)
                {
                    model.isEditing = false;
                }
            }
        }
        #endregion

        #region 我的下载操作

        public bool isDownloadEditing
        {
            get
            {
                return this.appBarDownloadBtn.IsChecked.Value;
            }
        }
        private void appBarDownloadBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
            {
                SimpleIoc.Default.Register<DownloadViewModel>(false);
            }
            var dlvm = SimpleIoc.Default.GetInstance<DownloadViewModel>();

            if (dlvm != null)
            {
                foreach (var model in dlvm.DownloadCollection)
                {
                    model.isEditing = true;
                }
            }
        }

        private void appBarDownloadBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
            {
                SimpleIoc.Default.Register<DownloadViewModel>(false);
            }
            var dlvm = SimpleIoc.Default.GetInstance<DownloadViewModel>();

            if (dlvm != null)
            {
                foreach (var model in dlvm.DownloadCollection)
                {
                    model.isEditing = false;
                }
            }
        }
        #endregion

        private async void lvBottomCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lvBottomCategoryIndex = this.lvBottomCategory.SelectedIndex;
            if (lvBottomCategoryIndex != -1)
            {
                for (int i = 0; i < navigationRootViewModel.EyeSightBottomCollection.Count; i++)
                {
                    if (i == lvBottomCategoryIndex)
                    {
                        navigationRootViewModel.EyeSightBottomCollection[i].IsSelected = true;
                    }
                    else
                    {
                        navigationRootViewModel.EyeSightBottomCollection[i].IsSelected = false;
                    }
                }

                var dtCtrl = this.lvBottomCategory.ContainerFromIndex(lvBottomCategoryIndex);
                if (dtCtrl != null)
                {
                    var img = CoreVisualTreeHelper.Instance.FindVisualChildByName<Image>(dtCtrl, "img");
                    if (img != null)
                    {
                        switch (lvBottomCategoryIndex)
                        {
                            case 0:
                                img.RotateAsync(0.3, 180);
                                await img.MoveToAsync(0.3, new Point(34, 0));
                                img.Opacity = 0;
                                await img.MoveToAsync(0.05, new Point(-34, 0)); 
                                img.Opacity = 1;
                                img.RotateAsync(0.3, 360);
                                await img.MoveToAsync(0.3, new Point(0, 0));
                                break;
                            case 1:
                                await img.AnimateAsync(new BounceAnimation() { DistanceY = -15 });
                                //await img.AnimateAsync(new TurnstileLeftInAnimation());
                                break;
                        }

                        img.AnimateAsync(new ResetAnimation());
                    }
                }
            }
            else
            {
                foreach (var item in navigationRootViewModel.EyeSightBottomCollection)
                {
                    item.IsSelected = false;
                }
            }
        }
    }
}
