//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.LeftSliderGuestureUICtrl
//类名称:       LeftSliderGuestureBox
//创建时间:     2015/10/27 14:47:43
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace EyeSight.UIControl.UserControls.LeftSliderGuestureUICtrl
{
    class LeftSliderGuestureBox : ClassBase<LeftSliderGuestureBox>
    {
        public LeftSliderGuestureBox() : base() { }

        Popup popup = null;

        public async void ShowLeftSliderGuestureUIControl()
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    //HideRetry();

                    LeftSliderGuestureUIControl lsguc = new LeftSliderGuestureUIControl();

                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = lsguc;

                        popup.IsLightDismissEnabled = false;

                        popup.HorizontalOffset = 0;
                        popup.VerticalOffset = 48;

                        popup.IsOpen = true;
                    }
                }
                catch { }
            });
        }

        public async void HideLeftSliderGuestureUIControl()
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

        public bool IsLeftSliderGuestureUIControlShow
        {
            get
            {
                return popup == null ? false : true;
            }
        }
    }
}
