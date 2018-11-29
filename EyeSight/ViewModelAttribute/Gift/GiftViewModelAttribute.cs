//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModelAttribute.Gift

//类名称:       GiftViewModelAttribute

//创建时间:     2016/7/6 19:05:14

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Base;
using EyeSight.Model.Gift;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelAttribute.Gift
{
    public class GiftViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<GiftModel> _GiftCollection = new ObservableCollection<GiftModel>();
        public ObservableCollection<GiftModel> GiftCollection
        {
            get
            {
                return _GiftCollection;
            }
            set
            {
                if (_GiftCollection != value)
                {
                    _GiftCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
