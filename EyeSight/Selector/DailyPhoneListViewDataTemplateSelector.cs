//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Selector
//类名称:       DailyPhoneListViewDataTemplateSelector
//创建时间:     2015/9/22 星期二 17:37:02
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Model.Daily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EyeSight.Selector
{
    public class DailyPhoneListViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DailyMobileDataTemplate { get; set; }
        public DataTemplate DailyMobileTodayDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var model = item as Videolist;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.today))
                {
                    return DailyMobileTodayDataTemplate;
                }
                else
                {
                    return DailyMobileDataTemplate;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
