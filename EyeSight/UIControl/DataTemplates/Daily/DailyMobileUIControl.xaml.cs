using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.View.Daily.Campaign;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class DailyMobileUIControl : UIControlBase
    {
        public DailyMobileUIControl()
        {
            this.InitializeComponent();

            this.grid.Width = Window.Current.Bounds.Width;
            this.grid.Height = this.grid.Width * 2 / 3;
        }

        protected override async void OnUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedReleaseHolding(sender, e);

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            if (IsCampaign)
            {
                await this.coverGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });
            }
            else
            {
                await this.coverGrid.AnimateAsync(new FadeInAnimation() { Duration = 0.1 });
            }
        }

        private bool IsCampaign
        {
            get
            {
                var model = this.grid.DataContext as ModelPropertyBase;
                if (model != null)
                {
                    if (model.duration == 0 && string.IsNullOrEmpty(model.category))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        protected override async void OnUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedHolding(sender, e);
            if (IsCampaign)
            {
                await this.coverGrid.AnimateAsync(new FadeInAnimation() { Duration = 0.1 });
            }
            else
            {
                await this.coverGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });
            }
            

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
            ViewModelLocator.Instance.ListViewScrollHandler += resetCoverGridState;
        }

        protected override async void OnUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedNavigate(sender, e);

            //throw new NullReferenceException();

            var control = sender as FrameworkElement;
            if (control != null)
            {
                var model = control.DataContext as ModelPropertyBase;
                if (model != null)
                {
                    //说明此处是专题
                    if (model.duration == 0 && string.IsNullOrEmpty(model.category))
                    {
                        if (AppEnvironment.IsInternetAccess)
                        {
                            var animationGrid = CommonHelper.Instance.GetCurrentAnimationGrid();
                            if (animationGrid != null)
                            {
                                if (AppEnvironment.IsPhone)
                                {
                                    await animationGrid.AnimateAsync(new BounceOutDownAnimation());
                                }
                            }

                            DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);

                            CommonHelper.Instance.NavigateWithOverride(typeof(CampaignWebPage));

                            await this.coverGrid.AnimateAsync(new ResetAnimation());

                            if (animationGrid != null)
                            {
                                await animationGrid.AnimateAsync(new ResetAnimation());
                            }
                        }
                        else
                        {
                            await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
                        }
                    }
                    else
                    {
                        var szPhone = CommonHelper.Instance.GetCurrentSemanticZoom("szPhone");
                        if (szPhone != null)
                        {
                            if (szPhone.IsZoomedInViewActive == true)
                            {
                                DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);

                                szPhone.IsZoomedInViewActive = false;

                                ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

                                await this.coverGrid.AnimateAsync(new ResetAnimation());
                            }
                        }
                    }
                }
            }
            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
        }

        private void resetCoverGridState()
        {
            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            if (IsCampaign)
            {
                this.coverGrid.Opacity = 0.0;
            }
            else
            {
                this.coverGrid.Opacity = 1.0;
            }

            Debug.WriteLine("我执行了resetCoverGridState");
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);
        }

        bool isLoaded = false;
        private async void grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //虚拟化会重新执行此方法绑定数据
            //await grid.AnimateAsync(new FadeInLeftAnimation());
            //await grid.AnimateAsync(new BounceInDownAnimation());

            if (IsCampaign)
            {
                this.coverGrid.Opacity = 0.0;
            }
            else
            {
                this.coverGrid.Opacity = 1.0;
            }

            var g = sender as Grid;

            if (g != null && !isLoaded)
            {
                await Task.Delay(500);

                var animationName = new FadeInDownAnimation();
                animationName.Distance = 150;
                g.AnimateAsync(animationName);

                isLoaded = true;
            }
        }
    }
}
