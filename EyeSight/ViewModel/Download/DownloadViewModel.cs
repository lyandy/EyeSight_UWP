//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModel.Download
//类名称:       DownloadViewModel
//创建时间:     2015/10/27 18:13:43
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Helper;
using EyeSight.ViewModelAttribute.Download;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModel.Download
{
    public class DownloadViewModel : DownloadViewModelAttribute
    {
        public async Task GetDownloadCollection()
        {
            DownloadCollection = await Task.Run(async () =>
            {
                return new ObservableCollection<ModelPropertyBase>(await DatabaseHelper.Instance.QueryCollections(true));
            });
        }
    }
}
