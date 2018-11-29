//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.RankListUICtrl
//类名称:       RankListBox
//创建时间:     2015/9/25 星期五 17:48:26
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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace EyeSight.UIControl.UserControls.RankListUICtrl
{
    class RankListBox : ClassBase<RankListBox>
    {
        public RankListBox() : base() { }

        Popup popup = null;

        public async void ShowRankList()
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    RankListUIControl rlu = new RankListUIControl();

                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = rlu;

                        popup.IsLightDismissEnabled = true;

                        popup.VerticalOffset = 47;
                        popup.HorizontalOffset = Window.Current.Bounds.Width - 100;

                        popup.IsOpen = true;

                        Canvas.SetZIndex(popup, 120);
                    }
                }
                catch { }
            });
        }

        public async void HideRankList()
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
