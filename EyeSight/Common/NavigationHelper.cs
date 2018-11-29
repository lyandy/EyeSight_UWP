//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Common
//类名称:       NavigationHelper
//创建时间:     2015/9/21 星期一 15:17:17
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.UIControl.UserControls.WPTostUICtrl;
using EyeSight.View.Daily;
using EyeSight.View.Daily.Campaign;
using EyeSight.View.Rank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EyeSight.Common
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper : DependencyObject
    {
        private Page Page { get; set; }
        private Frame Frame { get { return this.Page.Frame; } }

        public NavigationHelper(Page page)
        {
            this.Page = page;

            this.Page.Loaded += (sender, e) =>
            {
                //if (this.Page.ActualHeight == Window.Current.Bounds.Height &&
                //    this.Page.ActualWidth == Window.Current.Bounds.Width)
                //{
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                }
                else
                {
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;

                    ViewModelLocator.Instance.NavigateBackHandler += NavigateBackHandler;
                }
                //}
            };

            this.Page.Unloaded += (sender, e) =>
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                }
                else
                {
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed -=
                        this.CoreWindow_PointerPressed;

                    ViewModelLocator.Instance.NavigateBackHandler -= NavigateBackHandler;
                }
            };
        }

        private void NavigateBackHandler(ref bool handled)
        {
            if (this.GoBackCommand.CanExecute(null))
            {
                handled = true;
                this.GoBackCommand.Execute(null);
            }
        }
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.GoBackCommand.CanExecute(null) && this.Frame.BackStack.Count() >= 1)
            {
                e.Handled = true;
                this.GoBackCommand.Execute(null);
            }
            else
            {
                if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_SURE_TOGGLLESWITCH_ON, true))
                {
                    if (!WPToastBox.Instance.IsWPToastNotice)
                    {
                        WPToastBox.Instance.ShowWPToastNotice("再按一次返回键退出视野");
                        e.Handled = true;
                    }
                    else
                    {
                        //应用是否退出
                        Application.Current.Exit();
                    }
                }
                else
                {
                    //应用是否退出
                    Application.Current.Exit();
                }
            }
        }

        #region Navigation support

        RelayCommand _goBackCommand;
        RelayCommand _goForwardCommand;

        public RelayCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        () => this.GoBack(),
                        () => this.CanGoBack());
                }
                return _goBackCommand;
            }
            set
            {
                _goBackCommand = value;
            }
        }

        public RelayCommand GoForwardCommand
        {
            get
            {
                if (_goForwardCommand == null)
                {
                    _goForwardCommand = new RelayCommand(
                        () => this.GoForward(),
                        () => this.CanGoForward());
                }
                return _goForwardCommand;
            }
        }

        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }
        public virtual bool CanGoForward()
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        public async virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                try
                {
                    if ((this.Frame.Content as Page) != null && (this.Frame.Content as Page).DataContext != null && ((this.Frame.Content as Page).DataContext as ViewModelAttributeBase) != null && ((this.Frame.Content as Page).DataContext as ViewModelAttributeBase).IsBusy)
                    {
                        ((this.Frame.Content as Page).DataContext as ViewModelAttributeBase).IsBusy = false;
                    }
                    else
                    {
                        //页面一旦发生导航跳转都要隐藏重试提示
                        RetryBox.Instance.HideRetry();

                        var animationGrid = CommonHelper.Instance.GetCurrentAnimationGrid();
                        if (animationGrid != null)
                        {
                            //await animationGrid.AnimateAsync(new ResetAnimation());

                            //if (AppEnvironment.IsPhone)
                            //{
                            //    await animationGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });
                            //}
                            //else
                            //{
                            if (this.Frame.Content.GetType() == typeof(CampaignWebPage))
                            {
                                await animationGrid.AnimateAsync(new BounceOutUpAnimation());
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
                            //}
                        }

                        this.Frame.GoBack();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public virtual void GoForward()
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    e.Handled = true;
                    this.GoBackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    e.Handled = true;
                    this.GoForwardCommand.Execute(null);
                }
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed)
                return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) this.GoBackCommand.Execute(null);
                if (forwardPressed) this.GoForwardCommand.Execute(null);
            }
        }

        #endregion

        #region 生命周期管理

        private String _pageKey;

        public event LoadStateEventHandler LoadState;

        public event SaveStateEventHandler SaveState;

        public async void OnNavigatedTo(NavigationEventArgs e)
        {
            var tb = CommonHelper.Instance.GetRootPageSubTitle();
            if (tb != null)
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    var content = this.Frame.Content;
                    if (content != null)
                    {
                        if (content.GetType() == typeof(RankPage))
                        {
                            var rankSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, 0);
                            string selectedName = "周排行";
                            switch (rankSelectedIndex)
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
                            tb.Text = " / " + selectedName;
                        }
                        else
                        {
                            var name = DicStore.GetValueOrDefault<string>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_NAME, "");
                            if (string.IsNullOrEmpty(name))
                            {
                                tb.Text = "";
                            }
                            else
                            {
                                //if (!name.StartsWith("#"))
                                //{
                                //    name = "#" + name;
                                //}

                                tb.Text = " / " + name;
                            }
                        }
                    }
                }
                else
                {
                    tb.Text = "";
                }
            }

            var animationGrid = CommonHelper.Instance.GetCurrentAnimationGrid();

            if (e.NavigationMode == NavigationMode.Back)
            {
                if (animationGrid != null)
                {
                    if (e.SourcePageType == typeof(DailyPage))
                    {
                        if (AppEnvironment.IsPhone)
                        {
                            await animationGrid.AnimateAsync(new BounceInAnimation());
                        }
                    }
                    else
                    {
                        if (AppEnvironment.IsPhone)
                        {
                            await animationGrid.AnimateAsync(new FadeInRightAnimation() { Distance = 400, Duration = 0.25 });
                        }
                        else
                        {
                            await animationGrid.AnimateAsync(new FadeInRightAnimation() { Duration = 0.13, Distance = 600 });
                        }
                    }

                    await animationGrid.AnimateAsync(new ResetAnimation());
                }
            }

            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page to the
                // navigation stack
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Pass the navigation parameter to the new page
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, null));
                }
            }
            else
            {
                // Pass the navigation parameter and preserved page state to the page, using
                // the same strategy for loading suspended state and recreating pages discarded
                // from cache
                if (this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]));
                }
            }

            if (animationGrid != null)
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    if (AppEnvironment.IsPhone)
                    {
                        //await animationGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.25, Distance = 400 });
                    }
                    else
                    {
                        //await animationGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.13, Distance = 600 });
                    }
                }
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            if (this.SaveState != null)
            {
                this.SaveState(this, new SaveStateEventArgs(pageState));
            }
            frameState[_pageKey] = pageState;
        }

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.LoadState"/>event
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.SaveState"/>event
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Class used to hold the event data required when a page attempts to load state.
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// The parameter value passed to <see cref="Frame.Navigate(Type, Object)"/> 
        /// when this page was initially requested.
        /// </summary>
        public Object NavigationParameter { get; private set; }
        /// <summary>
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
        /// </summary>
        /// <param name="navigationParameter">
        /// The parameter value passed to <see cref="Frame.Navigate(Type, Object)"/> 
        /// when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </param>
        public LoadStateEventArgs(Object navigationParameter, Dictionary<string, Object> pageState)
            : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }
    }
    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// An empty dictionary to be populated with serializable state.
        /// </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        public SaveStateEventArgs(Dictionary<string, Object> pageState)
            : base()
        {
            this.PageState = pageState;
        }
    }
}
