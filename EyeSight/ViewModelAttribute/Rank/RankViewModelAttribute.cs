//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute.Rank
//类名称:       RankViewModelAttribute
//创建时间:     2015/9/25 星期五 19:27:01
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Model.Rank;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelAttribute.Rank
{
    public class RankViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<Videolist> _RankPerformanceCollection = new ObservableCollection<Videolist>();
        public ObservableCollection<Videolist> RankPerformanceCollection
        {
            get
            {
                return _RankPerformanceCollection;
            }
            set
            {
                if (_RankPerformanceCollection != value)
                {
                    _RankPerformanceCollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Videolist> _WeekCollection = new ObservableCollection<Videolist>();
        public ObservableCollection<Videolist> WeekCollection
        {
            get
            {
                return _WeekCollection;
            }
            set
            {
                if (_WeekCollection != value)
                {
                    _WeekCollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Videolist> _MonthCollection = new ObservableCollection<Videolist>();
        public ObservableCollection<Videolist> MonthCollection
        {
            get
            {
                return _MonthCollection;
            }
            set
            {
                if (_MonthCollection != value)
                {
                    _MonthCollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Videolist> _RankAllCollection = new ObservableCollection<Videolist>();
        public ObservableCollection<Videolist> RankAllCollection
        {
            get
            {
                return _RankAllCollection;
            }
            set
            {
                if (_RankAllCollection != value)
                {
                    _RankAllCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
