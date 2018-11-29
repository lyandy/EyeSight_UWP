using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class CategoryDetailCommonUIControl : UIControlBase
    {
        public CategoryDetailCommonUIControl()
        {
            this.InitializeComponent();

            if (AppEnvironment.IsPhone)
            {
                this.grid.Width = Window.Current.Bounds.Width;

                this.descGrid.RowDefinitions[3].Height = new GridLength(140);

                this.bottomSP.Margin = new Thickness(7, -50, 0, 0);
                this.appBarFavorite.Margin = new Thickness(0, -20, -27, 0);
                this.appBarDownload.Margin = new Thickness(0, -20, 45, 0);
                this.appBarCopy.Margin = new Thickness(0, -20, 143, 0);

                //this.descGrid.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
                //this.descGrid.ManipulationStarted += DescGrid_ManipulationStarted;
                //this.descGrid.ManipulationDelta += DescGrid_ManipulationDelta;

                //this.imageGrid.Visibility = Visibility.Visible;
                //this.animationImageGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.grid.Width = Window.Current.Bounds.Width - 46;

                this.descGrid.RowDefinitions[3].Height = new GridLength(70);

                this.bottomSP.Margin = new Thickness(7, -5, 0, 0);

                //this.imageGrid.Visibility = Visibility.Collapsed;
                //this.animationImageGrid.Visibility = Visibility.Visible;
            }

            this.grid.Height = Window.Current.Bounds.Height - 46;
            this.Loaded += (ss, ee) =>
            {
                this.coverPerformGrid.PointerPressed += coverPerformGrid_PointerPressed;
                this.coverPerformGrid.PointerExited += coverPerformGrid_PointerExited;
                this.coverPerformGrid.PointerReleased += coverPerformGrid_PointerReleased;
                this.coverPerformGrid.PointerCaptureLost += coverPerformGrid_PointerCaptureLost;
                this.coverPerformGrid.Tapped += coverPerformGrid_Tapped;

                if (AppEnvironment.IsPhone)
                {
                    this.descGrid.DoubleTapped += DescGrid_DoubleTapped;

                    this.descGrid.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.TranslateInertia | ManipulationModes.System;
                    this.descGrid.ManipulationStarted += DescGrid_ManipulationStarted;
                    this.descGrid.ManipulationDelta += DescGrid_ManipulationDelta;
                }
            };

            this.Unloaded += (ss, ee) =>
            {
                this.coverPerformGrid.PointerPressed -= coverPerformGrid_PointerPressed;
                this.coverPerformGrid.PointerExited -= coverPerformGrid_PointerExited;
                this.coverPerformGrid.PointerReleased -= coverPerformGrid_PointerReleased;
                this.coverPerformGrid.PointerCaptureLost -= coverPerformGrid_PointerCaptureLost;
                this.coverPerformGrid.Tapped -= coverPerformGrid_Tapped;

                this.descGrid.DoubleTapped -= DescGrid_DoubleTapped;

                this.descGrid.ManipulationMode = ManipulationModes.None;
                this.descGrid.ManipulationStarted -= DescGrid_ManipulationStarted;
                this.descGrid.ManipulationDelta -= DescGrid_ManipulationDelta;
            };
        }

        private void DescGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var szListPC = CommonHelper.Instance.GetCurrentSemanticZoom("szListPC");
            if (szListPC != null)
            {
                szListPC.IsZoomedInViewActive = true;
            }
        }

        private void descGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.imageGrid.Height = this.grid.Height - this.descGrid.ActualHeight + 0.2;
            this.clipRect.Rect = new Rect(0, 0, AppEnvironment.IsPhone ? Window.Current.Bounds.Width : Window.Current.Bounds.Width - 46, this.imageGrid.Height);
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            if (AppEnvironment.IsPhone)
            {
                this.grid.Width = Window.Current.Bounds.Width;
            }
            else
            {
                this.grid.Width = Window.Current.Bounds.Width - 46;
            }
            this.grid.Height = Window.Current.Bounds.Height - 46;

            this.imageGrid.Height = this.grid.Height - this.descGrid.ActualHeight + 0.2;
            this.clipRect.Rect = new Rect(0, 0, AppEnvironment.IsPhone ? Window.Current.Bounds.Width : Window.Current.Bounds.Width - 46, this.imageGrid.Height);
        }

        #region 黑色遮罩操作 和 上划手势识别

        Point start;
        private void DescGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                int threshold = 300;
                if (start.Y - e.Position.Y > threshold) //swipe left
                {
                    e.Complete();
                    DescGrid_DoubleTapped(null, null);
                }
            }
        }

        private void DescGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            start = e.Position;
        }

        bool isPress = false;

        DispatcherTimer timer = null;

        private void coverPerformGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (!isPress)
            {
                OnMyUIControlBaseNeedNavigate(sender, null);
            }
        }

        private void coverPerformGrid_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (isPress)
            {
                isPress = false;
                OnMyUIControlBaseNeedReleaseHolding(sender, e);
            }
        }

        private void coverPerformGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (isPress)
            {
                OnMyUIControlBaseNeedNavigate(sender, e);
            }
        }

        private void coverPerformGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (isPress)
            {
                isPress = false;
                OnMyUIControlBaseNeedReleaseHolding(sender, e);
            }
        }

        private void coverPerformGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            int count = 0;

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (timer == null)
            {
                timer = new DispatcherTimer();
            }

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += (ss, ee) =>
            {
                count++;

                if (count > 1)
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                        isPress = true;
                    }
                    OnMyUIControlBaseNeedHolding(sender, e);
                }
            };
            timer.Start();
        }
        #endregion

        private async void OnMyUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            await this.coverGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });
        }

        private async void OnMyUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            await this.coverGrid.AnimateAsync(new FadeInAnimation() { Duration = 0.1 });
        }

        private async void OnMyUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control != null)
            {
                var model = control.DataContext as ModelPropertyBase;
                if (model != null)
                {
                    var szPC = CommonHelper.Instance.GetCurrentSemanticZoom("szPC");
                    if (szPC != null)
                    {
                        if (model.isAleadyDownload)
                        {
                            DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);
                            szPC.IsZoomedInViewActive = false;
                        }
                        else if (AppEnvironment.IsInternetAccess)
                        {
                            if (AppEnvironment.IsWlanOrInternet)
                            {
                                DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);
                                szPC.IsZoomedInViewActive = false;
                            }
                            else
                            {
                                bool isPlayAccepted = false;

                                var messageDialog = new MessageDialog(AppNetworkMessageConst.NETWORK_IS_NOT_WIFI_OR_LAN_TO_PLAY, "播放提示");

                                messageDialog.Commands.Add(new UICommand("继续", new UICommandInvokedHandler(uc =>
                                {
                                    isPlayAccepted = true;
                                })));

                                messageDialog.Commands.Add(new UICommand("取消"));

                                messageDialog.DefaultCommandIndex = 1;

                                await messageDialog.ShowAsyncQueue();

                                if (isPlayAccepted)
                                {
                                    DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);
                                    szPC.IsZoomedInViewActive = false;
                                }
                            }
                        }
                        else
                        {
                            await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
                        }
                    }
                }
            }
        }


        //private void descGrid_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    var szPhone = CommonHelper.Instance.GetCurrentSemanticZoom("szPhone");
        //    if (szPhone != null)
        //    {
        //        szPhone.IsZoomedInViewActive = false;
        //    }
        //}
    }
}
