//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.TopTapGuestureUICtrl
//类名称:       TopTapGuestureBox
//创建时间:     2015/11/15 14:36:09
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

namespace EyeSight.UIControl.UserControls.TopTapGuestureUICtrl
{
    class TopTapGuestureBox : ClassBase<TopTapGuestureBox>
    {
        public TopTapGuestureBox() : base(){}

        Popup popup = null;

        public async void ShowTopTapGuestureUIControl()
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    //HideRetry();

                    TopTapGuestureUIControl lsguc = new TopTapGuestureUIControl();

                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = lsguc;

                        popup.IsLightDismissEnabled = false;

                        popup.HorizontalOffset = 48;
                        popup.VerticalOffset = 0;

                        popup.IsOpen = true;
                    }
                }
                catch { }
            });
        }

        public async void HideTopTapGuestureUIControl()
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

        public bool IsTopTapGuestureUIControlShow
        {
            get
            {
                return popup == null ? false : true;
            }
        }
    }
}
