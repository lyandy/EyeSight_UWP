//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.VideoSolutionUICtrl
//类名称:       VideoSolutionBox
//创建时间:     2015/10/9 12:19:10
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

namespace EyeSight.UIControl.UserControls.VideoSolutionUICtrl
{
    class VideoSolutionBox : ClassBase<VideoSolutionBox>
    {
        public VideoSolutionBox() : base() { }

        Popup popup = null;

        public async void ShowVideoSolution(double horizontalOffset, double verticalOffset, Microsoft.PlayerFramework.InteractiveViewModel vm, List<Playinfo> playInfoList)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                try
                {
                    //HideRetry();

                    VideoSolutionUIControl vsuc = new VideoSolutionUIControl();

                    vsuc.vm = vm;

                    vsuc.PlayInfoList = playInfoList;

                    if (popup == null)
                    {
                        popup = new Popup();
                    }

                    if (!popup.IsOpen)
                    {
                        popup.Child = vsuc;

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

        public async void HideVideoSolution()
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
