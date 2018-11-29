//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand.Daily
//类名称:       DailyViewModelExecuteCommand
//创建时间:     2015/9/23 星期三 16:09:01
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Api.ApiRoot;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModel.Daily;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace EyeSight.ViewModelExecuteCommand.Daily
{
    public class DailyViewModelExecuteCommand
    {
        /// <summary>
        /// ListView下拉刷新的处理
        /// </summary>
        private RelayCommand _ListViewRefreshCommand;
        public RelayCommand ListViewRefreshCommand
        {
            get
            {
                return _ListViewRefreshCommand
                    ?? (_ListViewRefreshCommand = new RelayCommand(async () =>
                    {
                        //隐藏加载错误提示
                        RetryBox.Instance.HideRetry();

                        if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                        {
                            SimpleIoc.Default.Register<DailyViewModel>(false);
                        }
                        var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
                        if (dailyViewModel != null)
                        {
                            if (AppEnvironment.IsInternetAccess)
                            {
                                //及时将当前的nextPageUrl置为null
                                DicStore.AddOrUpdateValue<string>(AppCommonConst.DAILY_NEXT_PAGE_URL, null);
                                dailyViewModel.GetDaily(dailyViewModel.DailyCollection, dailyViewModel.DailyFlipViewCollection, ApiDailyRoot.Instance.DailyUrl, AppCacheNewsFileNameConst.CACHE_DAILY_FILENAME, true);
                            }
                            else
                            {
                                await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
                            }
                        }
                    }, () =>
                    {
                        if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                        {
                            SimpleIoc.Default.Register<DailyViewModel>(false);
                        }
                        var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();

                        if (dailyViewModel != null && !dailyViewModel.IsBusy)
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

                        if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                        {
                            SimpleIoc.Default.Register<DailyViewModel>(false);
                        }
                        var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
                        if (dailyViewModel != null)
                        {
                            dailyViewModel.GetDaily(dailyViewModel.DailyCollection, dailyViewModel.DailyFlipViewCollection, ApiDailyRoot.Instance.DailyUrl);
                        }
                    }, () =>
                    {
                        if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                        {
                            SimpleIoc.Default.Register<DailyViewModel>(false);
                        }
                        var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();

                        if (dailyViewModel != null && !dailyViewModel.IsBusy && DicStore.GetValueOrDefault<bool>(AppCommonConst.DAILY_HAS_NEXT_PAGE, false) && AppEnvironment.IsInternetAccess)
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
