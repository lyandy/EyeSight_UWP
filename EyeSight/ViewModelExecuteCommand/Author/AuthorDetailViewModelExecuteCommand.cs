//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModelExecuteCommand.Author

//类名称:       AuthorDetailViewModelExecuteCommand

//创建时间:     2016/7/11 17:08:29

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Api.ApiRoot;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModel.Author;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModelExecuteCommand.Author
{
    public class AuthorDetailViewModelExecuteCommand
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

                        if (!SimpleIoc.Default.IsRegistered<AuthorDetailViewModel>())
                        {
                            SimpleIoc.Default.Register<AuthorDetailViewModel>(false);
                        }
                        var authorDetailViewModel = SimpleIoc.Default.GetInstance<AuthorDetailViewModel>();
                        if (authorDetailViewModel != null)
                        {
                            authorDetailViewModel.GetAthorDetailCollection(authorDetailViewModel.AuthorDetailCollection, ApiAuthorRoot.Instance.AuthorDetailUrl);
                        }
                    }, () =>
                    {
                        if (!SimpleIoc.Default.IsRegistered<AuthorDetailViewModel>())
                        {
                            SimpleIoc.Default.Register<AuthorDetailViewModel>(false);
                        }
                        var authorDetailViewModel = SimpleIoc.Default.GetInstance<AuthorDetailViewModel>();

                        if (authorDetailViewModel != null && !authorDetailViewModel.IsBusy && DicStore.GetValueOrDefault<bool>(AppCommonConst.AUTHOR_DETAIL_HAS_NEXT_PAGE, false) && AppEnvironment.IsInternetAccess)
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
