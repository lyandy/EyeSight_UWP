using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.View.Past;
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
using static EyeSight.Common.CommonEnum;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class VideoTagUIControl : UserControl
    {
        public VideoTagUIControl()
        {
            this.InitializeComponent();
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (AppEnvironment.IsInternetAccess)
            {
                var control = sender as FrameworkElement;
                if (control != null)
                {
                    var model = control.DataContext as VideoTag;
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
