//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Past
//类名称:       PastCategoryViewModelExecuteCommand
//创建时间:     2015/12/9 10:17:42
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelExecuteCommand.Past
{
    public class PastCategoryViewModelExecuteCommand
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
