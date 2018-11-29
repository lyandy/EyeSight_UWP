//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute.Past
//类名称:       PastDetailViewModelAttribute
//创建时间:     2015/9/25 星期五 13:54:28
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
    public class PastDetailViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<VideoDetailData> _CategoryDetailCollection = new ObservableCollection<VideoDetailData>();
        public ObservableCollection<VideoDetailData> CategoryDetailCollection
        {
            get
            {
                return _CategoryDetailCollection;
            }
            set
            {
                if (_CategoryDetailCollection != value)
                {
                    _CategoryDetailCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
