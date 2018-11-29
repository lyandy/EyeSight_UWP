//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       BoolToVisibilityConverter
//创建时间:     2015/9/21 星期一 15:26:37
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace EyeSight.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //paramter != null 说明是版面列表页顶部的切换模式和日期选择可见性的转换
            if (parameter != null)
            {
                if (value is string)
                {
                    var r = Boolean.Parse(value.ToString());
                    return r ? Visibility.Collapsed : Visibility.Visible;
                }
                else if (value is Visibility)
                {
                    return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            else
            {
                if (value is string)
                {
                    var r = Boolean.Parse(value.ToString());
                    return r ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    if (value is int)
                    {
                        if (int.Parse(value.ToString()) > 0)
                        {
                            return Visibility.Visible;
                        }
                        else
                        {
                            return Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
