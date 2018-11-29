//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute
//类名称:       DailyViewModelAttribute
//创建时间:     2015/9/21 星期一 17:56:32
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Model.Daily;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Daily.ViewModelAttribute
{
    public class DailyViewModelAttribute : ViewModelAttributeBase
    {
        /// <summary>
        /// 每日精选数据集合
        /// </summary>
        public ObservableCollection<Videolist> _DailyCollection = new ObservableCollection<Videolist>();
        public ObservableCollection<Videolist> DailyCollection
        {
            get
            {
                return _DailyCollection;
            }
            set
            {
                if (_DailyCollection != value)
                {
                    _DailyCollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 每日精选数据集合
        /// </summary>
        public ObservableCollection<Videolist> _DailyFlipViewCollection = new ObservableCollection<Videolist>();
        public ObservableCollection<Videolist> DailyFlipViewCollection
        {
            get
            {
                return _DailyFlipViewCollection;
            }
            set
            {
                if (_DailyFlipViewCollection != value)
                {
                    _DailyFlipViewCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
