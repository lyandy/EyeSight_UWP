//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Past
//类名称:       PastDetailViewModelExecuteCommand
//创建时间:     2015/9/25 星期五 14:34:31
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Api.ApiRoot;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModel.Past;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelExecuteCommand.Past
{
    public class PastDetailViewModelExecuteCommand
    {
        /// <summary>
        /// ListView加载更多
        /// </summary>
        private RelayCommand _ListViewLoadMoreCommand;
        public RelayCommand ListViewLoadMoreCommand
        {
            get
            {
                return _ListViewLoadMoreCommand
                    ?? (_ListViewLoadMoreCommand = new RelayCommand(() =>
                    {
                        //隐藏加载错误提示
                        RetryBox.Instance.HideRetry();

                        if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                        {
                            SimpleIoc.Default.Register<PastDetailViewModel>(false);
                        }
                        var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();
                        if (pastDetailViewModel != null)
                        {
                            pastDetailViewModel.GetPastCategoryDetail(pastDetailViewModel.CategoryDetailCollection, ApiPastRoot.Instance.CatrgoryDetailUrl);
                        }
                    }, () =>
                    {
                        if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                        {
                            SimpleIoc.Default.Register<PastDetailViewModel>(false);
                        }
                        var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();

                        if (pastDetailViewModel != null && !pastDetailViewModel.IsBusy && DicStore.GetValueOrDefault<bool>(AppCommonConst.PAST_CATEGORY_DETAIL_HAS_NEXT_PAGE, false) && AppEnvironment.IsInternetAccess)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                ));
            }
        }
    }
}
