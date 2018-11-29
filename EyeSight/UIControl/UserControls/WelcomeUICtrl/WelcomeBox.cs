//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.WelcomeUICtrl
//类名称:       WelcomeBox
//创建时间:     2015/9/21 星期一 16:08:36
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace EyeSight.UIControl.UserControls.WelcomeUICtrl
{
    public class WelcomeBox : ClassBase<WelcomeBox>
    {
        public WelcomeBox() : base() { }

        Popup popup = null;


        public async void ShowWelcome()
        {
            await DispatcherHelper.RunAsync(async () =>
            {
                try
                {
                    //HideRetry();

                    WelcomeUIControl wuc = new WelcomeUIControl();

                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = wuc;

                        popup.IsLightDismissEnabled = false;

                        popup.IsOpen = true;
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }
            });
        }

        public async void HideWelcome()
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

        public bool IsWelcomeShow
        {
            get
            {
                return popup == null ? false : true;
            }
        }
    }
}
