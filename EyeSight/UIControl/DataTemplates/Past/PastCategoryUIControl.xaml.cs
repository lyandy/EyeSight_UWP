using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.Model.Past;
using EyeSight.View.Past;
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
using Windows.UI.Xaml.Navigation;
using static EyeSight.Common.CommonEnum;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class PastCategoryUIControl : UIControlBase
    {
        CommonItemListData model = null;

        public PastCategoryUIControl()
        {
            this.InitializeComponent();
        }

        protected override async void OnUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedReleaseHolding(sender, e);

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            Debug.WriteLine("我执行了OnUIControlBaseNeedReleaseHolding");

            await this.coverGrid.AnimateAsync(new FadeInAnimation() { Duration = 0.1 });
        }

        protected override async void OnUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedHolding(sender, e);

            await this.coverGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
            ViewModelLocator.Instance.ListViewScrollHandler += resetCoverGridState;
        }

        protected override async void OnUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedNavigate(sender, e);

            if (AppEnvironment.IsInternetAccess)
            {
                var control = sender as FrameworkElement;
                if (control != null)
                {
                    if (model != null)
                    {
                        //如果解析出错
                        if (model.actionId == (int)CategoryErrorType.ERROR)
                        {
                            await new MessageDialog(AppNetworkMessageConst.DATA_PRE_COMBINE_IS_ERROR, "提示").ShowAsyncQueue();
                        }
                        //如果暂不支持此类型的跳转
                        else if (model.actionId == (int)CategoryErrorType.UNSUPPORT)
                        {
                            await new MessageDialog(AppNetworkMessageConst.DATA_UNSUPPORT, "提示").ShowAsyncQueue();
                        }
                        else
                        {
                            var animationGrid = CommonHelper.Instance.GetCurrentAnimationGrid();
                            if (animationGrid != null)
                            {
                                if (AppEnvironment.IsPhone)
                                {
                                    await animationGrid.AnimateAsync(new FadeOutLeftAnimation() { Duration = 0.25, Distance = 400 });
                                }
                                else
                                {
                                    await animationGrid.AnimateAsync(new FadeOutLeftAnimation() { Duration = 0.13, Distance = 600 });
                                }
                            }

                            DicStore.AddOrUpdateValue<string>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_NAME, model.convertName);
                            DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_ACTION_ID, model.actionId);
                            DicStore.AddOrUpdateValue<CategorySubType>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_ACTION_TYPE, model.categorySubType);

                            CommonHelper.Instance.NavigateWithOverride(typeof(PastDetailPage));

                            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

                            await this.coverGrid.AnimateAsync(new ResetAnimation());

                            if (animationGrid != null)
                            {
                                await animationGrid.AnimateAsync(new ResetAnimation());
                            }
                        }
                    }
                }
            }
            else
            {
                await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
            }

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
        }

        private void resetCoverGridState()
        {
            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            this.coverGrid.Opacity = 1.0;

            Debug.WriteLine("我执行了resetCoverGridState");
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            var with = e.NewSize.Width;

            if (with >= 700)
            {
                this.grid.Width = (with - 4 * 3) / 3;
            }
            else
            {
                this.grid.Width = (with - 3 * 3) / 2;
            }

            this.grid.Height = this.grid.Width;
        }

        bool isLoaded = false;
        private async void grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //虚拟化会重新执行此方法绑定数据
            //await grid.AnimateAsync(new FadeInLeftAnimation());
            //await grid.AnimateAsync(new BounceInDownAnimation());

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

        private void UIControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            var phoneWith = Window.Current.Bounds.Width;
            var PCWith = Window.Current.Bounds.Width - 48;

            model = this.DataContext as CommonItemListData;

            if (AppEnvironment.IsPhone)
            {
                if (model != null)
                {
                    if (model.dataType != null && model.dataType.ToLower() == CategoryType.RECTANGLECARD.ToString().ToLower())
                    {
                        this.grid.Width = phoneWith;
                        this.grid.Height = this.Width / 2;

                        this.coverGrid.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.grid.Width = (phoneWith - 2) / 2;
                        this.grid.Height = this.grid.Width;
                    }
                }
            }
            else
            {
                if (model != null)
                {
                    if (model.dataType != null && model.dataType.ToLower() == CategoryType.RECTANGLECARD.ToString().ToLower())
                    {
                        this.bgImage.Stretch = Stretch.None;
                    }
                }

                if (PCWith >= 700)
                {
                    this.grid.Width = (PCWith - 4 * 3) / 3;
                }
                else
                {
                    this.grid.Width = (PCWith - 3 * 3) / 2;
                }

                this.grid.Height = this.grid.Width;
            }
        }
    }
}
