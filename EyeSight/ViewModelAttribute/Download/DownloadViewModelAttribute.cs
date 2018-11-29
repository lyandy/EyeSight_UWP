//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute.Download
//类名称:       DownloadViewModelAttribute
//创建时间:     2015/10/27 18:14:18
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Model.Download;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelAttribute.Download
{
    public class DownloadViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<ModelPropertyBase> _DownloadCollection = new ObservableCollection<ModelPropertyBase>();
        public ObservableCollection<ModelPropertyBase> DownloadCollection
        {
            get
            {
                return _DownloadCollection;
            }
            set
            {
                if (_DownloadCollection != value)
                {
                    _DownloadCollection = value;

                    RaisePropertyChanged();
                }
            }
        }

        public List<DownloadingModel> _DownloadingList = new List<DownloadingModel>();
        public List<DownloadingModel> DownloadingList
        {
            get
            {
                return _DownloadingList;
            }
            set
            {
                if (_DownloadingList != value)
                {
                    _DownloadingList = value;

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
