//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.WinToastUICtrl
//类名称:       WinToastBox
//创建时间:     2015/10/15 17:28:31
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

namespace EyeSight.UIControl.UserControls.WinToastUICtrl
{
    public class WinToastBox : ClassBase<WinToastBox>
    {
        public WinToastBox() : base() { }

        Popup popup = null;
        WinToastUIControl wtuc = null;
        public async void ShowWinToastNotice(string msg, bool isFavorite,  double second = 3)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    if (popup == null)
                    {
                        popup = new Popup();

                        if (wtuc == null)
                        {
                            wtuc = new WinToastUIControl();

                            wtuc.Message = msg;
                            wtuc.IsFavorite = isFavorite;
                            popup.Child = wtuc;
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
                                            await wtuc.HidMsg();
                                            if(popup != null)
                                            {
                                                popup.IsOpen = false;
                                                popup.Child = null;
                                                wtuc = null;
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
                        if (wtuc != null)
                        {
                            wtuc.Message = msg;
                            wtuc.IsFavorite = isFavorite;
                        }
                    } 
                }
                catch { }
            });
        }
    }
}
