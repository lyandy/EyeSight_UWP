//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.CopyUICtrl
//类名称:       CopyBox
//创建时间:     2015/12/3 20:26:28
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

namespace EyeSight.UIControl.UserControls.CopyUICtrl
{
    public class CopyBox : ClassBase<CopyBox>
    {
        public CopyBox() : base() { }

        Popup popup = null;
        CopyUIControl cuc = null;
        public async void ShowCopyNotice( double second = 3)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    if (popup == null)
                    {
                        popup = new Popup();

                        if (cuc == null)
                        {
                            cuc = new CopyUIControl();

                            popup.Child = cuc;
                            popup.VerticalOffset = 0;
                            popup.HorizontalOffset = Window.Current.Bounds.Width;
                            popup.Opened += (s1, e1) =>
                            {
                                try
                                {
                                    DispatcherTimer timer = new DispatcherTimer();
                                    if (second == 3)
                                    {
                                        timer.Interval = TimeSpan.FromSeconds(3);
                                    }
                                    else
                                    {
                                        timer.Interval = TimeSpan.FromSeconds(second);
                                    }
                                    timer.Tick += async (s2, e2) =>
                                    {
                                        try
                                        {
                                            timer.Stop();
                                            await cuc.HidMsg();
                                            if (popup != null)
                                            {
                                                popup.IsOpen = false;
                                                popup.Child = null;
                                                cuc = null;
                                                popup = null;
                                            }

                                        }
                                        catch { }
                                    };
                                    timer.Start();
                                }
                                catch { }
                            };
                            popup.IsOpen = true;
                        }
                    }
                    else
                    {
                    }
                }
                catch { }
            });
        }
    }
}
