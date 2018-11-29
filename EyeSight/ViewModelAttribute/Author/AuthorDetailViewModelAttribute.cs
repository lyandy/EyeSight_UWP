//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModelAttribute.Author

//类名称:       AuthorDetailViewModelAttribute

//创建时间:     2016/7/11 16:18:09

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Base;
using EyeSight.Model.Author;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelAttribute.Author
{
    public class AuthorDetailViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<VideoData> _AuthorDetailCollection = new ObservableCollection<VideoData>();
        public ObservableCollection<VideoData> AuthorDetailCollection
        {
            get
            {
                return _AuthorDetailCollection;
            }
            set
            {
                if (_AuthorDetailCollection != value)
                {
                    _AuthorDetailCollection = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Pgcinfo _PgcAuthorinfo = null;
        public Pgcinfo PgcAuthorinfo
        {
            get
            {
                return _PgcAuthorinfo;
            }
            set
            {
                if (_PgcAuthorinfo != value)
                {
                    _PgcAuthorinfo = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
