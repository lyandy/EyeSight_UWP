//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       VisualStateEx
//创建时间:     2015/10/19 15:16:48
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Config;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace EyeSight.Extension.DependencyObjectEx
{
    public class VisualStateEx
    {
    }

    public class DeviceTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(nameof(Device), typeof(DeviceType), typeof(DeviceTrigger), new PropertyMetadata(DeviceType.Unknown, DeviceChanged));

        public DeviceType Device
        {
            get
            {
                return (DeviceType)GetValue(DeviceProperty);
            }
            set
            {
                SetValue(DeviceProperty, value);
            }
        }

        private static void DeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (DeviceTrigger)d;
            var value = (DeviceType)e.NewValue;
            //if (AppEnvironment.IsPhone)
            //{
            //    trigger.SetActive(trigger.Device == DeviceType.Mobile);
            //}
            //else
            //{
            //    trigger.SetActive(trigger.Device == DeviceType.Desktop);
            //}
            var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
            if (qualifiers.ContainsKey("DeviceFamily"))
            {
                switch (qualifiers["DeviceFamily"])
                {
                    case "Desktop":
                        trigger.SetActive(trigger.Device == DeviceType.Desktop);
                        break;

                    case "Mobile":
                        trigger.SetActive(trigger.Device == DeviceType.Mobile);
                        break;
                }
            }
        }
    }

    public enum DeviceType
    {
        Unknown,
        Desktop,
        Mobile
    }

    public class VideoSpecialTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty VideoSpecialProperty = DependencyProperty.Register(nameof(VideoSpecial), typeof(VideoSpecialType), typeof(VideoSpecialTrigger), new PropertyMetadata(VideoSpecialType.Hide, VideoSpecialChanged));

        public VideoSpecialType VideoSpecial
        {
            get
            {
                return (VideoSpecialType)GetValue(VideoSpecialProperty);
            }
            set
            {
                SetValue(VideoSpecialProperty, value);
            }
        }

        private static void VideoSpecialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (VideoSpecialTrigger)d;
            var value = (VideoSpecialType)e.NewValue;

            var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
            if (qualifiers.ContainsKey("DeviceFamily"))
            {
                switch (qualifiers["DeviceFamily"])
                {
                    case "Desktop":
                        var mp = CommonHelper.Instance.GetCurrentPlayerFramework();
                        if (mp != null && mp.IsFullScreen)
                        {
                            trigger.SetActive(trigger.VideoSpecial == VideoSpecialType.Show);
                        }
                        else
                        {
                            trigger.SetActive(trigger.VideoSpecial == VideoSpecialType.Hide);
                        }
                        break;
                    case "Mobile":
                        trigger.SetActive(trigger.VideoSpecial == VideoSpecialType.Show);
                        break;
                }
            }
            
        }
    }

    public enum VideoSpecialType
    {
        Show,
        Hide
    }

}
