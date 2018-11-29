//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       VolumeToMuteConverter
//创建时间:     2015/9/24 星期四 16:07:04
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
    public class VolumeToMuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var vm = value as Microsoft.PlayerFramework.InteractiveViewModel;
                if (vm != null)
                {
                    var me = vm.MediaPlayer;
                    if (me != null)
                    {
                        return me.Volume == 0 ? new SymbolIcon(Symbol.Mute) : new SymbolIcon(Symbol.Volume);
                    }
                }
            }

            return new SymbolIcon(Symbol.Volume);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new SymbolIcon(Symbol.Volume);
        }
    }
}
