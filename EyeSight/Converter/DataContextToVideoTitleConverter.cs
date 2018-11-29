//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       DataContextToVideoTitleConverter
//创建时间:     2015/11/9 14:50:46
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
    public class DataContextToVideoTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var model = ((value as Microsoft.PlayerFramework.InteractiveViewModel).MediaPlayer.DataContext) as ModelPropertyBase;
                if (model != null)
                {
                    return model.title;
                }
                else
                {
                    return "请您欣赏";
                }
            }
            catch
            {
                return "请您欣赏";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return "请您欣赏";
        }
    }
}
