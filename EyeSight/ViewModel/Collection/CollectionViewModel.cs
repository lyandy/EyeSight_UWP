//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModel.Collection
//类名称:       CollectionViewModel
//创建时间:     2015/10/12 21:54:10
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Helper;
using EyeSight.Model.Daily;
using EyeSight.ViewModelAttribute.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModel.Collection
{
    public class CollectionViewModel : CollectionViewModelAttribute
    {
        public async Task GetFavoriteCollection()
        {
            FavoriteCollection = await Task.Run(async () =>
            {
                return new ObservableCollection<ModelPropertyBase>(await DatabaseHelper.Instance.QueryCollections());
            });
        }
    }
}
