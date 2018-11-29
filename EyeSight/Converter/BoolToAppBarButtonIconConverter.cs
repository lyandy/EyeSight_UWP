//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       BoolToAppBarButtonIconConverter
//创建时间:     2015/9/21 星期一 15:25:34
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace EyeSight.Converter
{
    public class BoolToAppBarButtonIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return (bool)value ? new SymbolIcon(Symbol.Mute) : new SymbolIcon(Symbol.Volume);
            }
            else
            {
                return new SymbolIcon(Symbol.Mute);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new SymbolIcon(Symbol.Mute);
        }
    }
}
