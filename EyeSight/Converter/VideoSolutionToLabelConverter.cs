//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       VideoSolutionToLabelConverter
//创建时间:     2015/10/9 13:25:54
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using Microsoft.PlayerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace EyeSight.Converter
{
    public class VideoSolutionToLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == MediaQuality.HighDefinition.ToString() ? "高清" : "标清";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return "出错";
        }
    }
}
