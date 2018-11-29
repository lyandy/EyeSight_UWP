//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.WPTostUICtrl
//类名称:       WPToastBox
//创建时间:     2015/10/17 18:51:31
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

namespace EyeSight.UIControl.UserControls.WPTostUICtrl
{
    public class WPToastBox : ClassBase<WPToastBox>
    {
        public WPToastBox() : base() { }

        Popup popup = null;

        public async void ShowWPToastNotice(string msg, double second = 1)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    popup = new Popup();
                    WPToastUIControl wtuc = new WPToastUIControl();
                    wtuc.Message = msg;
                    popup.Child = wtuc;
                    popup.Margin = new Thickness(0, 0, 0, 0);
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
                                    await wtuc.HidMsg();
                                    if (popup != null)
                                    {
                                        popup.IsOpen = false;
                                        popup.Child = null;
                                        popup = null;
                                        wtuc = null;
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
                catch { }
            });
        }

        public bool IsWPToastNotice
        {
            get
            {
                return popup == null ? false : true;
            }
        }
    }
}
