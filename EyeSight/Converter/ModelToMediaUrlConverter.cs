//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Converter
//类名称:       ModelToMediaUrlConverter
//创建时间:     2015/9/22 星期二 12:16:35
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace EyeSight.Converter
{
    public class ModelToMediaUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var model = value as ModelPropertyBase;
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
                        return playInfo.url;
                    }
                    else
                    {
                        return model.playUrl;
                    }
                }
                else
                {
                    return model.playUrl;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
