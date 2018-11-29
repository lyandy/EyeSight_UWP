﻿//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Rank
//类名称:       RankViewModelExecuteCommand
//创建时间:     2015/12/8 14:20:39
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelExecuteCommand.Rank
{
    public class RankViewModelExecuteCommand
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
