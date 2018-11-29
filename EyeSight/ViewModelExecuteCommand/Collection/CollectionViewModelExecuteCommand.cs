//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Collection
//类名称:       CollectionViewModelExecuteCommand
//创建时间:     2015/10/17 12:26:51
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelExecuteCommand.Collection
{
    public class CollectionViewModelExecuteCommand
    {
        private RelayCommand _ListViewLoadMoreMockCommand;
        public RelayCommand ListViewLoadMoreMockCommand
        {
            get
            {
                return _ListViewLoadMoreMockCommand
                    ?? (_ListViewLoadMoreMockCommand = new RelayCommand(() =>
                    {
                    }
                ));
            }
        }
    }
}
