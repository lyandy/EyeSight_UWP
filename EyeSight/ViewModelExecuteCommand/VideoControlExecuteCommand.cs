//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Daily
//类名称:       VideoControlExecuteCommand
//创建时间:     2015/9/24 星期四 11:38:58
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.UIControl.UserControls.VideoSolutionUICtrl;
using EyeSight.UIControl.UserControls.VolumeUICtrl;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EyeSight.ViewModelExecuteCommand
{
    public class VideoControlExecuteCommand
    {
        private RelayCommand<object> _VolumeChangeCommand;
        public RelayCommand<object> VolumeChangeCommand
        {
            get
            {
                return _VolumeChangeCommand
                    ?? (_VolumeChangeCommand = new RelayCommand<object>( o =>
                    {
                        double horizontalOffset;
                        double verticalOffset;

                        if (AppEnvironment.IsPhone)
                        {
                            horizontalOffset = Window.Current.Bounds.Width - 45;
                            verticalOffset = (Window.Current.Bounds.Height - 185) / 2;
                        }
                        else
                        {
                            horizontalOffset = Window.Current.Bounds.Width - 265;
                            if (Window.Current.Bounds.Width >= 560)
                            {
                                verticalOffset = Window.Current.Bounds.Height - 95;
                            }
                            else
                            {
                                verticalOffset = Window.Current.Bounds.Height - 118;
                            }
                        }

                        var vm = o as Microsoft.PlayerFramework.InteractiveViewModel;
                        if (vm != null)
                        {
                            VolumeBox.Instance.ShowVolume(horizontalOffset, verticalOffset, vm);
                        }

                    }, o => { return o == null ? false : true; }
                    ));
            }
        }

        private RelayCommand<object> _FullScreenChangeCommand;
        public RelayCommand<object> FullScreenChangeCommand
        {
            get
            {
                return _FullScreenChangeCommand
                    ?? (_FullScreenChangeCommand = new RelayCommand<object>(o =>
                    {
                        var vm = o as Microsoft.PlayerFramework.InteractiveViewModel;
                        if (vm != null)
                        {
                            var me = vm.MediaPlayer;
                            if (me != null)
                            {
                                me.IsFullScreen = !me.IsFullScreen;
                                ViewModelLocator.Instance.VideoFullScreen(me.IsFullScreen);

                                //var mp = CommonHelper.Instance.GetCurrentPlayerFramework();
                                //if (mp != null)
                                //{
                                //    var ctrlTemplate = mp.ControlPanelTemplate;
                                //    var grid = CoreVisualTreeHelper.Instance.FindVisualChildByName<Grid>(ctrlTemplate, "grid");
                                //    if (grid != null)
                                //    {
                                //        VisualStateManager.GoToState(mp, "VideoSpecialShow", true);
                                //    }
                                //}
                            }
                        }

                    }, o => { return o == null ? false : true; }
                    ));
            }
        }

        private RelayCommand<object> _VideoSolutionChangeCommand;
        public RelayCommand<object> VideoSolutionChangeCommand
        {
            get
            {
                return _VideoSolutionChangeCommand
                    ?? (_VideoSolutionChangeCommand = new RelayCommand<object>(o =>
                    {
                        double horizontalOffset;
                        double verticalOffset;

                        if (AppEnvironment.IsPhone)
                        {
                            horizontalOffset = Window.Current.Bounds.Width - 97.5;
                            verticalOffset = Window.Current.Bounds.Height - 165;
                        }
                        else
                        {
                            horizontalOffset = Window.Current.Bounds.Width - 95;
                            if (Window.Current.Bounds.Width >= 560)
                            {
                                verticalOffset = Window.Current.Bounds.Height - 143;
                            }
                            else
                            {
                                verticalOffset = Window.Current.Bounds.Height - 167;
                            }
                        }

                        var vm = o as Microsoft.PlayerFramework.InteractiveViewModel;
                        if (vm != null)
                        {
                            var me = vm.MediaPlayer;
                            if (me != null)
                            {
                                var videolist = me.DataContext as ModelPropertyBase;
                                //只有在大于1个清晰度的时候才会弹出清晰度选择的选项
                                if (videolist != null && videolist.playInfo.Count > 1)
                                {
                                    VideoSolutionBox.Instance.ShowVideoSolution(horizontalOffset, verticalOffset, vm, videolist.playInfo);
                                }
                            }
                        }

                    }, o => { return o == null ? false : true; }
                    ));
            }
        }

        private RelayCommand<object> _VideoSourceChangeCommand;
        public RelayCommand<object> VideoSourceChangeCommand
        {
            get
            {
                return _VideoSourceChangeCommand
                    ?? (_VideoSourceChangeCommand = new RelayCommand<object>( o =>
                    {
                        Task.Run(() =>
                        {
                            var model = o as ModelPropertyBase;
                            if (model != null)
                            {

                                if (model.playInfo.Count > 1)
                                {
                                    var p = from m in model.playInfo
                                            where m.name == (AppEnvironment.IsWlanOrInternet ? "高清" : "标清")
                                            select m;
                                    var playInfo = p.FirstOrDefault() as Playinfo;
                                    if (playInfo != null)
                                    {
                                        DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, model.playInfo.IndexOf(playInfo));

                                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                        {
                                            var mp = CommonHelper.Instance.GetCurrentPlayerFramework();

                                            if (mp != null)
                                            {
                                                mp.Source = new Uri(playInfo.url, UriKind.RelativeOrAbsolute);

                                                //借助此属性来控制VideoSolutionButton的可用性。因为IsEnabled="{Binding IsFastForwardEnabled}"
                                                mp.IsFastForwardEnabled = true;
                                            }
                                        });
                                        
                                    }
                                }
                                else if (model.playInfo.Count == 1)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        var mp = CommonHelper.Instance.GetCurrentPlayerFramework();

                                        if (mp != null)
                                        {
                                            mp.Source = new Uri(model.playInfo[0].url, UriKind.RelativeOrAbsolute);

                                            mp.IsFastForwardEnabled = false;
                                        }
                                    });
                                }
                            }
                        });
                        
                    }, o => { return o == null ? false : true; }
                    ));
            }
        }

        private RelayCommand<object> _VideoBackCommand;
        public RelayCommand<object> VideoBackCommand
        {
            get
            {
                return _VideoBackCommand
                    ?? (_VideoBackCommand = new RelayCommand<object>(o =>
                    {
                        var vm = o as Microsoft.PlayerFramework.InteractiveViewModel;
                        if (vm != null)
                        {
                            var me = vm.MediaPlayer;
                            if (me != null)
                            {
                                //如果是
                                if (AppEnvironment.IsPhone)
                                {
                                    var szPC = CommonHelper.Instance.GetCurrentSemanticZoom("szPC");
                                    if (szPC != null)
                                    {
                                        szPC.IsZoomedInViewActive = true;
                                    }
                                }
                                //如果
                                else
                                {
                                    if (me.IsFullScreen)
                                    {
                                        me.IsFullScreen = false;
                                        ViewModelLocator.Instance.VideoFullScreen(me.IsFullScreen);
                                    }
                                }
                            }
                        }

                    }, o => { return o == null ? false : true; }
                    ));
            }
        }
    }
}
