//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModelAttribute.Author

//类名称:       AuthorViewModelAttribute

//创建时间:     2016/7/11 16:17:55

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
    public class AuthorViewModelAttribute : ViewModelAttributeBase
    {
        public ObservableCollection<AuthorData> _AuthorCollection = new ObservableCollection<AuthorData>();
        public ObservableCollection<AuthorData> AuthorCollection
        {
            get
            {
                return _AuthorCollection;
            }
            set
            {
                if (_AuthorCollection != value)
                {
                    _AuthorCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
