//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.RetryUICtrl
//类名称:       RetryBox
//创建时间:     2015/9/21 星期一 16:05:59
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.WelcomeUICtrl;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace EyeSight.UIControl.UserControls.RetryUICtrl
{
    class RetryBox : ClassBase<RetryBox>
    {
        public RetryBox() : base() { }

        Popup popup = null;


        public async void ShowRetry(string msg, Type fromType, string method, object[] parameters)
        {
            await DispatcherHelper.RunAsync(async () =>
            {
                try
                {
                    //HideRetry();

                    RetryUIControl ruc = new RetryUIControl();
                    ruc.msg = msg;
                    ruc.fromType = fromType;
                    ruc.method = method;
                    ruc.parameters = parameters;


                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = ruc;

                        popup.IsLightDismissEnabled = false;
                        //应当占据NavigationRootPage的rootFrame区域，此区域距离左和上的距离都为48，在加上Pivot的Header头高度为45（已取消）
                        //popup.VerticalOffset = 48;
                        //if (!AppEnvironment.IsPhone)
                        //{
                        //    popup.HorizontalOffset = 48;
                        //}


                        if (DicStore.GetValueOrDefault<bool>(AppCommonConst.IS_APP_FIRST_LAUNCH, false))
                        {
                            //如果是第一次显示，并且加载出错要显示RetryBox的时候，判断欢迎屏幕是否还在，如果欢迎屏幕还在则等待2300毫秒，如果不在的话，则立即显示RetryBox。
                            //情况举例：1、立刻显示的情况：比如网络比较慢，当欢迎屏幕消失后还在加载数据，结果加载失败了，(网络和本地都加在失败，本地加载失败有两种情况，一种是本地无缓存，二是本地缓存文件结构损坏了，就是页面上没数据)，此时应该立刻显示RetryBox，不应等待2300毫秒。2、等待2300毫秒显示的情况：当网络无连接，数据加载失败，本地和网络都没能正确获取到数据，此时IsBusy已经为false，Progress已经消失，但此时的欢迎屏幕还在，此时就应高等待2300毫秒，让欢迎屏幕消失之后再显示RetryBox。这次是正确的处理逻辑
                            if (WelcomeBox.Instance.IsWelcomeShow)
                            {
                                await Task.Delay(2000);
                            }
                        }

                        popup.IsOpen = true;

                        DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_APP_FIRST_LAUNCH, false);
                    }
                }
                catch { }
            });
        }

        public async void HideRetry()
        {
            try
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    try
                    {
                        if (popup != null)
                        {
                            if (popup.IsOpen)
                            {
                                popup.IsOpen = false;
                                popup.Child = null;
                                popup = null;
                            }
                        }
                    }
                    catch { }
                });
            }
            catch
            {
                string s = string.Empty;
            }
        }
    }
}
