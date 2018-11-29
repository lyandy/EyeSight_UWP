using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Model.Author;
using EyeSight.View.Author;
using System;
using System.Collections.Generic;
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
    public sealed partial class AuthorUIControl : UIControlBase
    {
        public AuthorUIControl()
        {
            this.InitializeComponent();
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            var with = e.NewSize.Width;

            if (with < 640)
            {
                this.gridPC.Visibility = Visibility.Collapsed;
                this.gridMobile.Visibility = Visibility.Visible;

                this.grid.Width = with;
                this.grid.Height = 65;

                this.lineMobileHorizontal.X2 = this.grid.Width;
            }
            else
            {
                this.gridPC.Visibility = Visibility.Visible;
                this.gridMobile.Visibility = Visibility.Collapsed;

                if (with >= 640 && with < 900)
                {
                    this.grid.Width = (with - 3 * 2) / 2;
                }
                else if (with >= 900 && with < 1250)
                {
                    this.grid.Width = (with - 4 * 2) / 3;
                }
                else if (with >= 1250)
                {
                    this.grid.Width = (with - 5 * 2) / 4;
                }

                this.grid.Height = this.grid.Width / 2;

                this.linePCHorizontal.X2 = this.grid.Width;
                this.linePCVertical.X1 = this.linePCVertical.X2 = this.grid.Width;
                this.linePCVertical.Y2 = this.grid.Height;
            }
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
                animationName.Distance = 100;
                g.AnimateAsync(animationName);

                isLoaded = true;
            }
        }

        private void UIControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            var phoneWith = Window.Current.Bounds.Width;
            var PCWith = Window.Current.Bounds.Width - 48;

            if (AppEnvironment.IsPhone)
            {
                this.gridMobile.Visibility = Visibility.Visible;
                this.gridPC.Visibility = Visibility.Collapsed;

                this.grid.Width = phoneWith;

                this.tbTitle.MaxWidth = this.grid.Width - 105;

                this.tbDesc.MaxWidth = this.grid.Width - 105;

                this.lineMobileHorizontal.X2 = this.grid.Width;
            }
            else
            {
                if (PCWith < 640)
                {
                    this.gridPC.Visibility = Visibility.Collapsed;
                    this.gridMobile.Visibility = Visibility.Visible;

                    this.grid.Width = PCWith;
                    this.grid.Height = 65;

                    this.lineMobileHorizontal.X2 = this.grid.Width;
                }
                else
                {
                    this.gridPC.Visibility = Visibility.Visible;
                    this.gridMobile.Visibility = Visibility.Collapsed;

                    if (PCWith >= 640 && PCWith < 900)
                    {
                        this.grid.Width = (PCWith - 3 * 2) / 2;
                    }
                    else if (PCWith >= 900 && PCWith < 1250)
                    {
                        this.grid.Width = (PCWith - 4 * 2) / 3;
                    }
                    else if (PCWith >= 1250)
                    {
                        this.grid.Width = (PCWith - 5 * 2) / 4;
                    }

                    this.grid.Height = this.grid.Width / 2;

                    this.linePCHorizontal.X2 = this.grid.Width;
                    this.linePCVertical.X1 = this.linePCVertical.X2 = this.grid.Width;
                    this.linePCVertical.Y2 = this.grid.Height;
                }
            }
        }

        private async void grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (AppEnvironment.IsInternetAccess)
            {
                var control = sender as FrameworkElement;
                if (control != null)
                {
                    var model = control.DataContext as AuthorData;
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

                            //名称的记录和暂存暂时用CURRENT_PAST_CATEGORY_DETAIL_NAME字段来替代
                            DicStore.AddOrUpdateValue<string>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_NAME, AppEnvironment.IsPhone ? "" : model.title);
                            DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_AUTHOR_DETAIL_ACTION_ID, model.actionId);

                            CommonHelper.Instance.NavigateWithOverride(typeof(AuthorDetailPage));

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
        }
    }
}
