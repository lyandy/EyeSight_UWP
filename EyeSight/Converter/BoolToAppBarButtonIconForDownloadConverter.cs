﻿
//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       BoolToAppBarButtonIconForDownloadConverter
//创建时间:     2015/10/28 11:06:08
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
    public class BoolToAppBarButtonIconForDownloadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return (bool)value ? new SymbolIcon(Symbol.Accept) : new SymbolIcon(Symbol.Download);
            }
            else
            {
                return new SymbolIcon(Symbol.Download);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new SymbolIcon(Symbol.Download);
        }
    }
}
