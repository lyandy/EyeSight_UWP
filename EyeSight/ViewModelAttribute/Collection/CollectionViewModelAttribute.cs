//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute.Collection
//类名称:       CollectionViewModelAttribute
//创建时间:     2015/10/12 21:48:20
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

namespace EyeSight.ViewModelAttribute.Collection
{
    public class CollectionViewModelAttribute : ViewModelAttributeBase
    {
        /// <summary>
        /// 我的收藏
        /// </summary>
        public ObservableCollection<ModelPropertyBase> _FavoriteCollection = new ObservableCollection<ModelPropertyBase>();
        public ObservableCollection<ModelPropertyBase> FavoriteCollection
        {
            get
            {
                return _FavoriteCollection;
            }
            set
            {
                if (_FavoriteCollection != value)
                {
                    _FavoriteCollection = value;

                    RaisePropertyChanged();
                }
            }
        }

        private bool _isEmptyShow = false;
        
        public bool isEmptyShow
        {
            get
            {
                return _isEmptyShow;
            }
            set
            {
                if (_isEmptyShow != value)
                {
                    _isEmptyShow = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
