//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute.Past
//类名称:       DailyViewModelAttribute
//创建时间:     2015/9/24 星期四 20:08:26
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Model.Past;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelAttribute.Past
{
    public class PastViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<CommonItemlist> _CategoryCollection = new ObservableCollection<CommonItemlist>();
        public ObservableCollection<CommonItemlist> CategoryCollection
        {
            get
            {
                return _CategoryCollection;
            }
            set
            {
                if (_CategoryCollection != value)
                {
                    _CategoryCollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 精彩发现 视频小分类集合
        /// </summary>
        public ObservableCollection<CommonItemListData> _CategorySubCollection = new ObservableCollection<CommonItemListData>();
        public ObservableCollection<CommonItemListData> CategorySubCollection
        {
            get
            {
                return _CategorySubCollection;
            }
            set
            {
                if (_CategorySubCollection != value)
                {
                    _CategorySubCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
