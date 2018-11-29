//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.VolumeUICtrl
//类名称:       VolumeBox
//创建时间:     2015/9/24 星期四 14:22:23
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace EyeSight.UIControl.UserControls.VolumeUICtrl
{
    class VolumeBox : ClassBase<VolumeBox>
    {
        public VolumeBox() : base() { }

        Popup popup = null;

        public async void ShowVolume(double horizontalOffset, double verticalOffset, Microsoft.PlayerFramework.InteractiveViewModel vm)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    //HideRetry();

                    VolumeUIControl vuc = new VolumeUIControl();

                    vuc.vm = vm;

                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = vuc;

                        popup.IsLightDismissEnabled = true;
                        //应当占据NavigationRootPage的rootFrame区域，此区域距离左和上的距离都为48，在加上Pivot的Header头高度为45（已取消）
                        //popup.VerticalOffset = 48;
                        //if (!AppEnvironment.IsPhone)
                        //{
                        //    popup.HorizontalOffset = 48;
                        //}

                        popup.HorizontalOffset = horizontalOffset;
                        popup.VerticalOffset = verticalOffset;

                        popup.IsOpen = true;
                    }
                }
                catch { }
            });
        }

        public async void HideVolume()
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
