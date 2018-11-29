//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       VideoSolutionToEnabledConverter
//创建时间:     2015/10/9 13:49:06
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace EyeSight.Converter
{
    public class VideoSolutionToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vm = value as Microsoft.PlayerFramework.InteractiveViewModel;
            if (vm != null)
            {
                var me = vm.MediaPlayer;
                if (me != null)
                {
                    var videolist = me.DataContext as ModelPropertyBase;
                    //只有在大于1个清晰度的时候才会弹出清晰度选择的选项
                    if (videolist != null && videolist.playInfo.Count > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}
