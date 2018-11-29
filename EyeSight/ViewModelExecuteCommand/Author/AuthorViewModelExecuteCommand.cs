//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModelExecuteCommand.Author

//类名称:       AuthorViewModelExecuteCommand

//创建时间:     2016/7/11 17:08:19

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
    public class AuthorViewModelExecuteCommand
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

                        if (!SimpleIoc.Default.IsRegistered<AuthorViewModel>())
                        {
                            SimpleIoc.Default.Register<AuthorViewModel>(false);
                        }
                        var authorViewModel = SimpleIoc.Default.GetInstance<AuthorViewModel>();
                        if (authorViewModel != null)
                        {
                            authorViewModel.GetAuthorCollection(authorViewModel.AuthorCollection, ApiAuthorRoot.Instance.AuthorListUrl);
                        }
                    }, () =>
                    {
                        if (!SimpleIoc.Default.IsRegistered<AuthorViewModel>())
                        {
                            SimpleIoc.Default.Register<AuthorViewModel>(false);
                        }
                        var authorViewModel = SimpleIoc.Default.GetInstance<AuthorViewModel>();

                        if (authorViewModel != null && !authorViewModel.IsBusy && DicStore.GetValueOrDefault<bool>(AppCommonConst.AUTHOR_HAS_NEXT_PAGE, false) && AppEnvironment.IsInternetAccess)
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
